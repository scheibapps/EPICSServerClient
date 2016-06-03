using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Helpers.Models;
using EPICSServerClient.Helpers.Services;
using EPICSServerClient.Modules.Views;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Modules.ViewModels
{
    public class ServerViewModel : ViewModelBase
    {
        private IRegionManager RegionManager;
        private IParseService ParseService;
        private DelegateCommand<Object> saveCommand;
        private DelegateCommand<Connection> connectionCommand;
        private DelegateCommand removeConnectionCommand;
        private ObservableCollection<Connection> connections = new ObservableCollection<Connection>();
        private Connection SelectedConnection { get; set; }
        private string databaseName = String.Empty;
        private string url = String.Empty;
        private string appId = String.Empty;
        private string errorMessage = String.Empty;

        public ServerViewModel(IRegionManager RegionManager, IParseService ParseService)
        {
            this.RegionManager = RegionManager;
            this.ParseService = ParseService;
            SaveCommand = new DelegateCommand<Object>(SaveParseServerInformation,CanSave);
            ConnectionCommand = new DelegateCommand<Connection>(PopulateConnection);
            RemoveConnectionCommand = new DelegateCommand(RemoveConnection, CanRemoveConnection);
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

        public DelegateCommand RemoveConnectionCommand
        {
            get { return removeConnectionCommand; }
            set { removeConnectionCommand = value; }
        }

        public ObservableCollection<Connection> Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
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
            ErrorMessage = String.Empty;
            ParseData.AppId = AppId;
            ParseData.Url = Url;
            var connection = new Connection()
            {
                DatabaseName = DatabaseName,
                Url = Url,
                AppId = AppId
            };
            if (!ConnectionData.Connections.Contains(connection))
            {
                ConnectionData.Connections.Add(connection);
                Connections.Add(connection);
            }
            else
            {
                ConnectionData.Connections.FirstOrDefault(c => c.DatabaseName == connection.DatabaseName).AppId = connection.AppId;
                ConnectionData.Connections.FirstOrDefault(c => c.DatabaseName == connection.DatabaseName).Url = connection.Url;
            }
            ConnectionData.ConvertConnectionsToXml();
            var response = ParseService.CheckForClass("Class");
            if(response != "OK")
            {
                var objectId = ParseService.CreateClass("Class");
                if (objectId == String.Empty)
                {
                    ErrorMessage = response;
                    return;
                }
            }
            ErrorMessage = String.Empty;
            var uri = new Uri(typeof(MenuView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.MenuRegion, uri);
        }

        private void RemoveConnection()
        {
            if (ConnectionData.Connections.Contains(SelectedConnection))
            {
                ConnectionData.Connections.Remove(SelectedConnection);
                Connections.Remove(SelectedConnection);
                ConnectionData.ConvertConnectionsToXml();
            }
        }

        private bool CanRemoveConnection()
        {
            return (SelectedConnection != null);
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
            SelectedConnection = connection;
            RemoveConnectionCommand.RaiseCanExecuteChanged();
            OnPropertyChanged("DatabaseName");
            OnPropertyChanged("Url");
            OnPropertyChanged("AppId");
        }
    }
}
