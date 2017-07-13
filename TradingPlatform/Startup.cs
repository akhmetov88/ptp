using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TradingPlatform.Startup))]
namespace TradingPlatform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
            app.MapSignalR("/signalr",new Microsoft.AspNet.SignalR.HubConfiguration() { EnableDetailedErrors = true, EnableJSONP = true});
        //    app.RunSignalR();
            //app.MapSignalR();
            //   app.MapSignalR<ChatConnection>("/chat");

           
        }
    }
}
