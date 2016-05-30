using EPICSServerClient.Helpers.Clients;
using EPICSServerClient.Helpers.Services;
using Microsoft.Practices.Unity;

namespace EPICSServerClient.Helpers.Regions
{
    public static class UnityConfig
    {
        public static void ConfigureUnity(IUnityContainer container)
        {
            container.RegisterType<IParseService, ParseClient>();
        }
    }
}
