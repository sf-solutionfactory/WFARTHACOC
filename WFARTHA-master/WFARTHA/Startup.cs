using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WFARTHA.Startup))]
namespace WFARTHA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
