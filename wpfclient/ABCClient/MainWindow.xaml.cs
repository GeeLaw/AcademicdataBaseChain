using System.Windows;

namespace ABCClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenBoE(object sender, RoutedEventArgs e)
        {
            (new BoEWindow()).Show();
        }

        private void OpenSend(object sender, RoutedEventArgs e)
        {
            (new SendTranscriptWindow()).Show();
        }

        private void OpenQuery(object sender, RoutedEventArgs e)
        {
            (new QueryWindow()).Show();
        }
    }
}
