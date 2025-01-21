using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.ViewModels;
using System.Windows;

namespace Projektledningsverktyg.Views.RecipeBook.Windows
{
    public partial class AddRecipeWindow : Window
    {
        private readonly RecipeViewModel _viewModel;
        public AddRecipeWindow()
        {
            InitializeComponent();
            _viewModel = new RecipeViewModel(new ApplicationDbContext());
            DataContext = _viewModel;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

    }
}
