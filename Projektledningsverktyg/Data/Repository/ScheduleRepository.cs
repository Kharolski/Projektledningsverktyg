using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Projektledningsverktyg.Data.Repository
{
    public class ScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _context.Schedules.ToList();
        }

        public IEnumerable<Schedule> GetSchedulesByDate(DateTime date)
        {
            // Only get schedules that START on this date (for time-based schedules)
            var schedulesFromTime = _context.Schedules
                .Where(s => s.StartTime.HasValue &&
                           s.StartTime.Value.Year == date.Year &&
                           s.StartTime.Value.Month == date.Month &&
                           s.StartTime.Value.Day == date.Day)
                .ToList();

            // Get schedules from MonthView (for schedules without specific times)
            var schedulesFromMonthView = _context.MonthViews
                .Where(mv => mv.Month.Year == date.Year &&
                             mv.Month.Month == date.Month &&
                             mv.Month.Day == date.Day)
                .Select(mv => mv.Schedule)
                .ToList();

            return schedulesFromTime.Union(schedulesFromMonthView);
        }

        public void AddScheduleWithDate(Schedule schedule, DateTime date)
        {
            // First save the schedule
            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            // Create MonthView entry
            var monthView = new MonthView
            {
                Month = date,
                ScheduleId = schedule.Id,
                Schedule = schedule
            };

            _context.MonthViews.Add(monthView);
            _context.SaveChanges();
        }


        public void UpdateSchedule(Schedule schedule)
        {
            _context.Entry(schedule).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteSchedule(int scheduleId)
        {
            // Delete from MonthView table first (foreign key relationship)
            var monthViewEntries = _context.MonthViews.Where(mv => mv.ScheduleId == scheduleId);
            _context.MonthViews.RemoveRange(monthViewEntries);

            // Delete the schedule
            var schedule = _context.Schedules.Find(scheduleId);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            _context.SaveChanges();
        }
    }
}
