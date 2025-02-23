using Projektledningsverktyg.Commands;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using System;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Views.Tasks.Components.Meals.Components;

namespace Projektledningsverktyg.ViewModels
{
    public class DayViewModel : ViewModelBase
    {
        #region Fields
        private string _dayName;
        private DateTime _date;
        private ObservableCollection<MealViewModel> _meals;
        public ObservableCollection<DayViewModel> WeekDays { get; set; }
        #endregion

        #region Properties
        public string FormattedDate => Date.ToString("d MMMM", new CultureInfo("sv-SE"));

        public string DayName
        {
            get => _dayName;
            set => SetProperty(ref _dayName, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public ObservableCollection<MealViewModel> Meals
        {
            get => _meals;
            set => SetProperty(ref _meals, value);
        }
        #endregion

        #region Commands
        public ICommand AddMealCommand { get; }
        #endregion

        #region Constructor
        public DayViewModel(DateTime date)
        {
            _date = date;
            _dayName = date.ToString("dddd", new CultureInfo("sv-SE"));

            Meals = new ObservableCollection<MealViewModel>();
            WeekDays = new ObservableCollection<DayViewModel>();
            AddMealCommand = new RelayCommand(ExecuteAddMeal);


        }
        #endregion

        #region Methods
        private void ExecuteAddMeal()
        {
            var dialog = new AddMealDialog();
            var viewModel = new AddMealDialogViewModel(WeekDays);
            dialog.DataContext = viewModel;

            // Subscribe to meal added event
            viewModel.MealAdded += (sender, meal) =>
            {
                Meals.Add(new MealViewModel
                {
                    Name = meal.Name,
                    Type = meal.Type,
                    PreparationTime = meal.CookingTime
                });
            };

            dialog.ShowDialog();
        }
        #endregion
    }
}
