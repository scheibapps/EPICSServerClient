using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EPICSServerClient.Helpers.Services;
using EPICSServerClient.Helpers.Data;
using Prism.Regions;
using EPICSServerClient.Modules.Views;
using EPICSServerClient.Helpers.Constants;

namespace EPICSServerClient.Modules.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private ObservableCollection<TabItem> tabs = new ObservableCollection<TabItem>();
        private int index = 0;
        private IParseService ParseService;
        private IRegionManager RegionManager;

        public MenuViewModel(IParseService ParseService, IRegionManager RegionManager)
        {
            this.ParseService = ParseService;
            this.RegionManager = RegionManager;
            TabSelectedCommand = new DelegateCommand(MenuItemSelectionChanged);
            Tabs.Add(new TabItem { Header = "Configuration" });
            var uri = new Uri(typeof(ServerView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.ContentRegion, uri);
        }

        public ObservableCollection<TabItem> Tabs
        {
            get { return tabs; }
            set { tabs = value; }
        }


        public DelegateCommand TabSelectedCommand { get; set; }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private void MenuItemSelectionChanged()
        {
            if (Index == -1)
                return;
            Uri uri = null;
            if (Index == 0)
            {
                ParseData.CurrentClass = Tabs[Index].Header.ToString();
                uri = new Uri(typeof(ServerView).FullName, UriKind.Relative);
                RegionManager.RequestNavigate(RegionConstants.ContentRegion, uri);
                return;
            }
            if(Index == 1)
            {
                return;
            }
            ParseData.CurrentClass = Tabs[Index].Header.ToString();
            uri = new Uri(typeof(ParseGridView).FullName, UriKind.Relative);
            RegionManager.RequestNavigate(RegionConstants.ContentRegion, uri);
        }

        private void PopulateTabs()
        {
            Tabs.Clear();
            Tabs.Add(new TabItem { Header = "Configuration" });
            Tabs.Add(new TabItem { Header = "New" });
            foreach (TabItem tab in ParseService.GetClasses())
                Tabs.Add(tab);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (ParseData.AppId == String.Empty && ParseData.AppId == String.Empty)
                return;
            else
            {
                PopulateTabs();
            }
        }
    }
}
