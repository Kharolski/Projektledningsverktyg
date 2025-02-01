using Projektledningsverktyg.ViewModels;
using Projektledningsverktyg.Views.RecipeBook.Windows;
using System.Windows;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.RecipeBook
{
    public partial class RecipeBookView : Page
    {
        public RecipeBookView()
        {
            var mainViewModel = new RecipeBookViewModel();
            DataContext = mainViewModel;
            InitializeComponent();
            SidePanel.Initialize(mainViewModel);
        }

        private void OpenAddRecipeWindow(object sender, RoutedEventArgs e)
        {
            var addRecipeWindow = new AddRecipeWindow(DataContext as RecipeBookViewModel);
            addRecipeWindow.ShowDialog();
        }

    }
}
