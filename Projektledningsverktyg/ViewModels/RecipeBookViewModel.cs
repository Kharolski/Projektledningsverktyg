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
        #region Navigation Properties
        // Navigation state using Flags
        private NavigationType _selectedNavigation;
        public NavigationType SelectedNavigation
        {
            get => _selectedNavigation;
            set
            {
                _selectedNavigation = value;
                OnPropertyChanged(nameof(SelectedNavigation));
            }
        }

        // Navigation state helpers
        public bool IsFavoritesActive => (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;
        public bool IsMealTypeActive => (_selectedNavigation & NavigationType.MealType) == NavigationType.MealType;
        public bool IsIngredientActive => (_selectedNavigation & NavigationType.MainIngredient) == NavigationType.MainIngredient;

        // Navigation state tracking
        public bool _isStartedWithMealType;
        public void SetStartedWithMealType(bool value) => _isStartedWithMealType = value;

        public bool _isStartedWithIngredient;
        public void SetStartedWithIngredient(bool value) => _isStartedWithIngredient = value;

        public event Action<NavigationType> NavigationChanged;

        /// <summary>
        /// Property for search text input
        /// </summary>
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ExecuteSearch();
            }
        }
        #endregion

        #region Properties
        // Database and Data Collections
        private readonly ApplicationDbContext _context;
        private ObservableCollection<Recipe> _recipes;
        private ObservableCollection<Recipe> _filteredRecipes;

        // Category Counts
        private Dictionary<MealType, int> _mealTypeCounts = new Dictionary<MealType, int>();
        public Dictionary<MealType, int> MealTypeCounts
        {
            get => _mealTypeCounts;
            set
            {
                _mealTypeCounts = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, int> _ingredientCounts = new Dictionary<string, int>();
        public Dictionary<string, int> IngredientCounts
        {
            get => _ingredientCounts;
            set
            {
                _ingredientCounts = value;
                OnPropertyChanged();
            }
        }

        // Meal type selection
        private MealTypes? _selectedMealType;
        private MealTypes? _primaryMealType;
        public MealTypes? SelectedMealType
        {
            get => _selectedMealType;
            set
            {
                _selectedMealType = value;
                OnPropertyChanged();
            }
        }

        // Main Ingredient selection
        private MainIngredients? _selectedMainIngredient;
        public MainIngredients? SelectedMainIngredient
        {
            get => _selectedMainIngredient;
            set
            {
                var currentMealType = _selectedMealType;
                _selectedMainIngredient = value;

                if (_isStartedWithMealType)
                {
                    _selectedMealType = currentMealType;
                }

                OnPropertyChanged();
            }
        }

        // Observable Collections
        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set
            {
                _filteredRecipes = value;
                OnPropertyChanged();
            }
        }

        // View Models
        private RecipeBookSidePanelViewModel _sidePanelViewModel;
        public RecipeBookSidePanelViewModel SidePanelViewModel
        {
            get => _sidePanelViewModel;
            set
            {
                _sidePanelViewModel = value;
                OnPropertyChanged();
            }
        }

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
            // Initialize database and view models
            _context = new ApplicationDbContext();
            _sidePanelViewModel = new RecipeBookSidePanelViewModel(this);
            _sidePanelViewModel.NavigationChanged += HandleNavigationChanged;

            // Load initial data
            LoadRecipes();

            // Initialize category counts with flag-based navigation types
            UpdatePrimaryCounts(_recipes, NavigationType.MealType);
            UpdatePrimaryCounts(_recipes, NavigationType.MainIngredient);

            // Initialize commands
            // Side panel commands
            ShowAllRecipesCommand = new RelayCommand(ShowAllRecipes);
            ShowFavoritesCommand = new RelayCommand(ShowFavorites);
            ShowRecentCommand = new RelayCommand(ShowRecent);

            // Recipe List commands
            ToggleFavoriteCommand = new RelayCommand<Recipe>(r => ToggleFavorite(r.Id));

            // Edit Recipe commands
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

            // Update counts in side panel
            

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

        #region Add Meal

        /// <summary>
        /// Notifies the UI that meal plan has been updated
        /// </summary>
        public void NotifyMealPlanChanged()
        {
            // Update UI elements
            OnPropertyChanged(nameof(FilteredRecipes));
        }

        #endregion

        #region Sido panel

        // Quick Access buttons
        public void ShowAllRecipes()
        {
            // Reset navigation state to AllRecipes
            _selectedNavigation = NavigationType.AllRecipes;

            // Reset all navigation state flags
            _isStartedWithMealType = false;
            _isStartedWithIngredient = false;
            _currentMealTypeFilter = null;
            _currentMainIngredientFilter = null;
            _selectedMealType = null;
            _selectedMainIngredient = null;

            LoadRecipes();
            FilteredRecipes = new ObservableCollection<Recipe>(_recipes);
            UpdatePrimaryCounts(_recipes, NavigationType.AllRecipes);
        }

        public void ShowFavorites()
        {
            // Update navigation state to include Favorites
            _selectedNavigation = NavigationType.Favorites; // Changed from |= to = to clear other flags

            // Set filter flags
            _showOnlyFavorites = true;
            _showOnlyRecent = false;

            // Show all favorites without category filters
            var favorites = _recipes.Where(r => r.IsFavorite);

            FilteredRecipes = new ObservableCollection<Recipe>(favorites);
            UpdateSidePanelCounts(favorites);
        }

        public void ShowRecent()
        {
            // Update navigation state to RecentlyAdded
            var recentRecipes = _recipes.OrderByDescending(r => r.CreatedDate)
                                       .Take(10)
                                       .ToList();
            FilteredRecipes = new ObservableCollection<Recipe>(recentRecipes);
            UpdatePrimaryCounts(recentRecipes, NavigationType.RecentlyAdded);
        }

        #endregion

        #region Favorit Toogle
        /// <summary>
        /// Toggles the favorite status of a recipe and updates the UI accordingly
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to toggle favorite status</param>
        /// <returns>The updated recipe with complete data</returns>
        public Recipe ToggleFavorite(int recipeId)
        {
            // Update favorite status in database
            using (var context = new ApplicationDbContext())
            {
                // Store current navigation state
                var currentNavigation = _selectedNavigation;
                bool wasInFavorites = (currentNavigation & NavigationType.Favorites) == NavigationType.Favorites;

                // Update favorite status
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

                // Refresh recipe list
                LoadRecipes();

                // Apply appropriate filters based on original view
                if (wasInFavorites)
                {
                    _selectedNavigation = currentNavigation;
                    ApplyFavoritesWithCategories();
                }
                else
                {
                    ApplyFilters();
                }

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

        #region Counts 
        /// <summary>
        /// Updates the recipe counts based on navigation state using flags. 
        /// Handles both single and combined category selections.
        /// </summary>
        /// <param name="recipes">Collection of recipes to count from</param>
        public void UpdateSidePanelCounts(IEnumerable<Recipe> recipes)
        {
            bool isMealTypeActive = (_selectedNavigation & NavigationType.MealType) == NavigationType.MealType;
            bool isIngredientActive = (_selectedNavigation & NavigationType.MainIngredient) == NavigationType.MainIngredient;
            bool isFavoritesActive = (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;

            // Skip count updates if Favorites is active
            if (isFavoritesActive)
                return;

            // Only update counts based on primary selection
            if (isMealTypeActive && _isStartedWithMealType)
            {
                // Update only Ingredient counts when MealType is primary
                var ingredientGroups = recipes.GroupBy(r => r.MainIngredient)
                                    .ToDictionary(g => g.Key.ToString(), g => g.Count());
                IngredientCounts = ingredientGroups;
                OnPropertyChanged(nameof(IngredientCounts));
            }
            else if (isIngredientActive && _isStartedWithIngredient)
            {
                // Update only MealType counts when Ingredient is primary
                var mealTypeGroups = recipes.GroupBy(r => (MealType)r.MealType)
                                  .ToDictionary(g => g.Key, g => g.Count());
                MealTypeCounts = mealTypeGroups;
                OnPropertyChanged(nameof(MealTypeCounts));
            }
            else
            {
                // Update both when no specific category is active
                UpdatePrimaryCounts(recipes, NavigationType.AllRecipes);
            }
        }

        /// <summary>
        /// Updates primary category counts based on navigation flags.
        /// Handles AllRecipes, MealType, and MainIngredient states.
        /// </summary>
        /// <param name="recipes">Collection of recipes to count from</param>
        /// <param name="primaryType">The navigation type determining which counts to update</param>
        private void UpdatePrimaryCounts(IEnumerable<Recipe> recipes, NavigationType primaryType)
        {
            if (recipes == null)
                return;

            if (primaryType == NavigationType.AllRecipes)
            {
                var mealTypeGroups = recipes.GroupBy(r => (MealType)r.MealType)
                                           .ToDictionary(g => g.Key, g => g.Count());
                var ingredientGroups = recipes.GroupBy(r => r.MainIngredient)
                                             .ToDictionary(g => g.Key.ToString(), g => g.Count());

                MealTypeCounts = mealTypeGroups;
                IngredientCounts = ingredientGroups;
            }
            else if ((primaryType & NavigationType.MealType) == NavigationType.MealType)
            {
                var mealTypeGroups = recipes.GroupBy(r => (MealType)r.MealType)
                                           .ToDictionary(g => g.Key, g => g.Count());
                MealTypeCounts = mealTypeGroups;
            }
            else if ((primaryType & NavigationType.MainIngredient) == NavigationType.MainIngredient)
            {
                var ingredientGroups = recipes.GroupBy(r => r.MainIngredient)
                                             .ToDictionary(g => g.Key.ToString(), g => g.Count());
                IngredientCounts = ingredientGroups;
            }
        }

        #endregion

        #region Navigation and Filtering

        // Current filter states for meal categories
        public MealTypes? _currentMealTypeFilter;        // Tracks active meal type filter (Breakfast, Lunch, etc.)
        public MainIngredients? _currentMainIngredientFilter;  // Tracks active main ingredient filter

        // Quick access filter flags
        public bool _showOnlyFavorites;      // Controls visibility of favorite recipes
        private bool _showOnlyRecent;        // Controls visibility of recently added recipes

        /// <summary>
        /// Handles navigation changes and applies appropriate filters based on the selected navigation type
        /// </summary>
        /// <param name="navigationType">The type of navigation selected</param>
        private void HandleNavigationChanged(NavigationType navigationType)
        {
            _selectedNavigation = navigationType;

            // Handle individual or combined navigation states
            if ((navigationType & NavigationType.Favorites) == NavigationType.Favorites)
            {
                HandleFavoritesNavigation();
            }

            if ((navigationType & NavigationType.MealType) == NavigationType.MealType)
            {
                HandleMealTypeNavigation(_selectedNavigation);
            }

            if ((navigationType & NavigationType.MainIngredient) == NavigationType.MainIngredient)
            {
                HandleMainIngredientNavigation(_selectedNavigation);
            }

            if (navigationType == NavigationType.AllRecipes)
            {
                HandleAllRecipesNavigation();
            }
        }

        /// <summary>
        /// Resets all filters and shows all recipes
        /// </summary>
        public void HandleAllRecipesNavigation()
        {
            // Reset all navigation states and filters
            _isStartedWithMealType = false;
            _isStartedWithIngredient = false;
            ResetCategoryFilters();
            ResetQuickAccessFilters();

            // Reset and update UI with unfiltered recipes
            FilteredRecipes = new ObservableCollection<Recipe>(_recipes);
            UpdateSidePanelCounts(_recipes);
        }

        /// <summary>
        /// Activates favorites view and applies any active category filters
        /// </summary>
        public void HandleFavoritesNavigation()
        {
            // Set flags for favorites view while maintaining category filters
            _showOnlyFavorites = true;
            _showOnlyRecent = false;

            ApplyFavoritesWithCategories();
        }

        /// <summary>
        /// Applies favorite filters while maintaining any active category selections
        /// </summary>
        public void ApplyFavoritesWithCategories()
        {
            // Start with favorite recipes
            var filtered = _recipes.Where(r => r.IsFavorite);

            // Apply active category filters using navigation flags
            if ((_selectedNavigation & NavigationType.MealType) == NavigationType.MealType && _currentMealTypeFilter.HasValue)
            {
                filtered = filtered.Where(r => r.MealType == _currentMealTypeFilter);
            }

            if ((_selectedNavigation & NavigationType.MainIngredient) == NavigationType.MainIngredient && _currentMainIngredientFilter.HasValue)
            {
                filtered = filtered.Where(r => r.MainIngredient == _currentMainIngredientFilter);
            }

            // Update UI with filtered favorites
            FilteredRecipes = new ObservableCollection<Recipe>(filtered);
            UpdateSidePanelCounts(filtered);
        }

        /// <summary>
        /// Handles navigation to recently added recipes
        /// </summary>
        private void HandleRecentNavigation()
        {
            _selectedNavigation = NavigationType.RecentlyAdded;
            _showOnlyFavorites = false;
            _showOnlyRecent = true;
            ApplyFilters();
        }

        /// <summary>
        /// Handles meal type navigation and maintains existing states
        /// </summary>
        public void HandleMealTypeNavigation(NavigationType incomingState)
        {
            bool isSecondarySelection = _isStartedWithIngredient;
            _currentMealTypeFilter = SelectedMealType;
            _selectedNavigation = incomingState;

            // Maintain primary category when Favorites is active
            if ((_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites)
            {
                if (isSecondarySelection)
                {
                    _selectedNavigation |= NavigationType.MainIngredient;
                    _isStartedWithMealType = false;
                    _isStartedWithIngredient = true;
                }
                else
                {
                    _selectedNavigation |= NavigationType.MealType;
                    _isStartedWithMealType = true;
                    _isStartedWithIngredient = false;
                }
            }
            else if ((_selectedNavigation & NavigationType.MainIngredient) != NavigationType.MainIngredient)
            {
                _isStartedWithMealType = true;
                _isStartedWithIngredient = false;
                _selectedNavigation |= NavigationType.MealType;
            }
            else if (isSecondarySelection)
            {
                _selectedNavigation |= NavigationType.MainIngredient;
                _isStartedWithMealType = false;
                _isStartedWithIngredient = true;
            }

            if ((_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites)
            {
                ApplyFavoritesWithCategories();
            }
            else
            {
                ApplyFilters();
            }
        }

        /// <summary>
        /// Handles main ingredient navigation while maintaining category and favorites states
        /// </summary>
        public void HandleMainIngredientNavigation(NavigationType incomingState)
        {
            _selectedNavigation = incomingState;
            bool isSecondarySelection = _isStartedWithMealType;
            _currentMainIngredientFilter = SelectedMainIngredient;

            if (isSecondarySelection)
            {
                _selectedNavigation |= NavigationType.MealType;
                _isStartedWithMealType = true;
                _isStartedWithIngredient = false;
            }
            else
            {
                _selectedNavigation |= NavigationType.MainIngredient;
                UpdateNavigationState(startWithMealType: false);
            }

            if ((_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites)
            {
                ApplyFavoritesWithCategories();
            }
            else
            {
                ApplyFilters();
            }
        }

        /// <summary>
        /// Resets all category filter states
        /// </summary>
        private void ResetCategoryFilters()
        {
            _currentMealTypeFilter = null;
            _currentMainIngredientFilter = null;
            SelectedMealType = null;
            SelectedMainIngredient = null;
            _selectedNavigation &= ~(NavigationType.MealType | NavigationType.MainIngredient);
        }

        /// <summary>
        /// Resets quick access filter states
        /// </summary>
        private void ResetQuickAccessFilters()
        {
            _showOnlyFavorites = false;
            _showOnlyRecent = false;
            _selectedNavigation &= ~(NavigationType.Favorites | NavigationType.RecentlyAdded);
        }

        /// <summary>
        /// Updates the navigation state based on primary category selection
        /// </summary>
        /// <param name="startWithMealType">True if MealType is primary, false if MainIngredient is primary</param>
        private void UpdateNavigationState(bool startWithMealType)
        {
            _isStartedWithMealType = startWithMealType;
            _isStartedWithIngredient = !startWithMealType;
        }

        #endregion

        #region Filter for Side panel

        /// <summary>
        /// Applies all active filters to the recipe collection and updates the UI
        /// </summary>
        public void ApplyFilters()
        {
            var filtered = _recipes.AsEnumerable();

            // Check for reset state using navigation flags
            bool isResetState = _selectedNavigation == NavigationType.AllRecipes;

            if (isResetState)
            {
                FilteredRecipes = new ObservableCollection<Recipe>(_recipes);
                UpdateSidePanelCounts(_recipes);

                return;
            }

            // Apply primary category filters and update counts only for primary selection
            if ((_selectedNavigation & NavigationType.MealType) == NavigationType.MealType && _isStartedWithMealType)
            {
                if (_currentMealTypeFilter.HasValue)
                {
                    filtered = filtered.Where(r => r.MealType == _currentMealTypeFilter);

                    // Update counts before applying secondary filter
                    UpdateSidePanelCounts(filtered);

                    // Apply secondary filter without updating counts
                    if (_currentMainIngredientFilter.HasValue)
                    {
                        filtered = filtered.Where(r => r.MainIngredient == _currentMainIngredientFilter);
                    }
                }
            }
            // Similar logic for MainIngredient as primary
            else if ((_selectedNavigation & NavigationType.MainIngredient) == NavigationType.MainIngredient && _isStartedWithIngredient)
            {
                if (_currentMainIngredientFilter.HasValue)
                {
                    filtered = filtered.Where(r => r.MainIngredient == _currentMainIngredientFilter);
                    UpdateSidePanelCounts(filtered);

                    // Apply secondary meal type filter
                    if (_currentMealTypeFilter.HasValue)
                    {
                        filtered = filtered.Where(r => r.MealType == _currentMealTypeFilter);
                    }
                }
            }

            // Apply quick access filters
            if ((_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites)
            {
                filtered = filtered.Where(r => r.IsFavorite);
            }

            if ((_selectedNavigation & NavigationType.RecentlyAdded) == NavigationType.RecentlyAdded)
            {
                filtered = filtered.OrderByDescending(r => r.CreatedDate).Take(10);
            }

            // Update UI with filtered results
            FilteredRecipes = new ObservableCollection<Recipe>(filtered);
        }

        #endregion

        /// <summary>
        /// Executes search based on current text and filters
        /// </summary>
        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Reset to current filter state
                ApplyFilters();
                return;
            }

            var searchQuery = SearchText.ToLower();
            var searchResults = FilteredRecipes
                .Where(recipe =>
                    recipe.Name.ToLower().Contains(searchQuery) ||
                    recipe.Description.ToLower().Contains(searchQuery) ||
                    recipe.Ingredients.Any(i => i.Name.ToLower().Contains(searchQuery))
                );

            FilteredRecipes = new ObservableCollection<Recipe>(searchResults);
            UpdateSidePanelCounts(FilteredRecipes);
        }
    }
}
