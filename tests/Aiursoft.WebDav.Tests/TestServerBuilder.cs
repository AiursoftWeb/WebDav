using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Aiursoft.WebDav.Tests;

public static class TestServerBuilder
{
    public static WebApplication BuildApp(
        string path, 
        int port, 
        bool webDavCanWrite)
    {
        var contentRoot = Path.GetFullPath(path);
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            WebRootPath = contentRoot,
            ContentRootPath = contentRoot
        });
        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(port));
        builder.Services
            .AddWebDav(x => x.IsReadOnly = !webDavCanWrite)
            .AddFilesystem(options => options.SourcePath = contentRoot);
        
        var host = builder.Build();
        host.UseWebDav(new PathString("/webdav"));
        return host;
    }
}
