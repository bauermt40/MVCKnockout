using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCKnockout.Web.Startup))]
namespace MVCKnockout.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
