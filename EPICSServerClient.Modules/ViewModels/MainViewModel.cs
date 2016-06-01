using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Helpers.Data;
using EPICSServerClient.Modules.Views;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICSServerClient.Modules.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel(IRegionManager RegionManager)
        {
            ConnectionData.InitiateConnectionsXml();
        }
    }
}
