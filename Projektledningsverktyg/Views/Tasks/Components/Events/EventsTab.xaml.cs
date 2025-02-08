using Projektledningsverktyg.ViewModels;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Events
{
    public partial class EventsTab : Page
    {
        public EventsTab()
        {
            InitializeComponent();
            DataContext = new EventViewModel();

        }

    }

}
