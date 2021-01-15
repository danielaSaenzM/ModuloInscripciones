using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IncripcionesCobach.Startup))]
namespace IncripcionesCobach
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
