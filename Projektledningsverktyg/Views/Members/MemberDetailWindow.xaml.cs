using Microsoft.Win32;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Projektledningsverktyg.Views.Members
{
    /// <summary>
    /// Interaction logic for MemberDetailWindow.xaml
    /// </summary>
    public partial class MemberDetailWindow : Window
    {
        private readonly Member _member;
        private string _originalImagePath;


        public MemberDetailWindow(Member member)
        {
             InitializeComponent();
            DataContext = this;
            _member = member;
            LoadMemberData();
        }

        private void LoadMemberData()
        {
            // Set text fields
            FirstNameTextBox.Text = _member.FirstName;
            LastNameTextBox.Text = _member.LastName;
            EmailTextBox.Text = _member.Email;

            // Set date
            BirthDateTextBox.Text = _member.BirthDate.ToString("yyyy-MM-dd");

            // Set role in ComboBox
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Content.ToString() == _member.Role)
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }

            // Set admin status
            AdminCheckBox.IsChecked = _member.IsAdmin;

            // Load profile image
            if (!string.IsNullOrEmpty(_member.ProfileImagePath))
            {
                if (_member.ProfileImagePath.StartsWith("/Images/"))
                {
                    // Avatar from resources
                    ProfileImage.ImageSource = new BitmapImage(new Uri($"pack://application:,,,{_member.ProfileImagePath}"));
                }
                else
                {
                    // Custom uploaded image
                    string fullPath = Path.Combine(
                        AppFolders.GetUserImagesPath(),
                        Path.GetFileName(_member.ProfileImagePath));
                    ProfileImage.ImageSource = new BitmapImage(new Uri(fullPath));
                }
            }
            else
            {
                ProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/user.png"));
            }


            _originalImagePath = _member.ProfileImagePath;
            AvatarButton.Content = "Välj bild";
        }

        public List<string> DefaultAvatars { get; } = new List<string>
        {
            "/Images/Avatars/avatar_m1.jpg",
            "/Images/Avatars/avatar_m2.jpg",
            "/Images/Avatars/avatar_m3.jpg",
            "/Images/Avatars/avatar_w1.jpg",
            "/Images/Avatars/avatar_w2.jpg",
            "/Images/Avatars/avatar_w3.jpg"
        };

        private void OnAvatarSelected(object sender, SelectionChangedEventArgs e)
        {
            if (AvatarListBox.SelectedItem != null)
            {
                // Create new bitmap image for the selected avatar
                var bitmap = new BitmapImage();
                bitmap.BeginInit();

                // Use pack URI format to access project resources
                // pack://application:,,, tells WPF to look in the application's resource directory
                // instead of trying to find files on the C: drive
                bitmap.UriSource = new Uri($"pack://application:,,,{AvatarListBox.SelectedItem}", UriKind.Absolute);

                // Cache the image for better performance
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();


                // Set the new image and update button text
                ProfileImage.ImageSource = bitmap;
                AvatarButton.Content = "Återställ";
            }
        }

        private void AvatarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvatarButton.Content.ToString() == "Välj bild")
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                    Title = "Välj en profilbild"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    string destinationPath = Path.Combine(AppFolders.GetUserImagesPath(), fileName);

                    File.Copy(openFileDialog.FileName, destinationPath, true);
                    ProfileImage.ImageSource = new BitmapImage(new Uri(destinationPath));

                    AvatarButton.Content = "Återställ";
                    AvatarListBox.SelectedItem = null;
                }
            }
            else // When "Återställ"
            {
                // Reset to original image
                if (!string.IsNullOrEmpty(_originalImagePath))
                {
                    string fullPath = Path.Combine(
                        AppFolders.GetUserImagesPath(),
                        Path.GetFileName(_originalImagePath));
                    ProfileImage.ImageSource = new BitmapImage(new Uri(fullPath));
                }
                else
                {
                    ProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/user.png"));
                }

                // Reset button and selection
                AvatarButton.Content = "Välj bild";
                AvatarListBox.SelectedItem = null;
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update member properties
                _member.FirstName = FirstNameTextBox.Text.Trim();
                _member.LastName = LastNameTextBox.Text.Trim();
                _member.Role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
                _member.IsAdmin = AdminCheckBox.IsChecked ?? false;

                // Handle profile image changes
                if (AvatarListBox.SelectedItem != null)
                {
                    // Selected avatar from ListBox
                    _member.ProfileImagePath = AvatarListBox.SelectedItem.ToString();
                }
                else if (ProfileImage.ImageSource is BitmapImage bitmapImage)
                {
                    // Uploaded custom image
                    string imagePath = bitmapImage.UriSource.LocalPath;
                    if (!imagePath.Contains("user.png"))
                    {
                        _member.ProfileImagePath = imagePath;
                    }
                }

                using (var db = new ApplicationDbContext())
                {
                    var existingMember = await db.Members.FindAsync(_member.Id);
                    db.Entry(existingMember).CurrentValues.SetValues(_member);
                    await db.SaveChangesAsync();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ett fel uppstod: {ex.Message}");
            }
        }

        // Validation -------------------------------------------------------------------
        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FirstNameError.Visibility = FirstNameTextBox.Text.Length < 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LastNameError.Visibility = LastNameTextBox.Text.Length < 2 ? Visibility.Visible : Visibility.Collapsed;
        }



    }
}
