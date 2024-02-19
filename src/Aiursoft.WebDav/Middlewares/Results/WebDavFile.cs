using System.Xml.Linq;

namespace Aiursoft.WebDav.Middlewares.Results
{
    /// <summary>
    /// WebDavFile
    /// </summary>
    public class WebDavFile(FileInfo file) : WebDavXmlResult
    {
        public string DisplayName { get; set; } = file.Name;

        public long Length { get; set; } = file.Length;

        public DateTime? CreatedAt { get; set; } = file.CreationTime;

        public DateTime? ModifiedAt { get; set; } = file.LastWriteTime;

        public override XElement ToXml(WebDavContext context)
        {
            return new XElement(dav + "response",
                        new XElement(dav + "href", $"{context.BaseUrl}/{DisplayName}"),
                        new XElement(dav + "propstat",
                            new XElement(dav + "prop",
                             new XElement(dav + "displayname", DisplayName),
                             new XElement(dav + "getcontentlength", $"{Length}"),
                             new XElement(dav + "creationdate", $"{CreatedAt:yyyy-MM-ddTHH:mm:sszzz}"),
                             new XElement(dav + "getlastmodified", $"{ModifiedAt:yyyy-MM-ddTHH:mm:sszzz}")
                             ))
                );
        }
    }
}
