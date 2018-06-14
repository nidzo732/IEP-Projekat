using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IEP_Projekat.Startup))]
namespace IEP_Projekat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
