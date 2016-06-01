using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using EPICSServerClient.Modules.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private DelegateCommand<Connection> connectionCommand;
        private ObservableCollection<Connection> connections = new ObservableCollection<Connection>();
        private string databaseName = String.Empty;
        private string url = String.Empty;
        private string appId = String.Empty;

        public ServerViewModel(IRegionManager RegionManager)
        {
            this.RegionManager = RegionManager;
            SaveCommand = new DelegateCommand<Object>(SaveParseServerInformation,CanSave);
            ConnectionCommand = new DelegateCommand<Connection>(PopulateConnection);
            PopulateConnections();
        }

        public DelegateCommand<object> SaveCommand
        {
            get { return saveCommand; }
            set { saveCommand = value; }
        }

        public DelegateCommand<Connection> ConnectionCommand
        {
            get { return connectionCommand; }
            set { connectionCommand = value; }
        }

        public ObservableCollection<Connection> Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        public string ErrorMessage
        {
            get { return ParseData.ErrorMessage; }
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
            var connection = new Connection()
            {
                DatabaseName = DatabaseName,
                Url = Url,
                AppId = AppId
            };
            if (!ConnectionData.Connections.Contains(connection))
                ConnectionData.Connections.Add(connection);
            ConnectionData.ConvertConnectionsToXml();
            var uri = new Uri(typeof(MenuView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.MenuRegion, uri);
        }

        private bool CanSave(object parm)
        {
            return (AppId != String.Empty && Url != String.Empty && DatabaseName != String.Empty) ? true : false;
        }

        private void PopulateConnections()
        {
            foreach(Connection connection in ConnectionData.ConvertXmlToConnections())
            {
                Connections.Add(connection);
            }
        }

        private void PopulateConnection(Connection connection)
        {
            DatabaseName = connection.DatabaseName;
            Url = connection.Url;
            AppId = connection.AppId;
            OnPropertyChanged("DatabaseName");
            OnPropertyChanged("Url");
            OnPropertyChanged("AppId");
        }
    }
}
