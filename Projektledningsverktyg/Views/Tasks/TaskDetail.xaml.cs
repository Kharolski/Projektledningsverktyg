using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Linq;
using Projektledningsverktyg.Data.Entities;

namespace Projektledningsverktyg.Views.Tasks
{
    public partial class TaskDetail : Window
    {
        public static readonly int CurrentUserId = App.CurrentUser.Id;

        private readonly ApplicationDbContext _context;
        public TaskDetail(TaskModel task)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();

            // Get fresh task data
            var freshTask = _context.Tasks
                .FirstOrDefault(t => t.Id == task.Id);

            var taskData = new TaskModel(_context)
            {
                Id = freshTask.Id,
                Title = freshTask.Title,
                Description = freshTask.Description,
                DueDate = freshTask.DueDate,
                IsCompleted = freshTask.Status == TaskStatus.Completed,
                Priority = freshTask.Priority,
                Status = freshTask.Status
            };

            // Load existing comments
            var existingComments = _context.Comments
                .Include(c => c.Member)
                .Where(c => c.TaskId == task.Id)
                .ToList()
                .Select(c => new Comment
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    MemberId = c.MemberId, 
                    Member = new Member
                    {
                        Id = c.Member.Id,
                        FirstName = c.Member.FirstName,
                        ProfileImagePath = c.Member.ProfileImagePath
                    }
                })
                .ToList();

            foreach (var comment in existingComments)
            {
                taskData.CurrentTaskComments.Add(comment);
            }

            DataContext = taskData;
        }
    }
}
