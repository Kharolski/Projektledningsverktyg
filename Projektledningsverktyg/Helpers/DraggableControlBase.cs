using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace Projektledningsverktyg.Helpers
{
    /// <summary>
    /// Base class for all draggable controls in the application
    /// Provides common drag and drop functionality
    /// </summary>
    public class DraggableControlBase : UserControl
    {
        // event for position changes
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        private static Border _draggedItem;
        private Point _mouseDownPosition;
        private DispatcherTimer _dragTimer;
        private Point _lastMousePosition;

        // properties for Canvas positioning
        public double XPosition { get; set; }
        public double YPosition { get; set; }
        public bool IsPositionSet { get; set; }
        public string ControlKey { get; set; }

        public DraggableControlBase()
        {
            this.Loaded += DraggableControlBase_Loaded;
        }

        private void DraggableControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            var border = this.FindName("MainBorder") as Border;
            if (border != null)
            {
                border.MouseDown += OnMouseDown;
                border.MouseMove += OnMouseMove;
                border.MouseUp += OnMouseUp;
            }
            else
            {
                Debug.WriteLine($"Border NOT found for {this.GetType().Name}");
            }
        }

        protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            //Debug.WriteLine($"Mouse down started on {this.GetType().Name}");
            _mouseDownPosition = e.GetPosition(Application.Current.MainWindow);
            _lastMousePosition = _mouseDownPosition;

            _dragTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _dragTimer.Tick += (s, args) =>
            {
                _dragTimer.Stop();
                //Debug.WriteLine($"Timer completed - starting drag on {this.GetType().Name}");
                StartDrag(border);
            };
            _dragTimer.Start();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragTimer != null && e.LeftButton != MouseButtonState.Pressed)
            {
                _dragTimer.Stop();
                _dragTimer = null;
                return;
            }

            if (_draggedItem != null)
            {
                var mainCanvas = FindParentByName<Canvas>("MainCanvas", _draggedItem);
                if (mainCanvas != null)
                {
                    var currentPosition = e.GetPosition(mainCanvas);
                    double deltaX = currentPosition.X - _lastMousePosition.X;
                    double deltaY = currentPosition.Y - _lastMousePosition.Y;

                    // Update the UserControl position, not the Border
                    Canvas.SetLeft(this, Math.Max(0, Math.Min(Canvas.GetLeft(this) + deltaX, mainCanvas.ActualWidth - ActualWidth)));
                    Canvas.SetTop(this, Math.Max(5, Math.Min(Canvas.GetTop(this) + deltaY, mainCanvas.ActualHeight - ActualHeight)));

                    _lastMousePosition = currentPosition;
                }
            }
        }

        private T FindParentByName<T>(string name, DependencyObject element) where T : FrameworkElement
        {
            while (element != null)
            {
                if (element is T && (element as FrameworkElement).Name == name)
                    return element as T;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private void StartDrag(Border border)
        {
            if (_draggedItem != null)
                return;

            var mainCanvas = FindParentByName<Canvas>("MainCanvas", border);
            if (mainCanvas != null)
            {
                _draggedItem = border;
                _draggedItem.Opacity = 0.7;
                Panel.SetZIndex(_draggedItem, 1000);

                // Get the actual Canvas positions
                XPosition = Canvas.GetLeft(this);
                YPosition = Canvas.GetTop(this);

                _lastMousePosition = Mouse.GetPosition(mainCanvas);
                border.CaptureMouse();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_draggedItem == null)
                return;

            // Animate to final position if needed
            var animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            _draggedItem.Opacity = 1.0;
            Panel.SetZIndex(_draggedItem, 0);
            _draggedItem.ReleaseMouseCapture();
            _draggedItem = null;
        }


    }
}
