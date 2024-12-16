using Projektledningsverktyg.Models;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    /// <summary>
    /// Interaction logic for WeekView.xaml
    /// </summary>
    public partial class WeekView : UserControl
    {
        private DateTime currentWeekStart;
        private ObservableCollection<Event> events;

        public WeekView()
        {
            InitializeComponent();
            currentWeekStart = GetStartOfWeek(DateTime.Now);
            LoadEvents();
            GenerateTimeGrid();
            UpdateWeekDisplay();
            UpdateDayHeaders();
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private void GenerateTimeGrid()
        {
            TimeGrid.RowDefinitions.Clear();

            // Add rows for each hour (24 hours)
            for (int i = 0; i < 24; i++)
            {
                TimeGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(60) });

                // Add time label
                var timeLabel = new TextBlock
                {
                    Text = $"{i:00}:00",
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(5, 5, 5, 0)
                };
                Grid.SetRow(timeLabel, i);
                Grid.SetColumn(timeLabel, 0);
                TimeGrid.Children.Add(timeLabel);

                // Add separator lines
                for (int j = 0; j < 8; j++)
                {
                    var border = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                        BorderThickness = new Thickness(0, 0, 1, 1)
                    };
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    TimeGrid.Children.Add(border);
                }
            }

            // Add events to the grid
            AddEventsToGrid();
        }

        private void AddEventsToGrid()
        {
            foreach (var evt in events)
            {
                if (evt.Date.Date >= currentWeekStart && evt.Date.Date < currentWeekStart.AddDays(7))
                {
                    var eventControl = CreateEventControl(evt);
                    TimeGrid.Children.Add(eventControl);
                }
            }
        }

        private Color GetEventColor(EventType type)
        {
            switch (type)
            {
                case EventType.Meeting:
                    return Colors.Blue;
                case EventType.Birthday:
                    return Colors.Green;
                case EventType.Task:
                    return Colors.Orange;
                default:
                    return Colors.Gray;
            }
        }

        private UIElement CreateEventControl(Event evt)
        {
            var dayIndex = (int)evt.Date.DayOfWeek;
            var hourIndex = evt.Time.Hours;

            var border = new Border
            {
                Background = new SolidColorBrush(GetEventColor(evt.Type)),
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(2),
                Padding = new Thickness(5)
            };

            var text = new TextBlock
            {
                Text = evt.Title,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = text;
            Grid.SetRow(border, hourIndex);
            Grid.SetColumn(border, dayIndex);

            return border;
        }

        private void UpdateWeekDisplay()
        {
            var weekNumber = currentWeekStart.AddDays(3).DayOfYear / 7 + 1;
            WeekText.Text = $"Vecka {weekNumber}, {currentWeekStart.Year}";
        }

        private void UpdateDayHeaders()
        {
            for (int i = 0; i < 7; i++)
            {
                var date = currentWeekStart.AddDays(i);
                var dayHeader = (TextBlock)DaysHeaderGrid.Children[i + 1];
                string dayName = GetSwedishDayName(date.DayOfWeek);
                dayHeader.Text = $"{dayName} {date.Day}";
            }
        }

        private string GetSwedishDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "Måndag";
                case DayOfWeek.Tuesday:
                    return "Tisdag";
                case DayOfWeek.Wednesday:
                    return "Onsdag";
                case DayOfWeek.Thursday:
                    return "Torsdag";
                case DayOfWeek.Friday:
                    return "Fredag";
                case DayOfWeek.Saturday:
                    return "Lördag";
                case DayOfWeek.Sunday:
                    return "Söndag";
                default:
                    return "";
            }
        }

        private void PreviousWeek_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(-7);
            GenerateTimeGrid();
            UpdateWeekDisplay();
            UpdateDayHeaders();
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(7);
            GenerateTimeGrid();
            UpdateWeekDisplay();
            UpdateDayHeaders();
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
                    Type = EventType.Meeting
                },
                new Event
                {
                    Title = "Lunch med team",
                    Date = DateTime.Now,
                    Time = new TimeSpan(12, 0, 0),
                    Type = EventType.Other
                }
            };
        }
    }
}
