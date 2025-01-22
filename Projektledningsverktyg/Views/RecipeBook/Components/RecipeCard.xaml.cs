using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Converters;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.ViewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.RecipeBook.Components
{
    public partial class RecipeCard : UserControl
    {
        private readonly ApplicationDbContext _context;

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
            Recipe.IsFavorite = !Recipe.IsFavorite;

            var dbRecipe = _context.Recipes.Find(Recipe.Id);
            dbRecipe.IsFavorite = Recipe.IsFavorite;
            _context.SaveChanges();

            // Find RecipeBookView in visual tree
            var parent = VisualTreeHelper.GetParent(this);
            while (parent != null && !(parent is RecipeBookView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var recipeBookViewModel = (parent as RecipeBookView).DataContext as RecipeBookViewModel;
            recipeBookViewModel.LoadRecipes();
        }

    }
}
