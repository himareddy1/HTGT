using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(htgt.Startup))]
namespace htgt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
