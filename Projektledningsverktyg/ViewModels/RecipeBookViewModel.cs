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
            ToggleFavoriteCommand = new RelayCommand<Recipe>(ToggleFavorite);

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

        #region Recipe List
        private void ToggleFavorite(Recipe recipe)
        {
            Debug.WriteLine($"Toggling favorite for recipe: {recipe.Name}");
            Debug.WriteLine($"Current favorite status: {recipe.IsFavorite}");

            // Toggle the favorite status
            recipe.IsFavorite = !recipe.IsFavorite;
            Debug.WriteLine($"New favorite status: {recipe.IsFavorite}");

            // Update in database
            var dbRecipe = _context.Recipes.Find(recipe.Id);
            dbRecipe.IsFavorite = recipe.IsFavorite;
            _context.SaveChanges();

            // Refresh lists
            LoadRecipes();
            Debug.WriteLine("Lists refreshed");
        }
        #endregion
    }
}
