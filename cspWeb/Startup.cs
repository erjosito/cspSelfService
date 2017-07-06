using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(cspWeb.Startup))]
namespace cspWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
 
    }
}
