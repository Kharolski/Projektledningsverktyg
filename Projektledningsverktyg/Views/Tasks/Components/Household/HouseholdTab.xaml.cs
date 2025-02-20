using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using Projektledningsverktyg.Data.Repository;
using Projektledningsverktyg.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.Tasks.Components.Household
{
    public partial class HouseholdTab : Page
    {
        private readonly HouseholdViewModel _viewModel;

        public HouseholdTab()
        {
            var context = new ApplicationDbContext();
            var repository = new HouseholdRepository(context);
            _viewModel = new HouseholdViewModel(repository);
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            _viewModel.DayStatesUpdated += ApplyVisualStates;
            DataContext = _viewModel;
        }

        #region Border/Visual Handling
        private void TaskBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border taskBorder && taskBorder.DataContext is Data.Entities.Household task)
            {
                _viewModel?.SelectTask(task);
                RefreshVisuals();
            }
        }
        private void MemberBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border memberBorder && memberBorder.DataContext is Member member)
            {
                _viewModel?.SelectMember(member);
                memberBorder.BorderBrush = _viewModel?.GetMemberBorderColor(member);
            }
        }
        private void RefreshVisuals()
        {
            foreach (var border in FindVisualChildren<Border>(this))
            {
                if (border.DataContext is Member member)
                {
                    border.BorderBrush = _viewModel.GetMemberBorderColor(member);
                }
                else if (border.DataContext is Data.Entities.Household task)
                {
                    border.BorderBrush = _viewModel.GetTaskBorderColor(task);
                }
            }
        }

        private void ApplyVisualStates(Dictionary<(string Day, int TaskId), (bool IsLocked, bool IsSelected)> states)
        {
            foreach (var border in FindVisualChildren<Border>(this))
            {
                if (border.Tag is string dayTag &&
                    border.DataContext is Data.Entities.Household task)
                {
                    if (states.TryGetValue((dayTag, task.Id), out var state))
                    {
                        ApplyBorderColor(border, state);
                    }
                    else
                    {
                        // Default state for unselected days
                        border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                    }
                }
            }
        }

        private void ApplyBorderColor(Border border, (bool IsLocked, bool IsSelected) state)
        {
            if (state.IsLocked)
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffcdd2"));
            else if (state.IsSelected)
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b3dcfa"));
            else
                border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
        }
        #endregion 

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HouseholdViewModel.UserMessage))
            {
                UpdateErrorMessage(_viewModel.UserMessage);
            }

            if (e.PropertyName == nameof(HouseholdViewModel.SuccessMessage))
            {
                UpdateSuccessMessage(_viewModel.SuccessMessage);
            }
        }
        private void UpdateSuccessMessage(string message)
        {
            SuccessMessage.Text = message;
            SuccessBorder.Visibility = string.IsNullOrEmpty(message)
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        private void UpdateErrorMessage(string message)
        {
            ErrorMessage.Text = message;
            ErrorBorder.Visibility = string.IsNullOrEmpty(message)
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        private void DayTag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border dayBorder && dayBorder.DataContext is Data.Entities.Household task)
            {
                e.Handled = true;
                string day = dayBorder.Tag as string;
                var viewModel = DataContext as HouseholdViewModel;

                if (!viewModel.IsSelectedTask(task))
                {
                    viewModel.UserMessage = "Vänligen välj en uppgift först";
                    return;
                }

                if (!task.Assignments.Any())
                {
                    viewModel.UserMessage = "Välj familjemedlem innan du väljer dagar";
                    return;
                }

                viewModel?.HandleDaySelection(day, task);
                dayBorder.Background = viewModel?.GetDayBackground(day, task);
            }
        }

        // Find all visual children of a certain type
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                yield return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                if (child is T t)
                    yield return t;

                foreach (T childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
        }
    }
}
