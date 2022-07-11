using MayEpCHADesktopApp.Core.Components;
using MayEpCHADesktopApp.Core.Model;
using MayEpCHADesktopApp.Core.Services.Interfaces;
using MayEpCHADesktopApp.Core.ViewModels.BaseViewModels;
using MayEpCHADesktopApp.Core.ViewModels.SettingsViewModels;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MayEpCHADesktopApp.Core.ViewModels.ReportViewModels
{

    public class ReportShiftViewModel : ViewModels.BaseViewModels.BaseViewModel
    {
        //Biến tạo shiftreport
        private DateTime date;
        private int id;
        private EShift ShiftNumber;
        private int shift;
        private Employee employee;
        private Machine machine;
        private Product product;
        private int totalQuantity;
        private DateTime startTime;
        private DateTime stopTime;
        private double workTime;
        private double pauseTime;
        private string note;
        private Mold mold;
        private int unit;
        private string totalQuantityStandard;
        public string TotalQuantityStandard { get => totalQuantityStandard; set { totalQuantityStandard = value; OnPropertyChanged(); } }
        private string moldId;
        public string MoldId { get => moldId; set { moldId = value; OnPropertyChanged(); } }
        private ShiftReport selectShiftReport;
        public DateTime Date { get => date; set { date = value; OnPropertyChanged(); } }
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public int Shift { get => shift; set { shift = value; OnPropertyChanged(); } }
        public Employee Employee { get => employee; set { employee = value; OnPropertyChanged(); } }
        public Machine Machine { get => machine; set { machine = value; OnPropertyChanged(); } }
        public Product Product { get => product; set { product = value; OnPropertyChanged(); Unit = (Product.Unit == EUnit.Kilogram) ? 1 : 0; MoldId = Product.MoldId; Mold = Product.Mold; } }
        public int TotalQuantity { get => totalQuantity; set { totalQuantity = value; OnPropertyChanged(); } }
        public DateTime StartTime { get => startTime; set { startTime = value; OnPropertyChanged(); } }
        public DateTime StopTime { get => stopTime; set { stopTime = value; OnPropertyChanged(); } }
        public double WorkTime { get => workTime; set { workTime = value; OnPropertyChanged(); } }
        public double PauseTime { get => pauseTime; set { pauseTime = value; OnPropertyChanged(); } }
        public string Note { get => note; set { note = value; OnPropertyChanged(); } }
        public Mold Mold { get => mold; set { mold = value; OnPropertyChanged(); } }
        public int Unit { get => unit; set { unit = value; OnPropertyChanged(); } }
        public static event Action ChangeEvent;
        public ShiftReport SelectShiftReport
        {
            get => selectShiftReport;
            set
            {
                selectShiftReport = value;
                OnPropertyChanged();
                if (SelectShiftReport != null)
                {
                    if (SelectShiftReport.Date != null)
                    {
                        Date = SelectShiftReport.Date;
                    }
                    if (SelectShiftReport.ShiftNumber != null)
                    {
                        Shift = (SelectShiftReport.ShiftNumber == EShift.Night) ? 0 : 1;
                    }
                    if (SelectShiftReport.Employee != null)
                    {
                        Employee = SelectShiftReport.Employee;
                    }
                    if (SelectShiftReport.Machine != null)
                    {
                        Machine = SelectShiftReport.Machine;
                    }
                    if (SelectShiftReport.Product != null)
                    {
                        Product = SelectShiftReport.Product;
                    }
                    if (SelectShiftReport.Product != null)
                    {
                        Mold = SelectShiftReport.Product.Mold;
                        MoldId = Mold.Id;
                    }

                    //if (SelectShiftReport.Product.Unit != null)
                    //{
                    //    Unit = (SelectShiftReport.Product.Unit == EUnit.Kilogram) ? 0 : 1;
                    //}


                    if (SelectShiftReport.TotalQuantity != null)
                    {
                        TotalQuantity = SelectShiftReport.TotalQuantity;
                    }
                    if (SelectShiftReport.StartTime != null)
                    {
                        StartTime = SelectShiftReport.StartTime;
                    }
                    if (SelectShiftReport.StopTime != null)
                    {
                        StopTime = SelectShiftReport.StopTime;
                    }
                    if (SelectShiftReport.WorkTime != null)
                    {
                        WorkTime = SelectShiftReport.WorkTime;
                        TotalQuantityStandard = (WorkTime * 60 / Convert.ToDouble(SelectShiftReport.Product.Mold.StandardInjectionCycle) + 100).ToString();
                    }

                    if (SelectShiftReport.PauseTime != null)
                    {
                        PauseTime = SelectShiftReport.PauseTime;
                    }
                    if (SelectShiftReport.Note != null)
                    {
                        Note = SelectShiftReport.Note;
                    }
                }
            }
        }
        private IApiServices _apiServices;
        private ObservableCollection<Product> listProduct;
        public ObservableCollection<Product> ListProduct
        {
            get => listProduct; set
            {
                listProduct = value; OnPropertyChanged();
            }
        }
        private ObservableCollection<Mold> listMold;
        public ObservableCollection<Mold> ListMold { get => listMold; set { listMold = value; } }
        private ObservableCollection<Employee> listEmployee;
        public ObservableCollection<Employee> ListEmployee { get => listEmployee; set { listEmployee = value; OnPropertyChanged(); } }
        private ObservableCollection<Machine> listMachine;
        public ObservableCollection<Machine> ListMachine { get => listMachine; set { listMachine = value; OnPropertyChanged(); } }
        private List<Shot> listShot;
        public ObservableCollection<ShiftReport> ListShiftReportTempt = new ObservableCollection<ShiftReport>();
        public List<Shot> ListShot { get => listShot; set { listShot = value; } }

        //
        public ICommand SaveExcelCommand { get; set; }
        public ICommand SaveDatagridCommand { get; set; }
        public ICommand ChangeCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        string[] DataCycle;
        string[] DataStatus;
        private IDatabaseServices _databaseServices;
        private ObservableCollection<ShiftReport> listShiftReport;
        public ObservableCollection<ShiftReport> ListShiftReport { get => listShiftReport; set { listShiftReport = value; } }
        public ReportShiftViewModel(IApiServices apiServices, IDatabaseServices databaseServices)
        {
            _apiServices = apiServices;
            _databaseServices = databaseServices;
            SaveExcelCommand = new RelayCommand(async () => SaveExcel());
            SaveDatagridCommand = new RelayCommand(async () => SaveDatagrid());
            ChangeCommand = new RelayCommand(async () => ChangeShiftReport());
            DeleteCommand = new RelayCommand(async () => DeleteShiftReport());
            ListShiftReport = new ObservableCollection<ShiftReport>();
            ListShot = new List<Shot>();
            GetTotalEmplyee();
            GetTotalMold();
            GetTotalProduct();
            GetTotalMachinne();
            SettingsNewViewModel.ActionChangeDatabase += Load;
            Date = DateTime.Now;
            //Load();
        }
        private async void DeleteShiftReport()
        {

            // SelectShiftReport

            MessageBoxResult result = CustomMessageBox.Show("Chỉnh sửa thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
            //   if(result == MessageBoxResult.OK)
            //    {
            ListShiftReport.Remove(SelectShiftReport);
            OnPropertyChanged("ListShiftReport");
            // }



        }
        private async void LoadCSVCycle(string machineId)
        {

            DataCycle = File.ReadAllLines(@"E:\SISTRAIN\WPF\App SISTRAIN\Data Molding Machine\File CSV\S1240522.CSV");

        }
        private double LoadWorkTime(string machineId)
        {

            DataStatus = File.ReadAllLines(@"E:\SISTRAIN\WPF\App SISTRAIN\Data Molding Machine\File CSV\S1240522.CSV");
            int count = 0;
            double TimeTotal = 0;
            string[] ContentCurrent;
            string[] ContentFuture;
            try
            {

                for (int i = 0; i < DataStatus.Length - 2; i++)
                {

                    ContentCurrent = DataStatus[i].Split(',');
                    ContentFuture = DataStatus[i + 1].Split(',');
                    if (ContentCurrent[1] == ContentFuture[1] && ContentCurrent[1] == "4")
                    {
                        while (true)
                        {
                            ContentCurrent = DataStatus[i + count].Split(',');
                            ContentFuture = DataStatus[i + 1 + count].Split(',');
                            if (ContentCurrent[1] == ContentFuture[1])
                            {
                                count++;

                            }
                            else
                            {
                                ContentCurrent = DataStatus[i].Split(',');
                                ContentFuture = DataStatus[i + count].Split(',');
                                TimeTotal += (Convert.ToDateTime(ContentFuture[0]) - Convert.ToDateTime(ContentCurrent[0])).TotalMinutes;
                                break;
                            }

                        }
                        i = i + count;
                    }
                    count = 0;
                }

            }
            catch
            {

            }
            return TimeTotal;

        }

        private async void Load()
        {
            int k = 0;
            ListProduct = await _apiServices.GetProductTotal("");
            //ListMachine = await _apiServices.GetMachineTotal("");
            ListMold = await _apiServices.GetMoldTotal("");
            //   Application.Current.Dispatcher.Invoke(new Action(() => {
            if (ListMachine != null)
            {
                //ListMachine.Clear();
            }
            ListShiftReport.Clear();
            try
            {
                //    int k = 0;
                foreach (var item in _databaseServices.LoadConfiguration())
                {
                    ShiftReport shiftReport = new ShiftReport();
                    LoadCSVCycle("L10");
                    //shiftReport.TotalQuantity = DataCycle.Length - 1;

                    shiftReport.Date = item.DateTime;
                    shiftReport.ShiftNumber = (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 11) ? EShift.Day : EShift.Night;
                    shiftReport.ProductId = item.ProductId;
                    foreach (var item1 in ListProduct)
                    {
                        if (item1.Id == item.ProductId)
                        {
                            shiftReport.Product = item1;
                        }
                    }
                    foreach (var item2 in ListMold)
                    {
                        if (item2.Id == item.MoldId)
                        {
                            shiftReport.Product.Mold = item2;
                        }
                    }
                    //shiftReport.StartTime = Convert.ToDateTime(DataCycle[1].Split(',')[0]);
                    //shiftReport.StopTime = Convert.ToDateTime(DataCycle[DataCycle.Length - 1].Split(',')[0]); ;

                    // shiftReport.WorkTime = LoadWorkTime("");
                    switch (k)
                    {
                        case 0:
                            shiftReport.WorkTime = 500;
                            shiftReport.TotalQuantity = 150;
                            break;
                        case 1:
                            shiftReport.WorkTime = 520;
                            shiftReport.TotalQuantity = 70;
                            break;
                        case 2:
                            shiftReport.WorkTime = 400;
                            shiftReport.TotalQuantity = 30;
                            break;
                        case 3:
                            shiftReport.WorkTime = 653;
                            shiftReport.TotalQuantity = 55;
                            break;
                        case 4:
                            shiftReport.WorkTime = 700;
                            shiftReport.TotalQuantity = 5;
                            break;
                        case 5:
                            shiftReport.WorkTime = 300;
                            shiftReport.TotalQuantity = 84;
                            break;
                    }
                    k++;
                    shiftReport.PauseTime = 720 - shiftReport.WorkTime;

                    //  shiftReport.Shots = ListShot;
                    foreach (var item1 in ListMachine)
                    {
                        if (item1.Id == item.MachineId)
                        {
                            shiftReport.Machine = item1;
                            shiftReport.MachineId = item1.Id;
                        }
                    }


                    try
                    {
                        ListShiftReport.Add(shiftReport);
                        //   CustomMessageBox.Show("Thêm thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            { }
            //      }));


        }
        private async void ChangeShiftReport()
        {
            // SelectShiftReport
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                SelectShiftReport.Date = Date;
                SelectShiftReport.ShiftNumber = (shift == 1) ? EShift.Night : EShift.Day;
                SelectShiftReport.EmployeeId = Employee.Id;
                SelectShiftReport.Employee = Employee;
                SelectShiftReport.ProductId = Product.Id;
                SelectShiftReport.Product = Product;
                SelectShiftReport.Product.Mold = Mold;
                SelectShiftReport.TotalQuantity = TotalQuantity;
                SelectShiftReport.StartTime = StartTime;
                SelectShiftReport.StopTime = StopTime;
                SelectShiftReport.WorkTime = WorkTime;
                SelectShiftReport.PauseTime = PauseTime;
                SelectShiftReport.Note = Note;
                SelectShiftReport.Shots = ListShot;
                SelectShiftReport.Machine = Machine;
                SelectShiftReport.MachineId = Machine.Id;
                CollectionViewSource.GetDefaultView(ListShiftReport).Refresh();
                OnPropertyChanged(nameof(ListShiftReport));
                CustomMessageBox.Show("Chỉnh sửa thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
            }));
        }

        private async void SaveDatagrid()
        {
            ShiftReport shiftReport = new ShiftReport();
            shiftReport.Date = Date;
            shiftReport.ShiftNumber = (shift == 1) ? EShift.Night : EShift.Day;
            shiftReport.EmployeeId = Employee.Id;
            shiftReport.Employee = Employee;
            shiftReport.ProductId = Product.Id;
            shiftReport.Product = Product;
            shiftReport.Product.Mold = Mold;
            shiftReport.TotalQuantity = TotalQuantity;
            shiftReport.StartTime = StartTime;
            shiftReport.StopTime = StopTime;
            shiftReport.WorkTime = WorkTime;
            shiftReport.PauseTime = PauseTime;
            shiftReport.Note = Note;
            shiftReport.Shots = ListShot;
            shiftReport.Machine = Machine;
            shiftReport.MachineId = Machine.Id;
            try
            {
                ListShiftReport.Add(shiftReport);
                //   CustomMessageBox.Show("Thêm thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {

            }




        }

        private async void SaveExcel()
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            string filePath = "";
            // tạo SaveFileDialog để lưu file excel
            SaveFileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                filePath = dialog.FileName;
            }
            try
            {
                using (ExcelPackage p = new ExcelPackage(new FileInfo(filePath)))
                {


                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets[3];

                    int rowIndex = 10;
                    while (ws.Cells["E" + rowIndex.ToString()].Value != null || ws.Cells["E" + (rowIndex + 1).ToString()].Value != null ||
                            ws.Cells["E" + (rowIndex + 2).ToString()].Value != null || ws.Cells["E" + (rowIndex + 3).ToString()].Value != null ||
                            ws.Cells["E" + (rowIndex + 4).ToString()].Value != null || ws.Cells["E" + (rowIndex + 5).ToString()].Value != null ||
                            ws.Cells["E" + (rowIndex + 6).ToString()].Value != null || ws.Cells["E" + (rowIndex + 7).ToString()].Value != null ||
                            ws.Cells["E" + (rowIndex + 8).ToString()].Value != null || ws.Cells["E" + (rowIndex + 9).ToString()].Value != null
                        )
                    {
                        rowIndex++;
                    }

                    foreach (ShiftReport item in ListShiftReport)
                    {
                        ws.Cells["B" + rowIndex.ToString()].Value = item.Date.ToShortDateString();
                        ws.Cells["C" + rowIndex.ToString()].Value = (item.ShiftNumber == EShift.Night) ? 1 : 2;
                        ws.Cells["E" + rowIndex.ToString()].Value = item.Employee.Id;
                        ws.Cells["G" + rowIndex.ToString()].Value = item.Machine.Id;
                        ws.Cells["H" + rowIndex.ToString()].Value = item.Product.Mold.Id;
                        ws.Cells["K" + rowIndex.ToString()].Value = item.Product.Id;
                        if (item.Product.Unit == EUnit.Pieces)
                        {
                            ws.Cells["S" + rowIndex.ToString()].Value = item.TotalQuantity;
                        }
                        else
                        {
                            ws.Cells["T" + rowIndex.ToString()].Value = item.TotalQuantity;
                        }
                        ws.Cells["X" + rowIndex.ToString()].Value = item.StartTime;
                        ws.Cells["Y" + rowIndex.ToString()].Value = item.StopTime;
                        ws.Cells["AB" + rowIndex.ToString()].Value = item.Note;
                        ws.Cells["AA" + rowIndex.ToString()].Value = item.PauseTime.ToString();
                        rowIndex++;
                        item.Shots = GetListShot("");
                        _apiServices.PostShiftReportSingle("", item);
                        StoreEvent("Xuất báo cáo từng máy",item.MachineId);
                    }
                    p.Save();
                }
                MessageBox.Show("Xuất excel thành công!");
                StoreEvent("Xuất báo cáo thành công","All Machine");
                ChangeEvent?.Invoke( );
            }
            catch (Exception EE)
            {
                MessageBox.Show("Có lỗi khi lưu file!");
                StoreEvent("Lỗi khi xuất báo cáo","All Machine");
                ChangeEvent?.Invoke( );
            }
        }
        public void StoreEvent (string NameEvent,string MachineId)
        {
            MayEpCHADesktopApp.Core.Database.ModelDatabase.EventMachine eventMachine1 = new Database.ModelDatabase.EventMachine( );
            eventMachine1.NameEvent=NameEvent;
            eventMachine1.Status=0;
            eventMachine1.DateTime=DateTime.UtcNow;
            eventMachine1.Status=0;
            eventMachine1.MachineId=MachineId;
            _databaseServices.InsertEventAsync(eventMachine1);
        }
        public List<Shot> GetListShot(string Id)
        {
            ListShot.Clear();
            DataCycle = File.ReadAllLines(@"E:\SISTRAIN\WPF\App SISTRAIN\Data Molding Machine\File CSV\C125042022.csv");
            for (int i = 1; i < DataCycle.Length - 1; i++)
            {
                string[] Content = DataCycle[i].Split(',');
                if (Convert.ToDouble(Content[1]) > 10)
                {
                    Shot shot = new Shot(Convert.ToDateTime(Convert.ToDateTime(Content[0])), Convert.ToDouble(Content[1]), Convert.ToDouble(Content[2]));
                    ListShot.Add(shot);
                }

            }
            return ListShot;
        }
        public async void GetTotalEmplyee()
        {
            ListEmployee = await _apiServices.GetEmployeeTotal("");
        }
        public async void GetTotalMold()
        {
            ListMold = await _apiServices.GetMoldTotal("");

        }
        public async void GetTotalProduct()
        {
            ListProduct = await _apiServices.GetProductTotal("");
        }
        public async void GetTotalMachinne()
        {
            ListMachine = await _apiServices.GetMachineTotal("");

        }
    }

}
