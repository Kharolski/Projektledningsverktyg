using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.ViewModels.Calendar.WeekModels;
using System;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class EventsControl : UserControl
    {
        #region Events
        public event EventHandler ContentSizeChanged;
        #endregion

        #region Properties
        public EventWeekViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public EventsControl()
        {
            InitializeComponent();

            // Skapa repository och ViewModel
            var context = new ApplicationDbContext();
            var repository = new EventRepository(context);

            // Skapa och sätt ViewModel
            ViewModel = new EventWeekViewModel(repository);
            DataContext = ViewModel;

            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Public Methods
        // Metod för att uppdatera när vald dag ändras
        public void UpdateSelectedDate(DateTime date)
        {
            ViewModel.UpdateSelectedDate(date);
        }
        #endregion
    }
}
