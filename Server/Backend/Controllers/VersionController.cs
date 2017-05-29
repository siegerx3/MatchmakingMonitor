using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Backend.Controllers
{
  [RoutePrefix("version")]
  public class VersionController : ApiController
  {
    private static readonly string CacheKey = "latest";

    [Route("latest"), HttpGet]
    public async Task<string> GetLatest()
    {
      return (await Latest()).ToString();
    }

    public static async Task<Version> Latest()
    {
      var cache = MemoryCache.Default;

      try
      {
        var cachedVersion = cache.Get(CacheKey) as Version;
        if (cachedVersion != null)
          return cachedVersion;

        var version = await GetVersion();
        cache.Add(CacheKey, version, new CacheItemPolicy() { Priority = CacheItemPriority.Default, AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10) });
        return version;
      }
      catch
      {
        return new Version();
      }
    }

    private static async Task<Version> GetVersion()
    {
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "versions", "latest.txt");
      var versionString = await Task.Run(() => File.ReadAllText(path));
      return new Version(versionString);
    }
  }
}
