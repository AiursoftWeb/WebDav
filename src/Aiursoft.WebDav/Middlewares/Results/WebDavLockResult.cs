using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    public class WebDavLockResult : WebDavXmlResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public override XElement ToXml(WebDavContext context)
        {
            return new XElement(Dav + "prop",
                new XAttribute(XNamespace.Xmlns + "d", Dav),
                        new XElement(Dav + "lockdiscovery",
                            new XElement(Dav + "activelock",
                                new XElement(Dav + "lockscope",
                                    new XElement(Dav + "exclusive")),
                                new XElement(Dav + "locktype",
                                    new XElement(Dav + "write")),
                                new XElement(Dav + "depth", "Infinity"),
                                new XElement(Dav + "timeout", "Second-604800"),
                                new XElement(Dav + "locktoken",
                                    new XElement(Dav + "href", $"opaquelocktoken:{Id}")))
                ));
        }
    }
}
