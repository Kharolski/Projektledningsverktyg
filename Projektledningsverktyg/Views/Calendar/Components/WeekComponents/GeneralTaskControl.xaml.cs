using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.Helpers;
using Projektledningsverktyg.ViewModels.Calendar.WeekModels;
using System;
using System.Diagnostics;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class GeneralTaskControl : DraggableControlBase
    {
        #region Events
        public event EventHandler ContentSizeChanged;
        public event EventHandler<Task> ShowTaskCommentsRequested;
        #endregion

        private bool _isEventInProgress = false;
        private TaskWeekViewModel _viewModel;

        public GeneralTaskControl()
        {
            InitializeComponent();

            // Skapa repository och ViewModel
            var context = new ApplicationDbContext();
            var taskRepository = new TaskRepository(context);
            _viewModel = new TaskWeekViewModel(taskRepository);

            DataContext = _viewModel;

            // Prenumerera på ShowCommentsRequested-eventet
            _viewModel.ShowCommentsRequested += (sender, task) =>
            {
                if (_isEventInProgress)
                    return;

                try
                {
                    _isEventInProgress = true;
                    System.Diagnostics.Debug.WriteLine($"Event utlöst från {Name}");
                    ShowTaskCommentsRequested?.Invoke(this, task);
                }
                finally
                {
                    _isEventInProgress = false;
                }
            };
        }

        /// <summary>
        /// Uppdatera valt datum och hämta uppgifter för det datumet
        /// </summary>
        /// <param name="date">Valt datum</param>
        public void UpdateSelectedDate(DateTime date)
        {
            if (_viewModel != null)
            {
                _viewModel.UpdateSelectedDate(date);
            }
        }

    }
}
