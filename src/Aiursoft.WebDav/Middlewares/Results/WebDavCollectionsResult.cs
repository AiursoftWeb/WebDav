using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    class WebDavCollectionsResult(DirectoryInfo directory) : WebDavXmlResult
    {
        public DirectoryInfo Directory { get; } = directory;

        public override XElement ToXml(WebDavContext context)
        {
            return new XElement(Dav + "multistatus",
                new XElement(Dav + "response",
                    new XElement(Dav + "href", $"{context.BaseUrl}/"),
                    new XElement(Dav + "propstat",
                        new XElement(Dav + "prop",
                            new XElement(Dav + "displayname", Directory.Name),
                            new XElement(Dav + "resourcetype",
                                new XElement(Dav + "collection"))),
                        new XElement(Dav + "status", "HTTP/1.1 200 OK")
                        )),
                context.Depth == DepthMode.One ? Directory.GetDirectories()
                                                    .Select(x => new WebDavCollectionResult(x).ToXml(context))
                                                    .ToArray()
                                                    .Concat(
                                                    Directory
                                                    .GetFiles()
                                                    .OrderBy(x => x.Name)
                                                    .Select(x => new WebDavFile(x).ToXml(context))
                                                    .ToArray()) : new XElement[0]
                );
        }
    }
}
