using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JPEGWatermarkingWeb.Startup))]
namespace JPEGWatermarkingWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
