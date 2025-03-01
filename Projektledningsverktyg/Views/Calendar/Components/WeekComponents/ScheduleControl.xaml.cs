using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.Helpers;
using Projektledningsverktyg.ViewModels.Calendar.WeekModels;
using System;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class ScheduleControl : DraggableControlBase
    {
        #region Events
        public event EventHandler ContentSizeChanged;
        #endregion

        #region Properties
        public ScheduleWeekViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public ScheduleControl()
        {
            InitializeComponent();

            // Create repository and ViewModel
            var context = new ApplicationDbContext();
            var repository = new ScheduleRepository(context);

            // Create and set ViewModel
            ViewModel = new ScheduleWeekViewModel(repository);
            DataContext = ViewModel;

            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Public Methods
        // Method to update when selected date changes
        public void UpdateSelectedDate(DateTime date)
        {
            ViewModel.UpdateSelectedDate(date);
        }
        #endregion
    }
}
