using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Entities;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Projektledningsverktyg.Extensions;
using System.Windows;

namespace Projektledningsverktyg.ViewModels.Calendar
{
    public class AddEditScheduleViewModel : ViewModelBase
    {
        #region Fields
        private DateTime _selectedDate;
        private bool _isDuringDay;
        private bool _hasEndTime = true;
        private string _title;
        private Schedule _schedule;
        private string _selectedType;
        private string _startTime;
        private string _endTime;
        private string _description;
        #endregion

        #region Events
        public event Action OnClose;
        public event Action<Schedule> OnSave;
        public event Action OnCancel;
        #endregion

        #region Properties
        public bool IsDuringDay
        {
            get => _isDuringDay;
            set
            {
                SetProperty(ref _isDuringDay, value);
                if (value)
                {
                    StartTime = null;
                    EndTime = null;
                }
            }
        }
        public bool HasEndTime
        {
            get => _hasEndTime;
            set
            {
                SetProperty(ref _hasEndTime, value);
                if (!value)
                {
                    EndTime = null;
                }
            }
        }

        // Property for display names in ComboBox
        public IEnumerable<string> ScheduleTypes => Enum.GetValues(typeof(ScheduleType))
            .Cast<ScheduleType>()
            .Select(type => type.GetDisplayName())
            .ToList();

        public List<string> TimeSlots { get; } = GenerateTimeSlots();

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public Schedule Schedule
        {
            get => _schedule;
            set => SetProperty(ref _schedule, value);
        }

        public string SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);

                // Convert display name back to enum when saving
                Schedule.Type = Enum.GetValues(typeof(ScheduleType))
                    .Cast<ScheduleType>()
                    .First(type => type.GetDisplayName() == value);
            }
        }

        public string StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        public string EndTime
        {
            get => _endTime;
            set => SetProperty(ref _endTime, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        #endregion

        #region Commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region Constructor
        public AddEditScheduleViewModel(Schedule schedule, DateTime selectedDate)
        {
            _selectedDate = selectedDate;
            Schedule = schedule ?? new Schedule();
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadSchedule(Schedule);
        }
        #endregion

        #region Methods
        private static List<string> GenerateTimeSlots()
        {
            var slots = new List<string>();
            for (int hour = 0; hour < 24; hour++)
            {
                slots.Add($"{hour:00}:00");
                slots.Add($"{hour:00}:15");
                slots.Add($"{hour:00}:30");
                slots.Add($"{hour:00}:45");
            }
            return slots;
        }

        private void LoadSchedule(Schedule schedule)
        {
            Title = schedule.Title;
            SelectedType = schedule.Type.GetDisplayName();
            StartTime = schedule.StartTime?.ToString("HH:mm") ?? "08:00";
            EndTime = schedule.EndTime?.ToString("HH:mm") ?? "17:00";
            Description = schedule.Description;
        }

        private void ExecuteSave()
        {
            Schedule.Title = Title;
            Schedule.Description = Description;

            if (IsDuringDay)
            {
                Schedule.StartTime = null;
                Schedule.EndTime = null;
            }
            else
            {
                var startTime = DateTime.Parse(StartTime);
                Schedule.StartTime = _selectedDate.Date.Add(startTime.TimeOfDay);

                if (HasEndTime)
                {
                    var endTime = DateTime.Parse(EndTime);
                    if (startTime.TimeOfDay > endTime.TimeOfDay)
                    {
                        // Night shift - end time is next day
                        Schedule.EndTime = _selectedDate.Date.AddDays(1).Add(endTime.TimeOfDay);
                    }
                    else
                    {
                        Schedule.EndTime = _selectedDate.Date.Add(endTime.TimeOfDay);
                    }
                }
                else
                {
                    Schedule.EndTime = null;
                }
            }

            OnSave?.Invoke(Schedule);
            OnClose?.Invoke();

        }

        private void ExecuteCancel()
        {
            // First trigger the OnCancel event for any subscribers
            OnCancel?.Invoke();

            // Then trigger OnClose which will hide the overlay if shown in CalendarViewModel
            OnClose?.Invoke();

            // For stand-alone window scenario (when used from edit)
            // Get the currently active window
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

            // Check if it's a specific Edit dialog window and not the list view window
            if (window != null && window.GetType().Name.Contains("Edit") && window.GetType().Name != "MainWindow")
            {
                window.Close();
            }
            // Otherwise, just close the dialog within the window but keep the window open

        }
        #endregion
    }
}
