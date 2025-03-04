using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Projektledningsverktyg.Widgets
{
    /// <summary>
    /// Basklass för alla dragbara widgets i applikationen.
    /// Innehåller logik för att hantera dragning och visuell design.
    /// </summary>
    public class DraggableWidget : UserControl
    {
        private Point _startPoint;
        private bool _isDragging;

        // Dependency property för widget ID
        public static readonly DependencyProperty WidgetIdProperty =
            DependencyProperty.Register("WidgetId", typeof(string), typeof(DraggableWidget),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Unikt ID för denna widget-typ
        /// </summary>
        public string WidgetId
        {
            get { return (string)GetValue(WidgetIdProperty); }
            set { SetValue(WidgetIdProperty, value); }
        }

        // Dependency property för widgetens rubrik
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DraggableWidget),
                new PropertyMetadata("Widget"));

        /// <summary>
        /// Rubriken som visas på widgeten
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Event som utlöses när widgeten börjar dras
        public static readonly RoutedEvent DragStartedEvent =
            EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DraggableWidget));

        /// <summary>
        /// Event som utlöses när widgeten börjar dras
        /// </summary>
        public event RoutedEventHandler DragStarted
        {
            add { AddHandler(DragStartedEvent, value); }
            remove { RemoveHandler(DragStartedEvent, value); }
        }

        // Event som utlöses när widgeten har släppts
        public static readonly RoutedEvent DragCompletedEvent =
            EventManager.RegisterRoutedEvent("DragCompleted", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(DraggableWidget));

        /// <summary>
        /// Event som utlöses när widgeten har släppts
        /// </summary>
        public event RoutedEventHandler DragCompleted
        {
            add { AddHandler(DragCompletedEvent, value); }
            remove { RemoveHandler(DragCompletedEvent, value); }
        }

        public DraggableWidget()
        {
            // Sätt standardstil för alla widgets
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            // Ta bort border
            this.BorderThickness = new Thickness(0);

            // Inaktivera standard focus visual
            this.FocusVisualStyle = null;

            // Aktivera automatisk höjdanpassning
            this.Height = double.NaN; // Auto

            // Aktivera mouse events för dragning
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            this.MouseMove += OnMouseMove;
            this.MouseLeftButtonUp += OnMouseLeftButtonUp;

            // För att förhindra att border visas när kontroll blir selected
            this.IsKeyboardFocusWithinChanged += (s, e) => this.BorderThickness = new Thickness(0);
            this.GotFocus += (s, e) => this.BorderThickness = new Thickness(0);
        }

        /// <summary>
        /// Hanterar klick på widgeten
        /// </summary>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.BorderThickness = new Thickness(0);

            // Viktig ändring: Använd Parent som referens
            _startPoint = e.GetPosition(this.Parent as UIElement);
            _isDragging = true;

            this.CaptureMouse();
            RaiseEvent(new RoutedEventArgs(DragStartedEvent, this));
            e.Handled = true;

        }

        /// <summary>
        /// Hanterar musrörelse för att flytta widgeten
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.BorderThickness = new Thickness(0);

            if (_isDragging)
            {
                // Hämta aktuell position - viktig ändring: använd Parent som referens
                Point currentPosition = e.GetPosition(this.Parent as UIElement);

                // Beräkna hur mycket widgeten ska flyttas
                double deltaX = currentPosition.X - _startPoint.X;
                double deltaY = currentPosition.Y - _startPoint.Y;

                // Flytta widgeten visuellt med TranslateTransform
                if (!(this.RenderTransform is TranslateTransform))
                {
                    this.RenderTransform = new TranslateTransform();
                }

                TranslateTransform transform = (TranslateTransform)this.RenderTransform;
                transform.X = deltaX;
                transform.Y = deltaY;

                // Uppdatera visuell återkoppling under dragning
                this.Opacity = 0.7;
                this.BorderThickness = new Thickness(2);
                this.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Hanterar släppning av widgeten
        /// </summary>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.BorderThickness = new Thickness(0);

            if (_isDragging)
            {
                // Avsluta dragning
                _isDragging = false;

                // Återställ utseende
                this.Opacity = 1.0;
                this.BorderThickness = new Thickness(1);
                this.BorderBrush = new SolidColorBrush(Colors.LightGray);

                // Släpp musknapturingen
                this.ReleaseMouseCapture();

                // Utlös event för avslutad dragning
                RaiseEvent(new RoutedEventArgs(DragCompletedEvent, this));

                e.Handled = true;
            }
        }
    }
}
