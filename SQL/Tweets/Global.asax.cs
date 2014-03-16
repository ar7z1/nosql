using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace Tweets
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var container = new Container();
            container.Configure(r => r.Scan(s =>
                                            {
                                                s.TheCallingAssembly();
                                                s.RegisterConcreteTypesAgainstTheFirstInterface()
                                                 .OnAddedPluginTypes(c => c.LifecycleIs(InstanceScope.Singleton));
                                                s.LookForRegistries();
                                            }));
            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllersFactory(container));
        }
    }
}