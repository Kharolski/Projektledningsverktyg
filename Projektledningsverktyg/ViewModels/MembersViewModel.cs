using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Projektledningsverktyg.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projektledningsverktyg.Data.Context;
using System.IO;
using Projektledningsverktyg.Helpers;

namespace Projektledningsverktyg.ViewModels
{
    public class MembersViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MemberModel> _members;
        private string _searchText;

        public ObservableCollection<MemberModel> Members
        {
            get => _members;
            set
            {
                _members = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterMembers();
            }
        }

        public MembersViewModel()
        {
            LoadMembers();
        }

        public void LoadMembers()
        {
            using (var db = new ApplicationDbContext())
            {
                var members = db.Members.Select(m => new MemberModel
                {
                    Id = m.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    IsActive = m.IsActive,
                    Role = m.Role,
                    ProfileImagePath = m.ProfileImagePath
                }).ToList();

                foreach (var member in members)
                {
                    if (!string.IsNullOrEmpty(member.ProfileImagePath))
                    {
                        if (member.ProfileImagePath.StartsWith("/Images/"))
                        {
                            // Avatar from resources
                            member.ProfileImagePath = $"pack://application:,,,{member.ProfileImagePath}";
                        }
                        else
                        {
                            // Custom uploaded image
                            member.ProfileImagePath = Path.Combine(
                                AppFolders.GetUserImagesPath(),
                                Path.GetFileName(member.ProfileImagePath));
                        }
                    }
                }
                Members = new ObservableCollection<MemberModel>(members);
            }
        }


        private void FilterMembers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadMembers();
                return;
            }

            Members = new ObservableCollection<MemberModel>(
                Members.Where(m =>
                    (m.FirstName?.ToLower().Contains(SearchText.ToLower()) ?? false) ||
                    (m.LastName?.ToLower().Contains(SearchText.ToLower()) ?? false) ||
                    (m.Email?.ToLower().Contains(SearchText.ToLower()) ?? false))
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
