using Projektledningsverktyg.Data.Entities;
using System;

namespace Projektledningsverktyg.Services
{
    public class ScheduleService
    {
        public bool IsNightShift(Schedule schedule)
        {
            if (!schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
                return false;

            return schedule.StartTime.Value.Date != schedule.EndTime.Value.Date;
        }

        public (DateTime start, DateTime end) GetNightShiftDates(Schedule schedule)
        {
            // For shifts like 21:45-07:15
            return (schedule.StartTime.Value, schedule.EndTime.Value);
        }

        public string FormatScheduleDisplay(Schedule schedule)
        {
            if (!schedule.StartTime.HasValue || !schedule.EndTime.HasValue)
                return schedule.Title;

            return $"{schedule.Title} {schedule.StartTime.Value:HH:mm}-{schedule.EndTime.Value:HH:mm}";
        }
    }
}
