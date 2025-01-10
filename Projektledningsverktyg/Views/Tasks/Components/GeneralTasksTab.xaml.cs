using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Models;
using Projektledningsverktyg.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projektledningsverktyg.Views.Tasks.Components
{
    /// <summary>
    /// Interaction logic for GeneralTasksTab.xaml
    /// </summary>
    public partial class GeneralTasksTab : Page
    {
        private TaskViewModel viewModel;
        public GeneralTasksTab()
        {
            InitializeComponent();
        }

        public GeneralTasksTab(Member currentMember)
        {
            InitializeComponent();

            var dbContext = new ApplicationDbContext();
            viewModel = new TaskViewModel(dbContext, currentMember);
            DataContext = viewModel;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is TaskModel task)
            {
                var detailWindow = new TaskDetail(task);
                detailWindow.ShowDialog();
            }
        }

        
    }
}
