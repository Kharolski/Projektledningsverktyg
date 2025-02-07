using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class ListMealsSection : UserControl
    {
        public ListMealsSectionViewModel ViewModel { get; }
        public ListMealsSection()
        {
            ViewModel = new ListMealsSectionViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            ListMealsSectionViewModel.MealsUpdated += () => ViewModel.RefreshMeals();
        }
    }
}
