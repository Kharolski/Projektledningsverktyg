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

namespace Projektledningsverktyg.Views.Auth
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Dölj login och visa huvudinnehållet
                var loginScreen = mainWindow.FindName("LoginScreen") as UIElement;
                var mainContent = mainWindow.FindName("MainContent") as UIElement;
                var dashboardButton = mainWindow.FindName("BtnDashboard") as RadioButton;

                if (loginScreen != null && mainContent != null && dashboardButton != null)
                {
                    loginScreen.Visibility = Visibility.Collapsed;
                    mainContent.Visibility = Visibility.Visible;
                    dashboardButton.IsChecked = true;
                }

                // Uppdatera fönsteregenskaper
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.Height = 720;
                mainWindow.Width = 1150;
            }
        }
    }
}
