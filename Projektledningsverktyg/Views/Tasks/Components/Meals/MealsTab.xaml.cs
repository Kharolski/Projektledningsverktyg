using Projektledningsverktyg.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class MealsTab : Page
    {
        public MealsTab()
        {
            InitializeComponent();
            DataContext = new MealsTabViewModel();

        }

    }
}
