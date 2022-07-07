using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayEpCHADesktopApp.Core.Model
{
    
    public class ShiftReport
    {
        
        public ShiftReport(DateTime date, int id, EShift shiftNumber, Employee employee, string employeeId, Machine machine, string machineId, Product product, string productId, int totalQuantity, DateTime startTime, DateTime stopTime, double workTime, double pauseTime, string note, List<Shot> shots)
        {
            Date = date;
            Id = id;
            ShiftNumber = shiftNumber;
            Employee = employee;
            EmployeeId = employeeId;
            Machine = machine;
            MachineId = machineId;
            Product = product;
            ProductId = productId;
            TotalQuantity = totalQuantity;
            StartTime = startTime;
            StopTime = stopTime;
            WorkTime = workTime;
            PauseTime = pauseTime;
            Note = note;
            Shots = shots;
        }
        
        public ShiftReport()
        {

        }

        public DateTime Date { get; set; }
        public int Id { get; set; }
        public EShift ShiftNumber { get; set; }
        public Employee Employee { get; set; }
        public string EmployeeId { get; set; }
        public Machine Machine { get; set; }
        public string MachineId { get; set; }
        public Product Product { get; set; }
        public string ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public double WorkTime { get; set; }
        public double PauseTime { get; set; }
        public string Note { get; set; }
        [JsonProperty("shots")]
        public List<Shot> Shots { get; set; }
    }
}
