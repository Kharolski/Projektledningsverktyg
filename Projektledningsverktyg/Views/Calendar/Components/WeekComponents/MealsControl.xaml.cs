using Projektledningsverktyg.Helpers;
using System;
using System.Windows;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class MealsControl : DraggableControlBase
    {
        public event EventHandler ContentSizeChanged;

        public MealsControl()
        {
            InitializeComponent();

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
