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
using Projektledningsverktyg.Widgets.WeekView;
using Projektledningsverktyg.Widgets;
using Projektledningsverktyg.Data.Entities;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    public partial class WeekView : UserControl
    {
        public WeekMonthViewModel ViewModel { get; set; }
        //private WeekViewLayoutManager _layoutManager;
        private TaskViewModel _commentsViewModel;
        private int _lastTaskId = -1;
        private WidgetPositionRepository _widgetRepository;

        // Reference till widgets
        private MealsWidget _mealsWidget;
        private ScheduleWidget _scheduleWidget;
        private HouseholdWidget _householdWidget;
        private GeneralTaskWidget _generalTaskWidget;
        private EventWidget _eventWidget;

        public WeekView()
        {
            InitializeComponent();

            // Skapa widget position repository
            _widgetRepository = new WidgetPositionRepository();

            // Initialize repositories
            var context = new ApplicationDbContext();

            // Initialize view model
            ViewModel = new WeekMonthViewModel();
            DataContext = ViewModel;

            // Subscribe to day change events
            ViewModel.OnSelectedDayChanged += ViewModel_OnSelectedDayChanged;

            Loaded += WeekView_Loaded;
        }

        // This method will be called when the selected day changes
        private void ViewModel_OnSelectedDayChanged(object sender, DateTime selectedDate)
        {
            // Uppdatera widgets med den valda dagens data
            if (_scheduleWidget != null)
            {
                (_scheduleWidget.Content as ScheduleControl)?.UpdateSelectedDate(selectedDate);
            }

            if (_householdWidget != null)
            {
                (_householdWidget.Content as HouseholdControl)?.UpdateSelectedDate(selectedDate);
            }

            if (_eventWidget != null)
            {
                (_eventWidget.Content as EventsControl)?.UpdateSelectedDate(selectedDate);
            }

            if (_generalTaskWidget != null)
            {
                (_generalTaskWidget.Content as GeneralTaskControl)?.UpdateSelectedDate(selectedDate);
            }

            if (_mealsWidget != null)
            {
                (_mealsWidget.Content as MealsControl)?.UpdateSelectedDate(selectedDate);
            }
        }

        private void WeekView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeWidgets();

            // Anslut till händelse för att visa kommentarer
            // Hitta GeneralTaskControl direkt från _generalTaskWidget.Content
            if (_generalTaskWidget != null)
            {
                var generalTaskControl = _generalTaskWidget.Content as GeneralTaskControl;
                if (generalTaskControl != null)
                {
                    generalTaskControl.ShowTaskCommentsRequested += GeneralTaskControl_ShowTaskCommentsRequested;
                }
            }

            // Skapa och konfigurera kommentar-ViewModel
            _commentsViewModel = new TaskViewModel(new ApplicationDbContext(), App.CurrentUser);
            taskComments.DataContext = _commentsViewModel;

            // Anslut till CloseRequested från taskComments
            taskComments.CloseRequested += TaskComments_CloseRequested;
            this.Unloaded += WeekView_Unloaded;
            _commentsViewModel.CommentCountChanged += UpdateTaskCommentCount;
        }

        private void InitializeWidgets()
        {
            // Återställ containerns layout till 3x2
            widgetContainer.ResetLayout(3, 2);

            // Rensa container först
            widgetContainer.Children.Clear();

            // Ladda sparade positioner från databasen
            var savedPositions = _widgetRepository.GetWidgetPositionsForMember(App.CurrentUser?.Id ?? 0);

            // Skapa lookup för snabb åtkomst till positioner
            var positionLookup = savedPositions.ToDictionary(p => p.WidgetId);

            // Skapa alla widgets
            _mealsWidget = new MealsWidget();
            _scheduleWidget = new ScheduleWidget();
            _householdWidget = new HouseholdWidget();
            _generalTaskWidget = new GeneralTaskWidget();
            _eventWidget = new EventWidget();

            // Registrera widgets med rätt positioner - använd sparade positioner om tillgängliga
            RegisterWidgetWithPosition(_mealsWidget, 0, 0, positionLookup);
            RegisterWidgetWithPosition(_scheduleWidget, 0, 1, positionLookup);
            RegisterWidgetWithPosition(_householdWidget, 1, 0, positionLookup);
            RegisterWidgetWithPosition(_generalTaskWidget, 1, 1, positionLookup);
            RegisterWidgetWithPosition(_eventWidget, 2, 0, positionLookup);

            // Anslut till positionsändring-eventet för att spara ändringar
            widgetContainer.PositionsChanged += WidgetContainer_PositionsChanged;
        }

        private void WidgetContainer_PositionsChanged(object sender, EventArgs e)
        {
            // Spara de uppdaterade positionerna
            if (App.CurrentUser?.Id > 0)
            {
                var newPositions = widgetContainer.GetCurrentPositions(App.CurrentUser.Id);
                _widgetRepository.SaveWidgetPositions(newPositions);
            }
        }

        private void WeekView_Unloaded(object sender, RoutedEventArgs e)
        {
            // Koppla bort händelser
            if (_generalTaskWidget != null)
            {
                var generalTaskControl = _generalTaskWidget.Content as GeneralTaskControl;
                if (generalTaskControl != null)
                {
                    generalTaskControl.ShowTaskCommentsRequested -= GeneralTaskControl_ShowTaskCommentsRequested;
                }
            }

            if (taskComments != null)
            {
                taskComments.CloseRequested -= TaskComments_CloseRequested;
            }

            // Avregistrera från PositionsChanged event
            if (widgetContainer != null)
            {
                widgetContainer.PositionsChanged -= WidgetContainer_PositionsChanged;
            }

            // Städa upp resurser
            if (_commentsViewModel != null)
            {
                _commentsViewModel.CommentCountChanged -= UpdateTaskCommentCount;
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
            var generalTaskControl = _generalTaskWidget.Content as GeneralTaskControl;
            if (generalTaskControl != null)
            {
                var taskWeekViewModel = generalTaskControl.DataContext as TaskWeekViewModel;

                if (taskWeekViewModel != null && _commentsViewModel.CurrentTask != null)
                {
                    int commentCount = _commentsViewModel.CurrentTaskComments?.Count ?? 0;
                    taskWeekViewModel.UpdateCommentCount(_commentsViewModel.CurrentTask.Id, commentCount);
                }
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
            var generalTaskControl = _generalTaskWidget.Content as GeneralTaskControl;
            if (generalTaskControl != null)
            {
                var taskWeekViewModel = generalTaskControl.DataContext as TaskWeekViewModel;
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
        }
        #endregion

        #region Helper Method
        // Hjälpmetod för att registrera widgets med sparade positioner
        private void RegisterWidgetWithPosition(DraggableWidget widget, int defaultRow, int defaultCol,
                                                Dictionary<string, WidgetPosition> positionLookup)
        {
            const int MAX_ROWS = 3;
            const int MAX_COLUMNS = 2;

            int row = defaultRow;
            int col = defaultCol;

            // Om vi har en sparad position för denna widget, använd den
            if (positionLookup.TryGetValue(widget.WidgetId, out var position))
            {
                // Begränsa positioner till giltigt område
                row = Math.Min(position.RowIndex, MAX_ROWS - 1);
                col = Math.Min(position.ColumnIndex, MAX_COLUMNS - 1);

                // Tillämpa synlighet om den är sparad
                widget.Visibility = position.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            // Registrera widget med rätt position
            widgetContainer.RegisterWidget(widget, row, col);
        }
        #endregion
    }
}
