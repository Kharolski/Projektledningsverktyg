using Projektledningsverktyg.ViewModels;
using System.Windows;
using System;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;

namespace Projektledningsverktyg.Views.Tasks.Components.Task
{
    public partial class AddGeneralTask : Window
    {
        private TaskViewModel _viewModel;
        private Data.Entities.Task _existingTask;

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

        public AddGeneralTask(Member member, DateTime initialDate)
        {
            InitializeComponent();
            var dbContext = new ApplicationDbContext();
            _viewModel = new TaskViewModel(dbContext, member);

            // Sätt det valda datumet
            _viewModel.SelectedDate = initialDate;

            DataContext = _viewModel;

            this.Title = "Lägg till ny uppgift";
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

            if (_existingTask != null)
            {
                // Vi redigerar en befintlig uppgift
                _viewModel.ExecuteUpdateTask(_existingTask);
            }
            else
            {
                // Vi skapar en ny uppgift
                _viewModel.ExecuteSaveTask();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
