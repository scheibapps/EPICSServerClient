using System.Windows;
using MahApps.Metro.Controls;
using Microsoft.Practices.ServiceLocation;
using Prism.Modularity;
using Prism.Unity;
using EPICSServerClient.Views;

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

            Application.Current.MainWindow = (MetroWindow)this.Shell;
            Application.Current.MainWindow.ShowDialog();

            var shellWindow = (MetroWindow)this.Shell;

            shellWindow.Activate();
            shellWindow.Focus();
            shellWindow.ShowDialog();

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            var moduleCatalog = (ModuleCatalog)this.ModuleCatalog;

           // moduleCatalog.AddModule(typeof(MainModule));
        }

        public object GetShell()
        {
            return  this.Shell;
        }
    }
}
