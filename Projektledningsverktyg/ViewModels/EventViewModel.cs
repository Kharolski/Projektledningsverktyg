using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.Views.Tasks.Components.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace Projektledningsverktyg.ViewModels
{
    public class EventViewModel : INotifyPropertyChanged
    {
        #region Properties
        private readonly EventRepository _eventRepository;
        private ObservableCollection<IGrouping<EventType, Event>> _eventsByCategory;

        // Property for UI binding
        public ObservableCollection<IGrouping<EventType, Event>> EventsByCategory
        {
            get => _eventsByCategory;
            set
            {
                _eventsByCategory = value;
                OnPropertyChanged();
            }
        }

        // Command for handling Event button click
        public ICommand AddEventCommand { get; private set; }
        public ICommand DeleteEventCommand { get; private set; }

        // Property for message visibility and text
        private string _messageText;
        private bool _isMessageVisible;
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }
        public bool IsMessageVisible
        {
            get => _isMessageVisible;
            set
            {
                _isMessageVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        // Parameterless constructor for XAML
        public EventViewModel()
        {
            _eventRepository = new EventRepository(new ApplicationDbContext());
            AddEventCommand = new RelayCommand(ExecuteAddEvent);
            DeleteEventCommand = new RelayCommand<int>(ExecuteDeleteEvent);
            LoadEvents();
        }

        //  Constructor for dependency injection
        public EventViewModel(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            LoadEvents();
        }

        #endregion

        #region Load

        // Loads and groups all events
        private void LoadEvents()
        {
            var events = _eventRepository.GetAllEvents();
            var grouped = events.GroupBy(e => e.Type)
                              .OrderBy(g => g.Key);
            EventsByCategory = new ObservableCollection<IGrouping<EventType, Event>>(grouped);
        }

        #endregion

        #region Add
        private void ExecuteAddEvent()
        {
            var addEventWindow = new AddEvent();
            if (addEventWindow.ShowDialog() == true)
            {
                // Save new event to database
                _eventRepository.AddEvent(addEventWindow.NewEvent);

                // Refresh the events list
                LoadEvents();

                // Show success message
                ShowSuccessMessage("✅ Händelse har lagts till!");
            }
        }
        #endregion

        #region Delete
        private void ExecuteDeleteEvent(int eventId)
        {
            var result = MessageBox.Show(
                "Är du säker på att du vill ta bort denna händelse?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _eventRepository.DeleteEvent(eventId);
                LoadEvents();
                ShowSuccessMessage("Händelse har tagits bort!");
            }
        }
        #endregion

        // Helper method to show temporary success message
        private async void ShowSuccessMessage(string message)
        {
            MessageText = message;
            IsMessageVisible = true;

            // Wait 3 seconds then fade out
            await System.Threading.Tasks.Task.Delay(3000);
            IsMessageVisible = false;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
