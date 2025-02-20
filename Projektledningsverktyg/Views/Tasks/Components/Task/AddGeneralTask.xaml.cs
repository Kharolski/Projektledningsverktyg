using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Controls.Primitives;
using System;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Diagnostics;

namespace Projektledningsverktyg.Views.Tasks.Components.Task
{
    public partial class AddGeneralTask : System.Windows.Window
    {
        private TaskViewModel _viewModel;

        public AddGeneralTask()
        {
            InitializeComponent();
        }

        public AddGeneralTask(Member member)
        {
            InitializeComponent();
            var dbContext = new ApplicationDbContext();
            _viewModel = new TaskViewModel(dbContext, member);
            DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (string.IsNullOrWhiteSpace(_viewModel.NewTaskTitle))
            {
                _viewModel.AddTaskErrorMessage = "Titel kan inte vara tom";
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            if (_viewModel.SelectedDate.Date < DateTime.Now.Date)
            {
                _viewModel.AddTaskErrorMessage = "Datum kan inte vara tidigare än idag";
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            _viewModel.ExecuteSaveTask();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
