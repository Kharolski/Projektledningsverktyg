using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Entities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Diagnostics;

namespace Projektledningsverktyg.ViewModels
{
    public class RecipeBookSidePanelViewModel : INotifyPropertyChanged
    {
        public event Action<NavigationType> NavigationChanged;
        private readonly RecipeBookViewModel _mainViewModel;

        #region Properties

        // MealType counts
        public int FrukostCount => _mainViewModel.MealTypeCounts.TryGetValue((MealType)MealTypes.Frukost, out int count) ? count : 0;
        public int LunchCount => _mainViewModel.MealTypeCounts.TryGetValue((MealType)MealTypes.Lunch, out int count) ? count : 0;
        public int MiddagCount => _mainViewModel.MealTypeCounts.TryGetValue((MealType)MealTypes.Middag, out int count) ? count : 0;
        public int EfterrättCount => _mainViewModel.MealTypeCounts.TryGetValue((MealType)MealTypes.Efterrätt, out int count) ? count : 0;
        public int MellanmålCount => _mainViewModel.MealTypeCounts.TryGetValue((MealType)MealTypes.Mellanmål, out int count) ? count : 0;

        // MainIngredient counts
        public int KöttCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Kött.ToString(), out int count) ? count : 0;
        public int FiskCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Fisk.ToString(), out int count) ? count : 0;
        public int KycklingCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Kyckling.ToString(), out int count) ? count : 0;
        public int PastaCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Pasta.ToString(), out int count) ? count : 0;
        public int VegetarisktCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Vegetariskt.ToString(), out int count) ? count : 0;
        public int VeganskCount => _mainViewModel.IngredientCounts.TryGetValue(MainIngredients.Vegansk.ToString(), out int count) ? count : 0;

        // Notification helpers
        private void NotifyCountsChanged()
        {
            OnPropertyChanged(nameof(FrukostCount));
            OnPropertyChanged(nameof(LunchCount));
            OnPropertyChanged(nameof(MiddagCount));
            OnPropertyChanged(nameof(EfterrättCount));
            OnPropertyChanged(nameof(MellanmålCount));
            OnPropertyChanged(nameof(KöttCount));
            OnPropertyChanged(nameof(FiskCount));
            OnPropertyChanged(nameof(KycklingCount));
            OnPropertyChanged(nameof(PastaCount));
            OnPropertyChanged(nameof(VegetarisktCount));
            OnPropertyChanged(nameof(VeganskCount));
        }
        private void NotifyAllCategorySelections()
        {
            // Notify all MealType selections
            OnPropertyChanged(nameof(IsFrukostSelected));
            OnPropertyChanged(nameof(IsLunchSelected));
            OnPropertyChanged(nameof(IsMiddagSelected));
            OnPropertyChanged(nameof(IsEfterrättSelected));
            OnPropertyChanged(nameof(IsMellanmålSelected));

            // Notify all MainIngredient selections
            OnPropertyChanged(nameof(IsKöttSelected));
            OnPropertyChanged(nameof(IsFiskSelected));
            OnPropertyChanged(nameof(IsKycklingSelected));
            OnPropertyChanged(nameof(IsPastaSelected));
            OnPropertyChanged(nameof(IsVegetarisktSelected));
            OnPropertyChanged(nameof(IsVeganskSelected));
        }

        // Navigation Section with Flags support
        private NavigationType _selectedNavigation;
        public NavigationType SelectedNavigation
        {
            get => _selectedNavigation;
            set
            {
                _selectedNavigation = value;

                // Handle secondary selection with flags
                bool isSecondarySelection =
                    (_mainViewModel.SelectedMealType.HasValue && (value & NavigationType.MainIngredient) == NavigationType.MainIngredient) ||
                    (_mainViewModel.SelectedMainIngredient.HasValue && (value & NavigationType.MealType) == NavigationType.MealType);

                if (!isSecondarySelection)
                {
                    if (value == NavigationType.AllRecipes)
                    {
                        _mainViewModel.SelectedMealType = null;
                        _mainViewModel.SelectedMainIngredient = null;
                        _mainViewModel._isStartedWithMealType = false;
                        _mainViewModel._isStartedWithIngredient = false;
                    }
                    else if ((value & NavigationType.MealType) == NavigationType.MealType)
                    {
                        _mainViewModel.SelectedMainIngredient = null;
                        _mainViewModel._isStartedWithIngredient = false;
                    }
                    else if ((value & NavigationType.MainIngredient) == NavigationType.MainIngredient)
                    {
                        _mainViewModel.SelectedMealType = null;
                        _mainViewModel._isStartedWithMealType = false;
                    }
                }

                OnPropertyChanged(nameof(SelectedNavigation));
                NotifyNavigationStates();
            }
        }

        // Navigation state checks
        public bool IsAllRecipesSelected =>
                        _selectedNavigation == NavigationType.AllRecipes &&
                        !_mainViewModel.SelectedMealType.HasValue &&
                        !_mainViewModel.SelectedMainIngredient.HasValue &&
                        !IsFavoritesSelected &&
                        !IsRecentlyAddedSelected;

        public bool IsFavoritesSelected
        {
            get
            {
                var isSelected = (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;
                return isSelected;
            }
        }
        public bool IsRecentlyAddedSelected => (_selectedNavigation & NavigationType.RecentlyAdded) == NavigationType.RecentlyAdded;

        // Helper method for notifying navigation states
        private void NotifyNavigationStates()
        {
            OnPropertyChanged(nameof(IsAllRecipesSelected));
            OnPropertyChanged(nameof(IsFavoritesSelected));
            OnPropertyChanged(nameof(IsRecentlyAddedSelected));
            NotifyAllCategorySelections();
        }

        // Meal type selection
        public bool IsFrukostSelected => _mainViewModel.SelectedMealType == MealTypes.Frukost;
        public bool IsLunchSelected => _mainViewModel.SelectedMealType == MealTypes.Lunch;
        public bool IsMiddagSelected => _mainViewModel.SelectedMealType == MealTypes.Middag;
        public bool IsEfterrättSelected => _mainViewModel.SelectedMealType == MealTypes.Efterrätt;
        public bool IsMellanmålSelected => _mainViewModel.SelectedMealType == MealTypes.Mellanmål;

        // Main Ingredient selection
        public bool IsKöttSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Kött;
        public bool IsFiskSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Fisk;
        public bool IsKycklingSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Kyckling;
        public bool IsPastaSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Pasta;
        public bool IsVegetarisktSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Vegetariskt;
        public bool IsVeganskSelected => _mainViewModel.SelectedMainIngredient == MainIngredients.Vegansk;

        #endregion

        #region Commands

        // Quick Access Section
        public ICommand AllRecipesCommand { get; private set; }
        public ICommand FavoritesCommand { get; private set; }
        public ICommand RecentlyAddedCommand { get; private set; }

        // MealType Section
        public ICommand FrukostCommand { get; private set; }
        public ICommand LunchCommand { get; private set; }
        public ICommand MiddagCommand { get; private set; }
        public ICommand EfterrättCommand { get; private set; }
        public ICommand MellanmålCommand { get; private set; }

        // Main ingredients Section
        public ICommand KöttCommand { get; private set; }
        public ICommand FiskCommand { get; private set; }
        public ICommand KycklingCommand { get; private set; }
        public ICommand PastaCommand { get; private set; }
        public ICommand VegetarisktCommand { get; private set; }
        public ICommand VeganskCommand { get; private set; }

        #endregion

        #region Constructor

        public RecipeBookSidePanelViewModel(RecipeBookViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            // Quick Access Section Commands
            AllRecipesCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.AllRecipes));
            FavoritesCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.Favorites));
            RecentlyAddedCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.RecentlyAdded));

            // MealType Section Commands
            FrukostCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MealType, MealTypes.Frukost));
            LunchCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MealType, MealTypes.Lunch));
            MiddagCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MealType, MealTypes.Middag));
            EfterrättCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MealType, MealTypes.Efterrätt));
            MellanmålCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MealType, MealTypes.Mellanmål));

            // Main Ingredients Section Commands
            KöttCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Kött));
            FiskCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Fisk));
            KycklingCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Kyckling));
            PastaCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Pasta));
            VegetarisktCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Vegetariskt));
            VeganskCommand = new RelayCommand(() => ExecuteNavigation(NavigationType.MainIngredient, mainIngredient: MainIngredients.Vegansk));
        }

        #endregion

        #region Execute
        
        /// <summary>
        /// Main navigation handler that coordinates all navigation operations
        /// </summary>
        private void ExecuteNavigation(NavigationType navigationType, MealTypes? mealType = null, MainIngredients? mainIngredient = null)
        {
            if (HandleCategoryDeselection(mealType, mainIngredient))
                return;

            if (navigationType == NavigationType.AllRecipes)
            {
                HandleAllRecipesNavigation();
            }
            else if (navigationType == NavigationType.Favorites)
            {
                HandleFavoritesNavigation();
            }
            else if (navigationType == NavigationType.RecentlyAdded)
            {
                HandleRecentlyAddedNavigation();
            }
            else
            {
                HandleCategoryNavigation(navigationType, mealType, mainIngredient);
            }

            UpdateUIState();
        }

        /// <summary>
        /// Handles deselection of meal types and ingredients while preserving favorites state
        /// </summary>
        private bool HandleCategoryDeselection(MealTypes? mealType, MainIngredients? mainIngredient)
        {
            // Check if Favorites is currently active
            bool isFavoritesActive = (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;
            // Check if we have an active MealType or MainIngredient selection
            bool hasMealType = _mainViewModel.SelectedMealType.HasValue;
            bool hasMainIngredient = _mainViewModel.SelectedMainIngredient.HasValue;

            // Check if we're deselecting the currently selected category
            if ((mealType.HasValue && _mainViewModel.SelectedMealType == mealType) ||
                (mainIngredient.HasValue && _mainViewModel.SelectedMainIngredient == mainIngredient))
            {
                if (isFavoritesActive)
                {
                    HandleFavoritesDeselection(mealType, mainIngredient, hasMealType, hasMainIngredient);
                }
                else
                {
                    HandleRegularDeselection(mealType, mainIngredient, hasMealType, hasMainIngredient);
                }

                // 🛠 Se till att vi NOLLSTÄLLER huvudkategori-flaggan om vi avmarkerar den
                if (mealType.HasValue && _mainViewModel.SelectedMealType == null)
                {
                    _mainViewModel._isStartedWithMealType = false;  
                }
                else if (mainIngredient.HasValue && _mainViewModel.SelectedMainIngredient == null)
                {
                    _mainViewModel._isStartedWithIngredient = false;
                }

                // Update UI and notify changes
                NotifyAllCategorySelections();
                NotifyCountsChanged();
                OnPropertyChanged(nameof(IsFavoritesSelected));
                OnPropertyChanged(nameof(IsAllRecipesSelected));
                NavigationChanged?.Invoke(_selectedNavigation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles category deselection when Favorites is active
        /// </summary>
        private void HandleFavoritesDeselection(MealTypes? mealType, MainIngredients? mainIngredient, bool hasMealType, bool hasMainIngredient)
        {
            if (mainIngredient.HasValue && hasMealType)
            {
                // If deselecting MainIngredient and MealType exists:
                // Keep both Favorites and MealType flags active
                ClearCurrentCategory(false); // Clear only MainIngredient
                _selectedNavigation = NavigationType.Favorites | NavigationType.MealType;
                _mainViewModel.HandleMealTypeNavigation(_selectedNavigation);
            }
            else if (mealType.HasValue && hasMainIngredient)
            {
                // If deselecting MealType and MainIngredient exists:
                // Keep both Favorites and MainIngredient flags active
                ClearCurrentCategory(true); // Clear only MealType
                _selectedNavigation = NavigationType.Favorites | NavigationType.MainIngredient;
                _mainViewModel.HandleMainIngredientNavigation(_selectedNavigation);
            }
            else
            {
                // If deselecting the only category:
                // Show all favorites
                ClearCurrentCategory(mealType != null);
                _selectedNavigation = NavigationType.Favorites;
                _mainViewModel.ShowFavorites();
            }
        }

        /// <summary>
        /// Handles category deselection when Favorites is not active
        /// </summary>
        private void HandleRegularDeselection(MealTypes? mealType, MainIngredients? mainIngredient, bool hasMealType, bool hasMainIngredient)
        {
            Debug.WriteLine($"Regular deselection - Current MealType: {_mainViewModel.SelectedMealType}, MainIngredient: {_mainViewModel.SelectedMainIngredient}");

            if (mainIngredient.HasValue && hasMealType)
            {
                // Store current MealType
                var currentMealType = _mainViewModel.SelectedMealType;

                // Clear MainIngredient completely
                ClearCurrentCategory(false);
                _mainViewModel._currentMainIngredientFilter = null;

                // Restore MealType and apply filtering
                _selectedNavigation = NavigationType.MealType;
                _mainViewModel.SelectedMealType = currentMealType;

                _mainViewModel._isStartedWithMealType = true;
                _mainViewModel.HandleMealTypeNavigation(_selectedNavigation);

                Debug.WriteLine($"Filtered recipe count: {_mainViewModel.FilteredRecipes.Count}");
            }
            else if (mealType.HasValue && hasMainIngredient)
            {
                // Store current Ingredient
                var currentIngredient = _mainViewModel.SelectedMainIngredient;

                // Clear MainType completely
                ClearCurrentCategory(true); // Clear only MealType
                _mainViewModel._currentMealTypeFilter = null;

                // Restore MainIngredient and apply filtering
                _selectedNavigation = NavigationType.MainIngredient;
                _mainViewModel.SelectedMainIngredient = currentIngredient;

                _mainViewModel._isStartedWithIngredient = true;
                _mainViewModel.HandleMainIngredientNavigation(_selectedNavigation);
            }
            else
            {
                // If deselecting the only category:
                // Show all recipes
                ClearCurrentCategory(mealType != null);
                _selectedNavigation = NavigationType.AllRecipes;
                _mainViewModel.HandleAllRecipesNavigation();
            }
        }

        /// <summary>
        /// Handles complete reset to all recipes view
        /// </summary>
        private void HandleAllRecipesNavigation()
        {
            ClearAllCategories();
            _selectedNavigation = NavigationType.AllRecipes;
            _mainViewModel.HandleAllRecipesNavigation();
            OnPropertyChanged(nameof(IsFavoritesSelected));
            OnPropertyChanged(nameof(IsRecentlyAddedSelected));
        }

        /// <summary>
        /// Handles favorites toggle while preserving category selections
        /// </summary>
        private void HandleFavoritesNavigation()
        {
            if ((_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites)
            {
                // Turn off favorites
                _selectedNavigation &= ~NavigationType.Favorites;
                if (_mainViewModel.SelectedMealType.HasValue || _mainViewModel.SelectedMainIngredient.HasValue)
                {
                    // Restore category view if active
                    if (_mainViewModel.SelectedMealType.HasValue)
                        _mainViewModel.HandleMealTypeNavigation(_selectedNavigation);
                    else
                        _mainViewModel.HandleMainIngredientNavigation(_selectedNavigation);
                }
                else
                {
                    // Return to all recipes if no category selected
                    _selectedNavigation = NavigationType.AllRecipes;
                    _mainViewModel.HandleAllRecipesNavigation();
                }
            }
            else
            {
                // Turn on favorites and clear other quick access states
                _selectedNavigation = NavigationType.Favorites;
                _mainViewModel.HandleFavoritesNavigation();
            }

            OnPropertyChanged(nameof(IsFavoritesSelected));
            OnPropertyChanged(nameof(IsAllRecipesSelected));
            OnPropertyChanged(nameof(IsRecentlyAddedSelected));
        }

        /// <summary>
        /// Handles recently added recipes toggle and state management
        /// </summary>
        private void HandleRecentlyAddedNavigation()
        {
            if ((_selectedNavigation & NavigationType.RecentlyAdded) == NavigationType.RecentlyAdded)
            {
                _selectedNavigation = NavigationType.AllRecipes;
                _mainViewModel.HandleAllRecipesNavigation();
            }
            else
            {
                // Complete reset of navigation state
                _selectedNavigation = NavigationType.AllRecipes;
                _selectedNavigation = NavigationType.RecentlyAdded;
                _mainViewModel.ShowRecent();
            }

            // Reset counts to show all available recipes
            _mainViewModel.UpdateSidePanelCounts(_mainViewModel.FilteredRecipes);
            OnPropertyChanged(nameof(IsRecentlyAddedSelected));
            OnPropertyChanged(nameof(IsFavoritesSelected));
        }

        /// <summary>
        /// Handles regular category navigation (MealType and MainIngredient)
        /// </summary>
        private void HandleCategoryNavigation(NavigationType navigationType, MealTypes? mealType, MainIngredients? mainIngredient)
        {
            if (HandleCategoryToggleOff(mealType, mainIngredient))
                return;

            _selectedNavigation |= navigationType;
            ApplyNewCategorySelection(mealType, mainIngredient);
        }

        /// <summary>
        /// Updates all UI elements after navigation changes
        /// </summary>
        private void UpdateUIState()
        {
            OnPropertyChanged(nameof(IsAllRecipesSelected));
            NotifyAllCategorySelections();
            NotifyCountsChanged();
            NavigationChanged?.Invoke(_selectedNavigation);
        }


        #endregion

        #region Helper funktions

        /// <summary>
        /// Clears the current category selection (MealType or MainIngredient)
        /// </summary>
        private void ClearCurrentCategory(bool isMealType)
        {
            if (isMealType)
                _mainViewModel.SelectedMealType = null;
            else
                _mainViewModel.SelectedMainIngredient = null;
        }

        /// <summary>
        /// Clears all category selections
        /// </summary>
        private void ClearAllCategories()
        {
            _mainViewModel.SelectedMealType = null;
            _mainViewModel.SelectedMainIngredient = null;
        }

        /// <summary>
        /// Handles toggling off category selections
        /// </summary>
        private bool HandleCategoryToggleOff(MealTypes? mealType, MainIngredients? mainIngredient)
        {
            if ((mealType.HasValue && _mainViewModel.SelectedMealType == mealType) ||
                (mainIngredient.HasValue && _mainViewModel.SelectedMainIngredient == mainIngredient))
            {
                ClearCurrentCategory(mealType.HasValue);
                _selectedNavigation = NavigationType.AllRecipes;
                _mainViewModel.ShowAllRecipes();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles toggling off category selections while maintaining favorites state
        /// </summary>
        private bool HandleCategoryToggleOffWithFavorites(MealTypes? mealType, MainIngredients? mainIngredient)
        {
            // Check if we're toggling off the currently selected category
            if ((mealType.HasValue && _mainViewModel.SelectedMealType == mealType) ||
                (mainIngredient.HasValue && _mainViewModel.SelectedMainIngredient == mainIngredient))
            {
                bool wasFavoritesActive = (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;
                bool hasMealType = _mainViewModel.SelectedMealType.HasValue;
                bool hasIngredient = _mainViewModel.SelectedMainIngredient.HasValue;

                ClearCurrentCategory(mealType.HasValue);

                if (wasFavoritesActive)
                {
                    if (hasMealType)
                    {
                        _selectedNavigation = NavigationType.Favorites | NavigationType.MealType;
                        _mainViewModel.ApplyFavoritesWithCategories();
                    }
                    else if (hasIngredient)
                    {
                        _selectedNavigation = NavigationType.Favorites | NavigationType.MainIngredient;
                        _mainViewModel.ApplyFavoritesWithCategories();
                    }
                    else
                    {
                        _selectedNavigation = NavigationType.Favorites;
                        _mainViewModel.ShowFavorites();
                    }
                }
                else
                {
                    _selectedNavigation = NavigationType.AllRecipes;
                    _mainViewModel.HandleAllRecipesNavigation();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Applies new category selection and resets other navigation states
        /// </summary>
        private void ApplyNewCategorySelection(MealTypes? mealType, MainIngredients? mainIngredient)
        {
            bool wasFavoritesActive = (_selectedNavigation & NavigationType.Favorites) == NavigationType.Favorites;

            // Clear RecentlyAdded state
            _selectedNavigation &= ~NavigationType.RecentlyAdded;

            if (mealType.HasValue)
            {
                _mainViewModel.SelectedMealType = mealType;
                _selectedNavigation |= NavigationType.MealType;
                _mainViewModel.HandleMealTypeNavigation(_selectedNavigation);
            }
            if (mainIngredient.HasValue)
            {
                _mainViewModel.SelectedMainIngredient = mainIngredient;
                _selectedNavigation |= NavigationType.MainIngredient;
                _mainViewModel.HandleMainIngredientNavigation(_selectedNavigation);
            }

            OnPropertyChanged(nameof(IsRecentlyAddedSelected));
        }


        #endregion

        #region Event INotifyPropertyChanged
        /// <summary>
        /// Event for notifying when a property value has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of the property that changed (automatically captured by CallerMemberName)</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
