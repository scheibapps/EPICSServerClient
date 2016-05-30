using System;
using Prism.Mvvm;
using Prism.Regions;

namespace EPICSServerClient.Helpers.Data
{
    public class ViewModelBase : BindableBase, IConfirmNavigationRequest, IRegionMemberLifetime
    {
        public virtual bool KeepAlive
        {
            get
            {
                return true;
            }
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {

            continuationCallback(true);
        }
    }
}