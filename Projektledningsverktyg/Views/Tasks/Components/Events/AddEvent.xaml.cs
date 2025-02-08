using Projektledningsverktyg.Data.Entities;
using System;
using System.Windows;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Projektledningsverktyg.Views.Tasks.Components.Events
{
    public partial class AddEvent : Window
    {
        public Event NewEvent { get; private set; }
        public string StartHour { get; set; }
        public string StartMinute { get; set; }
        public string EndHour { get; set; }
        public string EndMinute { get; set; }

        public AddEvent()
        {
            InitializeComponent();
            PopulateTimeComboBoxes();
            LoadEventTypes();
        }

        private void PopulateTimeComboBoxes()
        {
            // Hours (07-23)
            for (int i = 7; i < 24; i++)
            {
                StartHourCombo.Items.Add(i.ToString("00"));
                EndHourCombo.Items.Add(i.ToString("00"));
            }

            // Minutes (00, 15, 30, 45)
            for (int i = 0; i < 60; i += 15)
            {
                StartMinuteCombo.Items.Add(i.ToString("00"));
                EndMinuteCombo.Items.Add(i.ToString("00"));
            }
        }

        private void LoadEventTypes()
        {
            var types = Enum.GetValues(typeof(EventType))
                .Cast<EventType>()
                .Select(t => new
                {
                    Type = t,
                    Name = t.GetType()
                        .GetMember(t.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        .Name
                });

            TypeComboBox.ItemsSource = types;
            TypeComboBox.DisplayMemberPath = "Name";
            TypeComboBox.SelectedValuePath = "Type";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = DatePicker.Value ?? DateTime.Now;

            // Directly get values from ComboBoxes
            string startHourValue = StartHourCombo.SelectedItem?.ToString();
            string startMinuteValue = StartMinuteCombo.SelectedItem?.ToString();
            string endHourValue = EndHourCombo.SelectedItem?.ToString();
            string endMinuteValue = EndMinuteCombo.SelectedItem?.ToString();

            // Parse start time
            DateTime? startTime = null;
            if (!string.IsNullOrEmpty(startHourValue) && !string.IsNullOrEmpty(startMinuteValue))
            {
                var timeString = $"{startHourValue.PadLeft(2, '0')}:{startMinuteValue.PadLeft(2, '0')}";
                startTime = selectedDate.Date + TimeSpan.Parse(timeString);
            }

            // Parse end time
            DateTime? endTime = null;
            if (!string.IsNullOrEmpty(endHourValue) && !string.IsNullOrEmpty(endMinuteValue))
            {
                var timeString = $"{endHourValue.PadLeft(2, '0')}:{endMinuteValue.PadLeft(2, '0')}";
                endTime = selectedDate.Date + TimeSpan.Parse(timeString);
            }

            NewEvent = new Event
            {
                Title = TitleTextBox.Text,
                Date = selectedDate,
                StartTime = startTime,
                EndTime = endTime,
                Type = (EventType)TypeComboBox.SelectedValue,
                Description = DescriptionTextBox.Text
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
