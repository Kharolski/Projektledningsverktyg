using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Linq;
using System.Windows.Controls;

namespace Projektledningsverktyg.ViewModels.Calendar.WeekModels
{
    public class EventWeekViewModel : ViewModelBase
    {
        #region Fields
        private readonly EventRepository _eventRepository;
        private DateTime _selectedDate;
        private ObservableCollection<Event> _events;
        #endregion

        #region Properties
        public ObservableCollection<Event> Events
        {
            get => _events;
            set => SetProperty(ref _events, value);
        }

        public bool HasEvents => Events.Any();
        #endregion

        #region Commands
        public ICommand AddEventCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
        #endregion

        #region Constructor
        public EventWeekViewModel(EventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            Events = new ObservableCollection<Event>();

            // Initialize commands
            AddEventCommand = new RelayCommand(AddEvent);
            EditEventCommand = new RelayCommand<Event>(EditEvent);
            DeleteEventCommand = new RelayCommand<Event>(DeleteEvent);

            // Initialize with today's date
            _selectedDate = DateTime.Today;

            // Load initial data
            LoadEvents(_selectedDate);
        }
        #endregion

        #region Methods
        public void UpdateSelectedDate(DateTime date)
        {
            _selectedDate = date;
            LoadEvents(_selectedDate);
        }

        private void LoadEvents(DateTime date)
        {
            // Clear existing items
            Events.Clear();

            // Get all events
            var allEvents = _eventRepository.GetAllEvents();

            // Filter for events on the selected date
            var filteredEvents = allEvents.Where(e => e.Date.Date == date.Date).ToList();

            // Add to collection
            foreach (var evt in filteredEvents)
            {
                Events.Add(evt);
            }

            OnPropertyChanged(nameof(Events));
            OnPropertyChanged(nameof(HasEvents));
        }

        private void AddEvent()
        {
            // Create a new event with the selected date
            var newEvent = new Event
            {
                Date = _selectedDate
            };

            // Show dialog to add event
            ShowEditEventDialog(newEvent, true);
        }

        private void EditEvent(Event eventToEdit)
        {
            if (eventToEdit == null)
                return;

            // Show dialog to edit event
            ShowEditEventDialog(eventToEdit, false);
        }

        private void DeleteEvent(Event eventToDelete)
        {
            if (eventToDelete == null)
                return;

            // Delete the event
            _eventRepository.DeleteEvent(eventToDelete.Id);

            // Reload events
            LoadEvents(_selectedDate);
        }

        private void ShowEditEventDialog(Event eventToEdit, bool isNew)
        {
            Console.WriteLine("ShowEditEventDialog börjar");

            // Skapa dialogfönstret
            var dialog = new Views.Tasks.Components.Events.AddEvent();

            Console.WriteLine($"Event att redigera: {eventToEdit?.Title} - Datum: {eventToEdit?.Date}");

            // Om vi redigerar en befintlig händelse, ställ in initial data
            if (!isNew)
            {
                dialog.Title = "Redigera händelse";
                dialog.TitleTextBox.Text = eventToEdit.Title;
                dialog.DescriptionTextBox.Text = eventToEdit.Description;

                // Sätt datumet korrekt
                dialog.DatePicker.SelectedDate = eventToEdit.Date;
                dialog.TypeComboBox.SelectedValue = eventToEdit.Type;

                Console.WriteLine($"Satt DatePicker.SelectedDate till {dialog.DatePicker.SelectedDate}");

                // Ställ in start- och sluttider om de finns
                if (eventToEdit.StartTime.HasValue)
                {
                    // Säkerställ att värdena matchar de i comboboxen
                    dialog.StartHourCombo.SelectedItem = eventToEdit.StartTime.Value.ToString("HH");
                    dialog.StartMinuteCombo.SelectedItem = eventToEdit.StartTime.Value.ToString("mm");

                    Console.WriteLine($"StartHourCombo.SelectedItem = {dialog.StartHourCombo.SelectedItem}");
                    Console.WriteLine($"StartMinuteCombo.SelectedItem = {dialog.StartMinuteCombo.SelectedItem}");
                }

                if (eventToEdit.EndTime.HasValue)
                {
                    dialog.EndHourCombo.SelectedItem = eventToEdit.EndTime.Value.ToString("HH");
                    dialog.EndMinuteCombo.SelectedItem = eventToEdit.EndTime.Value.ToString("mm");
                }
            }
            else
            {
                // För ny händelse, sätt valt datum
                dialog.DatePicker.SelectedDate = _selectedDate;
            }

            // Visa dialogen
            if (dialog.ShowDialog() == true)
            {
                // Om användaren sparade, hämta den nya/uppdaterade händelsen
                Event updatedEvent = dialog.NewEvent;

                if (isNew)
                {
                    // Lägg till ny händelse
                    _eventRepository.AddEvent(updatedEvent);
                }
                else
                {
                    // Uppdatera befintlig händelse
                    // Preserve ID and other fields
                    eventToEdit.Title = updatedEvent.Title;
                    eventToEdit.Description = updatedEvent.Description;
                    eventToEdit.Date = updatedEvent.Date;
                    eventToEdit.StartTime = updatedEvent.StartTime;
                    eventToEdit.EndTime = updatedEvent.EndTime;
                    eventToEdit.Type = updatedEvent.Type;

                    // Uppdatera i databasen - vi behöver lägga till en UpdateEvent metod
                    UpdateEvent(eventToEdit);
                }

                // Ladda om händelser
                LoadEvents(_selectedDate);
            }
        }

        // Metod för att uppdatera event i databasen
        private void UpdateEvent(Event eventToUpdate)
        {
            // Vi behöver implementera detta i repository
            var existingEvent = _eventRepository.GetEventById(eventToUpdate.Id);
            if (existingEvent != null)
            {
                existingEvent.Title = eventToUpdate.Title;
                existingEvent.Description = eventToUpdate.Description;
                existingEvent.Date = eventToUpdate.Date;
                existingEvent.StartTime = eventToUpdate.StartTime;
                existingEvent.EndTime = eventToUpdate.EndTime;
                existingEvent.Type = eventToUpdate.Type;

                _eventRepository.UpdateEvent(existingEvent);
            }
        }

        #endregion
    }

}
