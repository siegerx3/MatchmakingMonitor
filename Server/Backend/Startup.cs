using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;

namespace Backend
{
  public class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      var config = new HttpConfiguration();
      var routes = RouteTable.Routes;

      config.MapHttpAttributeRoutes(new CentralizedPrefixProvider("api"));
      routes.Clear();


#if DEBUG
      config.EnableSwagger(c =>
      {
        c.PrettyPrint();
        c.SingleApiVersion("v1", "MatchmakingMonitor");
        c.DescribeAllEnumsAsStrings();
      }).EnableSwaggerUi(c =>
      {
        c.DisableValidator();
      });
#endif

      config.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

      ConfigureStaticFiles(app);

      app.UseWebApi(config);
    }

    private static void ConfigureStaticFiles(IAppBuilder app)
    {
      string root = AppDomain.CurrentDomain.BaseDirectory;
      string wwwroot = Path.Combine(root, "wwwroot");

      var fileServerOptions = new FileServerOptions()
      {
        EnableDefaultFiles = true,
        EnableDirectoryBrowsing = false,
        RequestPath = new PathString(string.Empty),
        FileSystem = new PhysicalFileSystem(wwwroot)
      };

      fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
      app.UseFileServer(fileServerOptions);
    }
  }
}
