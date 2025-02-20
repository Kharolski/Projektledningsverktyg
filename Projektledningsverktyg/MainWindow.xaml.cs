using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Views.AIAssistant;
using Projektledningsverktyg.Views.Calendar;
using Projektledningsverktyg.Views.Dashboard;
using Projektledningsverktyg.Views.Members;
using Projektledningsverktyg.Views.RecipeBook;
using Projektledningsverktyg.Views.Settings;
using Projektledningsverktyg.Views.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Projektledningsverktyg
{
    public partial class MainWindow : Window
    {
        private Member _currentMember;
        public MainWindow()
        {
            InitializeComponent();

            // Starta med Dashboard som default vy
            MainFrame.Navigate(new DashboardView());
        }

        // Set current member after successful login
        public void SetCurrentMember(Member member)
        {
            _currentMember = member;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void SwitchToView(string viewName)
        {
            // Hide all views
            LoginScreen.Visibility = Visibility.Collapsed;
            RegisterScreen.Visibility = Visibility.Collapsed;
            ForgotPasswordScreen.Visibility = Visibility.Collapsed;
            ResetPasswordScreen.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;

            // Show requested view
            switch (viewName)
            {
                case "Login":
                    LoginScreen.Visibility = Visibility.Visible;
                    break;
                case "Register":
                    RegisterScreen.Visibility = Visibility.Visible;
                    break;
                case "ForgotPassword":
                    ForgotPasswordScreen.Visibility = Visibility.Visible;
                    break;
                case "ResetPassword":
                    ResetPasswordScreen.Visibility = Visibility.Visible;
                    break;
                case "MainWindow":
                    MainContent.Visibility = Visibility.Visible;
                    break;
            }

            // Hantera titelraden
            TitleBar.Visibility = (viewName == "MainWindow") ? Visibility.Visible : Visibility.Collapsed;
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
            //Debug.WriteLine($"Current User: {App.CurrentUser?.Email}");
            if (_currentMember != null)
            {
                MainFrame.Navigate(new TasksView(App.CurrentUser));
            }
        }

        private void BtnMembers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MembersView());
        }

        private void BtnAIAssistant_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AIAssistantView());
        }

        private void BtnRecipeBook_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RecipeBookView());
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsView());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Reset window size and style
            WindowStyle = WindowStyle.None;
            Height = 700;  // Perfect height for login form
            Width = 500;

            // Clear navigation buttons
            BtnDashboard.IsChecked = false;
            BtnCalendar.IsChecked = false;
            BtnTasks.IsChecked = false;
            BtnMembers.IsChecked = false;
            BtnAIAssistant.IsChecked = false;
            BtnSettings.IsChecked = false;

            // Create fresh login view
            LoginScreen = new Views.Auth.LoginView();
            AuthContainer.Children.Clear();
            AuthContainer.Children.Add(LoginScreen);
            AuthContainer.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            TitleBar.Visibility = Visibility.Collapsed;
        }

        // titelraden
        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private async void ShowMaximizeOptions(object sender, MouseEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(300); // Vänta 300 ms innan menyn visas (kan justeras)
            MaximizeMenu.IsOpen = true; // Öppna menyn automatiskt
        }

        private void SetFullScreen(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void SetLeftHalf(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.WorkArea.Width / 2;
            this.Height = SystemParameters.WorkArea.Height;
        }

        private void SetRightHalf(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Left = SystemParameters.WorkArea.Width / 2;
            this.Top = 0;
            this.Width = SystemParameters.WorkArea.Width / 2;
            this.Height = SystemParameters.WorkArea.Height;
        }

        private void RestoreWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Width = 800;  // Standardbredd
            this.Height = 600; // Standardhöjd
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Gör så att man kan dra fönstret när man klickar på titelraden
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

    }
}
