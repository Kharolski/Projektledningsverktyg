using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projektledningsverktyg.Views.AIAssistant
{
    /// <summary>
    /// Interaction logic for AIAssistantView.xaml
    /// </summary>
    public partial class AIAssistantView : UserControl
    {
        private ObservableCollection<ChatMessage> messages;

        public AIAssistantView()
        {
            InitializeComponent();
            messages = new ObservableCollection<ChatMessage>();
            MessageList.ItemsSource = messages;
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(InputBox.Text))
            {
                // Add user message
                messages.Add(new ChatMessage
                {
                    Content = InputBox.Text,
                    IsUser = true
                });

                // Simulate AI response
                messages.Add(new ChatMessage
                {
                    Content = "Detta är ett exempel på AI-svar.",
                    IsUser = false
                });

                InputBox.Clear();
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage_Click(sender, e);
            }
        }

        public class BoolToAlignmentConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class ChatMessage
    {
        public string Content { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
