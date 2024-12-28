using Microsoft.Win32;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


using Path = System.IO.Path;


namespace Projektledningsverktyg.Views.Members
{
    /// <summary>
    /// Interaction logic for AddMemberDialog.xaml
    /// </summary>
    public partial class AddMemberDialog : Window
    {
        private PasswordBox _passwordBox;
        private PasswordBox _confirmPasswordBox;
        private TextBlock _passwordMatchIndicator;

        private TextBox _passwordTextBox;
        private TextBox _confirmPasswordTextBox;

        public AddMemberDialog()
        {
            InitializeComponent();
            _passwordBox = this.FindName("PasswordBox") as PasswordBox;
            _confirmPasswordBox = this.FindName("ConfirmPasswordBox") as PasswordBox;
            _passwordMatchIndicator = this.FindName("PasswordMatchIndicator") as TextBlock;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #region Left Field Section

        // Image Section --------------------------------------------------------------
        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                ProfileImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                NoImageText.Visibility = Visibility.Collapsed;
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (NoImageText.Visibility == Visibility.Visible)
            {
                // Choose image
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    ProfileImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                    NoImageText.Visibility = Visibility.Collapsed;
                    ImageButton.Content = "Radera bild";
                }
            }
            else
            {
                // Reset image
                ProfileImage.Source = new BitmapImage(new Uri("/Images/no_image.png", UriKind.Relative));
                NoImageText.Visibility = Visibility.Visible;
                ImageButton.Content = "Välj bild";
            }
        }
        // ---------------------------------------------------------------------------

