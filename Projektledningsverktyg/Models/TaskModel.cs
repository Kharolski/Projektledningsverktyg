using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using Projektledningsverktyg.Extensions;

namespace Projektledningsverktyg.Models
{

    public class TaskModel : INotifyPropertyChanged
    {
        private string _title;
        private string _description;
        private DateTime _dueDate;
        private bool _isCompleted;
        private TaskPriority _priority;
        private int? _assignedToMemberId;
        private TaskStatus _status;
        private int _projectId;
        private int _memberId;

        private readonly ApplicationDbContext _context;

        #region Constructors

        public TaskModel()
        {
            AddCommentCommand = new RelayCommand<string>(ExecuteAddComment);
            CurrentTaskComments = new ObservableCollection<Comment>();
        }

        public TaskModel(ApplicationDbContext context)
        {
            _context = context;
            AddCommentCommand = new RelayCommand<string>(ExecuteAddComment);
            SaveDescriptionCommand = new RelayCommand<string>(ExecuteSaveDescription);
            CurrentTaskComments = new ObservableCollection<Comment>();
        }

        #endregion

        #region Property

        public int Id { get; set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public DateTime DueDate
        {
            get => _dueDate;
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
            }
        }

        public TaskPriority Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        public int? AssignedToMemberId
        {
            get => _assignedToMemberId;
            set
            {
                _assignedToMemberId = value;
                OnPropertyChanged(nameof(AssignedToMemberId));
            }
        }

        public TaskStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public int ProjectId
        {
            get => _projectId;
            set
            {
                _projectId = value;
                OnPropertyChanged(nameof(ProjectId));
            }
        }

        public int MemberId
        {
            get => _memberId;
            set
            {
                _memberId = value;
                OnPropertyChanged(nameof(MemberId));
            }
        }

        // property to show enum extention
        public string PriorityDisplay => Priority.GetDisplayName();
        public string StatusDisplay => Status.GetDisplayName();


        public ICommand AddCommentCommand { get; private set; }
        public ICommand SaveDescriptionCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Load

        private ObservableCollection<Comment> _currentTaskComments;
        public ObservableCollection<Comment> CurrentTaskComments
        {
            get => _currentTaskComments ?? (_currentTaskComments = new ObservableCollection<Comment>());
            set
            {
                _currentTaskComments = value;
                OnPropertyChanged(nameof(CurrentTaskComments));
            }
        }


        #endregion

        #region Comments

        private async void ExecuteAddComment(object parameter)
        {
            if (parameter is string commentText && !string.IsNullOrEmpty(commentText))
            {
                var newComment = new Comment
                {
                    Content = commentText,
                    CreatedAt = DateTime.Now,
                    TaskId = this.Id,
                    MemberId = App.CurrentUser.Id,
                    Type = CommentType.Task
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                // Reload the comment with Member navigation property
                var savedComment = _context.Comments
                        .Include(c => c.Member)
                        .FirstOrDefault(c => c.Id == newComment.Id);

                CurrentTaskComments.Add(savedComment);
            }
        }

        #endregion

        #region Description
        private void ExecuteSaveDescription(string description)
        {
            var task = _context.Tasks.Find(Id);
            if (task != null)
            {
                task.Description = description;
                _context.SaveChanges();
                Description = description;  // Update the local property
                OnPropertyChanged(nameof(Description));
            }
        }

        #endregion




    }
}
