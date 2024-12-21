using Projektledningsverktyg.Data.Context;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Projektledningsverktyg.Data.Entities;

namespace Projektledningsverktyg.Views.Auth
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DragMove();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate all fields
            if (string.IsNullOrEmpty(EmailTextBox.Text.Trim()))
            {
                EmailErrorMessage.Text = "E-post måste anges";
                EmailErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrEmpty(FirstNameTextBox.Text.Trim()))
            {
                FirstNameErrorMessage.Text = "Förnamn måste anges";
                FirstNameErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrEmpty(LastNameTextBox.Text.Trim()))
            {
                LastNameErrorMessage.Text = "Efternamn måste anges";
                LastNameErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (PasswordBox.Password.Length < 8)
            {
                PasswordErrorMessage.Text = "Lösenordet måste innehålla minst 8 tecken";
                PasswordErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ConfirmPasswordErrorMessage.Text = "Lösenorden matchar inte";
                ConfirmPasswordErrorMessage.Visibility = Visibility.Visible;
                return;
            }


            // If all validations pass, create new user with hashed password
            using (var db = new ApplicationDbContext())
            {
                var newMember = new Member
                {
                    Email = EmailTextBox.Text.Trim().ToLower(),
                    FirstName = FirstNameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password),
                    IsActive = true,
                    Role = AdminRoleCheckBox.IsChecked == true ? "Admin" : "User"
                };

                db.Members.Add(newMember);
                db.SaveChanges();
            }

            // Navigate back to login
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                var loginScreen = mainWindow.FindName("LoginScreen") as UIElement;
                var registerScreen = mainWindow.FindName("RegisterScreen") as UIElement;

                if (loginScreen != null && registerScreen != null)
                {
                    loginScreen.Visibility = Visibility.Visible;
                    registerScreen.Visibility = Visibility.Collapsed;
                }
            }

        }


        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchToView("Login");
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            bool isValid = IsValidEmail(email);
            bool isEmailAvailable = CheckEmailAvailability(email);

            EmailValidationIcon.Visibility = (isValid && isEmailAvailable) ? Visibility.Visible : Visibility.Collapsed;
            EmailInvalidIcon.Visibility = (isValid && isEmailAvailable) ? Visibility.Collapsed : Visibility.Visible;

            if (string.IsNullOrEmpty(email))
            {
                EmailErrorMessage.Text = "E-postadressen får inte vara tom";
                EmailErrorMessage.Visibility = Visibility.Visible;
            }
            else if (!isValid)
            {
                EmailErrorMessage.Text = "Ogiltig e-postadress";
                EmailErrorMessage.Visibility = Visibility.Visible;
            }
            else if (!isEmailAvailable)
            {
                EmailErrorMessage.Text = "E-postadressen är redan registrerad";
                EmailErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                EmailErrorMessage.Visibility = Visibility.Collapsed;
            }
        }

        private bool CheckEmailAvailability(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return !db.Members.Any(m => m.Email.ToLower() == email.ToLower());
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(firstName))
            {
                FirstNameErrorMessage.Text = "Förnamn krävs";
                FirstNameErrorMessage.Visibility = Visibility.Visible;
                FirstNameInvalidIcon.Visibility = Visibility.Visible;
                FirstNameValidationIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                FirstNameErrorMessage.Visibility = Visibility.Collapsed;
                FirstNameInvalidIcon.Visibility = Visibility.Collapsed;
                FirstNameValidationIcon.Visibility = Visibility.Visible;
            }
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string lastName = LastNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(lastName))
            {
                LastNameErrorMessage.Text = "Efternamn krävs";
                LastNameErrorMessage.Visibility = Visibility.Visible;
                LastNameInvalidIcon.Visibility = Visibility.Visible;
                LastNameValidationIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                LastNameErrorMessage.Visibility = Visibility.Collapsed;
                LastNameInvalidIcon.Visibility = Visibility.Collapsed;
                LastNameValidationIcon.Visibility = Visibility.Visible;
            }
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;
            PasswordStrengthBar.Visibility = password.Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            double strength = 0;

            if (password.Length < 8)
            {
                PasswordErrorMessage.Text = "Lösenordet måste innehålla minst 8 tecken";
                PasswordErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                PasswordErrorMessage.Visibility = Visibility.Collapsed;
            }

            if (password.Length >= 8)
                strength += 0.25;
            if (password.Any(char.IsUpper))
                strength += 0.25;
            if (password.Any(char.IsLower))
                strength += 0.25;
            if (password.Any(char.IsDigit))
                strength += 0.25;

            PasswordStrengthBar.Value = strength * 100;

            if (strength < 0.5)
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Red);
            else if (strength < 0.75)
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Orange);
            else
                PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Green);

            // Check confirm password match
            ValidatePasswordMatch();
        }

        private void ValidatePasswordMatch()
        {
            if (!string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                bool passwordsMatch = ConfirmPasswordBox.Password == PasswordBox.Password;
                PasswordMatchIcon.Visibility = passwordsMatch ? Visibility.Visible : Visibility.Collapsed;
                PasswordMismatchIcon.Visibility = passwordsMatch ? Visibility.Collapsed : Visibility.Visible;

                if (!passwordsMatch)
                {
                    ConfirmPasswordErrorMessage.Text = "Lösenorden matchar inte";
                    ConfirmPasswordErrorMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    ConfirmPasswordErrorMessage.Visibility = Visibility.Collapsed;
                }
            }
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

        private bool _isConfirmPasswordVisible = false;
        private string _confirmPassword = string.Empty;

        private void ShowConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            if (_isConfirmPasswordVisible)
            {
                _confirmPassword = ConfirmPasswordBox.Password;
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordVisibilityBox.Text = _confirmPassword;
                ConfirmPasswordVisibilityBox.Visibility = Visibility.Visible;
                ShowConfirmPasswordButton.Content = "🔒";
            }
            else
            {
                ConfirmPasswordBox.Password = ConfirmPasswordVisibilityBox.Text;
                ConfirmPasswordBox.Visibility = Visibility.Visible;
                ConfirmPasswordVisibilityBox.Visibility = Visibility.Collapsed;
                ShowConfirmPasswordButton.Content = "👁";
            }
        }

        private void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            bool passwordsMatch = ConfirmPasswordBox.Password == PasswordBox.Password;

            PasswordMatchIcon.Visibility = passwordsMatch ? Visibility.Visible : Visibility.Collapsed;
            PasswordMismatchIcon.Visibility = passwordsMatch ? Visibility.Collapsed : Visibility.Visible;
        }


    }
}
