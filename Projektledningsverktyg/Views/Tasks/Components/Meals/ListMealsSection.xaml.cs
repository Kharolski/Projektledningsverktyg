using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class ListMealsSection : UserControl
    {
        public ListMealsSectionViewModel ViewModel { get; }
        public ListMealsSection()
        {
            InitializeComponent();

            ViewModel = new ListMealsSectionViewModel();
            DataContext = ViewModel;
            ListMealsSectionViewModel.MealsUpdated += () => ViewModel.RefreshMeals();
        }
    }
}
