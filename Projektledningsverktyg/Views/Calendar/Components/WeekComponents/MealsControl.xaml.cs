using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.Helpers;
using Projektledningsverktyg.ViewModels.Calendar.WeekModels;
using System;
using System.Windows;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class MealsControl : DraggableControlBase
    {
        #region Events
        public event EventHandler ContentSizeChanged;
        #endregion

        #region Properties
        public MealWeekViewModel ViewModel { get; private set; }
        #endregion

        #region Constructor
        public MealsControl()
        {
            InitializeComponent();

            // Skapa repository och ViewModel
            var context = new ApplicationDbContext();
            var repository = new MealRepository(context);

            // Skapa och sätt ViewModel
            ViewModel = new MealWeekViewModel(repository);
            DataContext = ViewModel;

            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Event Handlers
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
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
