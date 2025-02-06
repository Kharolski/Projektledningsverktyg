using System;

namespace Projektledningsverktyg.Helpers
{
    public class PositionChangedEventArgs : EventArgs
    {
        public double NewX { get; set; }
        public double NewY { get; set; }
        public string ControlKey { get; set; }
    }
}
