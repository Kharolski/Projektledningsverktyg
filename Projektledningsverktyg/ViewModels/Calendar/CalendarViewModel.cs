using Projektledningsverktyg.Data.Entities;
using System.Collections.ObjectModel;
using System;
using Projektledningsverktyg.Data.Repository;

namespace Projektledningsverktyg.ViewModels.Calendar
{
    public class CalendarViewModel : ViewModelBase
    {
        private readonly ScheduleRepository _scheduleRepository;
        public event Action OnCalendarRefreshNeeded;

        #region Properties
        private AddEditScheduleViewModel _addEditViewModel;
        public AddEditScheduleViewModel AddEditViewModel
        {
            get => _addEditViewModel;
            set => SetProperty(ref _addEditViewModel, value);
        }

        private bool _isAddEditVisible;
        public bool IsAddEditVisible
        {
            get => _isAddEditVisible;
            set => SetProperty(ref _isAddEditVisible, value);
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private ObservableCollection<Schedule> _schedules;
        public ObservableCollection<Schedule> Schedules
        {
            get => _schedules;
            set => SetProperty(ref _schedules, value);
        }
        #endregion

        #region Constructor
        public CalendarViewModel(ScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
            _selectedDate = DateTime.Today;
            _isAddEditVisible = false;
            _schedules = new ObservableCollection<Schedule>();
        }
        #endregion

        #region Methods
        public void ShowAddForm(DateTime date)
        {
            SelectedDate = date;
            AddEditViewModel = new AddEditScheduleViewModel(new Schedule(), date);
            AddEditViewModel.OnSave += SaveSchedule;
            AddEditViewModel.OnClose += () => IsAddEditVisible = false;
            AddEditViewModel.OnCancel += () => IsAddEditVisible = false;
            IsAddEditVisible = true;
        }

        public void HideAddForm()
        {
            IsAddEditVisible = false;
            AddEditViewModel = null;
        }

        private void SaveSchedule(Schedule schedule)
        {
            _scheduleRepository.AddScheduleWithDate(schedule, SelectedDate);
            OnCalendarRefreshNeeded?.Invoke();
        }
        #endregion
    }
}
