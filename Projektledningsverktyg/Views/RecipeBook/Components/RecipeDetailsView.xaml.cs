using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Projektledningsverktyg.Views.RecipeBook.Windows;
using System.Windows.Media;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace Projektledningsverktyg.Views.RecipeBook.Components
{
    public partial class RecipeDetailsView : UserControl
    {
        #region Properties
        private readonly ApplicationDbContext _context;
        private Recipe _currentRecipe;
        public event Action RecipeDeleted;
        public event Action RecipeFavoriteChanged;
        private bool _isFullView = false;
        private RecipeBookViewModel _recipeBookVM;
        #endregion

        #region Constructors
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

        #endregion

        #region Load
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
        #endregion

        #region ViewToogle
        private void ViewToggleButton_Click(object sender, RoutedEventArgs e)
        {
            _isFullView = !_isFullView;

            if (_isFullView)
            {
                ViewToggleButton.Content = "Kort vy";
                IngredientsPanel.Visibility = Visibility.Visible;
                InstructionsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ViewToggleButton.Content = "Full vy";
                IngredientsPanel.Visibility = Visibility.Collapsed;
                InstructionsPanel.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Favorite button
        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_recipeBookVM == null)
            {
                var parent = VisualTreeHelper.GetParent(this);
                while (parent != null && !(parent is RecipeBookView))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }
                _recipeBookVM = ((RecipeBookView)parent)?.DataContext as RecipeBookViewModel;
            }

            var recipe = (Recipe)DataContext;
            var currentImagePath = recipe.ImagePath;  // Save current image path
            var updatedRecipe = _recipeBookVM.ToggleFavorite(recipe.Id);
            updatedRecipe.ImagePath = currentImagePath;  // Restore image path
            DataContext = updatedRecipe;
        }
        #endregion

        #region Edit
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var recipe = DataContext as Recipe;

            // Close details window
            Window.GetWindow(this)?.Close();

            // Open edit window
            var addRecipeWindow = new AddRecipeWindow(_recipeBookVM, recipe);
            addRecipeWindow.ShowDialog();
        }
        #endregion

        #region Delete
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this recipe?",
                                        "Confirm Delete",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var dbRecipe = _context.Recipes.Find(_currentRecipe.Id);
                if (dbRecipe == null)
                    return; // Säkerhetskontroll om receptet inte finns

                // Remove related ingredients
                var ingredients = _context.Ingredients.Where(ri => ri.RecipeId == dbRecipe.Id);
                _context.Ingredients.RemoveRange(ingredients);

                // Remove related instructions
                var instructions = _context.Instructions.Where(i => i.RecipeId == dbRecipe.Id);
                _context.Instructions.RemoveRange(instructions);

                // Remove the recipe
                _context.Recipes.Remove(dbRecipe);
                _context.SaveChanges();

                // Force image disposal
                FreeImageResource();

                // Delete folder
                string recipeImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Recept", dbRecipe.Id.ToString());
                DeleteFolderSafely(recipeImageFolder);

                RecipeDeleted?.Invoke();
                Window.GetWindow(this)?.Close();
            }
        }

        private void FreeImageResource()
        {
            if (RecipeImage.Source is BitmapImage bitmapImage)
            {
                bitmapImage.StreamSource?.Close();  // Stäng filströmmen
                bitmapImage.StreamSource?.Dispose();
            }

            RecipeImage.Source = null;

            // Frigör resurser och vänta på att garbage collection ska avsluta alla öppna resurser
            DataContext = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void DeleteFolderSafely(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return;

            // Clear any image resources first
            _currentRecipe.ImagePath = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            try
            {
                foreach (string file in Directory.GetFiles(folderPath))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                Thread.Sleep(100);
                Directory.Delete(folderPath, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Folder deletion error: {ex.Message}");
            }
        }

        #endregion


    }
}
