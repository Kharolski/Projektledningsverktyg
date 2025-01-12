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
            }
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
            Debug.WriteLine($"Current User: {App.CurrentUser?.Email}");
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
        }

    }
}
