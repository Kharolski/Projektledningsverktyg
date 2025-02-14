using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Projektledningsverktyg.Data.Repository
{
    public class HouseholdRepository
    {
        private readonly ApplicationDbContext _context;

        public HouseholdRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all household tasks with their assigned members
        public List<Household> GetAllHouseholdTasks()
        {
            _context.Database.Connection.Close();
            _context.Database.Connection.Open();

            return _context.Households
                .Include(h => h.Assignments)
                .ThenInclude(a => a.Member)
                .AsNoTracking()
                .ToList();
        }

        // Add new household task
        public void AddHouseholdTask(Household task)
        {
            _context.Households.Add(task);
            _context.SaveChanges();
        }

        // Delete household task
        public void DeleteHouseholdTask(int taskId)
        {
            var task = _context.Households
                .Include(h => h.Assignments)
                .FirstOrDefault(h => h.Id == taskId);

            if (task != null)
            {
                foreach (var assignment in task.Assignments.ToList())
                {
                    _context.HouseholdAssignments.Remove(assignment);
                }
                _context.Households.Remove(task);
                _context.SaveChanges();
            }
        }

        public void AddHouseholdAssignment(HouseholdAssignment assignment)
        {
            _context.HouseholdAssignments.Add(assignment);
            _context.SaveChanges();
        }

        public void UpdateHouseholdAssignment(HouseholdAssignment assignment)
        {
            var existingAssignment = _context.HouseholdAssignments.Find(assignment.Id);
            if (existingAssignment != null)
            {
                existingAssignment.AssignedDays = assignment.AssignedDays;
                _context.SaveChanges();
            }
        }

        public void RemoveHouseholdAssignment(int householdId, int memberId)
        {
            _context.ChangeTracker.Entries().ToList().ForEach(e => e.State = System.Data.Entity.EntityState.Detached);

            var assignment = _context.HouseholdAssignments
                .FirstOrDefault(ha => ha.HouseholdId == householdId && ha.MemberId == memberId);

            if (assignment != null)
            {
                _context.HouseholdAssignments.Remove(assignment);
                _context.SaveChanges();
            }
        }

        // Get all available members
        public List<Member> GetAllMembers()
        {
            return _context.Members
                .Where(m => m.IsActive)
                .ToList();
        }
    }
}
