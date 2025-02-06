using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Projektledningsverktyg.Helpers
{
    public class WeekViewLayoutManager
    {
        private readonly Canvas _mainCanvas;
        private readonly Dictionary<string, DraggableControlBase> _controls;
        private readonly Dictionary<string, Point> _lastKnownPositions;

        public WeekViewLayoutManager(Canvas mainCanvas)
        {
            _mainCanvas = mainCanvas;
            _controls = new Dictionary<string, DraggableControlBase>();
            _lastKnownPositions = new Dictionary<string, Point>();
        }

        public void RegisterControl(string key, DraggableControlBase control)
        {
            _controls[key] = control;
            control.ControlKey = key;

            if (!control.IsPositionSet)
            {
                SetInitialPosition(key, control);
                control.IsPositionSet = true;
            }
        }

        private void SetInitialPosition(string key, DraggableControlBase control)
        {
            // Define default positions for each control
            switch (key)
            {
                // Left column controls
                case "Schedule":
                    Canvas.SetLeft(control, 0);
                    Canvas.SetTop(control, 5);
                    break;
                case "Meals":
                    Canvas.SetLeft(control, 0);
                    Canvas.SetTop(control, 307);
                    break;

                // Right column controls
                case "Household":
                    Canvas.SetLeft(control, 425);
                    Canvas.SetTop(control, 5);
                    break;
                case "GeneralTask":
                    Canvas.SetLeft(control, 425);
                    Canvas.SetTop(control, 165);
                    break;
                case "Events":
                    Canvas.SetLeft(control, 425);
                    Canvas.SetTop(control, 325);
                    break;
            }

            // Store the initial position
            _lastKnownPositions[key] = new Point(Canvas.GetLeft(control), Canvas.GetTop(control));
        }

        public void UpdateLayout()
        {
            // Only used for initial layout or window resize
            foreach (var key in _controls.Keys)
            {
                if (!_controls[key].IsPositionSet)
                {
                    SetInitialPosition(key, _controls[key]);
                }
            }
        }


        public double GetMaxContentHeight()
        {
            double maxHeight = 510;
            foreach (var control in _controls.Values)
            {
                double bottomPosition = Canvas.GetTop(control) + control.ActualHeight;
                maxHeight = Math.Max(maxHeight, bottomPosition + 20);
            }
            return maxHeight;
        }
        public IEnumerable<DraggableControlBase> GetAllControls()
        {
            return _controls.Values;
        }
    }

}
