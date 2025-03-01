using Projektledningsverktyg.Data.Context;
using System.Collections.Generic;
using System;
using Projektledningsverktyg.Data.Entities;
using System.Data.Entity;
using System.Linq;
using System.Diagnostics;

namespace Projektledningsverktyg.Data.Repository
{
    public class TaskRepository : ITaskRepository
    {
        #region Private Fields
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructor
        /// <summary>
        /// Konstruktor för TaskRepository
        /// </summary>
        /// <param name="context">Databaskontexten</param>
        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Hämta uppgifter för ett specifikt datum
        /// </summary>
        /// <param name="date">Datum att hämta uppgifter för</param>
        /// <returns>Lista med uppgifter</returns>
        public IEnumerable<Task> GetTasksByDate(DateTime date)
        {
            // Förenklad version utan ThenInclude
            return _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .Where(t => t.DueDate.Date == date.Date)
                .ToList();
        }

        /// <summary>
        /// Hämta en uppgift med ID
        /// </summary>
        /// <param name="id">Uppgiftens ID</param>
        /// <returns>Uppgiften eller null</returns>
        public Task GetTaskById(int id)
        {
            // Förenklad version utan ThenInclude
            return _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Lägg till en ny uppgift
        /// </summary>
        /// <param name="task">Uppgift att lägga till</param>
        public void AddTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        /// <summary>
        /// Uppdatera en befintlig uppgift
        /// </summary>
        /// <param name="task">Uppgift att uppdatera</param>
        public void UpdateTask(Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Ta bort en uppgift med ID
        /// </summary>
        /// <param name="id">Uppgiftens ID</param>
        public void DeleteTask(int id)
        {
            // Först tar vi bort alla kommentarer kopplade till uppgiften
            var taskComments = _context.Comments
                .Where(c => c.TaskId == id)
                .ToList();

            if (taskComments.Any())
            {
                _context.Comments.RemoveRange(taskComments);
            }

            // Sedan tar vi bort själva uppgiften
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            // Spara ändringarna till databasen
            _context.SaveChanges();
        }

        IEnumerable<Task> ITaskRepository.GetTasksByDate(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            // Hämta uppgifter med rätt datum
            var tasks = _context.Tasks
                .AsNoTracking()  // För bättre prestanda
                .Include(t => t.AssignedTo)
                .Where(t => t.DueDate >= startOfDay && t.DueDate <= endOfDay)
                .ToList();

            // För varje uppgift, ladda kommentarerna separat
            foreach (var task in tasks)
            {
                // Explicit ladda kommentarerna för denna uppgift
                var comments = _context.Comments
                    .AsNoTracking()
                    .Where(c => c.TaskId == task.Id)
                    .ToList();

                // Tilldela kommentarerna till uppgiften
                task.Comments = comments;
            }

            return tasks;
        }

        Task ITaskRepository.GetTaskById(int id)
        {
            return _context.Tasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == id);
        }

        public List<Comment> GetCommentsForTask(int taskId)
        {
            return _context.Comments
                .AsNoTracking()
                .Include(c => c.Member)
                .Where(c => c.TaskId == taskId)
                .ToList();
        }

        #endregion
    }
}
