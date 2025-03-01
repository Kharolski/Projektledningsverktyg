using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;

namespace Projektledningsverktyg.Data.Repository
{
    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasksByDate(DateTime date);
        Task GetTaskById(int id);
        void AddTask(Task task);
        void UpdateTask(Task task);
        void DeleteTask(int id);

        List<Comment> GetCommentsForTask(int taskId);
    }
}
