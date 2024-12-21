using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Services;
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
    /// Interaction logic for ForgotPasswordView.xaml
    /// </summary>
    public partial class ForgotPasswordView : UserControl
    {
        public ForgotPasswordView()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DragMove();
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            bool isValid = IsValidEmail(email);
            bool emailExists = CheckEmailExists(email);

            EmailValidationIcon.Visibility = (isValid && emailExists) ?
                Visibility.Visible : Visibility.Collapsed;
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

        private bool CheckEmailExists(string email)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Members.Any(m => m.Email.ToLower() == email.ToLower());
            }
        }

        private async void SendResetLink_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(email))
            {
                EmailErrorMessage.Text = "Vänligen ange din e-postadress.";
                EmailErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (!IsValidEmail(email))
            {
                EmailErrorMessage.Text = "Ogiltig e-postadress.";
                EmailErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            if (!CheckEmailExists(email))
            {
                EmailErrorMessage.Text = "E-postadressen finns inte registrerad.";
                EmailErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            var emailService = new EmailService();
            string resetToken = GenerateResetToken();

            try
            {
                await emailService.SendPasswordResetEmailAsync(email, resetToken);
                EmailErrorMessage.Visibility = Visibility.Collapsed;
                SuccessMessage.Text = "En återställningslänk har skickats till din e-post.";
                SuccessMessage.Visibility = Visibility.Visible;

                // Wait 2 seconds then return to login
                await System.Threading.Tasks.Task.Delay(2000);
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.SwitchToView("Login");
            }
            catch
            {
                EmailErrorMessage.Text = "Kunde inte skicka återställningslänk. Försök igen.";
                EmailErrorMessage.Visibility = Visibility.Visible;
            }
        }

        public string GenerateResetToken()
        {
            var token = Guid.NewGuid().ToString();
            using (var db = new ApplicationDbContext())
            {
                db.PasswordResets.Add(new PasswordReset
                {
                    Token = token,
                    Email = EmailTextBox.Text.ToLower(),
                    ExpirationDate = DateTime.Now.AddHours(24),
                    IsUsed = false
                });
                db.SaveChanges();
            }
            return token;
        }


        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Clear form
            EmailTextBox.Text = "";
            EmailErrorMessage.Visibility = Visibility.Collapsed;
            EmailValidationIcon.Visibility = Visibility.Collapsed;
            SuccessMessage.Visibility = Visibility.Collapsed;

            // Navigate back using new method
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.SwitchToView("Login");
        }

    }
}
