using Projektledningsverktyg.Views.AIAssistant;
using Projektledningsverktyg.Views.Calendar;
using Projektledningsverktyg.Views.Dashboard;
using Projektledningsverktyg.Views.Members;
using Projektledningsverktyg.Views.Settings;
using Projektledningsverktyg.Views.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projektledningsverktyg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Starta med Dashboard som default vy
            MainFrame.Navigate(new DashboardView());
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardView());
        }

        private void BtnCalendar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CalendarView());
        }

        private void BtnTasks_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TasksView());
        }

        private void BtnMembers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MembersView());
        }

        private void BtnAIAssistant_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AIAssistantView());
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsView());
        }
    }
}
