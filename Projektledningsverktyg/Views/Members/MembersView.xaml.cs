using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.ViewModels;
using System.Globalization;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Projektledningsverktyg.Views.Members
{
    /// <summary>
    /// Interaction logic for MembersView.xaml
    /// </summary>
    public partial class MembersView : UserControl
    {
        public MembersView()
        {
            InitializeComponent();
            DataContext = new MembersViewModel();
        }

        private async void AddMember_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMemberDialog();
            if (dialog.ShowDialog() == true)
            {
                SuccessMessage.Visibility = Visibility.Visible;
                LoadMembers();
                await Task.Delay(3000);
                SuccessMessage.Visibility = Visibility.Collapsed;
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var member = (MemberModel)((Button)sender).DataContext;
            var memberId = member.Id;

            var result = MessageBox.Show(
                "Är du säker på att du vill ta bort denna medlem?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                using (var db = new ApplicationDbContext())
                {
                    var memberToDelete = db.Members.Find(memberId);
                    if (memberToDelete != null)
                    {
                        db.Members.Remove(memberToDelete);
                        await db.SaveChangesAsync();

                        // Refresh the members list
                        ((MembersViewModel)DataContext).LoadMembers();
                    }
                }
            }
        }

        private void LoadMembers()
        {
            var viewModel = (MembersViewModel)DataContext;
            viewModel.LoadMembers();
        }


    }
}
