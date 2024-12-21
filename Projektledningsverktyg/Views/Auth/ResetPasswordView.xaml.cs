using Projektledningsverktyg.Data.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ResetPasswordView.xaml
    /// </summary>
    public partial class ResetPasswordView : UserControl
    {
        private string _resetToken;

        public ResetPasswordView()
        {
            InitializeComponent();

        }

        public ResetPasswordView(string resetToken) : this()
        {
            _resetToken = resetToken;
        }

        public void SetResetToken(string token)
        {
            _resetToken = token;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.DragMove();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidatePasswords();
        }

        private bool ValidatePasswords()
        {
            if (NewPasswordBox.Password.Length < 6)
            {
                ErrorMessage.Text = "Lösenordet måste vara minst 6 tecken långt";
                ErrorMessage.Visibility = Visibility.Visible;
                return false;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorMessage.Text = "Lösenorden matchar inte";
                ErrorMessage.Visibility = Visibility.Visible;
                return false;
            }

            ErrorMessage.Visibility = Visibility.Collapsed;
            return true;
        }

        private async void SavePassword_Click(object sender, RoutedEventArgs e)
        {
            await SavePassword_ClickAsync(sender, e);
        }

        private async Task SavePassword_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (!ValidatePasswords())
                return;

            using (var db = new ApplicationDbContext())
            {
                var resetRequest = db.PasswordResets.FirstOrDefault(r => r.Token == _resetToken);

                if (resetRequest != null)
                {
                    var member = db.Members.FirstOrDefault(m =>
                        m.Email.ToLower() == resetRequest.Email.ToLower());

                    if (member != null)
                    {
                        member.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPasswordBox.Password);
                        resetRequest.IsUsed = true;
                        db.SaveChanges();

                        SuccessMessage.Text = "Ditt lösenord har uppdaterats!";
                        SuccessMessage.Visibility = Visibility.Visible;

                        await Task.Delay(2000);
                        Window.GetWindow(this).Close();
                    }
                }
            }
        }

        private void ShowNewPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Visibility == Visibility.Visible)
            {
                NewPasswordVisibilityBox.Text = NewPasswordBox.Password;
                NewPasswordBox.Visibility = Visibility.Collapsed;
                NewPasswordVisibilityBox.Visibility = Visibility.Visible;
            }
            else
            {
                NewPasswordBox.Password = NewPasswordVisibilityBox.Text;
                NewPasswordBox.Visibility = Visibility.Visible;
                NewPasswordVisibilityBox.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordBox.Visibility == Visibility.Visible)
            {
                ConfirmPasswordVisibilityBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordVisibilityBox.Visibility = Visibility.Visible;
            }
            else
            {
                ConfirmPasswordBox.Password = ConfirmPasswordVisibilityBox.Text;
                ConfirmPasswordBox.Visibility = Visibility.Visible;
                ConfirmPasswordVisibilityBox.Visibility = Visibility.Collapsed;
            }
        }

    }
}
