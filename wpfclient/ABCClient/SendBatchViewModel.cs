using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace ABCClient
{
    public class SendBatchViewModel : INotifyPropertyChanged
    {
        public SendBatchViewModel()
        {
            submit = new SubmitCommand(this);
        }

        ObservableCollection<TranscriptCore> transcripts = new ObservableCollection<TranscriptCore>();
        public ObservableCollection<TranscriptCore> Transcripts { get { return transcripts; } }

        public IEnumerable<Entity> Entities { get { return Entity.Entities; } }

        bool isUIEnabled = true;
        public bool IsUIEnabled
        {
            get { return isUIEnabled; }
            set
            {
                if (value != isUIEnabled)
                {
                    isUIEnabled = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IsUIEnabled"));
                }
            }
        }

        string selectedEntity;
        public string SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                if (value != selectedEntity)
                {
                    selectedEntity = value;
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SelectedEntity"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

        class SubmitCommand : ICommand
        {
            SendBatchViewModel owner;

            public SubmitCommand(SendBatchViewModel vm)
            {
                owner = vm;
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public async void Execute(object parameter)
            {
                Excel.Application excel = null;
                dynamic workbook = null, worksheet = null;
                try
                {
                    owner.IsUIEnabled = false;
                    excel = new Excel.Application();
                    excel.Visible = true;
                    workbook = excel.Workbooks.Add();
                    worksheet = workbook.Worksheets[1];
                    int count = 2;
                    worksheet.Cells[1, 1] = "Student";
                    worksheet.Cells[1, 2] = "Year";
                    worksheet.Cells[1, 3] = "Term";
                    worksheet.Cells[1, 4] = "Course ID";
                    worksheet.Cells[1, 5] = "Grade";
                    worksheet.Cells[1, 6] = "School ID";
                    worksheet.Cells[1, 7] = "Transcript ID";
                    worksheet.Cells[1, 8] = "Key";
                    while (owner.Transcripts.Count != 0)
                    {
                        var entryToSend = owner.Transcripts[0];
                        string key, tid = Guid.NewGuid().ToString("n");
                        worksheet.Cells[count, 1] = entryToSend.Student.DisplayName;
                        worksheet.Cells[count, 2] = entryToSend.Year.ToString();
                        worksheet.Cells[count, 3] = entryToSend.Term.ToString();
                        worksheet.Cells[count, 4] = entryToSend.CourseId.ToString();
                        worksheet.Cells[count, 5] = entryToSend.Grade.ToString();
                        worksheet.Cells[count, 6] = owner.SelectedEntity;
                        worksheet.Cells[count, 7] = tid;
                        if (!await Client.SetTranscriptAsync(tid,
                            Crypto.Encode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entryToSend)), out key),
                            owner.SelectedEntity))
                        {
                            throw new OperationCanceledException("A transcript failed to send.");
                        }
                        worksheet.Cells[count, 8] = key;
                        ++count;
                        owner.Transcripts.RemoveAt(0);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "An exception occurred", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                finally
                {
                    owner.IsUIEnabled = true;
                    if (worksheet != null)
                    {
                        Marshal.ReleaseComObject(worksheet);
                        if (workbook != null)
                        {
                            Marshal.ReleaseComObject(workbook);
                            if (excel != null)
                            {
                                Marshal.ReleaseComObject(excel);
                            }
                        }
                    }
                }
            }
        }

        SubmitCommand submit;
        public ICommand Submit { get { return submit; } }

    }
}
