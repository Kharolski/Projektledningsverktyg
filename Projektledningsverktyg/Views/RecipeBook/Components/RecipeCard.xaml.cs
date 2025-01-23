using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using System;
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


    }
}
