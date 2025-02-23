using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows;

namespace Projektledningsverktyg.ViewModels
{
    public class AddMealDialogViewModel : ViewModelBase
    {
        #region Fields
        public event EventHandler<Meal> MealAdded;
        private Window _dialogWindow;
        private readonly ApplicationDbContext _context;
        private MealTypes _selectedMealType;
        private Recipe _selectedRecipe;
        private ObservableCollection<Recipe> _filteredRecipes;
        private DayViewModel _selectedDay;
        #endregion

        #region Properties
        public Action RefreshWeekView { get; set; }
        public ObservableCollection<DayViewModel> WeekDays { get; }
        public DayViewModel SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }
        public MealTypes SelectedMealType
        {
            get => _selectedMealType;
            set
            {
                SetProperty(ref _selectedMealType, value);
                FilterRecipes();
            }
        }

        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set => SetProperty(ref _selectedRecipe, value);
        }

        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set => SetProperty(ref _filteredRecipes, value);
        }

        public Array MealTypes => Enum.GetValues(typeof(MealTypes));

        public void SetDialogWindow(Window window)
        {
            _dialogWindow = window;
        }
        #endregion

        #region Commands
        public ICommand AddMealCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Constructor
        public AddMealDialogViewModel(ObservableCollection<DayViewModel> weekDays)
        {
            _context = new ApplicationDbContext();
            WeekDays = weekDays;
            SelectedDay = WeekDays.FirstOrDefault();
            FilteredRecipes = new ObservableCollection<Recipe>();
            AddMealCommand = new RelayCommand(ExecuteAddMeal);
            CancelCommand = new RelayCommand(ExecuteCancel);
            LoadRecipes();
        }
        #endregion

        #region Methods
        private void LoadRecipes()
        {
            FilteredRecipes.Clear();
            var recipes = _context.Recipes
                .Include("Ingredients")
                .Include("Instructions")
                .Where(r => r.MealType == SelectedMealType)
                .ToList();

            foreach (var recipe in recipes)
            {
                FilteredRecipes.Add(recipe);
            }
        }

        private void FilterRecipes()
        {
            var recipes = _context.Recipes
                .Include("Ingredients")
                .Include("Instructions")
                .Where(r => r.MealType == SelectedMealType)
                .ToList();

            FilteredRecipes.Clear();
            foreach (var recipe in recipes)
            {
                FilteredRecipes.Add(recipe);
            }
        }

        private void ExecuteAddMeal()
        {
            if (SelectedRecipe == null || SelectedDay == null)
            {
                MessageBox.Show("Välj både recept och dag.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var meal = new Meal
            {
                Name = SelectedRecipe.Name,
                Type = (MealType)SelectedRecipe.MealType,
                Date = SelectedDay.Date,
                Description = SelectedRecipe.Description,
                CookingTime = SelectedRecipe.CookingTime,
                MainIngredient = SelectedRecipe.MainIngredient,
                Servings = SelectedRecipe.Servings,
                RecipeId = SelectedRecipe.Id,
                ImagePath = SelectedRecipe.ImagePath
            };

            // Copy ingredients
            foreach (var ingredient in SelectedRecipe.Ingredients)
            {
                meal.Ingredients.Add(new MealIngredient
                {
                    Name = ingredient.Name,
                    Amount = ingredient.Amount,
                    Unit = ingredient.Unit
                });
            }

            // Copy instructions
            foreach (var instruction in SelectedRecipe.Instructions)
            {
                meal.Instructions.Add(new MealInstruction
                {
                    StepNumber = instruction.StepNumber,
                    Description = instruction.Description,
                    ImagePath = instruction.ImagePath,
                    PrepTime = instruction.PrepTime
                });
            }

            _context.Meals.Add(meal);
            _context.SaveChanges();

            RefreshWeekView?.Invoke();

            MealAdded?.Invoke(this, meal);
            _dialogWindow?.Close();
        }

        private void ExecuteCancel()
        {
            _dialogWindow?.Close();
        }
        #endregion
    }
}
