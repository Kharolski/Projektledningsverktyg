using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Projektledningsverktyg.Views.Tasks.Components.Meals.Components
{
    public partial class ViewMealDialog : Window
    {
        public ViewMealDialog()
        {
            InitializeComponent();
            ImageGrid.SizeChanged += (s, e) =>
            {
                var border = ImageGrid.Children.OfType<Border>().First();
                border.Clip = new RectangleGeometry(new Rect(0, 0, ImageGrid.ActualWidth, 300), 12, 12);
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
