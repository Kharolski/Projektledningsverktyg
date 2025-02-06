using Projektledningsverktyg.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

namespace Projektledningsverktyg.ViewModels
{
    public class DayModel
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; }
        public bool IsCurrentDay { get; set; }
    }

    public class WeekMonthViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        private DateTime _currentWeekStart;
        private DateTime _currentMonthStart;

        private readonly CultureInfo culture = new CultureInfo("sv-SE");
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
        #endregion

        #region Constructor
        public WeekMonthViewModel()
        {
            // Week
            _currentWeekStart = GetStartOfWeek(DateTime.Today);
            WeekDays = new ObservableCollection<DayModel>();
            PreviousWeekCommand = new RelayCommand(PreviousWeek);
            NextWeekCommand = new RelayCommand(NextWeek);
            UpdateWeekDays();

            // Month
            _currentMonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            UpdateMonthDays();
        }
        #endregion

        #region Commands
        public ICommand PreviousWeekCommand { get; }
        public ICommand NextWeekCommand { get; }
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

        //public string WeekText => $"Vecka {GetWeekNumber(_currentWeekStart)}, {_currentWeekStart.Year}";
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
                    IsCurrentDay = day.Date == DateTime.Today
                });
            }
            OnPropertyChanged(nameof(WeekDays));
            OnPropertyChanged(nameof(WeekText));
        }

        public void NextWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(7);
            UpdateWeekDays();
        }

        public void PreviousWeek()
        {
            _currentWeekStart = _currentWeekStart.AddDays(-7);
            UpdateWeekDays();
        }

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
