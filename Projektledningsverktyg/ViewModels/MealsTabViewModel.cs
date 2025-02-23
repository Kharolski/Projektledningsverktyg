using Projektledningsverktyg.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Globalization;
using Projektledningsverktyg.Views.Tasks.Components.Meals.Components;
using System.Linq;
using Projektledningsverktyg.Data.Context;

namespace Projektledningsverktyg.ViewModels
{
    public class MealsTabViewModel : ViewModelBase
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        private ObservableCollection<DayViewModel> _weekDays;
        private int _currentWeek;
        private string _currentMonth;
        private DateTime _currentDate;
        #endregion

        #region Properties
        public ObservableCollection<DayViewModel> WeekDays
        {
            get => _weekDays;
            set => SetProperty(ref _weekDays, value);
        }

        public int CurrentWeek
        {
            get => _currentWeek;
            set => SetProperty(ref _currentWeek, value);
        }

        public string CurrentMonth
        {
            get => _currentMonth;
            set => SetProperty(ref _currentMonth, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value?.ToLower()));
        }
        #endregion

        #region Commands
        public ICommand NextWeekCommand { get; }
        public ICommand PreviousWeekCommand { get; }
        public ICommand AddMealCommand { get; }
        #endregion

        #region Constructor
        public MealsTabViewModel()
        {
            _context = new ApplicationDbContext();
            WeekDays = new ObservableCollection<DayViewModel>();
            NextWeekCommand = new RelayCommand(ExecuteNextWeek);
            PreviousWeekCommand = new RelayCommand(ExecutePreviousWeek);
            AddMealCommand = new RelayCommand(ExecuteAddMeal);
            _currentDate = DateTime.Now;
            CurrentWeek = GetWeekNumber(_currentDate);
            LoadCurrentWeek();
        }
        #endregion

        #region Methods
        public void LoadCurrentWeek()
        {
            WeekDays.Clear();

            // Update current week
            _currentWeek = GetWeekNumber(_currentDate);
            CurrentMonth = _currentDate.ToString("MMMM", new CultureInfo("sv-SE"));

            // Get start of week
            var startOfWeek = _currentDate.AddDays(-(int)_currentDate.DayOfWeek + 1);

            // Create days and load their meals
            for (int i = 0; i < 7; i++)
            {
                var currentDate = startOfWeek.AddDays(i);
                var dayViewModel = new DayViewModel(currentDate);

                // Load meals for this day
                var mealsForDay = _context.Meals
                    .Where(m => m.Date.Year == currentDate.Year
                             && m.Date.Month == currentDate.Month
                             && m.Date.Day == currentDate.Day)
                    .ToList();

                foreach (var meal in mealsForDay)
                {
                    dayViewModel.Meals.Add(new MealViewModel
                    {
                        Id = meal.Id,
                        Name = meal.Name,
                        Type = meal.Type,
                        PreparationTime = meal.CookingTime,
                        RefreshWeekView = LoadCurrentWeek
                    });
                }

                WeekDays.Add(dayViewModel);
            }
        }
        private int GetWeekNumber(DateTime date)
        {
            var culture = new CultureInfo("sv-SE");
            return culture.Calendar.GetWeekOfYear(
                date,
                culture.DateTimeFormat.CalendarWeekRule,
                culture.DateTimeFormat.FirstDayOfWeek
            );
        }

        private void ExecuteNextWeek()
        {
            _currentDate = _currentDate.AddDays(7);
            CurrentWeek = GetWeekNumber(_currentDate);
            CurrentMonth = _currentDate.ToString("MMMM", new CultureInfo("sv-SE"));
            LoadCurrentWeek();
        }

        private void ExecutePreviousWeek()
        {
            _currentDate = _currentDate.AddDays(-7);
            CurrentWeek = GetWeekNumber(_currentDate);
            CurrentMonth = _currentDate.ToString("MMMM", new CultureInfo("sv-SE"));
            LoadCurrentWeek();
        }

        private void ExecuteAddMeal()
        {
            var dialog = new AddMealDialog();
            var viewModel = new AddMealDialogViewModel(WeekDays);
            viewModel.RefreshWeekView = LoadCurrentWeek;
            dialog.DataContext = viewModel;

            viewModel.SetDialogWindow(dialog);

            dialog.ShowDialog();
        }
        #endregion
    }
}
