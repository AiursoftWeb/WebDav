using System.Net;
using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    public abstract class WebDavXmlResult : IWebDavXmlResult
    {
        protected readonly XNamespace Dav = "DAV:";

        public virtual int StatusCode => (int)HttpStatusCode.MultiStatus;

        public abstract XElement ToXml(WebDavContext context);
    }
}
