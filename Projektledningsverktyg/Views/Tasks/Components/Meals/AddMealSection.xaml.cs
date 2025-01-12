using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals
{
    public partial class AddMealSection : UserControl
    {
        private AddMealSectionViewModel _viewModel;
        private bool _isInitialized;

        public AddMealSection()
        {
            InitializeComponent();
            _viewModel = new AddMealSectionViewModel();
            DataContext = _viewModel;
        }

        public void Initialize(ListMealsSectionViewModel listViewModel)
        {
            if (!_isInitialized)
            {
                _viewModel = new AddMealSectionViewModel(listViewModel);
                DataContext = _viewModel;
                _isInitialized = true;
            }
        }
    }
}
