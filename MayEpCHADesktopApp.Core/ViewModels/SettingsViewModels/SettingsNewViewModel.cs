using ImmServiceContainers;
using MassTransit;
using MayEpCHADesktopApp.Core.Components;
using MayEpCHADesktopApp.Core.Database.ModelDatabase;
using MayEpCHADesktopApp.Core.Model;
using MayEpCHADesktopApp.Core.Services.Interfaces;
using MayEpCHADesktopApp.Core.ViewModels.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace MayEpCHADesktopApp.Core.ViewModels.SettingsViewModels
{
    public class SettingsNewViewModel : ViewModels.BaseViewModels.BaseViewModel
    {
        #region var
        public bool IsShift1 { get => isShift1; set { isShift1 = value; OnPropertyChanged(); } }
        private bool isShift1;
        public bool IsShift2 { get => isShift2; set { isShift2 = value; OnPropertyChanged(); } }
        private bool isShift2;
        private string productId;
        private string machineId;
        private string moldId;
        private int quantity;
        private int shift;
        private double cycleInjection;
        public Product Product { get => product; set { product = value; 
                ProductId = Product.Id ;

                        Mold = Product.Mold;
                MoldId =  Mold.Id ; 
                CycleInjection = Mold.StandardInjectionCycle;

                
                
                OnPropertyChanged(); } }
        private Product product;
        private Mold mold;
        public Mold Mold { get => mold; set { mold = value;  OnPropertyChanged(); } }
        private Machine machine;
        public Machine Machine { get => machine; set { machine = value;if (Machine != null) { MachineId = Machine.Id; }; OnPropertyChanged(); } }
        public string ProductId { get => productId; set { productId = value; OnPropertyChanged(); } }
        public string MachineId { get => machineId; set { machineId = value; OnPropertyChanged(); } }
        public string MoldId { get => moldId; set { moldId = value; OnPropertyChanged(); } }
        public int Quantity { get => quantity; set { quantity = value; OnPropertyChanged(); } }
        public int Shift { get => shift; set { shift = value; OnPropertyChanged(); } }
        public Configuration ConfigurationSelect { get => configurationSelect; set { configurationSelect = value; OnPropertyChanged(); } }
        private Configuration configurationSelect;
        public double CycleInjection { get => cycleInjection; set { cycleInjection = value; try { Quantity = Convert.ToInt32(43200 / CycleInjection); } catch (Exception ex) { } OnPropertyChanged(); } }
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
        //private ObservableCollection<Configuration> listConfigurationShift1;
        //public ObservableCollection<Configuration> ListConfigurationShift1 { get => listConfigurationShift1; set { listConfigurationShift1 = value; OnPropertyChanged(); } }
        private ObservableCollection<Configuration> listConfigurationShift2;
        public ObservableCollection<Configuration> ListConfigurationShift2 { get => listConfigurationShift2; set { listConfigurationShift2 = value; OnPropertyChanged(); } }
        public static Action ActionChangeDatabase { get; set; }
        #endregion var
        public static event Action ActionLoad;
        public ICommand Shift1 { set; get; }
        public ICommand Shift2 { set; get; }
        public ICommand AddCommandShift2 { set; get; }
        public ICommand TextChangedCommand { set; get; }
        public ICommand DeleteCommandShift2 { set; get; }
        public ICommand ClearCommandShift2 { set; get; }
        
        private IDatabaseServices _databaseServices;
        private IApiServices _apiServices;
        private IBusControl _busControl;
        DispatcherTimer TSendConfigShift1 = new DispatcherTimer();
        DispatcherTimer TSendConfigShift2 = new DispatcherTimer();
        DispatcherTimer TSend = new DispatcherTimer();
        public SettingsNewViewModel(IDatabaseServices databaseServices, IApiServices apiServices, IBusControl busControl)
        {
            _databaseServices = databaseServices;
            _apiServices = apiServices;
            _busControl = busControl;
            _databaseServices.ClearConfig();
            ListConfigurationShift2 = new ObservableCollection<Configuration>();
            AddCommandShift2 = new RelayCommand(async () => await Add());
            DeleteCommandShift2 = new RelayCommand(async () => await Delete());
            ClearCommandShift2 = new RelayCommand(async () => await Clear());
            TSendConfigShift2.Tick += ActionTimer;
            TSendConfigShift1.Tick += ActionTimer;

            TSend.Tick += ActionTimer;
            GetTotalMachine();
            GetTotalMold();
            GetTotalProduct();
            GetTotalEmployee();
            Load();
            // SendTest();
            //  Send();
            //xóa sạch database
            //_databaseServices.ClearConfig();
            //xóa database 2 ngày trước
            DeleteConfig();
            //chạy thực tế
            // StartTimer();
            //chạy mô phỏng
            // StartTimerTest();
           // SendConfigToESP3219h00();

        }
        //chạy timer mô phỏng
        public void StartTimerTest()
        {
            int time;
            //set thời gian gửi xuống esp
            int SetPointHourTime = 24;
            int SetPointMinuteTime = 0;
            if ((SetPointMinuteTime - DateTime.Now.Minute ) < 0)
            {
                time = (SetPointHourTime - DateTime.Now.Hour - 1) * 3600 + (SetPointMinuteTime - DateTime.Now.Minute + 60) * 60 ;
                TSendConfigShift1.Interval = TimeSpan.FromSeconds(10);
                TSendConfigShift1.Start();
            }
            else
            {
                time = (SetPointHourTime - DateTime.Now.Hour ) * 3600 + (SetPointMinuteTime - DateTime.Now.Minute) * 60;
                TSendConfigShift1.Interval = TimeSpan.FromSeconds(10);
                TSendConfigShift1.Start();

            }

        }
        //chạy timer 
        public void StartTimer()
        {

            if(DateTime.Now.Hour > 7 && DateTime.Now.Hour < 19)
            {
                int time;
                if (DateTime.Now.Minute != 0)
                {
                    time = (19 - DateTime.Now.Hour -1)*3600 + (60-DateTime.Now.Minute)*60 +60-DateTime.Now.Second;
                    TSendConfigShift2.Interval = TimeSpan.FromSeconds(time);
                    TSendConfigShift2.Start();
                }
                else
                {
                    time = (19 - DateTime.Now.Hour) * 3600;
                    TSendConfigShift2.Interval = TimeSpan.FromSeconds(time);
        
                    TSendConfigShift2.Start();
                }
            }
            else
            {
                int time;
                if (DateTime.Now.Minute != 0)
                {
                    time = (7 - DateTime.Now.Hour - 1) * 3600 +(60- DateTime.Now.Minute) * 60 +60- DateTime.Now.Second;
                    TSendConfigShift1.Interval = TimeSpan.FromSeconds(time);
                    TSendConfigShift1.Start();

                }
                else
                {
                    time = (7 - DateTime.Now.Hour) * 3600;
                    TSendConfigShift1.Interval = TimeSpan.FromSeconds(time);
                    
                    TSendConfigShift1.Start(); 

                }
            }

        }
        //xóa database 2 ngày trước đó
        public void DeleteConfig()
        {
            foreach( var configuration in _databaseServices.LoadConfiguration())
            {
                if ((Convert.ToInt32(configuration.DateTime.Day) < (Convert.ToInt32(DateTime.Now.Day.ToString()) - 2)))
                {
                    _databaseServices.DeleteConfigAsync(configuration);

                }
            }
            foreach( var item in _databaseServices.LoadEventMachine())
            {
                if ((Convert.ToInt32(item.DateTime.Day) < (Convert.ToInt32(DateTime.Now.Day.ToString()) - 2)))
                {
                    _databaseServices.DeleteEventAsync(item);

                }
            }
        }
        //khi khởi động app hiện cái gì
        public void Load()
        {
            //if (ListConfigurationShift2 != null)
            //{
            //    ListConfigurationShift2.Clear();
            //}

            //if (DateTime.Now.Hour > 19 && DateTime.Now.Hour < 24)
            //{


            //    foreach (var configuration in _databaseServices.LoadConfiguration())
            //    {
            //        if (configuration.DateTime.Day == DateTime.Now.Day && configuration.DateTime.Hour > 17 && configuration.DateTime.Hour < 21)
            //        {
            //            ListConfigurationShift2.Add(configuration);
            //        }
            //    }

            //}
            //else if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 19)
            //{
            //    foreach (var configuration in _databaseServices.LoadConfiguration())
            //    {
            //        if ((Convert.ToInt32(configuration.DateTime.Day) == (Convert.ToInt32(DateTime.Now.Day.ToString()) - 1) && configuration.DateTime.Hour > 17 && configuration.DateTime.Hour < 19) || configuration.DateTime.Day == DateTime.Now.Day)
            //        {
            //            ListConfigurationShift2.Add(configuration);
            //        }
            //    }
            //}

            //test neen cho hieenr thi heets
            ListConfigurationShift2.Clear();
            foreach (var configuration in _databaseServices.LoadConfiguration())
            {

                ListConfigurationShift2.Add(configuration);

            }


        }
        //khi tới 19h
        public async void SendConfigToESP3219h00()
        {

            foreach (var configuration in _databaseServices.LoadConfiguration())
            {
                //if(configuration.DateTime.Day == DateTime.Now.Day && configuration.DateTime.Hour > 17 && configuration.DateTime.Hour < 19)
                if (configuration.DateTime.Day == DateTime.Now.Day && configuration.DateTime.Hour > 0 && configuration.DateTime.Hour < 19)
                {
                    //var endpoint = await _busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));
                    //await endpoint.Send<ConfigurationMessage>(new ConfigurationMessage
                    //{
                    //    MachineId = configuration.MachineId,
                    //    Timestamp = DateTime.UtcNow,
                    //    MoldId = configuration.MoldId,
                    //    CycleTime = configuration.CycleInjection,
                    //    ProductId = configuration.ProductId

                    //});
                    ShiftReport shiftReport = new ShiftReport();
                    shiftReport.Date = configuration.DateTime.Date;
                    shiftReport.ShiftNumber = (shift == 1) ? EShift.Night : EShift.Day;
                    //shiftReport.EmployeeId = ListEmployee[2].Id;
                    //shiftReport.Employee = ListEmployee[2];
                  //  shiftReport.Employee = Employee;
                    shiftReport.ProductId = configuration.ProductId;
                    foreach(var item in ListProduct)
                    {
                        if(item.Id == configuration.ProductId)
                        {
                            shiftReport.Product = item;
                            foreach (var item2 in ListMold)
                            {
                                if(item.MoldId == item2.Id)
                                {
                                    shiftReport.Product.Mold = item2;
                                }
                            }
                        }
                    }
                    
                    
                    shiftReport.TotalQuantity = configuration.Quantity;
                    shiftReport.StartTime = DateTime.Now;
                    shiftReport.StopTime = DateTime.Now;
                    shiftReport.WorkTime = 0;
                    shiftReport.PauseTime = 0;
                    shiftReport.Note = "oo";
                  //  shiftReport.Shots = ListShot;
                    shiftReport.Machine = Machine;
                    shiftReport.MachineId = configuration.MachineId;
                    foreach (var item in ListMachine)
                    {
                        if (item.Id == configuration.MachineId)
                        {
                            shiftReport.Machine = item;
                        }
                    }
                    shiftReport.EmployeeId = ListEmployee[2].Id;
                    shiftReport.Employee = ListEmployee[2];
                    _apiServices.PostShiftReportSingle("", shiftReport);
                    // post
                    //ShiftReport shiftReport = new ShiftReport();
                    //shiftReport.MachineId = configuration.MachineId;
                    //shiftReport.Date = configuration.DateTime.Date;
                    //shiftReport.TotalQuantity = configuration.Quantity;
                    //shiftReport.ProductId = configuration.ProductId;
                    //shiftReport.ShiftNumber = EShift.Night;
                    //_apiServices.PostShiftReportSingle("",shiftReport);

                }
            }
           // _databaseServices.InsertEventAsync(new EventMachine() { DateTime = DateTime.Now, NameEvent = "Ca2" });
        }
        public async void SendConfigToESP327h00()
        {
            foreach (var configuration in _databaseServices.LoadConfiguration())
            {
                //if ((Convert.ToInt32( configuration.DateTime.Date) == (Convert.ToInt32(DateTime.Now.Date.ToString()) -1) && configuration.DateTime.Hour > 17 && configuration.DateTime.Hour < 19) || configuration.DateTime.Date == DateTime.Now.Date)
                //{
                    //var endpoint = await _busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));
                    //await endpoint.Send<ConfigurationMessage>(new ConfigurationMessage
                    //{
                    //    MachineId = configuration.MachineId,
                    //    Timestamp = DateTime.UtcNow,
                    //    MoldId = configuration.MoldId,
                    //    CycleTime = configuration.CycleInjection,
                    //    ProductId = configuration.ProductId

                    //});
                ShiftReport shiftReport = new ShiftReport();
                shiftReport.Date = configuration.DateTime.Date;
                shiftReport.ShiftNumber = (shift == 1) ? EShift.Night : EShift.Day;
                shiftReport.EmployeeId = "";
                //  shiftReport.Employee = Employee;
                shiftReport.ProductId = configuration.ProductId;
                foreach (var item in ListProduct)
                {
                    if (item.Id == configuration.ProductId)
                    {
                        shiftReport.Product = item;
                        foreach (var item2 in ListMold)
                        {
                            if (item.MoldId == item2.Id)
                            {
                                shiftReport.Product.Mold = item2;
                            }
                        }
                    }
                }


                shiftReport.TotalQuantity = configuration.Quantity;
                shiftReport.StartTime = DateTime.Now;
                shiftReport.StopTime = DateTime.Now;
                shiftReport.WorkTime = 0;
                shiftReport.PauseTime = 0;
                shiftReport.Note = "";
                //  shiftReport.Shots = ListShot;
                shiftReport.Machine = Machine;
                shiftReport.MachineId = configuration.MachineId;
                foreach (var item in ListMachine)
                {
                    if (item.Id == configuration.MachineId)
                    {
                        shiftReport.Machine = item;
                    }
                }
                _apiServices.PostShiftReportSingle("", shiftReport);

                //ShiftReport shiftReport = new ShiftReport();
                //shiftReport.MachineId = configuration.MachineId;
                //shiftReport.Date = configuration.DateTime;
                //shiftReport.TotalQuantity = configuration.Quantity;
                //shiftReport.ProductId = configuration.ProductId;
                //shiftReport.ShiftNumber = EShift.Day;
                //_apiServices.PostShiftReportSingle("", shiftReport);

                //       }
            }
            _databaseServices.InsertEventAsync(new EventMachine() { DateTime = DateTime.Now, NameEvent = "Ca2" });
        }
        //Xú lí khi 19h không mở áp
        public void ActionTimer(object? sender, EventArgs e)
        {
            CustomMessageBox.Show("juhjygvjhgc");
           // Send();
            SendTest();
        }
        public async void SendTest()
        {
            //foreach (var configuration in _databaseServices.LoadConfiguration())
            //{
            //    if (configuration.DateTime.Day == DateTime.Now.Day )
            //    {
            //        var endpoint = await _busControl.GetSendEndpoint(new Uri("http://127.0.0.1:8181/send-config"));
            //        await endpoint.Send<ConfigurationMessage>(new ConfigurationMessage
            //        {
            //            MachineId = configuration.MachineId,
            //            Timestamp = DateTime.UtcNow,
            //            MoldId = configuration.MoldId,
            //            CycleTime = configuration.CycleInjection,
            //            ProductId = configuration.ProductId

            //        });
            //        // post
            //        //ShiftReport shiftReport = new ShiftReport();
            //        //shiftReport.MachineId = configuration.MachineId;
            //        //shiftReport.Date = configuration.DateTime.Date;
            //        //shiftReport.TotalQuantity = configuration.Quantity;
            //        //shiftReport.ProductId = configuration.ProductId;
            //        //shiftReport.ShiftNumber = EShift.Night;
            //        //_apiServices.PostShiftReportSingle("", shiftReport);



            //    }

            //}
            SendConfigToESP3219h00();
        }
        public void Send()
        {
            if(DateTime.Now.Hour > 19 && DateTime.Now.Hour < 20)
            {
                bool check = false;
                foreach(var item in _databaseServices.LoadEventMachine())
                {
                    if(item.DateTime.Date == DateTime.Now && item.NameEvent == "ca2")
                    {
                        check = true;
                    }
                }
                if (!check)
                {
                    SendConfigToESP3219h00();
                }
            }
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 8)
            {
                bool check = false;
                foreach (var item in _databaseServices.LoadEventMachine())
                {
                    if (item.DateTime.Day == DateTime.Now.Day && item.NameEvent == "ca1")
                    {
                        check = true;
                    }
                }
                if (!check)
                {
                    SendConfigToESP327h00();
                }
            }
        }
        private Task Clear()
        {
            ListConfigurationShift2.Clear();
            return Task.CompletedTask;
        }
        private Task Delete()
        {
            //DateTime.Now.Hour > 10 && DateTime.Now.Hour < 20
            if (true)
            {
                try
                {
                    _databaseServices.DeleteConfigAsync(ConfigurationSelect);
                    ListConfigurationShift2.Remove(ConfigurationSelect);
                    ActionChangeDatabase?.Invoke();

                }
                catch (Exception ex) { }

            }

            
            return Task.CompletedTask;
        }
        private Task Add()
        {
            try {
                AddDataGrid();
                ActionChangeDatabase?.Invoke();
            } catch { }

            return Task.CompletedTask;
        }
        public void AddDataGrid()
        {
            if (DateTime.Now.Hour > 7 && DateTime.Now.Hour < 20|| true)
            {

                try
                {
                    Configuration configuration = new Configuration();
                    configuration.Quantity = Quantity;
                    configuration.Shift = 2;
                    configuration.MoldId = Mold.Id;
                    configuration.MachineId = Machine.Id;
                    configuration.ProductId = Product.Id;
                    configuration.CycleInjection = CycleInjection;
                    configuration.DateTime = DateTime.UtcNow;
                    ListConfigurationShift2.Add(configuration);
                    AddDatabase(configuration);
                }
                catch
                {
                    CustomMessageBox.Show("Vui lòng nhập đầy đủ thông tin", " Cảnh báo", System.Windows.MessageBoxButton.OKCancel);
                }
            }


        }
        public void AddDatabase(Configuration configuration)
        {
            _databaseServices.InsertConfigAsync(configuration);
            ActionChangeDatabase?.Invoke();
        }
        public async void GetTotalMold()
        {
            ListMold = new ObservableCollection<Mold>();
            ListMold = await _apiServices.GetMoldTotal("");

        }
        public async void GetTotalProduct()
        {
            ListProduct = new ObservableCollection<Product>();
            ListProduct = await _apiServices.GetProductTotal("");
        }
        public async void GetTotalMachine()
        {
            ListMachine = new ObservableCollection<Machine>();
            ListMachine = await _apiServices.GetMachineTotal("");
        }
        public async void GetTotalEmployee()
        {
            ListEmployee = new ObservableCollection<Employee>();
            ListEmployee = await _apiServices.GetEmployeeTotal("");
        }
    }
}
