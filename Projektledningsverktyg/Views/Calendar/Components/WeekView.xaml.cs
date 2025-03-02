using Projektledningsverktyg.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Projektledningsverktyg.Helpers;
using System;
using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Repository;
using System.Diagnostics;
using Projektledningsverktyg.ViewModels.Calendar.WeekModels;
using System.Windows.Media.Animation;
using System.Linq;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    public partial class WeekView : UserControl
    {
        public WeekMonthViewModel ViewModel { get; set; }
        private WeekViewLayoutManager _layoutManager;
        private TaskViewModel _commentsViewModel;
        private int _lastTaskId = -1;

        public WeekView()
        {
            InitializeComponent();

            // Initialize layout manager early
            _layoutManager = new WeekViewLayoutManager(null); // Will be properly set in Loaded event

            // Initialize repositories
            var context = new ApplicationDbContext();
            //_scheduleRepository = new ScheduleRepository(context);

            // Initialize view model
            ViewModel = new WeekMonthViewModel();
            DataContext = ViewModel;

            // Subscribe to day change events
            ViewModel.OnSelectedDayChanged += ViewModel_OnSelectedDayChanged;

            Loaded += WeekView_Loaded;
            RegisterContentChangeHandlers();
        }

        // This method will be called when the selected day changes
        private void ViewModel_OnSelectedDayChanged(object sender, DateTime selectedDate)
        {
            // Uppdatera ScheduleControl med den valda dagens data
            var scheduleControl = FindName("ScheduleControl") as ScheduleControl;
            if (scheduleControl != null)
            {
                scheduleControl.UpdateSelectedDate(selectedDate);
            }

            // Uppdatera HouseholdControl med den valda dagens data
            var householdControl = FindName("HouseholdControl") as HouseholdControl;
            if (householdControl != null)
            {
                householdControl.UpdateSelectedDate(selectedDate);
            }

            // Uppdatera EventsControl med den valda dagens data
            var eventsControl = FindName("EventsControl") as EventsControl;
            if (eventsControl != null)
            {
                eventsControl.UpdateSelectedDate(selectedDate);
            }

            var generalTaskControl = FindName("GeneralTaskControl") as GeneralTaskControl;
            if (generalTaskControl != null)
            {
                generalTaskControl.UpdateSelectedDate(selectedDate);
            }

            // Uppdatera MealsControl med den valda dagens data
            var mealsControl = FindName("MealsControl") as MealsControl;
            if (mealsControl != null)
            {
                mealsControl.UpdateSelectedDate(selectedDate);
            }
        }

        private void WeekView_Loaded(object sender, RoutedEventArgs e)
        {
            _layoutManager = new WeekViewLayoutManager(MainCanvas);

            // Register controls with specific keys
            _layoutManager.RegisterControl("Schedule", FindName("ScheduleControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Meals", FindName("MealsControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Household", FindName("HouseholdControl") as DraggableControlBase);
            _layoutManager.RegisterControl("GeneralTask", FindName("GeneralTaskControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Events", FindName("EventsControl") as DraggableControlBase);

            _layoutManager.UpdateLayout();

            // Check initial content height
            MainCanvas.UpdateLayout();
            MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();

            // Monitor all controls for size changes
            foreach (var control in _layoutManager.GetAllControls())
            {
                control.SizeChanged += (s, args) =>
                {
                    MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();
                };
            }

            // Anslut till händelse för att visa kommentarer
            GeneralTaskControl.ShowTaskCommentsRequested += GeneralTaskControl_ShowTaskCommentsRequested;

            // Skapa och konfigurera kommentar-ViewModel
            _commentsViewModel = new TaskViewModel(new ApplicationDbContext(), App.CurrentUser);
            taskComments.DataContext = _commentsViewModel;

            // Anslut till CloseRequested från taskComments
            taskComments.CloseRequested += TaskComments_CloseRequested;
            this.Unloaded += WeekView_Unloaded;
            _commentsViewModel.CommentCountChanged += UpdateTaskCommentCount;
        }

        private void WeekView_Unloaded(object sender, RoutedEventArgs e)
        {
            // Koppla bort händelser
            if (GeneralTaskControl != null)
            {
                GeneralTaskControl.ShowTaskCommentsRequested -= GeneralTaskControl_ShowTaskCommentsRequested;
            }

            if (taskComments != null)
            {
                taskComments.CloseRequested -= TaskComments_CloseRequested;
            }

            // Städa upp resurser
            if (_commentsViewModel != null)
            {
                // Avregistrera CommentCountChanged - detta saknas!
                _commentsViewModel.CommentCountChanged -= UpdateTaskCommentCount;

                // Om TaskViewModel implementerar IDisposable, anropa Dispose här
                (_commentsViewModel as IDisposable)?.Dispose();
            }

            // Avregistrera från Unloaded-eventet
            this.Unloaded -= WeekView_Unloaded;
        }

        // Keep your existing FindVisualChildren helper method
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;
                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        private void RegisterContentChangeHandlers()
        {
            // This method might be called before controls are loaded
            // We'll add guards to prevent null reference exceptions

            var eventsControl = FindName("EventsControl") as EventsControl;
            var mealsControl = FindName("MealsControl") as MealsControl;
            var householdControl = FindName("HouseholdControl") as HouseholdControl;
            var generalTaskControl = FindName("GeneralTaskControl") as GeneralTaskControl;

            if (eventsControl != null)
                eventsControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (mealsControl != null)
                mealsControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (householdControl != null)
                householdControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (generalTaskControl != null)
                generalTaskControl.ContentSizeChanged += Control_ContentSizeChanged;
        }

        private void Control_ContentSizeChanged(object sender, EventArgs e)
        {
            // Add null check to prevent NullReferenceException
            if (_layoutManager != null)
            {
                MainCanvas.UpdateLayout();
                MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();
            }
        }

        #region Comments section
        private void GeneralTaskControl_ShowTaskCommentsRequested(object sender, Data.Entities.Task task)
        {
            // Ignorera upprepade anrop för samma uppgift
            if (_lastTaskId == task.Id)
            {
                return;
            }

            _lastTaskId = task.Id;

            // Rensa flaggan efter en kort tid
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (s, e) => {
                _lastTaskId = -1;
                timer.Stop();
            };
            timer.Start();

            // Konvertera Task till TaskModel för TaskViewModel
            _commentsViewModel.CurrentTask = new Models.TaskModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate
                // Lägg till andra egenskaper som behövs...
            };

            // Ladda kommentarer explicit
            _commentsViewModel.LoadComments();

            // Sätt DataContext
            taskComments.DataContext = _commentsViewModel;
            taskComments.UpdateComments(_commentsViewModel.CurrentTaskComments);

            // Förbered panelen innan den visas
            SlideTransform.X = 500; // Sätt startposition utanför skärmen

            // Visa panelen
            CommentsPanel.Visibility = Visibility.Visible;

            // Låt layoutsystemet uppdatera först
            CommentsPanel.UpdateLayout();

            // Animera panelen in
            var slideAnimation = new DoubleAnimation
            {
                From = 500,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
        }

        private void TaskComments_CloseRequested(object sender, EventArgs e)
        {
            // Uppdatera kommentarantalet i ViewModel
            var taskWeekViewModel = GeneralTaskControl.DataContext as TaskWeekViewModel;

            if (taskWeekViewModel != null && _commentsViewModel.CurrentTask != null)
            {
                int commentCount = _commentsViewModel.CurrentTaskComments?.Count ?? 0;
                taskWeekViewModel.UpdateCommentCount(_commentsViewModel.CurrentTask.Id, commentCount);
            }

            // Animera panelen ut
            var slideAnimation = new DoubleAnimation
            {
                From = 0,
                To = 500,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            slideAnimation.Completed += (s, args) =>
            {
                CommentsPanel.Visibility = Visibility.Collapsed;
            };

            SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
        }

        private void UpdateTaskCommentCount(int taskId, int newCount)
        {
            var taskWeekViewModel = GeneralTaskControl.DataContext as TaskWeekViewModel;
            if (taskWeekViewModel != null)
            {
                var task = taskWeekViewModel.Tasks.FirstOrDefault(t => t.Id == taskId);
                if (task != null)
                {
                    task.CommentCount = newCount;

                    // Ta en kopia av uppgiften
                    var taskCopy = task;
                    // Ta bort den
                    taskWeekViewModel.Tasks.Remove(task);
                    // Lägg till den igen
                    taskWeekViewModel.Tasks.Add(taskCopy);
                }
            }
        }
        #endregion
    }
}
