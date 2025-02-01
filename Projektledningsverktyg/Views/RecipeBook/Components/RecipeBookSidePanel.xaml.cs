using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.RecipeBook.Components
{
    /// <summary>
    /// Interaction logic for RecipeBookSidePanel.xaml
    /// </summary>
    public partial class RecipeBookSidePanel : UserControl
    {
        public RecipeBookSidePanel()
        {
            InitializeComponent();
        }

        public void Initialize(RecipeBookViewModel mainViewModel)
        {
            DataContext = new RecipeBookSidePanelViewModel(mainViewModel);
        }
    }
}
