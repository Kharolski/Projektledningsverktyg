using Projektledningsverktyg.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Projektledningsverktyg.ViewModels
{
    public class DayModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public bool IsCurrentDay { get; set; }

        // New property to track selection state
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class WeekMonthViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Fields
        private DateTime _currentWeekStart;
        private DateTime _currentMonthStart;
        private readonly CultureInfo culture = new CultureInfo("sv-SE");

        // New field to track selected day
        private DayModel _selectedDay;
        #endregion

        #region Properties
        public ObservableCollection<DayModel> WeekDays { get; set; }
        public ObservableCollection<DayModel> MonthDays { get; set; }

        // Canvas positioning for draggable controls
        private double _xPosition;
        private double _yPosition;

        public double XPosition
        {
            get => _xPosition;
            set
            {
                _xPosition = value;
                OnPropertyChanged(nameof(XPosition));
            }
        }
        public double YPosition
        {
            get => _yPosition;
            set
            {
                _yPosition = value;
                OnPropertyChanged(nameof(YPosition));
            }
        }

        // New property for selected day
        public DayModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                // Only update if selection changed
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged(nameof(SelectedDay));

                    // When selected day changes, raise an event to notify subscribers
                    OnSelectedDayChanged?.Invoke(this, _selectedDay?.Date ?? DateTime.Today);
                }
            }
        }

        // Event for when selected day changes (views will subscribe to this)
        public event EventHandler<DateTime> OnSelectedDayChanged;
        #endregion

        #region Constructor
        public WeekMonthViewModel()
        {
            // Week
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            WeekDays = new ObservableCollection<DayModel>();

            // Initialize commands
            PreviousWeekCommand = new RelayCommand(PreviousWeek);
            NextWeekCommand = new RelayCommand(NextWeek);
            SelectDayCommand = new RelayCommand<DayModel>(ExecuteSelectDay);

            // Initialize week days and select today
            UpdateWeekDays();

            // Set today as the selected day
            SelectToday();

            // Month
            _currentMonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            UpdateMonthDays();
        }
        #endregion

        #region Commands
        public ICommand PreviousWeekCommand { get; }
        public ICommand NextWeekCommand { get; }

        // New command for selecting a day
        public ICommand SelectDayCommand { get; }
        #endregion

        #region Month
        private void UpdateMonthDays()
        {
            MonthDays = new ObservableCollection<DayModel>();
            int daysInMonth = DateTime.DaysInMonth(_currentMonthStart.Year, _currentMonthStart.Month);
            for (int i = 1; i <= daysInMonth; i++)
            {
                DateTime day = new DateTime(_currentMonthStart.Year, _currentMonthStart.Month, i);
                MonthDays.Add(new DayModel
                {
                    Date = day,
                    DayName = day.ToString("dddd", new CultureInfo("sv-SE")),
                    IsCurrentDay = day.Date == DateTime.Today
                });
            }
            OnPropertyChanged(nameof(MonthDays));
        }

        public void NextMonth()
        {
            _currentMonthStart = _currentMonthStart.AddMonths(1);
            UpdateMonthDays();
        }

        public void PreviousMonth()
        {
            _currentMonthStart = _currentMonthStart.AddMonths(-1);
            UpdateMonthDays();
        }
        #endregion

        #region Week
        public string WeekText => $"Vecka {GetWeekNumber(_currentWeekStart)}, {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.DateTimeFormat.GetMonthName(_currentWeekStart.Month))} {_currentWeekStart.Year}";

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-diff).Date;
        }

        private int GetWeekNumber(DateTime date)
        {
            return culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void UpdateWeekDays()
        {
            WeekDays.Clear();
            for (int i = 0; i < 7; i++)
            {
                DateTime day = _currentWeekStart.AddDays(i);
                WeekDays.Add(new DayModel
                {
                    Date = day,
                    DayName = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(culture.DateTimeFormat.GetDayName(day.DayOfWeek))} {day.Day}",
                    IsCurrentDay = day.Date == DateTime.Today,
                    // Add a property to track if this day is selected
                    IsSelected = _selectedDay != null && day.Date == _selectedDay.Date
                });
            }

            // After updating week days, make sure we have a selected day in the current week
            EnsureSelectedDayInCurrentWeek();

            OnPropertyChanged(nameof(WeekDays));
            OnPropertyChanged(nameof(WeekText));
        }

        // Preserve selected day of week when possible
        public void NextWeek()
        {
            // Store the day of week of the currently selected day
            DayOfWeek selectedDayOfWeek = _selectedDay?.Date.DayOfWeek ?? DateTime.Today.DayOfWeek;

            _currentWeekStart = _currentWeekStart.AddDays(7);
            UpdateWeekDays();

            // Try to select the same day of week in the new week
            SelectDayOfWeek(selectedDayOfWeek);
        }

        // Preserve selected day of week when possible
        public void PreviousWeek()
        {
            // Store the day of week of the currently selected day
            DayOfWeek selectedDayOfWeek = _selectedDay?.Date.DayOfWeek ?? DateTime.Today.DayOfWeek;

            _currentWeekStart = _currentWeekStart.AddDays(-7);
            UpdateWeekDays();

            // Try to select the same day of week in the new week
            SelectDayOfWeek(selectedDayOfWeek);
        }
        #endregion

        #region Day Selection Methods
        // Command handler for selecting a day
        private void ExecuteSelectDay(DayModel day)
        {
            if (day != null)
            {
                // Om användaren klickar på en dag som redan är vald, avmarkera den
                if (_selectedDay != null && day.Date == _selectedDay.Date)
                {
                    // Avmarkera alla dagar
                    foreach (var d in WeekDays)
                    {
                        d.IsSelected = false;
                    }

                    // Återgå till dagens datum för datahämtning
                    var today = DateTime.Today;
                    OnSelectedDayChanged?.Invoke(this, today);

                    // Sätt _selectedDay till null för att indikera att ingen dag är explicit vald
                    _selectedDay = null;
                    OnPropertyChanged(nameof(SelectedDay));
                }
                else
                {
                    // Normalt val av en dag
                    foreach (var d in WeekDays)
                    {
                        d.IsSelected = (d.Date == day.Date);
                    }

                    // Uppdatera vald dag
                    _selectedDay = day;
                    OnPropertyChanged(nameof(SelectedDay));

                    // Meddela om ändrad dag
                    OnSelectedDayChanged?.Invoke(this, _selectedDay.Date);
                }
            }
        }

        // Helper method to select a specific day of week in the current week
        private void SelectDayOfWeek(DayOfWeek dayOfWeek)
        {
            foreach (var day in WeekDays)
            {
                if (day.Date.DayOfWeek == dayOfWeek)
                {
                    ExecuteSelectDay(day);
                    break;
                }
            }
        }

        // Ensures we always have a selected day in the current week
        private void EnsureSelectedDayInCurrentWeek()
        {
            // If no day is selected, or selected day is not in current week
            if (_selectedDay == null ||
                _selectedDay.Date < _currentWeekStart ||
                _selectedDay.Date >= _currentWeekStart.AddDays(7))
            {
                // Try to find a day in this week that matches the day of week of the last selection
                DayOfWeek targetDayOfWeek = _selectedDay?.Date.DayOfWeek ?? DateTime.Today.DayOfWeek;

                var dayToSelect = WeekDays.FirstOrDefault(d => d.Date.DayOfWeek == targetDayOfWeek);

                // If not found, select first day of week
                if (dayToSelect == null && WeekDays.Count > 0)
                {
                    dayToSelect = WeekDays[0];
                }

                if (dayToSelect != null)
                {
                    ExecuteSelectDay(dayToSelect);
                }
            }
            else
            {
                // Update IsSelected flag for all days
                foreach (var day in WeekDays)
                {
                    day.IsSelected = (day.Date == _selectedDay.Date);
                }
            }
        }

        // Helper to select today's date when possible
        public void SelectToday()
        {
            var today = DateTime.Today;

            // If today is not in the current week, navigate to the week containing today
            if (today < _currentWeekStart || today >= _currentWeekStart.AddDays(7))
            {
                _currentWeekStart = GetStartOfWeek(today);
                UpdateWeekDays();
            }

            // Select today
            var todayModel = WeekDays.FirstOrDefault(d => d.Date.Date == today.Date);
            if (todayModel != null)
            {
                ExecuteSelectDay(todayModel);
            }
        }
        #endregion

        #region Helpers
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
