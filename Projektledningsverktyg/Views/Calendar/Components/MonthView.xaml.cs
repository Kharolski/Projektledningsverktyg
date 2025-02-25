using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.ViewModels.Calendar;
using Projektledningsverktyg.Views.Calendar.Components.MonthComponents;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    public partial class MonthView : UserControl
    {
        private readonly ScheduleRepository _scheduleRepository;

        #region Fields
        private DateTime currentDate = DateTime.Now;
        #endregion

        #region Constructor
        public MonthView()
        {
            InitializeComponent();
            var context = new ApplicationDbContext();
            _scheduleRepository = new ScheduleRepository(context);
            var viewModel = new CalendarViewModel(_scheduleRepository);
            viewModel.OnCalendarRefreshNeeded += GenerateCalendar;
            DataContext = viewModel;
            GenerateCalendar();
        }
        #endregion

        #region Calendar Generation
        private void GenerateCalendar()
        {
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            MonthYearText.Text = currentDate.ToString("MMMM yyyy");
            CalendarGrid.Children.Clear();

            int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            for (int i = 0; i < offset; i++)
            {
                CalendarGrid.Children.Add(CreateEmptyDay());
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                CalendarGrid.Children.Add(CreateDayCell(day));
            }

            int remainingCells = 42 - (offset + daysInMonth);
            for (int i = 0; i < remainingCells; i++)
            {
                CalendarGrid.Children.Add(CreateEmptyDay());
            }
        }

        private UIElement CreateDayCell(int day)
        {
            // Create border for the cell
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(1)
            };

            // Create date for the current cell
            var dayDate = new DateTime(currentDate.Year, currentDate.Month, day);
            var dayCell = new DayCell();
            dayCell.SetDay(dayDate);

            // Highlight current day with different background
            if (day == DateTime.Now.Day && currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
            {
                dayCell.Background = new SolidColorBrush(Color.FromRgb(179, 229, 252));
            }
            else
            {
                dayCell.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            }

            // Load and display schedule indicators for this day
            var schedules = _scheduleRepository.GetSchedulesByDate(dayDate);
            dayCell.UpdateIndicators(schedules);

            // Set up the cell and click handling
            border.Child = dayCell;
            border.MouseLeftButtonDown += (s, e) => DayCell_Click(dayDate);

            return border;
        }

        private UIElement CreateEmptyDay()
        {
            return new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(250, 250, 250))
            };
        }
        #endregion

        #region Navigation Events
        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            GenerateCalendar();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            GenerateCalendar();
        }
        #endregion

        #region Context Menu Events
        private void DayCell_Click(DateTime selectedDate)
        {
            var contextMenu = (ContextMenu)FindResource("DayOptionsMenu");
            contextMenu.Tag = selectedDate;
            contextMenu.IsOpen = true;
        }

        private void AddWorkSchedule_Click(object sender, RoutedEventArgs e)
        {
            var clickedDate = GetSelectedDateFromMenuItem(sender);
            var calendarViewModel = (CalendarViewModel)DataContext;

            calendarViewModel.ShowAddForm(clickedDate);
        }

        private void AddDeviation_Click(object sender, RoutedEventArgs e)
        {
            var clickedDate = GetSelectedDateFromMenuItem(sender);
            var calendarViewModel = (CalendarViewModel)DataContext;

            calendarViewModel.ShowAddForm(clickedDate);
        }

        private void ViewDaySchedule_Click(object sender, RoutedEventArgs e)
        {
            var selectedDate = GetSelectedDateFromMenuItem(sender);
            ShowDaySchedules(selectedDate);
        }
        #endregion

        #region Helper Methods
        public void RefreshCalendar()
        {
            GenerateCalendar();
        }
        private DateTime GetSelectedDateFromMenuItem(object sender)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            return (DateTime)contextMenu.Tag;
        }

        #endregion

        #region Dialog Methods

        private void ShowDaySchedules(DateTime selectedDate)
        {
            var schedules = _scheduleRepository.GetSchedulesByDate(selectedDate);
            var scheduleListView = new ScheduleListView(_scheduleRepository)
            {
                Date = selectedDate,
                WorkSchoolSchedules = schedules.Where(s => s.Type == ScheduleType.WorkSchool),
                DeviationSchedules = schedules.Where(s => s.Type == ScheduleType.Deviation)
            };
            scheduleListView.OnScheduleDeleted += GenerateCalendar;

            var window = new ScheduleWindow(scheduleListView);

            window.Show();
        }
        #endregion
    }
}
