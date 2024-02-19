using System.Net;
using Aiursoft.WebDav.Filesystems;
using Aiursoft.WebDav.Helpers;
using Aiursoft.WebDav.Middlewares.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aiursoft.WebDav.Middlewares
{
    /// <summary>
    /// WebDavMiddleware
    /// </summary>
    public class WebDavMiddleware(
        RequestDelegate next,
        IOptions<WebDavOptions> options,
        IWebDavFilesystem filesystem,
        ILogger<WebDavMiddleware> logger)
    {
        // ReSharper disable once UnusedMember.Global
        public RequestDelegate Next { get; } = next;
        private readonly WebDavOptions _options = options.Value;

        public async Task InvokeAsync(HttpContext context)
        {
            var webDavContext = new WebDavContext(
                baseUrl : $"{context.Request.Scheme}://{context.Request.Host}",
                path: (string?)context.GetRouteValue("filePath") ?? string.Empty,
                depth: context.Request.Headers["Depth"].FirstOrDefault() switch
                {
                    "0" => DepthMode.Zero,
                    "1" => DepthMode.One,
                    "infinity" => DepthMode.Infinity,
                    _ => DepthMode.None
                });

            context.SetWebDavContext(webDavContext);

            logger.LogTrace($"HTTP Request: {context.Request.Method} {context.Request.Path}");

            var action =  context.Request.Method switch
            {
                "OPTIONS" => ProcessOptionsAsync(context),
                "PROPFIND" => ProcessPropfindAsync(context),
                "PROPPATCH" => ProcessPropPatchAsync(context),
                "MKCOL" => ProcessMkcolAsync(context),
                "GET" => ProcessGetAsync(context),
                "PUT" => ProcessPutAsync(context),
                "HEAD" => ProcessHeadAsync(),
                "LOCK" => ProcessLockAsync(context),
                "UNLOCK" => ProcessUnlockAsync(),
                "MOVE" => ProcessMoveAsync(context),
                "DELETE" => ProcessDeleteAsync(context),
                _ => ProcessUnknown(context)
            };

            await action;
        }

        private Task ProcessOptionsAsync(HttpContext context)
        {
            context.Response.Headers.Append("Allow",
                _options.IsReadOnly
                    ? "OPTIONS, TRACE, GET, HEAD, PROPFIND"
                    : "OPTIONS, TRACE, GET, HEAD, DELETE, PUT, POST, COPY, MOVE, MKCOL, PROPFIND, PROPPATCH, LOCK, UNLOCK");

            context.Response.Headers.Append("DAV", "1, 2");
            context.Response.Headers.Append("MS-Author-Via", "DAV");
            return Task.CompletedTask;
        }

        private async Task ProcessGetAsync(HttpContext context)
        {
            await using var fs = await filesystem.OpenFileStreamAsync(context.GetWebDavContext());
            await fs.CopyToAsync(context.Response.Body);
        }

        private async Task ProcessMkcolAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to create a collection.");
            }

            var result = await filesystem.CreateCollectionAsync(context.GetWebDavContext());
            
            context.Response.StatusCode = result.StatusCode;
        }

        private Task ProcessPutAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to modify the file.");
            }

            context.Response.StatusCode = StatusCodes.Status201Created;
            return filesystem.WriteFileAsync(context.Request.Body, context.GetWebDavContext());
        }

        private Task ProcessHeadAsync()
        {
            return Task.CompletedTask;
        }

        private async Task ProcessPropPatchAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to modify the file.");
            }
            await context.Request.ReadContentAsString();
        }

        private async Task ProcessPropfindAsync(HttpContext context)
        {
            var result = await filesystem.FindPropertiesAsync(context.GetWebDavContext());

            context.Response.StatusCode = result.StatusCode;

            if (result is IWebDavXmlResult xmlResult)
            {
                var xml = xmlResult.ToXml(context.GetWebDavContext());

                context.Response.ContentType = "application/xml";

                await context.Response.WriteAsync(xml.ToString());
            }
        }

        private async Task ProcessLockAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to modify the file.");
            }

            await context.Request.ReadContentAsString();
            var t = new WebDavLockResult().ToXml(context.GetWebDavContext()).ToString();
            await context.Response.WriteAsync(t);
        }

        private Task ProcessUnlockAsync()
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to modify the file.");
            }

            return Task.CompletedTask;
        }

        private Task ProcessDeleteAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to delete the file.");
            }

            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return filesystem.DeleteAsync(context.GetWebDavContext());
        }

        private async Task ProcessMoveAsync(HttpContext context)
        {
            if (_options.IsReadOnly)
            {
                throw new InvalidOperationException("The server is read-only. But the request is trying to move the file.");
            }

            if (context.Request.Headers.TryGetValue("destination", out var destinations) == false
                || destinations.Any() == false)
            {
                throw new InvalidOperationException("The destination header is missing.");
            }

            var destination = destinations.First();
            // destination may be `http://localhost:12345/webdav/new-folder/dest.txt`
            // But what we actually need is `/new-folder/dest.txt`
            var newUri = new Uri(destination!);
            var path = newUri.PathAndQuery.UrlDecode(); // /webdav/new-folder/dest.txt
            var newPath = path[path.IndexOf('/', 1)..]; // /new-folder/dest.txt
            await filesystem.MoveToAsync(context.GetWebDavContext(), newPath);
            context.Response.StatusCode = StatusCodes.Status201Created;
        }

        private Task ProcessUnknown(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            return Task.CompletedTask;
        }

    }
}
