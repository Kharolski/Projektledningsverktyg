using Projektledningsverktyg.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Projektledningsverktyg.Views.Tasks.Components.Events
{
    /// <summary>
    /// Interaction logic for EventsTab.xaml
    /// </summary>
    public partial class EventsTab : Page
    {
        private ObservableCollection<Event> events;
        public EventsTab()
        {
            InitializeComponent();
            LoadEvents();
            EventsList.ItemsSource = events;
        }

        private void LoadEvents()
        {
            events = new ObservableCollection<Event>
            {
                new Event
                {
                    Title = "Teammöte",
                    Date = DateTime.Now,
                    Time = new TimeSpan(14, 0, 0),
                    Type = EventType.Meeting,
                    Description = "Veckomöte med utvecklingsteamet"
                },
                new Event
                {
                    Title = "Lunch med team",
                    Date = DateTime.Now,
                    Time = new TimeSpan(12, 0, 0),
                    Type = EventType.Other,
                    Description = "Lunch på restaurang Seaside"
                }
            };
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            var newEventWindow = new AddEventWindow();
            if (newEventWindow.ShowDialog() == true)
            {
                events.Add(newEventWindow.NewEvent);
            }
        }
    }

    // Separate window for adding new events
    public class AddEventWindow : Window
    {
        public Event NewEvent { get; private set; }
        private TextBox titleBox;
        private DatePicker datePicker;
        private ComboBox timeHourBox;
        private ComboBox timeMinuteBox;
        private ComboBox typeBox;
        private TextBox descriptionBox;

        public AddEventWindow()
        {
            Title = "Lägg till ny händelse";
            Width = 400;
            Height = 400;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Title
            grid.Children.Add(new TextBlock { Text = "Titel:", Margin = new Thickness(0, 0, 0, 5) });
            titleBox = new TextBox { Margin = new Thickness(0, 0, 0, 15), Height = 30 };
            Grid.SetRow(titleBox, 1);
            grid.Children.Add(titleBox);

            // Date
            grid.Children.Add(new TextBlock { Text = "Datum:", Margin = new Thickness(0, 0, 0, 5) });
            Grid.SetRow(grid.Children[grid.Children.Count - 1], 2);
            datePicker = new DatePicker { Margin = new Thickness(0, 0, 0, 15) };
            Grid.SetRow(datePicker, 3);
            grid.Children.Add(datePicker);

            // Time
            var timePanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            timeHourBox = new ComboBox { Width = 70, Margin = new Thickness(0, 0, 10, 0) };
            timeMinuteBox = new ComboBox { Width = 70 };

            for (int i = 0; i < 24; i++)
                timeHourBox.Items.Add(i.ToString("00"));
            for (int i = 0; i < 60; i += 15)
                timeMinuteBox.Items.Add(i.ToString("00"));

            timePanel.Children.Add(timeHourBox);
            timePanel.Children.Add(new TextBlock { Text = ":", Margin = new Thickness(0, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center });
            timePanel.Children.Add(timeMinuteBox);
            Grid.SetRow(timePanel, 4);
            grid.Children.Add(timePanel);

            // Type
            typeBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
            typeBox.Items.Add(EventType.Meeting);
            typeBox.Items.Add(EventType.Birthday);
            typeBox.Items.Add(EventType.Task);
            typeBox.Items.Add(EventType.Other);
            Grid.SetRow(typeBox, 5);
            grid.Children.Add(typeBox);

            // Buttons
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var saveButton = new Button { Content = "Spara", Width = 100, Height = 30, Margin = new Thickness(0, 0, 10, 0) };
            var cancelButton = new Button { Content = "Avbryt", Width = 100, Height = 30 };

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, 6);
            grid.Children.Add(buttonPanel);

            Content = grid;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleBox.Text) || datePicker.SelectedDate == null ||
                timeHourBox.SelectedItem == null || timeMinuteBox.SelectedItem == null || typeBox.SelectedItem == null)
            {
                MessageBox.Show("Vänligen fyll i alla fält", "Validering", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewEvent = new Event
            {
                Title = titleBox.Text,
                Date = datePicker.SelectedDate.Value,
                Time = new TimeSpan(int.Parse(timeHourBox.SelectedItem.ToString()),
                                  int.Parse(timeMinuteBox.SelectedItem.ToString()), 0),
                Type = (EventType)typeBox.SelectedItem
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
