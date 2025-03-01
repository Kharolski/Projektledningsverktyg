using Projektledningsverktyg.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.Data.Entities;
using System.Windows;
using System.Linq;

namespace Projektledningsverktyg.ViewModels.Calendar.WeekModels
{
    public class TaskWeekViewModel : ViewModelBase
    {
        public event EventHandler<Task> ShowCommentsRequested;

        #region Private Fields
        private readonly ITaskRepository _taskRepository;
        private DateTime _selectedDate;
        private ObservableCollection<Task> _tasks;
        private bool _hasTasks;
        private Task _selectedTask;
        #endregion

        #region Public Properties
        /// <summary>
        /// Samling av aktuella uppgifter för vald dag
        /// </summary>
        public ObservableCollection<Task> Tasks
        {
            get => _tasks;
            set
            {
                _tasks = value;
                OnPropertyChanged();
            }
        }
        public void UpdateCommentCount(int taskId, int newCount)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.CommentCount = newCount;
                // Tvinga uppdatering av gränssnittet
                OnPropertyChanged(nameof(Tasks));
            }
        }
        public ObservableCollection<Comment> CurrentTaskComments { get; set; } = new ObservableCollection<Comment>();
        public int CurrentUserId => App.CurrentUser?.Id ?? 0;

        /// <summary>
        /// Om det finns uppgifter att visa
        /// </summary>
        public bool HasTasks
        {
            get => _hasTasks;
            set
            {
                _hasTasks = value;
                OnPropertyChanged(nameof(HasTasks));
            }
        }

        /// <summary>
        /// Vald uppgift för redigering/borttagning
        /// </summary>
        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Kommando för att lägga till en ny uppgift
        /// </summary>
        public ICommand AddTaskCommand { get; private set; }

        /// <summary>
        /// Kommando för att redigera en befintlig uppgift
        /// </summary>
        public ICommand EditTaskCommand { get; private set; }

        /// <summary>
        /// Kommando för att ta bort en uppgift
        /// </summary>
        public ICommand DeleteTaskCommand { get; private set; }

        /// <summary>
        /// Kommando för att visa kommentarer för en uppgift
        /// </summary>
        public ICommand ShowCommentsCommand { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Konstruktor för TaskWeekViewModel
        /// </summary>
        /// <param name="taskRepository">Repository för att hantera uppgifter</param>
        public TaskWeekViewModel(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _selectedDate = DateTime.Today;
            Tasks = new ObservableCollection<Task>();

            // Initiera kommandon
            AddTaskCommand = new RelayCommand(AddTask);
            EditTaskCommand = new RelayCommand<Task>(EditTask);
            DeleteTaskCommand = new RelayCommand<Task>(DeleteTask);
            ShowCommentsCommand = new RelayCommand<Task>(ShowComments);

            // Initialize with today's date
            _selectedDate = DateTime.Today;

            // Ladda uppgifter för aktuell dag
            LoadTasks(_selectedDate);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Uppdatera vald dag och ladda uppgifter
        /// </summary>
        /// <param name="date">Valt datum</param>
        public void UpdateSelectedDate(DateTime date)
        {
            _selectedDate = date;
            LoadTasks(_selectedDate);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Ladda uppgifter för valt datum
        /// </summary>
        /// <param name="date">Valt datum</param>
        private void LoadTasks(DateTime date)
        {
            var tasks = _taskRepository.GetTasksByDate(date);
            Tasks.Clear();

            foreach (var task in tasks)
            {
                task.CommentCount = task.Comments?.Count ?? 0;
                Tasks.Add(task);
            }

            HasTasks = Tasks.Count > 0;
        }

        /// <summary>
        /// Öppna dialog för att lägga till ny uppgift
        /// </summary>
        private void AddTask()
        {
            // Använd den nya konstruktorn som tar både användare och datum
            var dialog = new Views.Tasks.Components.Task.AddGeneralTask(App.CurrentUser, _selectedDate);

            if (dialog.ShowDialog() == true)
            {
                // Uppdatera uppgiftslistan
                LoadTasks(_selectedDate);
            }
        }

        /// <summary>
        /// Öppna dialog för att redigera en uppgift
        /// </summary>
        /// <param name="task">Uppgift att redigera</param>
        private void EditTask(Task task)
        {
            if (task == null)
                return;

            // Skapa en ny instans av dialogen med den inloggade användaren
            var dialog = new Views.Tasks.Components.Task.AddGeneralTask(App.CurrentUser);

            // Hämta dialog-ViewModel och konfigurera den för redigering
            var dialogViewModel = dialog.DataContext as TaskViewModel;
            if (dialogViewModel != null)
            {
                // Ladda in uppgift för redigering
                dialogViewModel.LoadTaskForEditing(task);
            }

            // Visa dialogen och uppdatera om användaren sparar
            if (dialog.ShowDialog() == true)
            {
                // Uppdatera uppgiftslistan
                LoadTasks(_selectedDate);
            }
        }

        /// <summary>
        /// Ta bort en uppgift
        /// </summary>
        /// <param name="task">Uppgift att ta bort</param>
        private void DeleteTask(Task task)
        {
            if (task == null)
                return;

            // Fråga användaren om bekräftelse
            var result = MessageBox.Show(
                $"Är du säker på att du vill ta bort uppgiften '{task.Title}'?",
                "Ta bort uppgift",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // Om användaren bekräftar, ta bort uppgiften
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _taskRepository.DeleteTask(task.Id);
                    Tasks.Remove(task);
                    HasTasks = Tasks.Count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Ett fel uppstod när uppgiften skulle tas bort: {ex.Message}",
                        "Fel",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Visa kommentarer för en uppgift
        /// </summary>
        /// <param name="task">Uppgift att visa kommentarer för</param>
        private void ShowComments(Task task)
        {
            if (task == null)
                return;

            // Ladda kommentarer för uppgiften
            LoadCommentsForTask(task.Id);

            // Utlös event för att meddela View att visa kommentarer
            ShowCommentsRequested?.Invoke(this, task);
        }

        private void LoadCommentsForTask(int taskId)
        {
            if (taskId <= 0)
                return;

            var comments = _taskRepository.GetCommentsForTask(taskId);
            CurrentTaskComments.Clear();

            foreach (var comment in comments)
            {
                CurrentTaskComments.Add(comment);
            }

            OnPropertyChanged(nameof(CurrentTaskComments));
        }


        #endregion
    }
}
