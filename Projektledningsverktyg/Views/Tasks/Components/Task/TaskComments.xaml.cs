using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Models;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.Tasks.Components.Task
{
    public partial class TaskComments : UserControl
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskModel _currentTask;

        public event EventHandler CloseRequested;

        public TaskComments()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

    }
}
