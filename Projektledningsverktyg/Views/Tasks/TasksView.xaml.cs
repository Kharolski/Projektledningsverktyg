using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using Projektledningsverktyg.Views.Tasks.Components;
using System.Diagnostics;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks
{
    public partial class TasksView : Page
    {
        private readonly Member _currentMember;
        public TasksView(Member currentMember)
        {

            _currentMember = currentMember;
            InitializeComponent();

            var dbContext = new ApplicationDbContext();
            DataContext = new TaskViewModel(dbContext, currentMember);
            GeneralTasksFrame.Navigate(new GeneralTasksTab(currentMember));

        }
    }
}
