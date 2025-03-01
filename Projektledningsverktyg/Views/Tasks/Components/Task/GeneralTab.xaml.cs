using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Models;
using Projektledningsverktyg.ViewModels;
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Projektledningsverktyg.Views.Tasks.Components.Task
{
    public partial class GeneralTab : Page
    {
        private TaskViewModel viewModel;
        private readonly Member currentMember;
        private bool isPanelOpen = false;

        public GeneralTab()
        {
            InitializeComponent();
        }

        public GeneralTab(Member _currentMember)
        {
            InitializeComponent();

            currentMember = _currentMember;
            var dbContext = new ApplicationDbContext();
            viewModel = new TaskViewModel(dbContext, currentMember);
            DataContext = viewModel;

            viewModel.CommentCountChanged += (taskId, newCount) => {
                // Uppdatera UI om det behövs
                viewModel.LoadTasks(); 
            };

            taskComments.CloseRequested += (s, e) =>
            {
                DoubleAnimation slideAnimation = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                isPanelOpen = false;
            };
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddGeneralTask(currentMember); 
            addWindow.Owner = Window.GetWindow(this);
            if (addWindow.ShowDialog() == true)
            {
                viewModel.LoadTasks(); // Refresh the list when dialog closes successfully
            }
        }
        #region Comments click
        private void TaskCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TaskModel task)
            {
                // Sätt vald uppgift
                viewModel.SelectTask(task);

                // Ladda kommentarerna explicit
                viewModel.LoadComments();

                // Uppdatera taskComments-kontrollen
                taskComments.DataContext = viewModel;
                taskComments.UpdateComments(viewModel.CurrentTaskComments);

                // Animera panelen
                double targetPosition = isPanelOpen ? 0 : -600;

                DoubleAnimation slideAnimation = new DoubleAnimation
                {
                    To = targetPosition,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                isPanelOpen = !isPanelOpen;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isPanelOpen && !CommentsPanel.IsMouseOver)
            {
                DoubleAnimation slideAnimation = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideAnimation);
                isPanelOpen = false;
            }
        }

        #endregion

    }
}
