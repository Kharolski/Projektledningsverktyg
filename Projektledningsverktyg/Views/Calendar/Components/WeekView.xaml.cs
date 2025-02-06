using Projektledningsverktyg.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Projektledningsverktyg.Helpers;
using System;
using Projektledningsverktyg.Views.Calendar.Components.WeekComponents;

namespace Projektledningsverktyg.Views.Calendar.Components
{
    public partial class WeekView : UserControl
    {
        public WeekMonthViewModel ViewModel { get; set; }
        private WeekViewLayoutManager _layoutManager;

        public WeekView()
        {
            _layoutManager = new WeekViewLayoutManager(MainCanvas);

            InitializeComponent();
            ViewModel = new WeekMonthViewModel();
            DataContext = ViewModel;
            Loaded += WeekView_Loaded;

            RegisterContentChangeHandlers();
        }

        private void WeekView_Loaded(object sender, RoutedEventArgs e)
        {
            _layoutManager = new WeekViewLayoutManager(MainCanvas);

            // Register controls with specific keys
            _layoutManager.RegisterControl("Schedule", FindName("ScheduleControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Meals", FindName("MealsControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Household", FindName("HouseholdControl") as DraggableControlBase);
            _layoutManager.RegisterControl("GeneralTask", FindName("GeneralTaskControl") as DraggableControlBase);
            _layoutManager.RegisterControl("Events", FindName("EventsControl") as DraggableControlBase);

            _layoutManager.UpdateLayout();

            // Check initial content height
            MainCanvas.UpdateLayout();
            MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();

            // Monitor all controls for size changes
            foreach (var control in _layoutManager.GetAllControls())
            {
                control.SizeChanged += (s, args) =>
                {
                    MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();
                };
            }
        }

        // Keep your existing FindVisualChildren helper method
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        private void RegisterContentChangeHandlers()
        {
            var eventsControl = FindName("EventsControl") as EventsControl;
            var mealsControl = FindName("MealsControl") as MealsControl;
            var householdControl = FindName("HouseholdControl") as HouseholdControl;
            var generalTaskControl = FindName("GeneralTaskControl") as GeneralTaskControl;

            if (eventsControl != null)
                eventsControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (mealsControl != null)
                mealsControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (householdControl != null)
                householdControl.ContentSizeChanged += Control_ContentSizeChanged;
            if (generalTaskControl != null)
                generalTaskControl.ContentSizeChanged += Control_ContentSizeChanged;
        }
        private void Control_ContentSizeChanged(object sender, EventArgs e)
        {
            MainCanvas.UpdateLayout();
            MainCanvas.MinHeight = _layoutManager.GetMaxContentHeight();
        }

    }
}