        // Birthday DatePicker Section --------------------------------------
        private void BirthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BirthDatePicker.SelectedDate.HasValue)
            {
                if (!BirthDatePicker.SelectedDate.HasValue)
                {
                    BirthDateError.Text = "Födelsedatum krävs";
                    BirthDateError.Visibility = Visibility.Visible;
                    BirthDatePicker.BorderBrush = new SolidColorBrush(Colors.Red);
                    return;
                }

                var age = CalculateAge(BirthDatePicker.SelectedDate.Value);

                if (age < 13)
                {
                    BirthDateError.Text = "Du måste vara minst 13 år";
                    BirthDateError.Visibility = Visibility.Visible;
                    BirthDatePicker.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else if (age > 120)
                {
                    BirthDateError.Text = "Ogiltigt födelsedatum";
                    BirthDateError.Visibility = Visibility.Visible;
                    BirthDatePicker.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    BirthDateError.Visibility = Visibility.Collapsed;
                    BirthDatePicker.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
                }
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age;
        }
        // -------------------------------------------------------------------------

        // Role Section ------------------------------------------------------------
        private void RoleComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            RoleComboBox.SelectedIndex = 0;
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleComboBox.SelectedIndex == 0 && RoleError != null)
            {
                RoleError.Text = "Du måste välja en roll";
                RoleError.Visibility = Visibility.Visible;
                RoleComboBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (RoleError != null)
            {
                RoleError.Visibility = Visibility.Collapsed;
                RoleComboBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
            }
        }
        // -------------------------------------------------------------------------


        #endregion

        #region Field Validetion

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            string firstName = textBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                FirstNameError.Text = "Förnamn krävs";
                FirstNameError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (firstName.Length < 2)
            {
                FirstNameError.Text = "Förnamn måste vara minst 2 tecken";
                FirstNameError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                FirstNameError.Visibility = Visibility.Collapsed;
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
            }
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            string lastName = textBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(lastName))
            {
                LastNameError.Text = "Efternamn krävs";
                LastNameError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (lastName.Length < 2)
            {
                LastNameError.Text = "Efternamn måste vara minst 2 tecken";
                LastNameError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                LastNameError.Visibility = Visibility.Collapsed;
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            string email = textBox.Text.Trim();
            ValidateEmail(email, textBox);
        }

        private async void ValidateEmail(string email, TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                EmailError.Text = "E-post krävs";
                EmailError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (!IsValidEmail(email))
            {
                EmailError.Text = "Ogiltig e-postadress";
                EmailError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (await IsEmailAlreadyRegistered(email))
            {
                EmailError.Text = "E-postadressen används redan";
                EmailError.Visibility = Visibility.Visible;
                textBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                EmailError.Visibility = Visibility.Collapsed;
                textBox.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0"));
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

        private async Task<bool> IsEmailAlreadyRegistered(string email)
        {
            // Example list of registered emails
            var existingEmails = new List<string>
            {
                "test@test.com",
                "example@example.com"
            };

            return await System.Threading.Tasks.Task.Run(() => existingEmails.Contains(email.ToLower()));
        }


        #endregion

        #region Password Section

        private void ShowPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_passwordTextBox == null)
            {
                _passwordTextBox = new TextBox();
                _passwordTextBox.Height = PasswordBox.Height;
                _passwordTextBox.Background = PasswordBox.Background;
                _passwordTextBox.Foreground = PasswordBox.Foreground;
                _passwordTextBox.BorderThickness = PasswordBox.BorderThickness;
                _passwordTextBox.BorderBrush = PasswordBox.BorderBrush;
                _passwordTextBox.Padding = PasswordBox.Padding;
                _passwordTextBox.VerticalContentAlignment = PasswordBox.VerticalContentAlignment;
                _passwordTextBox.TextChanged += PasswordTextBox_TextChanged;
            }

            if (PasswordBox.Visibility == Visibility.Visible)
            {
                _passwordTextBox.Text = PasswordBox.Password;
                PasswordGrid.Children.Add(_passwordTextBox);
                PasswordBox.Visibility = Visibility.Collapsed;
                ShowPasswordButton.Content = "🔒";
            }
            else
            {
                PasswordBox.Password = _passwordTextBox.Text;
                PasswordGrid.Children.Remove(_passwordTextBox);
                PasswordBox.Visibility = Visibility.Visible;
                ShowPasswordButton.Content = "👁";
            }
        }

        private void ShowConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_confirmPasswordTextBox == null)
            {
                _confirmPasswordTextBox = new TextBox();
                _confirmPasswordTextBox.Height = ConfirmPasswordBox.Height;
                _confirmPasswordTextBox.Background = ConfirmPasswordBox.Background;
                _confirmPasswordTextBox.Foreground = ConfirmPasswordBox.Foreground;
                _confirmPasswordTextBox.BorderThickness = ConfirmPasswordBox.BorderThickness;
                _confirmPasswordTextBox.BorderBrush = ConfirmPasswordBox.BorderBrush;
                _confirmPasswordTextBox.Padding = ConfirmPasswordBox.Padding;
                _confirmPasswordTextBox.VerticalContentAlignment = ConfirmPasswordBox.VerticalContentAlignment;
                _confirmPasswordTextBox.TextChanged += ConfirmPasswordTextBox_TextChanged;
            }

            if (ConfirmPasswordBox.Visibility == Visibility.Visible)
            {
                _confirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordGrid.Children.Add(_confirmPasswordTextBox);
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ShowConfirmPasswordButton.Content = "🔒";
            }
            else
            {
                ConfirmPasswordBox.Password = _confirmPasswordTextBox.Text;
                ConfirmPasswordGrid.Children.Remove(_confirmPasswordTextBox);
                ConfirmPasswordBox.Visibility = Visibility.Visible;
                ShowConfirmPasswordButton.Content = "👁";
            }
        }

        private void ConfirmPasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string confirmPassword = ((TextBox)sender).Text;
            string password = PasswordBox.Visibility == Visibility.Visible
                ? PasswordBox.Password
                : _passwordTextBox?.Text;

            UpdatePasswordMatch(password, confirmPassword);
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var password = ((TextBox)sender).Text;
            int strength = 0;

            if (password.Length >= 6)
                strength++;
            if (password.Any(char.IsUpper) || password.Any(char.IsDigit))
                strength++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
                strength++;

            PasswordStrength.Value = strength * 33;

            // Check if passwords match
            string confirmPassword = ConfirmPasswordBox.Visibility == Visibility.Visible
                ? ConfirmPasswordBox.Password
                : _confirmPasswordTextBox?.Text;

            UpdatePasswordMatch(password, confirmPassword);
        }

        private void UpdatePasswordMatch(string password, string confirmPassword)
        {
            // Get current password value regardless of visibility state
            string currentPassword = PasswordBox.Visibility == Visibility.Visible
                ? PasswordBox.Password
                : _passwordTextBox?.Text ?? "";

            // Get current confirm password value regardless of visibility state
            string currentConfirmPassword = ConfirmPasswordBox.Visibility == Visibility.Visible
                ? ConfirmPasswordBox.Password
                : _confirmPasswordTextBox?.Text ?? "";

            if (currentPassword == currentConfirmPassword)
            {
                PasswordMatchIndicator.Text = "✓ Lösenorden matchar";
                PasswordMatchIndicator.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                PasswordMatchIndicator.Text = "✗ Lösenorden matchar inte";
                PasswordMatchIndicator.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var placeholder = PasswordGrid.Children.OfType<TextBlock>().FirstOrDefault();

            if (placeholder != null)
            {
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }

            var password = ((PasswordBox)sender).Password;
            int strength = 0;

            if (password.Length >= 6)   // Changed from 8 to 6
                strength++; 
            if (password.Any(char.IsUpper) || password.Any(char.IsDigit))   // Simplified requirement
                strength++; 
            if (password.Any(ch => !char.IsLetterOrDigit(ch)))
                strength++;


            PasswordStrength.Value = strength * 33; // Now divided into three levels instead of four

            // Change color based on strength
            if (strength <= 1)
                PasswordStrength.Foreground = new SolidColorBrush(Colors.Red);
            else if (strength <= 2)
                PasswordStrength.Foreground = new SolidColorBrush(Colors.Orange);
            else if (strength <= 3)
                PasswordStrength.Foreground = new SolidColorBrush(Colors.Yellow);
            else
                PasswordStrength.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var placeholder = ConfirmPasswordGrid.Children.OfType<TextBlock>().FirstOrDefault();

            if (placeholder != null)
            {
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }

            if (_passwordBox.Password == _confirmPasswordBox.Password)
            {
                _passwordMatchIndicator.Text = "✓ Lösenorden matchar";
                _passwordMatchIndicator.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                _passwordMatchIndicator.Text = "✗ Lösenorden matchar inte";
                _passwordMatchIndicator.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        #endregion

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAllFields())
            {
                FormError.Text = "Vänligen fyll i alla obligatoriska fält korrekt.";
                FormError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                string imagePath = null;
                if (ProfileImage.Source != null)
                {
                    var bitmapImage = ProfileImage.Source as BitmapImage;
                    if (bitmapImage != null && bitmapImage.UriSource != null)
                    {
                        imagePath = SaveImageFile(bitmapImage.UriSource.LocalPath);
                    }
                }

                var newMember = new Member
                {
                    FirstName = FirstNameTextBox.Text.Trim(),
                    LastName = LastNameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordBox.Password),
                    BirthDate = BirthDatePicker.SelectedDate.Value,
                    Role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString(),
                    ProfileImagePath = imagePath ?? "",
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    IsAdmin = AdminRightsCheckBox.IsChecked ?? false
                };

                using (var db = new ApplicationDbContext())
                {
                    db.Members.Add(newMember);
                    await db.SaveChangesAsync();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                var fullErrorMessage = "";
                var currentException = ex;

                while (currentException != null)
                {
                    fullErrorMessage += $"{currentException.Message}\n\n";
                    currentException = currentException.InnerException;
                }

                FormError.Text = "Ett fel uppstod vid sparande av medlem:";
                MessageBox.Show(fullErrorMessage);
                FormError.Visibility = Visibility.Visible;
            }
        }


        private string SaveImageFile(string sourcePath)
        {
            // Check if source path is empty
            if (string.IsNullOrEmpty(sourcePath))
                return null;

            // Create unique filename with GUID and keep original extension
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(sourcePath)}";

            // Create path to profile images folder in application directory
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Profiles");

            // Combine target directory with new filename
            string targetPath = Path.Combine(targetDirectory, fileName);

            // Create target directory if it doesn't exist
            Directory.CreateDirectory(targetDirectory);

            // Load original image
            var image = new BitmapImage(new Uri(sourcePath));

            // Scale image to 250x250 pixels while maintaining proportions
            var resizedImage = new TransformedBitmap(image, new ScaleTransform(250.0 / image.PixelWidth, 250.0 / image.PixelHeight));

            // Save the resized image as PNG
            using (var fileStream = new FileStream(targetPath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(resizedImage));
                encoder.Save(fileStream);
            }

            // Return the relative path to the saved image
            return $"/Images/Profiles/{fileName}";
        }



        private bool ValidateAllFields()
        {
            return !string.IsNullOrWhiteSpace(FirstNameTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(LastNameTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(EmailTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                   BirthDatePicker.SelectedDate.HasValue &&
                   RoleComboBox.SelectedIndex > 0 &&
                   EmailError.Visibility != Visibility.Visible;
        }

    }
}
