using Projektledningsverktyg.ViewModels;
using System.Windows;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class AddIngredientsWindow : Window
    {
        public AddIngredientsWindow(int mealId)
        {
            InitializeComponent();
            var viewModel = new AddIngredientsWindowViewModel(mealId);
            viewModel.CloseWindow = () => this.Close();
            DataContext = viewModel;
        }
    }
}
