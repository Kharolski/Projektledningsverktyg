using Projektledningsverktyg.Views.Dashboard;
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

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardView());
        }

        private void BtnTasks_Click(object sender, RoutedEventArgs e)
        {
            // Här kommer vi senare lägga till Tasks-vyn
        }

        private void BtnEvents_Click(object sender, RoutedEventArgs e)
        {
            // Här kommer vi senare lägga till Events-vyn
        }

        private void BtnMembers_Click(object sender, RoutedEventArgs e)
        {
            // Här kommer vi senare lägga till Members-vyn
        }
    }
}
