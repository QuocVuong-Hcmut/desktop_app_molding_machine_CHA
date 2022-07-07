using System;
using System.Collections.Generic;
using MayEpCHADesktopApp.Core.Database.ModelDatabase;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MayEpCHADesktopApp.Core.Services.Interfaces
{
    public interface IDatabaseServices
    {
        void InsertConfigAsync(Configuration configution);
        void DeleteConfigAsync(Configuration configution);
        void ClearConfig();
        void InsertEventAsync(EventMachine configution);
        void DeleteEventAsync(EventMachine configution);
        public ObservableCollection<Configuration> LoadConfiguration();
        public ObservableCollection<EventMachine> LoadEventMachine();
        void ClearEvent();
    }
}
