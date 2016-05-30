using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using EPICSServerClient.Helpers;

namespace EPICSServerClient.Modules
{
    public class MainModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public MainModule(IRegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        public void Initialize()
        {
            //_regionManager.RegisterViewWithRegion(RegionConstants.ShellRegion, typeof());

            //_container.RegisterType<Object, MainView>(typeof(MainView).FullName);
            //_container.RegisterType<Object, MenuView>(typeof(MenuView).FullName);

            //_container.RegisterType<Object, AddDoorScheduleView>(typeof(AddDoorScheduleView).FullName);
            //_container.RegisterType<Object, ExportDoorScheduleView>(typeof(ExportDoorScheduleView).FullName);
            //_container.RegisterType<Object, ImportDoorScheduleView>(typeof(ImportDoorScheduleView).FullName);
            //_container.RegisterType<Object, ParametersSelectionView>(typeof(ParametersSelectionView).FullName);
            //_container.RegisterType<Object, AboutView>(typeof(AboutView).FullName);

            //_container.RegisterType<IGridService, GridService>();
        }
    }
}