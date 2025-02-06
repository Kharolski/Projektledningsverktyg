using Projektledningsverktyg.Helpers;
using System;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class ScheduleControl : DraggableControlBase
    {
        public event EventHandler ContentSizeChanged;
        public ScheduleControl()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
