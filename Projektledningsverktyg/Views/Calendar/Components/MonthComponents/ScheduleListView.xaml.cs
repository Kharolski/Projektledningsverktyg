using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.ViewModels.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projektledningsverktyg.Views.Calendar.Components.MonthComponents
{
    public partial class ScheduleListView : UserControl
    {
        private readonly ScheduleRepository _scheduleRepository;

        #region Properties
        public DateTime Date { get; set; }
        public event Action OnScheduleDeleted;

        public static readonly DependencyProperty WorkSchoolSchedulesProperty =
            DependencyProperty.Register("WorkSchoolSchedules", typeof(IEnumerable<Schedule>), typeof(ScheduleListView));

        public static readonly DependencyProperty DeviationSchedulesProperty =
            DependencyProperty.Register("DeviationSchedules", typeof(IEnumerable<Schedule>), typeof(ScheduleListView));

        public IEnumerable<Schedule> WorkSchoolSchedules
        {
            get => (IEnumerable<Schedule>)GetValue(WorkSchoolSchedulesProperty);
            set => SetValue(WorkSchoolSchedulesProperty, value);
        }

        public IEnumerable<Schedule> DeviationSchedules
        {
            get => (IEnumerable<Schedule>)GetValue(DeviationSchedulesProperty);
            set => SetValue(DeviationSchedulesProperty, value);
        }
        #endregion

        public ScheduleListView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ScheduleListView(ScheduleRepository repository)
        {
            InitializeComponent();
            _scheduleRepository = repository;
            DataContext = this;
        }

        #region Methods
        private void EditSchedule_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var schedule = (Schedule)button.DataContext;

            var viewModel = new AddEditScheduleViewModel(schedule, Date);
            viewModel.OnSave += (updatedSchedule) =>
            {
                _scheduleRepository.UpdateSchedule(updatedSchedule);
                RefreshSchedules();
                OnScheduleDeleted?.Invoke();
            };
            viewModel.OnClose += () =>
            {
                var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                currentWindow?.Close();
            };

            var editView = new AddEditScheduleView { DataContext = viewModel };
            var editWindow = new Window
            {
                Content = editView,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            editWindow.ShowDialog();
        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var schedule = (Schedule)button.DataContext;
            _scheduleRepository.DeleteSchedule(schedule.Id);

            // Refresh the list
            RefreshSchedules();
            OnScheduleDeleted?.Invoke();
        }

        private void RefreshSchedules()
        {
            var allSchedules = _scheduleRepository.GetSchedulesByDate(Date);

            // Filter to only show schedules that start on this date or are all-day events
            var relevantSchedules = allSchedules.Where(s =>
                s.StartTime?.Date == Date ||
                (s.StartTime == null && s.EndTime == null));

            WorkSchoolSchedules = relevantSchedules.Where(s => s.Type == ScheduleType.WorkSchool);
            DeviationSchedules = relevantSchedules.Where(s => s.Type == ScheduleType.Deviation);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.Close();
        }
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window?.DragMove();
        }
        #endregion
    }
}
