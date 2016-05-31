using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Modules.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Modules.ViewModels
{
    public class ServerViewModel : ViewModelBase
    {
        private IRegionManager RegionManager;
        private DelegateCommand<Object> saveCommand;
        private string databaseName = String.Empty;
        private string url = String.Empty;
        private string appId = String.Empty;

        public ServerViewModel(IRegionManager RegionManager)
        {
            this.RegionManager = RegionManager;
            SaveCommand = new DelegateCommand<Object>(SaveParseServerInformation,CanSave);
        }

        public DelegateCommand<object> SaveCommand
        {
            get { return saveCommand; }
            set { saveCommand = value; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set {
                databaseName = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public string AppId
        {
            get { return appId; }
            set
            {
                appId = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void SaveParseServerInformation(object parm)
        {
            ParseData.AppId = AppId;
            ParseData.Url = Url;
            var uri = new Uri(typeof(MenuView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.MenuRegion, uri);
        }

        private bool CanSave(object parm)
        {
            return (AppId != String.Empty && Url != String.Empty && DatabaseName != String.Empty) ? true : false;
        }
    }
}
