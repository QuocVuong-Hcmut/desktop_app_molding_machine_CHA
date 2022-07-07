using MayEpCHADesktopApp.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayEpCHADesktopApp.Core.Services.Interfaces
{
    public interface IApiServices
    {
         Task<ObservableCollection<Employee>> GetEmployeeTotal(string auth);
         Task<ObservableCollection<Mold>> GetMoldTotal(string auth);
         Task<ObservableCollection<Product>> GetProductTotal(string auth);
         Task<ObservableCollection<Machine>> GetMachineTotal(string auth);
         Task<ObservableCollection<ShiftReport>> GetShiftReportTotal(string auth);
         Task PostShiftReport(string auth, ObservableCollection<ShiftReport> ListShiftReport);
        Task PostShiftReportSingle(string auth, ShiftReport shiftReport );

    }
}
