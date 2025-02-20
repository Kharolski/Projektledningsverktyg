using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace Projektledningsverktyg.Views.Tasks.Components.Household
{
    public partial class AddHouseholdTask : Window
    {
        private readonly HouseholdViewModel _viewModel;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddHouseholdTask()
        {
            InitializeComponent();

            var context = new ApplicationDbContext();
            var repository = new HouseholdRepository(context);
            _viewModel = new HouseholdViewModel(repository);

            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = _viewModel;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HouseholdViewModel;

            if (string.IsNullOrWhiteSpace(viewModel.NewTaskTitle))
            {
                ErrorMessage.Text = "Titel kan inte vara tom";
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            viewModel?.ExecuteSaveTask();
        }
        private void UpdateErrorMessage(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = string.IsNullOrEmpty(message)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HouseholdViewModel.AddTaskErrorMessage))
            {
                UpdateErrorMessage(_viewModel.AddTaskErrorMessage);
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
