using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Views.Tasks.Components.Meals.Components;
using System;
using System.Linq;
using System.Windows.Input;

namespace Projektledningsverktyg.ViewModels
{
    public class MealViewModel : ViewModelBase
    {
        #region Fields
        public Action RefreshWeekView { get; set; }
        private string _name;
        private MealType _type;
        private int _preparationTime;
        private string _description;
        private MainIngredients _mainIngredient;
        private int _servings;
        #endregion

        #region Properties
        public int Id { get; set; }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public MealType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public int PreparationTime
        {
            get => _preparationTime;
            set => SetProperty(ref _preparationTime, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public MainIngredients MainIngredient
        {
            get => _mainIngredient;
            set => SetProperty(ref _mainIngredient, value);
        }

        public int Servings
        {
            get => _servings;
            set => SetProperty(ref _servings, value);
        }
        #endregion

        #region Commands
        public ICommand CopyMealCommand { get; }
        public ICommand DeleteMealCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        #endregion

        #region Constructor
        public MealViewModel()
        {
            CopyMealCommand = new RelayCommand(ExecuteCopyMeal);
            DeleteMealCommand = new RelayCommand(ExecuteDeleteMeal);
            ViewDetailsCommand = new RelayCommand(ExecuteViewDetails);
        }
        #endregion

        #region Methods

        private void ExecuteDeleteMeal()
        {
            using (var context = new ApplicationDbContext())
            {
                var meal = context.Meals
                    .Include("Ingredients")
                    .Include("Instructions")
                    .FirstOrDefault(m => m.Id == Id);

                if (meal != null)
                {
                    context.MealIngredients.RemoveRange(meal.Ingredients);
                    context.MealInstructions.RemoveRange(meal.Instructions);
                    context.Meals.Remove(meal);
                    context.SaveChanges();
                    RefreshWeekView?.Invoke();
                }
            }
        }

        private void ExecuteViewDetails()
        {
            var dialog = new ViewMealDialog();
            var viewModel = new ViewMealDialogViewModel(Id);
            dialog.DataContext = viewModel;
            viewModel.SetDialogWindow(dialog);
            dialog.ShowDialog();
        }
        #endregion

        #region Mapping Methods
        public static MealViewModel FromEntity(Meal meal)
        {
            return new MealViewModel
            {
                Id = meal.Id,
                Name = meal.Name,
                Type = meal.Type,
                PreparationTime = meal.CookingTime,
                Description = meal.Description,
                MainIngredient = meal.MainIngredient,
                Servings = meal.Servings
            };
        }

        public Meal ToEntity()
        {
            return new Meal
            {
                Name = this.Name,
                Type = this.Type,
                CookingTime = PreparationTime,
                Description = Description,
                MainIngredient = MainIngredient,
                Servings = Servings
            };
        }
        #endregion
    }
}
