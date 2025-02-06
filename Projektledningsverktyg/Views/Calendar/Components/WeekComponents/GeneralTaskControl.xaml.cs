using Projektledningsverktyg.Helpers;
using System;

namespace Projektledningsverktyg.Views.Calendar.Components.WeekComponents
{
    public partial class GeneralTaskControl : DraggableControlBase
    {
        public event EventHandler ContentSizeChanged;
        public GeneralTaskControl()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => ContentSizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
