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
    public class SettingsPreviousViewModel: ViewModels.BaseViewModels.BaseViewModel
    {
        private IDatabaseServices _databaseServices;
        private ObservableCollection<Configuration> listConfigurationShift2;
        DispatcherTimer TLoad = new DispatcherTimer( );
        public ObservableCollection<Configuration> ListConfigurationShift2 { get => listConfigurationShift2; set { listConfigurationShift2=value; OnPropertyChanged( ); } }

        public SettingsPreviousViewModel (IApiServices apiServices,IDatabaseServices databaseServices)
        {
            _databaseServices=databaseServices;
            ListConfigurationShift2=new ObservableCollection<Configuration>( );
            TLoad.Interval=TimeSpan.FromMinutes(15);
            TLoad.Tick+=TLoad_Tick;
            //xóa hết config
            //databaseServices.ClearConfig();
            SettingsNewViewModel.ActionChangeDatabase+=Load;
            Load( );
        }

        private void TLoad_Tick (object? sender,EventArgs e)
        {
            Load( );
        }

        public void Load ( )
        {
            if ( ListConfigurationShift2!=null )
            {
                ListConfigurationShift2.Clear( );
            }

            if ( DateTime.Now.Hour>19&&DateTime.Now.Hour<24 )
            {
                foreach ( var configuration in _databaseServices.LoadConfiguration( ) )
                {
                    if ( configuration.DateTime.Day==DateTime.Now.Day&&configuration.DateTime.Hour>17&&configuration.DateTime.Hour<21 )
                    {
                        ListConfigurationShift2.Add(configuration);
                    }
                }
            }
            else if ( DateTime.Now.Hour>0&&DateTime.Now.Hour<19 )
            {
                foreach ( var configuration in _databaseServices.LoadConfiguration( ) )
                {
                    if ( (Convert.ToInt32(configuration.DateTime.Day)==(Convert.ToInt32(DateTime.Now.Day.ToString( ))-1)&&configuration.DateTime.Hour>17&&configuration.DateTime.Hour<19)||configuration.DateTime.Day==DateTime.Now.Day )
                    {
                        ListConfigurationShift2.Add(configuration);
                    }
                }
            }

        }
    }
}
