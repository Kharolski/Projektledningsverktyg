using Projektledningsverktyg.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    public partial class MonthView : UserControl
    {
        private DateTime currentDate = DateTime.Now;
        private ObservableCollection<Event> events;

        public MonthView()
        {
            InitializeComponent();
            LoadEvents();
            GenerateCalendar();
        }

        private void GenerateCalendar()
        {
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Update header
            MonthYearText.Text = currentDate.ToString("MMMM yyyy");

            // Clear existing days
            CalendarGrid.Children.Clear();

            // Calculate the offset for the first day
            int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            // Add empty cells for offset
            for (int i = 0; i < offset; i++)
            {
                CalendarGrid.Children.Add(CreateEmptyDay());
            }

            // Add days of the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                CalendarGrid.Children.Add(CreateDayCell(day));
            }

            // Fill remaining cells
            int remainingCells = 42 - (offset + daysInMonth);
            for (int i = 0; i < remainingCells; i++)
            {
                CalendarGrid.Children.Add(CreateEmptyDay());
            }
        }

        private UIElement CreateDayCell(int day)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(1)
            };

            var grid = new Grid();

            // Highlight today
            if (day == DateTime.Now.Day && currentDate.Month == DateTime.Now.Month && currentDate.Year == DateTime.Now.Year)
            {
                grid.Background = new SolidColorBrush(Color.FromRgb(179, 229, 252)); // Light blue for today
            }
            else
            {
                grid.Background = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            }

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            // Date number
            var textBlock = new TextBlock
            {
                Text = day.ToString(),
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold
            };
            Grid.SetRow(textBlock, 0);
            grid.Children.Add(textBlock);

            // Event indicators
            var indicatorPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 2)
            };
            Grid.SetRow(indicatorPanel, 1);

            // Example: Add indicators if events exist
            if (HasEvents(day))  // You'll need to implement this method
            {
                indicatorPanel.Children.Add(CreateEventDot(Colors.Red));    // Meeting
                indicatorPanel.Children.Add(CreateEventDot(Colors.Green));  // Birthday
                indicatorPanel.Children.Add(CreateEventDot(Colors.Orange)); // Task
            }

            grid.Children.Add(indicatorPanel);
            border.Child = grid;
            return border;
        }

        private UIElement CreateEventDot(Color color)
        {
            return new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new SolidColorBrush(color),
                Margin = new Thickness(1, 0, 1, 0)
            };
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
                    Title = "Lisas födelsedag",
                    Date = DateTime.Now.AddDays(3),
                    Type = EventType.Birthday
                }
                // Add more events here
            };
        }

        private bool HasEvents(int day)
        {
            var checkDate = new DateTime(currentDate.Year, currentDate.Month, day);
            return events.Any(e => e.Date.Date == checkDate.Date);
        }

        private IEnumerable<Event> GetEventsForDay(int day)
        {
            var checkDate = new DateTime(currentDate.Year, currentDate.Month, day);
            return events.Where(e => e.Date.Date == checkDate.Date);
        }

        private Color GetEventColor(EventType type)
        {
            switch (type)
            {
                case EventType.Meeting:
                    return Colors.Blue;
                case EventType.Birthday:
                    return Colors.Green;
                case EventType.Travel:
                    return Colors.Orange;
                default:
                    return Colors.Gray;
            }
        }

        private UIElement CreateEmptyDay()
        {
            return new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(0.5),
                Margin = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(250, 250, 250))
            };
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            GenerateCalendar();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            GenerateCalendar();
        }
    }
}
