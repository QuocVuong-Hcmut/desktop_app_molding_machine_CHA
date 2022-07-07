using MayEpCHADesktopApp.Core.Database.ModelDatabase;
using MayEpCHADesktopApp.Core.Database.Repository.Interface;
using MayEpCHADesktopApp.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MayEpCHADesktopApp.Core.Services
{
    public class DatabaseService:IDatabaseServices
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IEventMachineRepository _eventMachineRepository;
        public DatabaseService(IConfigurationRepository configurationRepository, IEventMachineRepository eventMachineRepository)
        {
            _configurationRepository = configurationRepository;
            _eventMachineRepository = eventMachineRepository;
        }

        public void ClearConfig()
        {
          //  _configurationRepository.Clear();
        }

        public void ClearEvent()
        {
            //_eventMachineRepository.Clear();
        }

        public void DeleteConfigAsync(Configuration configution)
        {
            _configurationRepository.DeleteAsync(configution);
        }

        public void DeleteEventAsync(EventMachine eventMachine)
        {
            _eventMachineRepository.DeleteAsync(eventMachine);
        }

        public void InsertConfigAsync(Configuration configution)
        {
            _configurationRepository.InsertAsync(configution);
        }

        public void InsertEventAsync(EventMachine eventMachine)
        {
            _eventMachineRepository.InsertAsync(eventMachine);
        }

        public ObservableCollection<Configuration> LoadConfiguration()
        {
            return _configurationRepository.Load();
        }
        public ObservableCollection<EventMachine> LoadEventMachine()
        {
            return _eventMachineRepository.Load();
        }
    }
}
