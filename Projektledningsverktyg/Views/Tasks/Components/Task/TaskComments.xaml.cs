using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Models;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Projektledningsverktyg.Data.Entities;
using System.Collections.ObjectModel;
using Projektledningsverktyg.ViewModels;

namespace Projektledningsverktyg.Views.Tasks.Components.Task
{
    public partial class TaskComments : UserControl
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskModel _currentTask;

        public ObservableCollection<Comment> Comments
        {
            get => commentsListBox.ItemsSource as ObservableCollection<Comment>;
            set => commentsListBox.ItemsSource = value;
        }
        public void UpdateComments(ObservableCollection<Comment> comments)
        {
            // Sätt ItemsSource direkt utan binding
            commentsListBox.ItemsSource = comments;
        }

        public event EventHandler CloseRequested;

        public TaskComments()
        {
            InitializeComponent();
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            // Hämta TaskViewModel från DataContext
            var viewModel = DataContext as TaskViewModel;
            if (viewModel != null && !string.IsNullOrWhiteSpace(CommentBox.Text))
            {
                // Anropa ViewModel-metoden direkt
                viewModel.CommentText = CommentBox.Text;
                // Anropa kommandot om det finns
                if (viewModel.AddCommentCommand != null && viewModel.AddCommentCommand.CanExecute(null))
                {
                    viewModel.AddCommentCommand.Execute(null);
                }

                // Rensa textboxen
                CommentBox.Clear();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

    }
}
