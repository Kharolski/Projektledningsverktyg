using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Views.Tasks.Components.Meals;
using System;
using System.Windows.Input;

namespace Projektledningsverktyg.ViewModels
{
    public class AddMealSectionViewModel : ViewModelBase
    {
        private string _mealName;
        private DateTime _selectedDate = DateTime.Now;
        private MealType _selectedMealType;
        private readonly ListMealsSectionViewModel _listViewModel;

        #region Properties

        public string MealName
        {
            get => _mealName;
            set
            {
                _mealName = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public MealType SelectedMealType
        {
            get => _selectedMealType;
            set
            {
                _selectedMealType = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddMealCommand { get; }
        public ICommand GetAISuggestionCommand { get; }

        #endregion

        #region Constructors

        public AddMealSectionViewModel()
        {
            System.Diagnostics.Debug.WriteLine("Constructor 1 called");
            AddMealCommand = new RelayCommand(ExecuteAddMeal);
            GetAISuggestionCommand = new RelayCommand(ExecuteGetAISuggestion);
        }

        public AddMealSectionViewModel(ListMealsSectionViewModel listViewModel)
        {
            System.Diagnostics.Debug.WriteLine("Constructor 2 called");
            _listViewModel = listViewModel;
            AddMealCommand = new RelayCommand(ExecuteAddMeal);
            GetAISuggestionCommand = new RelayCommand(ExecuteGetAISuggestion);
        }

        #endregion

        #region Add

        private void ExecuteAddMeal()
        {
            System.Diagnostics.Debug.WriteLine("Starting meal save process");
            using (var context = new ApplicationDbContext())
            {
                var meal = new Meal
                {
                    Name = MealName,
                    Type = SelectedMealType,
                    Date = SelectedDate
                };

                context.Meals.Add(meal);
                context.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"Meal saved with ID: {meal.Id}");

                MealName = string.Empty;
                SelectedDate = DateTime.Now;

                // Open ingredients dialog without notifying updates yet
                OpenIngredientsDialog(meal.Id);

                // Only notify once after everything is complete
                ListMealsSectionViewModel.NotifyMealsUpdated();
            }
        }

        #endregion

        #region AI 

        private void ExecuteGetAISuggestion()
        {
            // AI suggestion logic will go here
        }

        #endregion


        private void OpenIngredientsDialog(int mealId)
        {
            var ingredientsWindow = new AddIngredientsWindow(mealId);
            ingredientsWindow.ShowDialog();
        }


    }

}
