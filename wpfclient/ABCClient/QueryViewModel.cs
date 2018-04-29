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
    public class QueryViewModel : INotifyPropertyChanged
    {
        public class QueryItem
        {
            public QueryItem() { }
            public Entity School { get; set; }
            public string TranscriptId { get; set; }
            public string SecretKey { get; set; }
        }

        public QueryViewModel()
        {
            query = new QueryCommand(this);
        }

        ObservableCollection<QueryItem> items = new ObservableCollection<QueryItem>();
        public ObservableCollection<QueryItem> Items { get { return items; } }

        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

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

        class QueryCommand : ICommand
        {
            QueryViewModel owner;

            public QueryCommand(QueryViewModel vm)
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
                    worksheet.Cells[1, 6] = "School";
                    worksheet.Cells[1, 7] = "Transcript ID";
                    while (owner.Items.Count != 0)
                    {
                        var currentItem = owner.Items[0];
                        string result = await Client.QueryTranscriptAsync(currentItem.School.AccountAddress, currentItem.TranscriptId);
                        var structed = JsonConvert.DeserializeObject<TranscriptCore>(Encoding.UTF8.GetString(Crypto.Decode(result, currentItem.SecretKey)));
                        worksheet.Cells[count, 1] = structed.Student?.DisplayName ?? "???";
                        worksheet.Cells[count, 2] = structed.Year.ToString();
                        worksheet.Cells[count, 3] = structed.Term.ToString();
                        worksheet.Cells[count, 4] = structed.CourseId ?? "???";
                        worksheet.Cells[count, 5] = structed.Grade.ToString();
                        worksheet.Cells[count, 6] = currentItem.School.DisplayName;
                        worksheet.Cells[count, 7] = currentItem.TranscriptId;
                        ++count;
                        owner.Items.RemoveAt(0);
                    }
                }
                catch(Exception e)
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
        QueryCommand query;
        public ICommand Query { get { return query; } }
    }
}
