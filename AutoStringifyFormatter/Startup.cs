using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(AutoStringifyFormatter.Startup))]

namespace AutoStringifyFormatter
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configure(config =>
            {
                config.Formatters.Insert(0, new FooAutoStringifyFormatter());
                config.MapHttpAttributeRoutes();
                config.EnsureInitialized();
                app.UseWebApi(config);
            });
        }
    }
}
