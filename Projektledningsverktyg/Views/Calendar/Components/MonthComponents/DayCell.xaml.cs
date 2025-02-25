using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Calendar.Components.MonthComponents
{
    public partial class DayCell : UserControl
    {
        #region Properties
        public static readonly DependencyProperty HasWorkScheduleProperty =
            DependencyProperty.Register("HasWorkSchedule", typeof(bool), typeof(DayCell));

        public static readonly DependencyProperty HasNightShiftProperty =
            DependencyProperty.Register("HasNightShift", typeof(bool), typeof(DayCell));

        public static readonly DependencyProperty HasDeviationProperty =
            DependencyProperty.Register("HasDeviation", typeof(bool), typeof(DayCell));
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(DayCell));

        public DateTime Date
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public bool HasWorkSchedule
        {
            get => (bool)GetValue(HasWorkScheduleProperty);
            set => SetValue(HasWorkScheduleProperty, value);
        }

        public bool HasNightShift
        {
            get => (bool)GetValue(HasNightShiftProperty);
            set => SetValue(HasNightShiftProperty, value);
        }

        public bool HasDeviation
        {
            get => (bool)GetValue(HasDeviationProperty);
            set => SetValue(HasDeviationProperty, value);
        }
        #endregion

        public DayCell()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Methods
        public void UpdateIndicators(IEnumerable<Schedule> schedules)
        {
            // Only set indicators if we have schedules for this day
            if (schedules != null && schedules.Any())
            {
                var workSchedules = schedules.Where(s => s.Type == ScheduleType.WorkSchool);

                // Show work dot for regular and null time schedules
                HasWorkSchedule = workSchedules.Any(s =>
                    (s.StartTime?.Date == Date && !IsNightShift(s)) ||
                    (s.StartTime == null && s.EndTime == null));

                // Show night shift indicator ONLY on the start date
                HasNightShift = workSchedules.Any(s =>
                    s.StartTime?.Date == Date && IsNightShift(s));

                HasDeviation = schedules.Any(s => s.Type == ScheduleType.Deviation);
            }
            else
            {
                HasWorkSchedule = false;
                HasNightShift = false;
                HasDeviation = false;
            }
        }

        private bool IsNightShift(Schedule schedule)
        {
            return schedule.StartTime.HasValue && schedule.EndTime.HasValue &&
                   schedule.StartTime.Value.Date == Date && // Only check start date
                   schedule.StartTime.Value.TimeOfDay > schedule.EndTime.Value.TimeOfDay;
        }

        public void SetDay(DateTime date)
        {
            Date = date;
        }
        #endregion
    }
}
