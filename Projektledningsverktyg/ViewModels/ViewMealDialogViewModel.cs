using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Projektledningsverktyg.ViewModels
{
    public class ViewMealDialogViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private Window _dialogWindow;
        private string _imagePath;
        private Data.Entities.Meal _meal;

        public string Name { get; set; }
        public MealType Type { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public string Description { get; set; }
        public string ImagePath
        {
            get => System.IO.Path.GetFullPath(_imagePath);
            set => SetProperty(ref _imagePath, value);
        }
        public ObservableCollection<MealIngredient> Ingredients { get; set; }
        public ObservableCollection<MealInstruction> Instructions { get; set; }

        public ViewMealDialogViewModel(int mealId)
        {
            _context = new ApplicationDbContext();
            Ingredients = new ObservableCollection<MealIngredient>();
            Instructions = new ObservableCollection<MealInstruction>();
            LoadMealDetails(mealId);
        }

        private void LoadMealDetails(int mealId)
        {
            _meal = _context.Meals
                .Include("Ingredients")
                .Include("Instructions")
                .FirstOrDefault(m => m.Id == mealId);

            if (_meal != null)
            {
                Name = _meal.Name;
                Type = _meal.Type;
                CookingTime = _meal.CookingTime;
                Servings = _meal.Servings;
                Description = _meal.Description;
                ImagePath = _meal.ImagePath;

                foreach (var ingredient in _meal.Ingredients)
                    Ingredients.Add(ingredient);

                foreach (var instruction in _meal.Instructions)
                    Instructions.Add(instruction);
            }
        }

        public void SetDialogWindow(Window window)
        {
            _dialogWindow = window;
        }
    }
}
