using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Excel = Microsoft.Office.Interop.Excel;

namespace ABCClient
{
    /// <summary>
    /// Interaction logic for SendTranscriptWindow.xaml
    /// </summary>
    public partial class SendTranscriptWindow : Window
    {
        public SendTranscriptWindow()
        {
            InitializeComponent();
            DataContext = new SendBatchViewModel();
        }
    }
}
