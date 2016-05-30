using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace EPICSServerClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            using (var win = new HomeWindow())
            {
                win.Content = bootstrapper.GetShell();
                win.Activate();
                win.Focus();
                win.ShowDialog();
            }
        }

        public void Dispose()
        {
            
        }

        [STAThreadAttribute]
        public static void Main()
        {
            App app = new App();
            app.Run();
        }

    }
}
