﻿using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using EPICSServerClient.Helpers.Constants;
using EPICSServerClient.Modules.Views;
using System;
using EPICSServerClient.Helpers.Data;

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
            _regionManager.RegisterViewWithRegion(RegionConstants.ShellRegion, typeof(MainView));

            _container.RegisterType<Object, MenuView>(typeof(MenuView).FullName);
            _container.RegisterType<Object, MainView>(typeof(MainView).FullName);
            _container.RegisterType<Object, ParseGridView>(typeof(ParseGridView).FullName);
            _container.RegisterType<Object, ServerView>(typeof(ServerView).FullName);

            var uri = new Uri(typeof(MainView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionConstants.ShellRegion, uri);
            uri = new Uri(typeof(MenuView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionConstants.MenuRegion, uri);
            uri = new Uri(typeof(ServerView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionConstants.ContentRegion, uri);
        }
    }
}