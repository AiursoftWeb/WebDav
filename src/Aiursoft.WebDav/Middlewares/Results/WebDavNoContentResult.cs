using System.Net;

namespace Aiursoft.WebDav.Middlewares.Results
{
    public class WebDavNoContentResult(HttpStatusCode statusCode) : IWebDavResult
    {
        public virtual int StatusCode { get; } = (int)statusCode;
    }
}
