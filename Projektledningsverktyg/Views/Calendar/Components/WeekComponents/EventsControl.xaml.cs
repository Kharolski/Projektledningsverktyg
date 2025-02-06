using Projektledningsverktyg.Helpers;
using System;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    /// <summary>
    /// Interaction logic for EventsControl.xaml
    /// </summary>
    public partial class EventsControl : DraggableControlBase
    {
        public event EventHandler ContentSizeChanged;
        public EventsControl()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
