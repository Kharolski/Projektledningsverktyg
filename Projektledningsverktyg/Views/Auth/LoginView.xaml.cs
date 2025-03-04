using Projektledningsverktyg.Data.Context;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Task = System.Threading.Tasks.Task;

namespace Projektledningsverktyg.Views.Auth
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            LoadSavedUsername();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DragMove();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingSpinner.Visibility = Visibility.Visible;
            ErrorMessage.Visibility = Visibility.Collapsed;

            await Task.Delay(1000);     // Simulate network delay

            using (var db = new ApplicationDbContext())
            {
                var member = db.Members
                    .Select(m => new
                    {
                        m.Id,
                        m.Email,
                        m.PasswordHash,
                        m.IsActive
                    })
                    .FirstOrDefault(m =>
                        m.Email == UsernameTextBox.Text.Trim().ToLower());

                bool isValidLogin = false;

                if (member != null)
                {
                    if (member.IsActive == false)
                    {
                        ErrorMessage.Text = "Detta konto är inaktiverat. Kontakta administratören.";
                        ErrorMessage.Visibility = Visibility.Visible;
                        var shakeAnimation = (Storyboard)FindResource("ShakeAnimation");
                        shakeAnimation.Begin();
                    }
                    else if (BCrypt.Net.BCrypt.Verify(PasswordBox.Password, member.PasswordHash))
                    {
                        isValidLogin = true;
                        SaveUsername();

                        var storyboard = (Storyboard)FindResource("FadeOut");
                        storyboard.Completed += (s, _) =>
                        {
                            var mainWindow = Window.GetWindow(this) as MainWindow;
                            if (mainWindow != null)
                            {
                                var loginScreen = mainWindow.FindName("LoginScreen") as UIElement;
                                var mainContent = mainWindow.FindName("MainContent") as UIElement;
                                var kalenderButton = mainWindow.FindName("BtnCalendar") as RadioButton;

                                if (loginScreen != null && mainContent != null && kalenderButton != null)
                                {
                                    loginScreen.Visibility = Visibility.Collapsed;
                                    mainContent.Visibility = Visibility.Visible;
                                    kalenderButton.IsChecked = true;
                                }

                                mainWindow.WindowStyle = WindowStyle.None;
                                mainWindow.Height = 750;
                                mainWindow.Width = 1150;
                            }
                        };
                        storyboard.Begin(this);
                    }
                }
                // If login successful, get full member data for App.CurrentUser
                if (isValidLogin)
                {
                    var currentUser = db.Members.Find(member.Id);
                    App.CurrentUser = currentUser;

                    // Ensure the current member is properly set and available throughout our application
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow?.SetCurrentMember(currentUser);
                    mainWindow.SwitchToView("MainWindow");
                }

                if (!isValidLogin)
                {
                    ErrorMessage.Text = "Fel e-post eller lösenord";
                    ErrorMessage.Visibility = Visibility.Visible;
                    var shakeAnimation = (Storyboard)FindResource("ShakeAnimation");
                    shakeAnimation.Begin();
                }
            }

            LoadingSpinner.Visibility = Visibility.Collapsed;

        }


        private void Input_TextChanged(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }

        // For RememberMeCheckBox
        private void LoadSavedUsername()
        {
            string savedUsername = Properties.Settings.Default.SavedUsername;
            if (!string.IsNullOrEmpty(savedUsername))
            {
                UsernameTextBox.Text = savedUsername;
                RememberMeCheckBox.IsChecked = true;
            }
        }

        // For RememberMeCheckBox
        private void SaveUsername()
        {
            if (RememberMeCheckBox.IsChecked == true)
            {
                Properties.Settings.Default.SavedUsername = UsernameTextBox.Text;
            }
            else
            {
                Properties.Settings.Default.SavedUsername = "";
            }
            Properties.Settings.Default.Save();
        }

        private bool _isPasswordVisible = false;
        private string _password = string.Empty;

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            if (_isPasswordVisible)
            {
                _password = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordVisibilityBox.Text = _password;
                PasswordVisibilityBox.Visibility = Visibility.Visible;
                ShowPasswordButton.Content = "🔒";
            }
            else
            {
                PasswordBox.Password = PasswordVisibilityBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordVisibilityBox.Visibility = Visibility.Collapsed;
                ShowPasswordButton.Content = "👁";
            }
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchToView("Register");
        }

        private void ForgotPasswordLink_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchToView("ForgotPassword");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
