﻿using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    class WebDavCollectionResult(DirectoryInfo directory) : WebDavXmlResult
    {
        public DirectoryInfo Directory { get; } = directory;

        public override XElement ToXml(WebDavContext context)
        {
            return new XElement(dav + "response",
                new XElement(dav + "href", $"{context.BaseUrl}/{Directory.Name}"),
                new XElement(dav + "propstat",
                    new XElement(dav + "prop",
                        new XElement(dav + "displayname", Directory.Name),
                        new XElement(dav + "resourcetype",
                            new XElement(dav + "collection"))),
                    new XElement(dav + "status", "HTTP/1.1 200 OK")
                    ));
        }
    }
}
