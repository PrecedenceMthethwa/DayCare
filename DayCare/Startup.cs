using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DayCare.Startup))]
namespace DayCare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
