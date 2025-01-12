using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Data.Entity;
using System.Linq;

namespace Projektledningsverktyg.ViewModels
{
    public class AddIngredientsWindowViewModel : ViewModelBase
    {
        private readonly int _mealId;
        private string _newIngredient;
        private ObservableCollection<MealIngredient> _ingredients;

        #region Properties

        public string NewIngredient
        {
            get => _newIngredient;
            set
            {
                _newIngredient = value;
                OnPropertyChanged();
            }
        }
        public Action CloseWindow { get; set; }

        public ICommand AddIngredientCommand { get; }
        public ICommand RemoveIngredientCommand { get; }
        public ICommand SaveIngredientsCommand { get; }

        public ObservableCollection<MealIngredient> Ingredients
        {
            get => _ingredients;
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public AddIngredientsWindowViewModel(int mealId)
        {
            _mealId = mealId;
            Ingredients = new ObservableCollection<MealIngredient>();

            AddIngredientCommand = new RelayCommand(ExecuteAddIngredient);
            RemoveIngredientCommand = new RelayCommand<MealIngredient>(ExecuteRemoveIngredient);
            SaveIngredientsCommand = new RelayCommand(ExecuteSaveIngredients);
            LoadIngredients();
        }

        #endregion

        #region Load

        private void LoadIngredients()
        {
            using (var context = new ApplicationDbContext())
            {
                var mealWithIngredients = context.Meals
                    .Include(m => m.Ingredients)
                    .FirstOrDefault(m => m.Id == _mealId);

                if (mealWithIngredients != null)
                {
                    Ingredients = new ObservableCollection<MealIngredient>(mealWithIngredients.Ingredients);
                }
            }
        }

        #endregion

        #region Add

        private void ExecuteAddIngredient()
        {
            System.Diagnostics.Debug.WriteLine($"Current meal ID being used: {_mealId}");
            if (string.IsNullOrWhiteSpace(NewIngredient))
                return;

            using (var context = new ApplicationDbContext())
            {
                // First verify the meal exists
                var existingMeal = context.Meals.Find(_mealId);
                if (existingMeal != null)
                {
                    var ingredient = new MealIngredient
                    {
                        MealId = _mealId,
                        Name = NewIngredient
                    };

                    context.MealIngredients.Add(ingredient);
                    context.SaveChanges();
                    NewIngredient = string.Empty;
                    LoadIngredients();
                    ListMealsSectionViewModel.NotifyMealsUpdated();
                }
            }
        }

        #endregion

        #region Delete

        private void ExecuteRemoveIngredient(MealIngredient ingredient)
        {
            using (var context = new ApplicationDbContext())
            {
                var ingredientToDelete = context.MealIngredients.Find(ingredient.Id);
                if (ingredientToDelete != null)
                {
                    context.MealIngredients.Remove(ingredientToDelete);
                    context.SaveChanges();
                    LoadIngredients();
                    ListMealsSectionViewModel.NotifyMealsUpdated();
                }
            }
        }

        #endregion

        #region Save

        private void ExecuteSaveIngredients()
        {
            // Close the window
            CloseWindow?.Invoke();
        }

        #endregion
    }

}
