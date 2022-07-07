using MayEpCHADesktopApp.Core.Components;
using MayEpCHADesktopApp.Core.Model;
using MayEpCHADesktopApp.Core.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MayEpCHADesktopApp.Core.Services
{
    public class ApiServices:IApiServices
    {
        private HttpClient httpClient;
        private  HttpRequestMessage httpRequest;
        //private string Address = "https://localhost:7202/";
        private string Address = "http://10.84.70.81:8082/";
        public ObservableCollection<Employee> ListEmployee = new ObservableCollection<Employee>();
        public  ObservableCollection<Mold> ListMold = new ObservableCollection<Mold>();
        public ObservableCollection<Product> ListProduct = new ObservableCollection<Product>();
        public ObservableCollection<Machine> ListMachine = new ObservableCollection<Machine>();
        public ObservableCollection<ShiftReport> ListShiftReport = new ObservableCollection<ShiftReport>();
        public ShiftReports ShiftReports = new ShiftReports();
        public ApiServices() {

            //HttpRequest.Headers.Add("Authorization", "Bearer  " + auth);
            
        }

        public async Task<ObservableCollection<Machine>> GetMachineTotal(string auth)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        httpRequest.Headers.Add("User-Agent", "Mozilla/5.0");
                        string Url = Address +"api/machines";
                        httpRequest.Method = System.Net.Http.HttpMethod.Get;
                        httpRequest.RequestUri = new Uri(Url);
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        var ob = await httpResponse.Content.ReadAsStringAsync();
                        ListMachine = JsonConvert.DeserializeObject<ObservableCollection<Machine>>(ob);
                    }

                }

            }
            catch
            {
              //  CustomMessageBox.Show("Lỗi trong quá trình lấy dữ liệu từ Sever!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }

            return ListMachine;
        }
        public  async Task<ObservableCollection<Employee>> GetEmployeeTotal(string auth)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    using( httpRequest = new HttpRequestMessage())
                    {
                        httpRequest.Headers.Add("User-Agent", "Mozilla/5.0");
                        string Url = Address+"api/employees";
                        httpRequest.Method = System.Net.Http.HttpMethod.Get;
                        httpRequest.RequestUri = new Uri(Url);
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        var ob = await httpResponse.Content.ReadAsStringAsync();
                        ListEmployee = JsonConvert.DeserializeObject<ObservableCollection<Employee>>(ob);
                    }

                }

            }
            catch
            {
              //  CustomMessageBox.Show("Lỗi trong quá trình lấy dữ liệu từ Sever!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }
      
            return ListEmployee;   
        }

        public async Task<ObservableCollection<Mold>> GetMoldTotal(string auth)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        string Url = Address+"api/molds";
                        httpRequest.Method = System.Net.Http.HttpMethod.Get;
                        httpRequest.RequestUri = new Uri(Url);
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        var ob = await httpResponse.Content.ReadAsStringAsync();
                        ListMold = JsonConvert.DeserializeObject<ObservableCollection<Mold>>(ob);

                    }

                }
   }
            catch
            {
            //    CustomMessageBox.Show("Lỗi trong quá trình lấy dữ liệu từ Sever!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }

            return ListMold;
        }

        public async Task<ObservableCollection<Product>> GetProductTotal(string auth)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        string Url = Address+"api/products/details";
                        httpRequest.Method = System.Net.Http.HttpMethod.Get;
                        httpRequest.RequestUri = new Uri(Url);
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        var ob = await httpResponse.Content.ReadAsStringAsync();
                        ListProduct = JsonConvert.DeserializeObject<ObservableCollection<Product>>(ob);

                    }

                }

            }
            catch
            {
              //  CustomMessageBox.Show("Lỗi trong quá trình lấy dữ liệu từ Sever!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }

            return ListProduct;

        }
        public async Task<ObservableCollection<ShiftReport>> GetShiftReportTotal(string auth)
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        string Url = Address+"api/shiftreports";
                        httpRequest.Method = System.Net.Http.HttpMethod.Get;
                        httpRequest.RequestUri = new Uri(Url);
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        var ob = await httpResponse.Content.ReadAsStringAsync();
                        ShiftReports = JsonConvert.DeserializeObject<ShiftReports>(ob );
                        foreach(ShiftReport item in ShiftReports.items)
                        {
                            ListShiftReport.Add(item);
                        }
                    }

                }

            }
            catch
            {
               // CustomMessageBox.Show("Lỗi trong quá trình lấy dữ liệu từ Sever!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }

            return ListShiftReport;

        }
        public async Task PostShiftReport(string auth, ObservableCollection<ShiftReport> ListShiftReport)
        {

            try
            {
                string data = JsonConvert.SerializeObject(ListShiftReport);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        string Url = Address+"api/shiftreports";
                        httpRequest.Method = System.Net.Http.HttpMethod.Post;
                        httpRequest.RequestUri = new Uri(Url);
                        httpRequest.Content = content;
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        if(httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            CustomMessageBox.Show("Gửi dữ liệu thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
                        }
                        else
                        {
                       //     CustomMessageBox.Show("Gửi dữ liệu không thành công.", "Lỗi", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error);
                        }

                    }

                }

            }
            catch
            {
             //   CustomMessageBox.Show("Lỗi trong quá trình gửi dữ liệu lên server!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }

            
        }
        public async Task PostShiftReportSingle(string auth, ShiftReport shiftReport)
        {

            try
            {
                string data = JsonConvert.SerializeObject(shiftReport);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                using (httpClient = new HttpClient())
                {
                    using (httpRequest = new HttpRequestMessage())
                    {
                        string Url = Address + "api/shiftreports/single";
                        httpRequest.Method = System.Net.Http.HttpMethod.Post;
                        httpRequest.RequestUri = new Uri(Url);
                        httpRequest.Content = content;
                        HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                        if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            CustomMessageBox.Show("Gửi dữ liệu thành công", "Thông báo", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Asterisk);
                        }
                        else
                        {
                                CustomMessageBox.Show("Gửi dữ liệu không thành công.", "Lỗi", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error);
                        }

                    }

                }

            }
            catch
            {
                //   CustomMessageBox.Show("Lỗi trong quá trình gửi dữ liệu lên server!", "Cảnh bảo", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning);
            }


        }
    }
}
