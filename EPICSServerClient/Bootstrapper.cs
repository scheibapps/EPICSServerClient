using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using Prism.Modularity;
using Prism.Unity;
using EPICSServerClient.Views;
using EPICSServerClient.Modules;
using EPICSServerClient.Helpers.Regions;
using Prism.Regions;
using System.Windows.Controls;

namespace EPICSServerClient
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            UnityConfig.ConfigureUnity(Container);
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings regionAdapterMappings = ServiceLocator.Current.GetInstance<RegionAdapterMappings>();
            if (regionAdapterMappings != null)
            {
                regionAdapterMappings.RegisterMapping(typeof(ContentControl), ServiceLocator.Current.GetInstance<ContentControlRegionAdapter>());
            }
            return regionAdapterMappings;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            var moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

            moduleCatalog.AddModule(typeof(MainModule));
        }

        public object GetShell()
        {
            return this.Shell;
        }
    }
}
