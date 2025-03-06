using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Views.Calendar;
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
        private double _previousWidth = 800;
        private double _previousHeight = 600;
        private double _previousLeft = 0;
        private double _previousTop = 0;
        private WindowState _previousWindowState = WindowState.Normal;

        private Member _currentMember;
        public MainWindow()
        {
            InitializeComponent();

            // Starta med Calendar som default vy
            MainFrame.Navigate(new CalendarView());
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

            // Hantera AuthContainer synlighet 
            AuthContainer.Visibility = (viewName == "MainWindow") ? Visibility.Collapsed : Visibility.Visible;

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
            // Rensa currentUser
            App.CurrentUser = null;
            _currentMember = null;

            // Reset window size
            Height = 700;
            Width = 500;

            // Clear navigation buttons
            BtnCalendar.IsChecked = false;
            BtnTasks.IsChecked = false;
            BtnMembers.IsChecked = false;
            BtnSettings.IsChecked = false;

            // Skapa alla autentiseringsvyer på nytt
            LoginScreen = new Views.Auth.LoginView();
            RegisterScreen = new Views.Auth.RegisterView();
            ForgotPasswordScreen = new Views.Auth.ForgotPasswordView();
            ResetPasswordScreen = new Views.Auth.ResetPasswordView();

            // Rensa och lägg till alla vyer i containern
            AuthContainer.Children.Clear();
            AuthContainer.Children.Add(LoginScreen);
            AuthContainer.Children.Add(RegisterScreen);
            AuthContainer.Children.Add(ForgotPasswordScreen);
            AuthContainer.Children.Add(ResetPasswordScreen);

            // Använd SwitchToView för att visa login
            SwitchToView("Login");
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
            await System.Threading.Tasks.Task.Delay(500); // Vänta 300 ms innan menyn visas (kan justeras)
            MaximizeMenu.IsOpen = true; // Öppna menyn automatiskt
        }

        private void SaveCurrentWindowState()
        {
            if (WindowState == WindowState.Normal)
            {
                _previousWidth = Width;
                _previousHeight = Height;
                _previousLeft = Left;
                _previousTop = Top;
                _previousWindowState = WindowState;
            }
        }

        private void SetFullScreen(object sender, RoutedEventArgs e)
        {
            SaveCurrentWindowState();
            this.WindowState = WindowState.Maximized;
        }

        private void SetLeftHalf(object sender, RoutedEventArgs e)
        {
            SaveCurrentWindowState();
            this.WindowState = WindowState.Normal;
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.WorkArea.Width / 2;
            this.Height = SystemParameters.WorkArea.Height;
        }

        private void SetRightHalf(object sender, RoutedEventArgs e)
        {
            SaveCurrentWindowState();
            this.WindowState = WindowState.Normal;
            this.Left = SystemParameters.WorkArea.Width / 2;
            this.Top = 0;
            this.Width = SystemParameters.WorkArea.Width / 2;
            this.Height = SystemParameters.WorkArea.Height;
        }

        private void RestoreWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = _previousWindowState;
            this.Width = _previousWidth;
            this.Height = _previousHeight;
            this.Left = _previousLeft;
            this.Top = _previousTop;
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
