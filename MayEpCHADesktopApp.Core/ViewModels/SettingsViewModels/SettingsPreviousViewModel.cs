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

namespace MayEpCHADesktopApp.Core.ViewModels.SettingsViewModels
{
    public class SettingsPreviousViewModel : ViewModels.BaseViewModels.BaseViewModel
    {
        private IDatabaseServices _databaseServices;
        private ObservableCollection<Configuration> listConfigurationShift2;
        public ObservableCollection<Configuration> ListConfigurationShift2 { get => listConfigurationShift2; set { listConfigurationShift2 = value; OnPropertyChanged(); } }

        public SettingsPreviousViewModel(IApiServices apiServices, IDatabaseServices databaseServices)
        {
            ListConfigurationShift2 = new ObservableCollection<Configuration>();
            _databaseServices = databaseServices;
            //xóa hết config
            //databaseServices.ClearConfig();
            SettingsNewViewModel.ActionChangeDatabase += Load;
            Load();
        }
        public void Load()
        {
            //if(ListConfigurationShift2 != null)
            //{
            //    ListConfigurationShift2.Clear();
            //}

            //if (DateTime.Now.Hour > 19 && DateTime.Now.Hour < 24)
            //{


            //    foreach (var configuration in _databaseServices.LoadConfiguration())
            //    {
            //        if (configuration.DateTime.Day == DateTime.Now.Day && configuration.DateTime.Hour >17 && configuration.DateTime.Hour <21)
            //        {
            //            ListConfigurationShift2.Add(configuration);
            //        }
            //    }

            //}else if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 19) {
            //    foreach (var configuration in _databaseServices.LoadConfiguration())
            //    {
            //        if ((Convert.ToInt32(configuration.DateTime.Day) == (Convert.ToInt32(DateTime.Now.Day.ToString()) - 1) && configuration.DateTime.Hour > 17 && configuration.DateTime.Hour < 19)|| configuration.DateTime.Day == DateTime.Now.Day)
            //        {
            //            ListConfigurationShift2.Add(configuration);
            //        }
            //    }
            //}

            ////test neen cho hieenr thi heets
            ListConfigurationShift2.Clear();
            foreach (var configuration in _databaseServices.LoadConfiguration())
            {

                ListConfigurationShift2.Add(configuration);

            }


        }
    }
}
