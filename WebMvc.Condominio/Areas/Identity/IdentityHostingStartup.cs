using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(WebMvc.Condominio.Areas.Identity.IdentityHostingStartup))]
namespace WebMvc.Condominio.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}