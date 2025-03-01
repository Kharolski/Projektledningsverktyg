using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Views.Calendar.Components.MonthComponents;
using System.Windows;

namespace Projektledningsverktyg.ViewModels.Calendar.WeekModels
{
    public class ScheduleWeekViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Fields
        private readonly ScheduleRepository _scheduleRepository;
        private DateTime _selectedDate;
        #endregion

        #region Properties
        // Collections for different schedule types
        public ObservableCollection<Schedule> WorkSchoolSchedules { get; set; }
        public ObservableCollection<Schedule> DeviatingSchedules { get; set; }

        // Lägg till dessa nya egenskaper
        public bool HasWorkSchoolSchedules => WorkSchoolSchedules.Count > 0;
        public bool HasDeviatingSchedules => DeviatingSchedules.Count > 0;
        #endregion

        #region Commands
        public ICommand AddScheduleCommand { get; }
        public ICommand EditScheduleCommand { get; }
        #endregion

        #region Constructor
        public ScheduleWeekViewModel(ScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
            WorkSchoolSchedules = new ObservableCollection<Schedule>();
            DeviatingSchedules = new ObservableCollection<Schedule>();

            // Initialize commands
            AddScheduleCommand = new RelayCommand(AddSchedule);
            EditScheduleCommand = new RelayCommand<Schedule>(EditSchedule);

            // Initialize with today's date
            _selectedDate = DateTime.Today;

            // Load initial data
            LoadSchedules(_selectedDate);
        }
        #endregion

        #region Public Methods
        // Method to be called when selected date changes
        public void UpdateSelectedDate(DateTime newDate)
        {
            _selectedDate = newDate;
            LoadSchedules(_selectedDate);
        }
        #endregion

        #region Private Methods
        private void LoadSchedules(DateTime date)
        {
            // Clear existing items
            WorkSchoolSchedules.Clear();
            DeviatingSchedules.Clear();

            // Get schedules for the selected date
            var schedules = _scheduleRepository.GetSchedulesByDate(date);

            // Split schedules by type
            foreach (var schedule in schedules)
            {
                if (schedule.Type == ScheduleType.WorkSchool)
                {
                    WorkSchoolSchedules.Add(schedule);
                }
                else if (schedule.Type == ScheduleType.Deviation)
                {
                    DeviatingSchedules.Add(schedule);
                }
            }

            // Notify all property changes
            OnPropertyChanged(nameof(WorkSchoolSchedules));
            OnPropertyChanged(nameof(DeviatingSchedules));
            OnPropertyChanged(nameof(HasWorkSchoolSchedules));
            OnPropertyChanged(nameof(HasDeviatingSchedules));
        }

        private void AddSchedule()
        {
            // Skapa ett nytt tomt schema
            Schedule newSchedule = new Schedule();

            // Skapa ViewModel för redigering
            var viewModel = new AddEditScheduleViewModel(newSchedule, _selectedDate);

            // Skapa innehållsvyn
            var addEditView = new AddEditScheduleView { DataContext = viewModel };

            // Skapa och visa fönstret
            var window = new Window
            {
                Title = "Lägg till schema",
                Content = addEditView,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            // Prenumerera på händelser
            viewModel.OnSave += (schedule) => {
                // Spara schemat i databasen med det valda datumet
                _scheduleRepository.AddScheduleWithDate(schedule, _selectedDate);
                // Ladda om scheman för att uppdatera vyn
                LoadSchedules(_selectedDate);
                // Stäng fönstret
                window.Close();
            };

            viewModel.OnCancel += () => {
                // Stäng fönstret
                window.Close();
            };

            window.ShowDialog();
        }

        private void EditSchedule(Schedule schedule)
        {
            if (schedule == null)
                return;

            // Skapa en kopia av schemat för redigering
            Schedule scheduleToEdit = new Schedule
            {
                Id = schedule.Id,
                Title = schedule.Title,
                Type = schedule.Type,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Description = schedule.Description
            };

            // Skapa ViewModel för redigering
            var viewModel = new AddEditScheduleViewModel(scheduleToEdit, _selectedDate);

            // Skapa innehållsvyn
            var addEditView = new AddEditScheduleView { DataContext = viewModel };

            // Skapa och visa fönstret
            var window = new Window
            {
                Title = "Redigera schema",
                Content = addEditView,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            // Prenumerera på händelser
            viewModel.OnSave += (updatedSchedule) => {
                // Uppdatera schemat i databasen
                _scheduleRepository.UpdateSchedule(updatedSchedule);
                // Ladda om scheman för att uppdatera vyn
                LoadSchedules(_selectedDate);
                // Stäng fönstret
                window.Close();
            };

            viewModel.OnCancel += () => {
                // Stäng fönstret
                window.Close();
            };

            window.ShowDialog();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
