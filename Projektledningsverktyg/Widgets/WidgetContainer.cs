using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Media;

namespace Projektledningsverktyg.Widgets
{
    /// <summary>
    /// Container för att organisera widgets i ett grid-baserat system,
    /// med stöd för att dra och släppa widgets mellan platser.
    /// </summary>
    public class WidgetContainer : Grid
    {
        // Fält för att spåra dropzoner
        private List<Border> _dropZones;

        // Lista över alla registrerade widgets
        private List<DraggableWidget> _widgets = new List<DraggableWidget>();

        // Widget som för närvarande dras
        private DraggableWidget _draggingWidget;

        // Position där dragning påbörjades
        private int _startRow;
        private int _startColumn;

        /// <summary>
        /// Skapa ny widget-container med angiven storlek
        /// </summary>
        public WidgetContainer()
        {
            // Grundläggande inställningar
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.ShowGridLines = false;

            // Definiera standardstorlek för grid (3x3)
            for (int i = 0; i < 3; i++)
            {
                RowDefinitions.Add(new RowDefinition());
                ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        /// <summary>
        /// Registrera en widget i containern
        /// </summary>
        public void RegisterWidget(DraggableWidget widget, int row, int column)
        {
            // Kontrollera att positionen är giltig
            if (row < 0 || row >= RowDefinitions.Count ||
                column < 0 || column >= ColumnDefinitions.Count)
            {
                throw new ArgumentOutOfRangeException("Positionen är utanför giltigt område.");
            }

            // Tvinga widget att fylla hela kolumnens bredd
            widget.VerticalAlignment = VerticalAlignment.Stretch;
            widget.HorizontalAlignment = HorizontalAlignment.Stretch;

            // Registrera widgets event-handlers
            widget.DragStarted += Widget_DragStarted;
            widget.DragCompleted += Widget_DragCompleted;

            // Placera widget i grid
            SetRow(widget, row);
            SetColumn(widget, column);

            // Lägg till widget i containern och listan
            Children.Add(widget);
            _widgets.Add(widget);
        }

        /// <summary>
        /// Hantera när en widget börjar dras
        /// </summary>
        private void Widget_DragStarted(object sender, RoutedEventArgs e)
        {
            // Spara referens till widget som dras
            _draggingWidget = sender as DraggableWidget;

            // Spara startposition
            _startRow = GetRow(_draggingWidget);
            _startColumn = GetColumn(_draggingWidget);

            // Visa visuell feedback för möjliga drop-zoner
            UpdateDropZoneVisibility(true);
        }

        /// <summary>
        /// Hantera när en widget släpps
        /// </summary>
        private void Widget_DragCompleted(object sender, RoutedEventArgs e)
        {
            // Identifiera widget som släppts
            var widget = sender as DraggableWidget;
            if (widget == null)
                return;

            // Återställ renderingstransform
            if (widget.RenderTransform is TranslateTransform transform)
            {
                transform.X = 0;
                transform.Y = 0;
            }

            // Hämta positionen där widgeten släpptes
            var mousePosition = System.Windows.Input.Mouse.GetPosition(this);

            // Fördefinierade max-värden
            const int MAX_ROWS = 3;
            const int MAX_COLUMNS = 2;

            // Hitta grid-positionen för denna punkt
            int targetRow = -1, targetColumn = -1;

            // Beräkna målraden baserat på musposition
            double accHeight = 0;
            for (int r = 0; r < RowDefinitions.Count; r++)
            {
                double rowHeight = RowDefinitions[r].ActualHeight;
                if (mousePosition.Y >= accHeight && mousePosition.Y < accHeight + rowHeight)
                {
                    targetRow = r;
                    break;
                }
                accHeight += rowHeight;
            }

            // Beräkna målkolumnen baserat på musposition
            double accWidth = 0;
            for (int c = 0; c < ColumnDefinitions.Count; c++)
            {
                double colWidth = ColumnDefinitions[c].ActualWidth;
                if (mousePosition.X >= accWidth && mousePosition.X < accWidth + colWidth)
                {
                    targetColumn = c;
                    break;
                }
                accWidth += colWidth;
            }

            // Om utanför giltigt område, återställ till startposition
            if (targetRow < 0 || targetRow >= RowDefinitions.Count ||
                targetColumn < 0 || targetColumn >= ColumnDefinitions.Count)
            {
                targetRow = _startRow;
                targetColumn = _startColumn;
            }

            // Debug-utskrift för att se vilken position vi beräknat
            Console.WriteLine($"Drop position: {targetRow},{targetColumn}");

            // Om vi identifierat en giltig position och den skiljer sig från startpositionen
            if ((targetRow != _startRow || targetColumn != _startColumn) &&
                targetRow >= 0 && targetColumn >= 0)
            {
                Console.WriteLine("Position differs from start position");

                // Hitta widget på målplatsen (om någon) med en mer robust metod
                DraggableWidget widgetAtTarget = null;
                foreach (var child in Children)
                {
                    if (child is DraggableWidget w && w != widget)
                    {
                        int childRow = Grid.GetRow(w);
                        int childCol = Grid.GetColumn(w);

                        if (childRow == targetRow && childCol == targetColumn)
                        {
                            widgetAtTarget = w;
                            Console.WriteLine($"Found widget at target: {w.GetType().Name}");
                            break;
                        }
                    }
                }

                // Flytta widgets
                if (widgetAtTarget != null)
                {
                    Console.WriteLine($"Swapping positions with widget at {targetRow},{targetColumn}");

                    // Byt plats mellan widgets
                    Grid.SetRow(widgetAtTarget, _startRow);
                    Grid.SetColumn(widgetAtTarget, _startColumn);

                    // Återställ renderingstransform för widgeten på målplatsen
                    if (widgetAtTarget.RenderTransform is TranslateTransform targetTransform)
                    {
                        targetTransform.X = 0;
                        targetTransform.Y = 0;
                    }
                }

                // Flytta aktuell widget till ny position
                Grid.SetRow(widget, targetRow);
                Grid.SetColumn(widget, targetColumn);

                // Utlös event för att informera om ändrad position
                WidgetPositionsChanged();
            }

            // Återställ visuell feedback
            UpdateDropZoneVisibility(false);

            // Nollställ drag-state
            _draggingWidget = null;
        }

        /// <summary>
        /// Visar eller döljer visuell feedback för möjliga drop-zoner
        /// </summary>
        private void UpdateDropZoneVisibility(bool show)
        {
            // Om vi inte har några dropzone-element, skapa dem
            if (_dropZones == null || _dropZones.Count == 0)
            {
                _dropZones = new List<Border>();

                // Skapa en dropzone för varje cell i rutnätet
                for (int row = 0; row < RowDefinitions.Count; row++)
                {
                    for (int col = 0; col < ColumnDefinitions.Count; col++)
                    {
                        var dropZone = new Border
                        {
                            Background = new SolidColorBrush(Color.FromArgb(40, 0, 120, 215)),
                            BorderBrush = new SolidColorBrush(Colors.DodgerBlue),
                            BorderThickness = new Thickness(2),
                            CornerRadius = new CornerRadius(4),
                            Opacity = 0
                        };

                        // Placera i rutnätet
                        Grid.SetRow(dropZone, row);
                        Grid.SetColumn(dropZone, col);

                        // Lägg till i containern
                        this.Children.Add(dropZone);
                        _dropZones.Add(dropZone);

                        // Lägg till i Z-Index som gör att det hamnar under widgets
                        Panel.SetZIndex(dropZone, -1);
                    }
                }
            }

            // Visa eller dölj alla dropzones
            foreach (var dropZone in _dropZones)
            {
                dropZone.Opacity = show ? 0.7 : 0;
            }
        }

        /// <summary>
        /// Hämtar de aktuella positionerna för alla widgets i containern
        /// </summary>
        /// <param name="memberId">Användar-ID som äger dessa positioner</param>
        public List<WidgetPosition> GetCurrentPositions(int memberId)
        {
            List<WidgetPosition> positions = new List<WidgetPosition>();

            foreach (var child in Children)
            {
                if (child is DraggableWidget widget)
                {
                    // Hämta position i Grid
                    int row = Grid.GetRow(widget);
                    int column = Grid.GetColumn(widget);

                    // Skapa en WidgetPosition-post
                    positions.Add(new WidgetPosition
                    {
                        MemberId = memberId,
                        WidgetId = widget.WidgetId,
                        RowIndex = row,
                        ColumnIndex = column,
                        IsVisible = widget.Visibility == Visibility.Visible,
                        LastUpdated = DateTime.Now
                    });
                }
            }

            return positions;
        }

        /// <summary>
        /// Event som utlöses när widget-positioner ändras
        /// </summary>
        public event EventHandler PositionsChanged;

        /// <summary>
        /// Utlöser PositionsChanged-eventet
        /// </summary>
        private void WidgetPositionsChanged()
        {
            PositionsChanged?.Invoke(this, EventArgs.Empty);
        }

        // Anropas när layouten behöver återställas
        public void ResetLayout(int rowCount = 3, int columnCount = 2)
        {
            // Rensa definitioner
            RowDefinitions.Clear();
            ColumnDefinitions.Clear();

            // Lägg till rader - använd Auto istället för fast höjd eller Star
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Lägg till kolumner - behåll Star för kolumner
            for (int i = 0; i < columnCount; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
        }
    }
}
