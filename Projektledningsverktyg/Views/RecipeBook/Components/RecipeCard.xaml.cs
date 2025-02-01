using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.RecipeBook.Components
{
    public partial class RecipeCard : UserControl
    {
        private readonly ApplicationDbContext _context;
        public event Action RecipeFavoriteChanged;
        public event Action RecipeDeleted;
        
        public RecipeCard()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
        }

        public static readonly DependencyProperty RecipeProperty =
        DependencyProperty.Register("Recipe", typeof(Recipe), typeof(RecipeCard));

        public Recipe Recipe
        {
            get => (Recipe)GetValue(RecipeProperty);
            set => SetValue(RecipeProperty, value);
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            var recipe = (Recipe)DataContext;
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is RecipeBookView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var recipeBookVM = ((RecipeBookView)parent).DataContext as RecipeBookViewModel;
            recipeBookVM?.ToggleFavorite(recipe.Id);
        }

        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is RecipeBookView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            var recipeBookVM = ((RecipeBookView)parent).DataContext as RecipeBookViewModel;

            var detailsView = new RecipeDetailsView(recipeBookVM);

            detailsView.RecipeDeleted += () => recipeBookVM?.LoadRecipes();
            detailsView.RecipeFavoriteChanged += () => recipeBookVM?.LoadRecipes();

            detailsView.LoadRecipe(Recipe);

            var window = new Window
            {
                Content = detailsView,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 550,
                SizeToContent = SizeToContent.Height,
                Title = Recipe.Name
            };

            window.ShowDialog();
        }

        public static readonly DependencyProperty IsInMealPlanProperty =
    DependencyProperty.Register("IsInMealPlan", typeof(bool), typeof(RecipeCard));

        public bool IsInMealPlan
        {
            get { return (bool)GetValue(IsInMealPlanProperty); }
            set { SetValue(IsInMealPlanProperty, value); }
        }
        private void MealPlanButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the current recipe from DataContext
            var recipe = (Recipe)DataContext;

            // Set up date range for meal plan (today through next week)
            var today = DateTime.Today;
            var weekFromNow = today.AddDays(7);

            using (var newContext = new ApplicationDbContext())
            {
                // Check if recipe already exists in meal plan for this week
                var existingMeal = newContext.Meals
                    .FirstOrDefault(m => m.Name == recipe.Name &&
                                   m.Date >= today &&
                                   m.Date <= weekFromNow);

                if (existingMeal != null)
                {
                    // Recipe found - remove it from meal plan
                    newContext.Meals.Remove(existingMeal);
                }
                else
                {
                    // Recipe not found - add it to meal plan
                    var newMeal = new Meal
                    {
                        Name = recipe.Name,
                        Notes = recipe.Description,
                        Date = DateTime.Today,
                        Type = (MealType)recipe.MealType
                    };
                    newContext.Meals.Add(newMeal);
                }

                // Save changes to database
                newContext.SaveChanges();
            }

            // Update UI to show new meal plan status
            SetValue(IsInMealPlanProperty, !IsInMealPlan);

            // Find the parent RecipeBookView to access ViewModel
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is RecipeBookView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            // Get ViewModel and refresh recipe list
            var recipeBookVM = ((RecipeBookView)parent).DataContext as RecipeBookViewModel;
            recipeBookVM?.LoadRecipes();
        }
    }
}
