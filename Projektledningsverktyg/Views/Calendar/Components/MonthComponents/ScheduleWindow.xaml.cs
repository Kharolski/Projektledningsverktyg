using System.Windows;
using System.Windows.Forms;

namespace Projektledningsverktyg.Views.Calendar.Components.MonthComponents
{
    public partial class ScheduleWindow : Window
    {
        public ScheduleWindow(ScheduleListView view)
        {
            InitializeComponent();
            DataContext = view;
        }
    }
}
