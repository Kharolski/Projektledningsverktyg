using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Projektledningsverktyg.Helpers;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;

namespace Projektledningsverktyg.ViewModels
{
    public class TaskViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event Action<int, int> CommentCountChanged;

        // This handles the UI logic - how tasks are displayed and manipulated
        public TaskViewModel(ApplicationDbContext context, Member currentMember)
        {
            _context = context;  // Stores database connection
            _currentMember = currentMember;
            SelectedPriority = TaskPriority.Low;
            LoadTasks();
            InitializeCommands();
        }

        // Main collection for tasks
        private readonly Member _currentMember;
        private ObservableCollection<TaskModel> _tasks;
        private string _newTaskTitle = string.Empty;
        private string _newTaskDescription = string.Empty;
        private string _commentText = string.Empty;
        private DateTime _selectedDate = DateTime.Now;
        private TaskPriority _selectedPriority = TaskPriority.Low;
        private TaskStatus _selectedStatus = TaskStatus.NotStarted;
        private string _addTaskErrorMessage;
        private readonly ApplicationDbContext _context;
        private ObservableCollection<IGrouping<string, TaskModel>> _tasksByMonth;

        #region Property
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
        public ObservableCollection<IGrouping<string, TaskModel>> TasksByMonth
        {
            get => _tasksByMonth;
            set
            {
                _tasksByMonth = value;
                OnPropertyChanged(nameof(TasksByMonth));
            }
        }

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
        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                _newTaskDescription = value;
                OnPropertyChanged(nameof(NewTaskDescription));
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
        // Display priority by Name
        public string PriorityDisplay
        {
            get
            {
                return EnumDisplayHelper.GetDisplayName(SelectedPriority);
            }
        }
        public string AddTaskErrorMessage
        {
            get => _addTaskErrorMessage;
            set
            {
                _addTaskErrorMessage = value;
                OnPropertyChanged(nameof(AddTaskErrorMessage));
            }
        }

        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                OnPropertyChanged();
            }
        }

        // ID för uppgiften som redigeras, null om det är en ny uppgift
        public int? EditingTaskId { get; private set; }

        // Flagga för att markera om vi är i redigeringsläge
        public bool IsEditMode { get; private set; }
        #endregion

        #region Commands for Execute

        // Property that will hold our command - UI elements will bind to this
        public ICommand AddTaskCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand AddCommentCommand { get; private set; }
        public ICommand DeleteCommentCommand { get; private set; }

        private void InitializeCommands()
        {
            AddTaskCommand = new RelayCommand(ExecuteSaveTask);
            DeleteTaskCommand = new RelayCommand<TaskModel>(ExecuteDeleteTask);
            AddCommentCommand = new RelayCommand<string>(ExecuteAddComment);
            DeleteCommentCommand = new RelayCommand<int>(commentId =>
                        ExecuteDeleteSingleComment(commentId, App.CurrentUser.Id));
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
                MemberId = App.CurrentUser.Id,
                Type = CommentType.Task,
                TaskId = CurrentTask?.Id,
                //Member = _currentMember
            };

            // Get fresh member data for display
            var member = _context.Members.Find(App.CurrentUser.Id);
            newComment.Member = member;

            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            CurrentTaskComments.Add(newComment);

            // Clear the TextBox through binding
            CommentText = string.Empty;

            // Utlös event med uppdaterad räkning
            CommentCountChanged?.Invoke(CurrentTask.Id, CurrentTaskComments.Count);
        }

        public async void ExecuteSaveTask()
        {
            if (IsEditMode && EditingTaskId > 0)
            {
                // Vi är i redigeringsläge - hitta och uppdatera den befintliga uppgiften
                var existingTask = await _context.Tasks.FindAsync(EditingTaskId);

                if (existingTask != null)
                {
                    // Använd den befintliga ExecuteUpdateTask-metoden
                    ExecuteUpdateTask(existingTask);
                }
            }
            else
            {
                // Vi skapar en ny uppgift
                var newTask = new Data.Entities.Task
                {
                    Title = NewTaskTitle,
                    Description = NewTaskDescription,
                    DueDate = SelectedDate,
                    Priority = SelectedPriority,
                    Status = TaskStatus.NotStarted,
                    MemberId = _currentMember.Id
                };

                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();
            }

            // Återställ efter redigering
            IsEditMode = false;
            EditingTaskId = 0;

            LoadTasks(); // Refresh the task list
        }
        #endregion

        #region Load

        public void LoadTasks()
        {
            // Get all tasks from database
            var dbTasks = _context.Tasks
                // Load all comments for each task
                .Include(t => t.Comments)
                    // For each comment, load the member who wrote it
                    .ThenInclude(c => c.Member)
                // Sort tasks by due date
                .OrderBy(t => t.DueDate)
                // Execute query and get results
                .ToList();

            // Convert database tasks to TaskModel objects
            var taskModels = dbTasks.Select(t => new TaskModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status,

                // Create collection of comments with author details
                CurrentTaskComments = new ObservableCollection<Comment>(
                    t.Comments.Select(c => new Comment
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        MemberId = c.MemberId,
                        Member = new Member
                        {
                            Id = c.Member.Id,
                            FirstName = c.Member.FirstName,
                            ProfileImagePath = c.Member.ProfileImagePath
                        }
                    }).ToList())
            });

            // Group tasks by month and year
            var groupedTasks = taskModels
                .GroupBy(t => t.DueDate.ToString("MMMM yyyy"))
                .OrderBy(g => DateTime.ParseExact(g.Key, "MMMM yyyy", CultureInfo.CurrentCulture));

            // Update the observable collection
            TasksByMonth = new ObservableCollection<IGrouping<string, TaskModel>>(groupedTasks);
        }

        public void SelectTask(TaskModel task)
        {
            CurrentTask = task;
        }

        /// <summary>
        /// Förbereder en befintlig uppgift för redigering
        /// </summary>
        /// <param name="taskToEdit">Uppgiften som ska redigeras</param>
        public void LoadTaskForEditing(Task taskToEdit)
        {
            if (taskToEdit == null)
                return;

            // Spara uppgiftens ID för att kunna uppdatera rätt uppgift
            EditingTaskId = taskToEdit.Id;

            // Fyll i formulärfält med befintlig information
            NewTaskTitle = taskToEdit.Title;
            NewTaskDescription = taskToEdit.Description;
            SelectedDate = taskToEdit.DueDate;
            SelectedPriority = taskToEdit.Priority;

            // Sätt flagga att vi är i redigeringsläge
            IsEditMode = true;
        }

        public void LoadComments()
        {
            // Check if we have a selected task
            if (CurrentTask != null)
            {
                // Get comments from database
                var comments = _context.Comments
                    .AsNoTracking()           // For better performance
                    .Include(c => c.Member)   // Load the comment author details
                    .Where(c => c.TaskId == CurrentTask.Id)  // Get comments for current task only
                    .ToList()
                    .Select(c => new Comment  // Map to new Comment objects with additional properties
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        MemberId = c.MemberId,
                        Member = c.Member
                    })
                    .ToList();

                // Update the observable collection with new comments
                CurrentTaskComments = new ObservableCollection<Comment>(comments);
            }
        }

        public void ExecuteUpdateTask(Task existingTask)
        {
            // Uppdatera befintlig uppgift med nya värden
            existingTask.Title = NewTaskTitle;
            existingTask.Description = NewTaskDescription;
            existingTask.DueDate = SelectedDate;
            existingTask.Priority = SelectedPriority;

            // Spara ändringarna i databasen
            _context.SaveChanges();
        }
        #endregion

        #region Delete Task Command

        private void ExecuteDeleteTask(TaskModel taskModel)
        {
            ExecuteDeleteAllTaskComments(taskModel.Id);

            var taskToDelete = _context.Tasks
                .FirstOrDefault(t => t.Id == taskModel.Id);

            if (taskToDelete != null)
            {
                _context.Tasks.Remove(taskToDelete);
                _context.SaveChanges();
                LoadTasks();
            }
        }

        // Delete all comments for a task
        private void ExecuteDeleteAllTaskComments(int taskId)
        {
            var taskComments = _context.Comments
                .Where(c => c.TaskId == taskId)
                .ToList();

            _context.Comments.RemoveRange(taskComments);
            _context.SaveChanges();
        }

        private void ExecuteDeleteSingleComment(int commentId, int currentUserId)
        {
            var comment = _context.Comments.Find(commentId);
            if (comment != null && comment.MemberId == currentUserId)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();

                // Refresh the comments list
                CurrentTaskComments.Remove(CurrentTaskComments.First(c => c.Id == commentId));

                // Utlös event med uppdaterad räkning efter borttagning
                CommentCountChanged?.Invoke(CurrentTask.Id, CurrentTaskComments.Count);
            }
        }
        #endregion

    }
}
