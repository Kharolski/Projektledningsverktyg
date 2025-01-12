using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class MealsTab : Page
    {
        public MealsTab()
        {
            InitializeComponent();
            var listSection = (ListMealsSection)FindName("ListMealsSection");
            var addSection = (AddMealSection)FindName("AddMealSection");

            if (listSection != null && addSection != null)
            {
                addSection.Initialize(listSection.ViewModel);
            }
        }
    }
}
