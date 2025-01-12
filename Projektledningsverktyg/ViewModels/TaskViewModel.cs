using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;
using System.Data.SQLite;
using System.Collections.Generic;

namespace Projektledningsverktyg.ViewModels
{
    public class TaskViewModel : ViewModelBase
    {
        private readonly Member _currentMember;

        // This handles the UI logic - how tasks are displayed and manipulated
        public TaskViewModel(ApplicationDbContext context, Member currentMember)
        {
            _context = context;  // Stores database connection
            _currentMember = currentMember;
            LoadTasks();
            InitializeCommands();
        }

        // Main collection for tasks
        private ObservableCollection<TaskModel> _tasks;
        private string _newTaskTitle = string.Empty;
        private DateTime _selectedDate = DateTime.Now;
        private TaskPriority _selectedPriority = TaskPriority.Low;
        private TaskStatus _selectedStatus = TaskStatus.NotStarted;
        private readonly ApplicationDbContext _context;

        #region Property

        private TaskModel _currentTask;
        public TaskModel CurrentTask
        {
            get => _currentTask;
            set
            {
                _currentTask = value;
                LoadComments();
                OnPropertyChanged(nameof(CurrentTask));
            }
        }
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                _newTaskTitle = value;
                OnPropertyChanged(nameof(NewTaskTitle));
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        public TaskPriority SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                _selectedPriority = value;
                OnPropertyChanged(nameof(SelectedPriority));
            }
        }

        #endregion

        #region Commands for Execute

        // Property that will hold our command - UI elements will bind to this
        public ICommand AddTaskCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand AddCommentCommand { get; private set; }
        public ICommand SaveDescriptionCommand { get; private set; }

        private void InitializeCommands()
        {
            AddTaskCommand = new RelayCommand(ExecuteAddTask);
            DeleteTaskCommand = new RelayCommand<TaskModel>(ExecuteDeleteTask);
            AddCommentCommand = new RelayCommand<string>(ExecuteAddComment);
            SaveDescriptionCommand = new RelayCommand<string>(SaveDescription);
        }

        #endregion

        #region Comments

        private ObservableCollection<Comment> _currentTaskComments;
        public ObservableCollection<Comment> CurrentTaskComments
        {
            get => _currentTaskComments;
            set
            {
                _currentTaskComments = value;
                OnPropertyChanged(nameof(CurrentTaskComments));
            }
        }
        private async void ExecuteAddComment(string content)
        {
            var newComment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                MemberId = _currentMember.Id,
                Type = CommentType.Task,
                TaskId = CurrentTask?.Id
            };

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            CurrentTaskComments.Add(newComment);
        }

        #endregion

        #region Load

        private void LoadTasks()
        {
            var dbTasks = _context.Tasks
                .Include(t => t.AssignedTo)
                .ToList();

            var taskModels = dbTasks.Select(t => new TaskModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status
                //MemberId null by default
            });

            Tasks = new ObservableCollection<TaskModel>(taskModels);
        }

        public void SelectTask(TaskModel task)
        {
            CurrentTask = task;
            Debug.WriteLine($"Task selected: {task?.Id}");
        }

        private void LoadComments()
        {
            if (CurrentTask != null)
            {
                var comments = _context.Comments
                    .AsNoTracking()
                    .Include(c => c.Member)
                    .Where(c => c.TaskId == CurrentTask.Id)
                    .ToList();

                CurrentTaskComments = new ObservableCollection<Comment>(comments);
            }
        }

        #endregion

        #region Add Task Command

        // Add command execution methods
        private async void ExecuteAddTask()
        {
            var newTask = new Data.Entities.Task
            {
                Title = NewTaskTitle,
                DueDate = SelectedDate,
                Priority = SelectedPriority,
                Status = TaskStatus.NotStarted,
                Description = string.Empty
                // MemberId is null by default
            };

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            var taskModel = new TaskModel
            {
                Id = newTask.Id,
                Title = newTask.Title,
                DueDate = newTask.DueDate,
                Priority = newTask.Priority,
                Status = newTask.Status,
                Description = newTask.Description
            };

            Tasks.Add(taskModel);

            // Reset the form
            NewTaskTitle = string.Empty;
            SelectedDate = DateTime.Now;
            SelectedPriority = TaskPriority.Low;
        }

        private void SaveDescription(string description)
        {
            // Save description to database
            var task = _context.Tasks.Find(CurrentTask.Id);
            if (task != null)
            {
                task.Description = description;
                _context.SaveChanges();

                // Update the current task model
                CurrentTask.Description = description;

                // Force refresh of button text binding
                OnPropertyChanged(nameof(CurrentTask));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion

        #region Delete Task Command

        private void ExecuteDeleteTask(TaskModel taskModel)
        {
            var taskToDelete = _context.Tasks
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == taskModel.Id);

            if (taskToDelete != null)
            {
                _context.Tasks.Remove(taskToDelete);
                _context.SaveChanges();
                Tasks.Remove(taskModel);
            }
        }


        #endregion

        // Observable collection for UI binding
        public ObservableCollection<TaskModel> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        
    }
}
