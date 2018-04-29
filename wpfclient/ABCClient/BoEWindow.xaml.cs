using System.Windows;

namespace ABCClient
{
    /// <summary>
    /// Interaction logic for BoEWindow.xaml
    /// </summary>
    public partial class BoEWindow : Window
    {
        public BoEWindow()
        {
            InitializeComponent();
            DataContext = new BoEViewModel();
        }
    }
}
