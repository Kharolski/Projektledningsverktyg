using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System;
using Projektledningsverktyg.Views.Tasks.Components.Meals.Components;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace Projektledningsverktyg.ViewModels.Calendar.WeekModels
{
    public class MealWeekViewModel : ViewModelBase
    {
        #region Fields
        private readonly MealRepository _repository;
        private DateTime _selectedDate;
        private ObservableCollection<Meal> _meals;
        private bool _hasMeals;
        private Meal _selectedMeal;
        #endregion

        #region Properties
        public ObservableCollection<Meal> Meals
        {
            get => _meals;
            set
            {
                _meals = value;
                OnPropertyChanged(nameof(Meals));
                HasMeals = _meals != null && _meals.Count > 0;
            }
        }

        public bool HasMeals
        {
            get => _hasMeals;
            set
            {
                _hasMeals = value;
                OnPropertyChanged(nameof(HasMeals));
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                LoadMealsForSelectedDate();
            }
        }

        public Meal SelectedMeal
        {
            get => _selectedMeal;
            set
            {
                _selectedMeal = value;
                OnPropertyChanged(nameof(SelectedMeal));
            }
        }
        #endregion

        #region Commands
        public ICommand AddMealCommand { get; private set; }
        public ICommand DeleteMealCommand { get; private set; }
        public ICommand ShowMealDetailsCommand { get; private set; }
        #endregion

        #region Constructor
        public MealWeekViewModel(MealRepository repository)
        {
            _repository = repository;
            _selectedDate = DateTime.Today;
            _meals = new ObservableCollection<Meal>();

            // Initiera commands
            AddMealCommand = new RelayCommand(AddMeal);
            DeleteMealCommand = new RelayCommand<Meal>(DeleteMeal);
            ShowMealDetailsCommand = new RelayCommand(ShowMealDetails);

            // Ladda måltider för idag
            LoadMealsForSelectedDate();
        }
        #endregion

        #region Methods
        // Ladda måltider för vald dag
        private void LoadMealsForSelectedDate()
        {
            var mealsForDate = _repository.GetMealsByDate(SelectedDate);
            Meals = new ObservableCollection<Meal>(mealsForDate.OrderBy(m => m.Type));

            // Om det finns måltider, välj den första
            if (Meals.Count > 0)
                SelectedMeal = Meals[0];
            else
                SelectedMeal = null;
        }

        // Uppdatera vid datumändring
        public void UpdateSelectedDate(DateTime date)
        {
            SelectedDate = date;
        }

        // Lägg till ny måltid
        private void AddMeal()
        {
            var dialog = new AddMealDialog();

            // Skapa en ObservableCollection med dagar 
            var weekDays = new ObservableCollection<DayViewModel>();

            // Hämta aktuellt datum och skapa veckans dagar
            var currentDate = SelectedDate;
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);

            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                // Använd konstruktorn korrekt genom att skicka DateTime
                weekDays.Add(new DayViewModel(date));
            }

            var viewModel = new AddMealDialogViewModel(weekDays);

            // Sätt den valda dagen till aktuellt valt datum
            var selectedDay = weekDays.FirstOrDefault(d => d.Date.Date == SelectedDate.Date);
            if (selectedDay != null)
            {
                viewModel.SelectedDay = selectedDay;
            }

            // Sätt uppdateringsfunktion
            viewModel.RefreshWeekView = () => LoadMealsForSelectedDate();

            dialog.DataContext = viewModel;
            viewModel.SetDialogWindow(dialog);

            // Inaktivera ComboBox för dagar
            dialog.Loaded += (sender, e) =>
            {
                var dayComboBox = dialog.FindName("DayComboBox") as System.Windows.Controls.ComboBox;
                if (dayComboBox != null)
                {
                    dayComboBox.IsEnabled = false;
                }
            };

            dialog.ShowDialog();
        }

        // Ta bort måltid
        private void DeleteMeal(Meal meal)
        {
            if (meal != null)
            {
                _repository.DeleteMeal(meal.Id);
                LoadMealsForSelectedDate();
            }
        }

        // Visa detaljerad information om måltiden
        private void ShowMealDetails()
        {
            if (SelectedMeal == null)
                return;

            var dialog = new ViewMealDialog();
            var viewModel = new ViewMealDialogViewModel(SelectedMeal.Id);
            dialog.DataContext = viewModel;
            viewModel.SetDialogWindow(dialog);
            dialog.ShowDialog();
        }
        #endregion

    }
}
