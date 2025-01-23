using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace Projektledningsverktyg.Views.RecipeBook.Components
{
    public partial class RecipeDetailsView : UserControl
    {
        private readonly ApplicationDbContext _context;
        private Recipe _currentRecipe;
        public event Action RecipeDeleted;
        public event Action RecipeFavoriteChanged;
        private bool _isFullView = false;
        private readonly RecipeBookViewModel _recipeBookVM;

        public RecipeDetailsView()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
        }
        public RecipeDetailsView(RecipeBookViewModel recipeBookVM)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _recipeBookVM = recipeBookVM;
        }

        public void LoadRecipe(Recipe recipe)
        {
            var recipeWithDetails = _context.Recipes.Find(recipe.Id);

            recipeWithDetails.Ingredients = _context.Ingredients
                .Where(i => i.RecipeId == recipe.Id)
                .ToList();

            recipeWithDetails.Instructions = _context.Instructions
                .Where(i => i.RecipeId == recipe.Id)
                .ToList();

            if (!string.IsNullOrEmpty(recipe.ImagePath))
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                recipeWithDetails.ImagePath = Path.Combine(basePath, recipe.ImagePath);
            }
            else
            {
                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                recipeWithDetails.ImagePath = Path.Combine(basePath, "Images", "recept.png");
            }

            _currentRecipe = recipeWithDetails;
            DataContext = recipeWithDetails;
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            var recipe = (Recipe)DataContext;
            var currentImagePath = recipe.ImagePath;  // Save current image path
            var updatedRecipe = _recipeBookVM.ToggleFavorite(recipe.Id);
            updatedRecipe.ImagePath = currentImagePath;  // Restore image path
            DataContext = updatedRecipe;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this recipe?",
                                        "Confirm Delete",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var dbRecipe = _context.Recipes.Find(_currentRecipe.Id);

                // Remove related ingredients
                var ingredients = _context.Ingredients.Where(ri => ri.RecipeId == dbRecipe.Id);
                _context.Ingredients.RemoveRange(ingredients);

                // Remove related instructions
                var instructions = _context.Instructions.Where(i => i.RecipeId == dbRecipe.Id);
                _context.Instructions.RemoveRange(instructions);

                // Remove the recipe
                _context.Recipes.Remove(dbRecipe);
                _context.SaveChanges();

                RecipeDeleted?.Invoke();
                Window.GetWindow(this).Close();
            }
        }

        private void ViewToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isFullView = !_isFullView;

            if (_isFullView)
            {
                ViewToggleButton.Content = "Brief View";
                IngredientsPanel.Visibility = Visibility.Visible;
                InstructionsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ViewToggleButton.Content = "Full Details";
                IngredientsPanel.Visibility = Visibility.Collapsed;
                InstructionsPanel.Visibility = Visibility.Collapsed;
            }
        }



    }
}
