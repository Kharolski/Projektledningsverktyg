using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;
using System;
using System.IO;
using Projektledningsverktyg.Views.RecipeBook.Windows;
using System.Windows;
using Projektledningsverktyg.Views.RecipeBook.Components;

namespace Projektledningsverktyg.ViewModels
{
    public class RecipeBookViewModel : ViewModelBase
    {
        #region Properties

        private readonly ApplicationDbContext _context;
        private ObservableCollection<Recipe> _recipes;
        private ObservableCollection<Recipe> _filteredRecipes;

        // Side panel
        public Dictionary<MealType, int> MealTypeCounts { get; private set; }
        public Dictionary<string, int> IngredientCounts { get; private set; }
        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set
            {
                _filteredRecipes = value;
                OnPropertyChanged();
            }
        }

        // Recipe List 

        #endregion

        #region Commands

        // Side panel
        public ICommand ShowAllRecipesCommand { get; private set; }
        public ICommand ShowFavoritesCommand { get; private set; }
        public ICommand ShowRecentCommand { get; private set; }
        public ICommand FilterByMealTypeCommand { get; private set; }
        public ICommand FilterByIngredientCommand { get; private set; }

        // Recipe List
        public ICommand ToggleFavoriteCommand { get; private set; }

        // Edit Recipe
        public ICommand EditRecipeCommand { get; private set; }


        #endregion

        #region Constructor

        public RecipeBookViewModel()
        {
            _context = new ApplicationDbContext();
            LoadRecipes();

            // Side panel
            ShowAllRecipesCommand = new RelayCommand(ShowAllRecipes);
            ShowFavoritesCommand = new RelayCommand(ShowFavorites);
            ShowRecentCommand = new RelayCommand(ShowRecent);

            // Recipe List
            ToggleFavoriteCommand = new RelayCommand<Recipe>(r => ToggleFavorite(r.Id));

            // Edit Recipe
            EditRecipeCommand = new RelayCommand<Recipe>(ExecuteEditRecipe);

        }

        #endregion

        #region Load
        public void LoadRecipes()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            var recipes = _context.Recipes
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Description,
                    r.MealType,
                    r.MainIngredient,
                    r.CookingTime,
                    r.Servings,
                    r.ImagePath,
                    r.IsFavorite
                })
                .ToList()
                .Select(r => new Recipe
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    MealType = r.MealType,
                    MainIngredient = r.MainIngredient,
                    CookingTime = r.CookingTime,
                    Servings = r.Servings,
                    ImagePath = r.ImagePath != null ? Path.Combine(basePath, r.ImagePath) : null,
                    IsFavorite = r.IsFavorite
                });

            _recipes = new ObservableCollection<Recipe>(recipes);
            FilteredRecipes = new ObservableCollection<Recipe>(_recipes);
            OnPropertyChanged(nameof(FilteredRecipes));
        }

        public void RefreshRecipes()
        {
            _context.Recipes.Load();
            LoadRecipes();
            OnPropertyChanged(nameof(FilteredRecipes));
        }

        // Show recipe details after canceling edit
        public void ShowRecipeDetails(Recipe recipe)
        {
            var detailsWindow = new Window
            {
                Content = new RecipeDetailsView { DataContext = recipe },
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            detailsWindow.ShowDialog();
        }


        #endregion

        #region Sido panel

        // Quick Access buttons
        private void ShowAllRecipes()
        {
            LoadRecipes();
        }

        private void ShowFavorites()
        {
            var favorites = _recipes.Where(r => r.IsFavorite).ToList();
            FilteredRecipes = new ObservableCollection<Recipe>(favorites);
        }

        private void ShowRecent()
        {
            var recentRecipes = _recipes.OrderByDescending(r => r.CreatedDate)
                                       .Take(10)
                                       .ToList();
            FilteredRecipes = new ObservableCollection<Recipe>(recentRecipes);
        }

        #endregion

        #region Favorit Toogle
        public Recipe ToggleFavorite(int recipeId)
        {
            using (var context = new ApplicationDbContext())
            {
                var recipe = context.Recipes.Find(recipeId);
                recipe.IsFavorite = !recipe.IsFavorite;
                context.SaveChanges();

                // Get complete recipe data
                var updatedRecipe = context.Recipes.Find(recipeId);
                updatedRecipe.Ingredients = context.Ingredients
                    .Where(i => i.RecipeId == recipeId)
                    .ToList();
                updatedRecipe.Instructions = context.Instructions
                    .Where(i => i.RecipeId == recipeId)
                    .ToList();

                LoadRecipes();
                return updatedRecipe;
            }
        }

        #endregion

        #region Edit
        private void ExecuteEditRecipe(Recipe recipe)
        {
            var mainWindow = Application.Current.MainWindow;
            var recipeBookVM = mainWindow.DataContext as RecipeBookViewModel;

            // Close details window
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive)?.Close();

            // Open edit window with correct ViewModel
            var addRecipeWindow = new AddRecipeWindow(recipeBookVM, recipe);
            addRecipeWindow.ShowDialog();
        }
        #endregion
    }
}
