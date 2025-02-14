using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Projektledningsverktyg.ViewModels
{
    public class HouseholdViewModel
    {
        #region Events and Fields
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<Dictionary<(string Day, int TaskId), (bool IsLocked, bool IsSelected)>> DayStatesUpdated;
        private Dictionary<(string Day, int TaskId), (bool IsLocked, bool IsSelected)> _dayStates
                    = new Dictionary<(string, int), (bool, bool)>();

        private HashSet<Household> _selectedTasks = new HashSet<Household>();
        private HashSet<Member> _selectedMembers = new HashSet<Member>();

        private readonly HouseholdRepository _repository;
        private ObservableCollection<Household> _tasks;
        private ObservableCollection<Member> _members;

        private string _userMessage;
        private System.Timers.Timer _messageTimer;
        private bool _isTeamTask;
        #endregion

        #region Properties
        public event Action<List<string>, int, int> DaysLoaded;

        public ObservableCollection<Household> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }
        public ObservableCollection<Member> Members
        {
            get => _members;
            set
            {
                _members = value;
                OnPropertyChanged(nameof(Members));
            }
        }

        public string UserMessage
        {
            get => _userMessage;
            set
            {
                _userMessage = value;
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(UserMessage)));

                // Clear message after 4 seconds
                StartMessageTimer();
            }
        } 
        public bool IsTeamTask
        {
            get => _isTeamTask;
            set
            {
                _isTeamTask = value;
                OnPropertyChanged(nameof(IsTeamTask));
            }
        }

        #endregion

        #region Commands
        public ICommand AddTaskCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand SelectTaskCommand { get; private set; }
        public ICommand SelectMemberCommand { get; private set; }
        #endregion

        #region Constructor and Initialization
        public HouseholdViewModel(HouseholdRepository repository)
        {
            _repository = repository;
            InitializeCommands();
            LoadData();

        }
        private void InitializeCommands()
        {
            AddTaskCommand = new RelayCommand(ExecuteAddTask);
            DeleteTaskCommand = new RelayCommand<Household>(ExecuteDeleteTask);

            SelectTaskCommand = new RelayCommand<Household>(SelectTask);
            SelectMemberCommand = new RelayCommand<Member>(SelectMember);
        }

        private void LoadData()
        {
            Tasks = new ObservableCollection<Household>(_repository.GetAllHouseholdTasks());
            Members = new ObservableCollection<Member>(_repository.GetAllMembers());

        }
        #endregion

        #region Execute
        private void ExecuteAddTask()
        {
            var newTask = new Household
            {
                Title = "Ny uppgift",
                Description = "Beskrivning av uppgiften"
            };

            _repository.AddHouseholdTask(newTask);
            Tasks.Add(newTask);
        }
        private void ExecuteDeleteTask(Household task)
        {
            _repository.DeleteHouseholdTask(task.Id);
            Tasks.Remove(task);
        }

        #endregion

        #region Task Management
        public void SelectTask(Household task)
        {
            if (_selectedTasks.Contains(task))
            {
                HandleTaskDeselection(task);
            }
            else
            {
                HandleTaskSelection(task);
            }
            OnPropertyChanged(nameof(IsSelectedTask));
            OnPropertyChanged(nameof(IsSelectedMember));
        }
        private void HandleTaskDeselection(Household task)
        {
            _selectedTasks.Remove(task);
            _selectedMembers.Clear();
            _dayStates.Clear();
            DayStatesUpdated?.Invoke(_dayStates);
        }
        private void HandleTaskSelection(Household task)
        {
            var previousTask = _selectedTasks.FirstOrDefault();
            if (previousTask != null)
            {
                _dayStates.Clear();
            }

            _selectedTasks.Clear();
            _selectedTasks.Add(task);
            _selectedMembers.Clear();

            LoadTaskMembers(task);
            UpdateDayStates(task);
        }
        private void LoadTaskMembers(Household task)
        {
            foreach (var assignment in task.Assignments)
            {
                var member = Members.FirstOrDefault(m => m.Id == assignment.MemberId);
                if (member != null)
                {
                    _selectedMembers.Add(member);
                }
            }
        }
        #endregion

        #region Member Management
        public void SelectMember(Member member)
        {
            var selectedTask = _selectedTasks.FirstOrDefault();
            if (selectedTask == null)
                return;

            if (_selectedMembers.Contains(member))
            {
                HandleMemberDeselection(member, selectedTask);
            }
            else
            {
                HandleMemberSelection(member, selectedTask);
            }
            OnPropertyChanged(nameof(IsSelectedMember));
        }

        private void HandleMemberDeselection(Member member, Household task)
        {
            // Remove member from selected set
            _selectedMembers.Remove(member);

            // Find and remove the assignment
            var assignment = task.Assignments.FirstOrDefault(a => a.MemberId == member.Id);
            if (assignment != null)
            {
                // Remove from database
                _repository.RemoveHouseholdAssignment(task.Id, member.Id);
                task.Assignments.Remove(assignment);

                // Clear day states for this task
                _dayStates.Clear();
                UpdateDayStates(task);
            }
        }

        private void HandleMemberSelection(Member member, Household task)
        {
            _selectedMembers.Add(member);
            AssignMemberToTask(member);
            UpdateDayStates(task);
        }
        #endregion

        #region Day State Management
        private void UpdateDayStates(Household task)
        {
            _dayStates.Clear();
            var currentMember = _selectedMembers.LastOrDefault();

            if (currentMember != null)
            {
                foreach (var assignment in task.Assignments)
                {
                    var days = JsonSerializer.Deserialize<List<string>>(assignment.AssignedDays);
                    bool isCurrentMember = assignment.MemberId == currentMember.Id;

                    foreach (var day in days)
                    {
                        // Store state with task ID
                        _dayStates[(day, task.Id)] = (!isCurrentMember, isCurrentMember);
                    }
                }
            }
            DayStatesUpdated?.Invoke(_dayStates);
        }

        public void HandleDaySelection(string day, Household task)
        {
            var currentMember = _selectedMembers.LastOrDefault();
            if (currentMember == null)
                return;

            // Check state for specific task
            if (_dayStates.TryGetValue((day, task.Id), out var state) && state.IsLocked)
                return;

            var assignment = task.Assignments.FirstOrDefault(a => a.MemberId == currentMember.Id);
            if (assignment != null)
            {
                UpdateAssignmentDays(assignment, day);
                UpdateDayStates(task);
            }
        }

        private void UpdateAssignmentDays(HouseholdAssignment assignment, string day)
        {
            var days = JsonSerializer.Deserialize<List<string>>(assignment.AssignedDays ?? "[]");
            if (days.Contains(day))
                days.Remove(day);
            else
                days.Add(day);

            assignment.AssignedDays = JsonSerializer.Serialize(days);
            _repository.UpdateHouseholdAssignment(assignment);
        }
        #endregion

        #region UI Helpers
        public SolidColorBrush GetTaskBorderColor(Household task)
        {
            return IsSelectedTask(task)
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
        }
        public SolidColorBrush GetMemberBorderColor(Member member)
        {
            return IsSelectedMember(member)
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE"));
        }
        private void StartMessageTimer()
        {
            if (_messageTimer == null)
            {
                _messageTimer = new System.Timers.Timer(4000);
                _messageTimer.Elapsed += (s, e) =>
                {
                    UserMessage = string.Empty;
                    _messageTimer.Stop();
                };
            }
            _messageTimer.Start();
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region State Checks
        public bool IsSelectedTask(Household task) => _selectedTasks.Contains(task);
        public bool IsSelectedMember(Member member) => _selectedMembers.Contains(member);
        #endregion


        public void AssignMemberToTask(Member member)
        {
            var selectedTask = _selectedTasks.FirstOrDefault();
            if (selectedTask == null || member == null)
                return;

            var assignment = new HouseholdAssignment
            {
                HouseholdId = selectedTask.Id,
                MemberId = member.Id,
                AssignedDays = "[]"
            };

            _repository.AddHouseholdAssignment(assignment);
            selectedTask.Assignments.Add(assignment);
        }

        public SolidColorBrush GetDayBackground(string day, Household task)
        {
            var currentMember = _selectedMembers.LastOrDefault();
            if (currentMember == null)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));

            // Check if day is locked by other members
            var otherMemberAssignments = task.Assignments
                .Where(a => a.MemberId != currentMember.Id);

            if (otherMemberAssignments.Any(a => JsonSerializer.Deserialize<List<string>>(a.AssignedDays).Contains(day)))
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcdd2")); // Light red for locked
            }

            // Current member's days
            var currentAssignment = task.Assignments.FirstOrDefault(a => a.MemberId == currentMember.Id);
            return currentAssignment?.AssignedDays.Contains(day) == true
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3dcfa"))  // Light blue
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")); // Light gray
        }
        
    }
}
