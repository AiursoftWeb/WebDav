using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    class WebDavCollectionResult(DirectoryInfo directory) : WebDavXmlResult
    {
        public DirectoryInfo Directory { get; } = directory;

        public override XElement ToXml(WebDavContext context)
        {
            return new XElement(Dav + "response",
                new XElement(Dav + "href", $"{context.BaseUrl}/{Directory.Name}"),
                new XElement(Dav + "propstat",
                    new XElement(Dav + "prop",
                        new XElement(Dav + "displayname", Directory.Name),
                        new XElement(Dav + "resourcetype",
                            new XElement(Dav + "collection"))),
                    new XElement(Dav + "status", "HTTP/1.1 200 OK")
                    ));
        }
    }
}
