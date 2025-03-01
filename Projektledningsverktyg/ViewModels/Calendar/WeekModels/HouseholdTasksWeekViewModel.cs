using Projektledningsverktyg.Data.Repository;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Text.Json;
using System.Linq;
using System.Windows.Input;
using Projektledningsverktyg.Commands;
using System.Collections.Generic;
using System.Windows;
using Projektledningsverktyg.Views.Tasks;
using System.Windows.Controls;

namespace Projektledningsverktyg.ViewModels.Calendar.WeekModels
{
    public class HouseholdTasksWeekViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Fields
        private readonly HouseholdRepository _householdRepository;
        private DateTime _selectedDate;
        #endregion

        #region Properties
        // Collection for household tasks assigned to the selected day
        public ObservableCollection<HouseholdTaskModel> HouseholdTasks { get; set; }

        // Property to check if we have tasks to display
        public bool HasHouseholdTasks => HouseholdTasks.Count > 0;
        public ICommand RemoveDayFromTaskCommand { get; }
        public ICommand NavigateToHouseholdTabCommand { get; }
        #endregion

        #region Constructor
        public HouseholdTasksWeekViewModel(HouseholdRepository householdRepository)
        {
            _householdRepository = householdRepository;
            HouseholdTasks = new ObservableCollection<HouseholdTaskModel>();

            // Initialize commands
            RemoveDayFromTaskCommand = new RelayCommand<HouseholdTaskModel>(RemoveDayFromTask);
            NavigateToHouseholdTabCommand = new RelayCommand(NavigateToHouseholdTab);

            // Initialize with today's date
            _selectedDate = DateTime.Today;

            // Load initial data
            LoadHouseholdTasks(_selectedDate);
        }
        #endregion

        #region Public Methods
        // Method to be called when selected date changes
        public void UpdateSelectedDate(DateTime newDate)
        {
            _selectedDate = newDate;
            LoadHouseholdTasks(_selectedDate);
        }
        #endregion

        #region Private Methods
        private void LoadHouseholdTasks(DateTime date)
        {
            // Clear existing items
            HouseholdTasks.Clear();

            // Get the current day of week as string
            string currentDayOfWeek = date.DayOfWeek.ToString();

            // Get all household tasks
            var allTasks = _householdRepository.GetAllHouseholdTasks();

            // Filter tasks for the current day of week
            foreach (var task in allTasks)
            {
                foreach (var assignment in task.Assignments)
                {
                    // Parse the assigned days JSON
                    var assignedDays = JsonSerializer.Deserialize<string[]>(assignment.AssignedDays);

                    // Check if the current day is in the assigned days
                    if (assignedDays.Contains(currentDayOfWeek))
                    {
                        // Add the task to our collection
                        HouseholdTasks.Add(new HouseholdTaskModel
                        {
                            Id = task.Id,
                            Title = task.Title,
                            AssignedTo = assignment.Member.FirstName,
                            MemberId = assignment.MemberId,
                            AssignmentId = assignment.Id
                        });
                    }
                }
            }

            OnPropertyChanged(nameof(HouseholdTasks));
            OnPropertyChanged(nameof(HasHouseholdTasks));
        }

        private void RemoveDayFromTask(HouseholdTaskModel taskModel)
        {
            if (taskModel == null)
                return;

            // Hämta tilldelningen från databasen
            var household = _householdRepository.GetAllHouseholdTasks()
                .FirstOrDefault(h => h.Id == taskModel.Id);

            if (household != null)
            {
                var assignment = household.Assignments
                    .FirstOrDefault(a => a.MemberId == taskModel.MemberId);

                if (assignment != null)
                {
                    // Hämta aktuell dag som string
                    string currentDayOfWeek = _selectedDate.DayOfWeek.ToString();

                    // Deserialisera dagar, ta bort aktuell dag, och serialisera igen
                    var days = JsonSerializer.Deserialize<List<string>>(assignment.AssignedDays);
                    if (days.Contains(currentDayOfWeek))
                    {
                        days.Remove(currentDayOfWeek);
                        assignment.AssignedDays = JsonSerializer.Serialize(days);

                        // Uppdatera i databasen
                        _householdRepository.UpdateHouseholdAssignment(assignment);

                        // Ladda om data
                        LoadHouseholdTasks(_selectedDate);
                    }
                }
            }
        }
        private void NavigateToHouseholdTab()
        {
            // Navigera till uppgifter-vyn först, sedan till hushållstabben
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                // Simulera klick på uppgiftsknappen
                var btnTasks = mainWindow.FindName("BtnTasks") as RadioButton;
                if (btnTasks != null)
                {
                    btnTasks.IsChecked = true;

                    // Ge UI tid att uppdatera och navigera
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        // Hitta TabControl i TasksView och välj rätt tab
                        if (mainWindow.MainFrame.Content is TasksView tasksView)
                        {
                            var tabControl = tasksView.FindName("TaskTabControl") as TabControl;
                            if (tabControl != null)
                            {
                                // Anta att hushållstabben är index 2 (Andra tabben)
                                tabControl.SelectedIndex = 1;
                            }
                        }
                    }, System.Windows.Threading.DispatcherPriority.Loaded);
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    // Model för vyn att visa hushållsuppgifter
    public class HouseholdTaskModel
    {
        public int Id { get; set; }             // Household Id
        public string Title { get; set; }
        public string AssignedTo { get; set; }
        public int MemberId { get; set; }     
        public int AssignmentId { get; set; }
    }
}
