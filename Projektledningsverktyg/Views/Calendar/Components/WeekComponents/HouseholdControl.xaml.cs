using Projektledningsverktyg.Helpers;
using System;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class HouseholdControl : DraggableControlBase
    {
        public event EventHandler ContentSizeChanged;
        public HouseholdControl()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
