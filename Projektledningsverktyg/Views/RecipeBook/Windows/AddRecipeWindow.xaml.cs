using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using Projektledningsverktyg.Views.RecipeBook.Components;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Projektledningsverktyg.Views.RecipeBook.Windows
{
    public partial class AddRecipeWindow : Window
    {
        #region Properties
        private readonly RecipeViewModel _viewModel;
        private readonly RecipeBookViewModel _recipeBookVM; // property for storing RecipeBookViewModel for editing existing recipe
        #endregion

        #region Constructor
        // Constructor for adding new recipe
        public AddRecipeWindow(RecipeBookViewModel recipeBookVM)
        {
            InitializeComponent();
            _recipeBookVM = recipeBookVM;                   // store the RecipeBookViewModel for editing existing recipe
            _viewModel = new RecipeViewModel(new ApplicationDbContext());
            _viewModel.RecipeAdded += () => recipeBookVM.RefreshRecipes();
            DataContext = _viewModel;
        }

        // Constructor for editing existing recipe
        public AddRecipeWindow(RecipeBookViewModel recipeBookVM, Recipe recipeToEdit)
        {
            InitializeComponent();
            _recipeBookVM = recipeBookVM;                   // store the RecipeBookViewModel for editing existing recipe
            _viewModel = new RecipeViewModel(new ApplicationDbContext());
            _viewModel.LoadRecipeForEdit(recipeToEdit);
            _viewModel.RecipeAdded += () => recipeBookVM.RefreshRecipes();
            DataContext = _viewModel;
        }

        #endregion

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsEditMode)
            {
                var recipe = _viewModel.CurrentRecipe;
                // Use the RecipeBookViewModel from constructor
                var recipeBookVM = _recipeBookVM;  // This is the one passed in AddRecipeWindow constructor

                var detailsView = new RecipeDetailsView(recipeBookVM);
                detailsView.RecipeDeleted += () => recipeBookVM?.LoadRecipes();
                detailsView.RecipeFavoriteChanged += () => recipeBookVM?.LoadRecipes();
                detailsView.LoadRecipe(recipe);

                var detailsWindow = new Window
                {
                    Content = detailsView,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Width = 550,
                    SizeToContent = SizeToContent.Height,
                    Title = recipe.Name
                };

                Window.GetWindow(this).Close();
                detailsWindow.ShowDialog();
            }
            else
            {
                Window.GetWindow(this).Close();
            }
        }


    }
}
