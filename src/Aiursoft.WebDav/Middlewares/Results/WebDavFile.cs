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
            return new XElement(Dav + "response",
                        new XElement(Dav + "href", $"{context.BaseUrl}/{DisplayName}"),
                        new XElement(Dav + "propstat",
                            new XElement(Dav + "prop",
                             new XElement(Dav + "displayname", DisplayName),
                             new XElement(Dav + "getcontentlength", $"{Length}"),
                             new XElement(Dav + "creationdate", $"{CreatedAt:yyyy-MM-ddTHH:mm:sszzz}"),
                             new XElement(Dav + "getlastmodified", $"{ModifiedAt:yyyy-MM-ddTHH:mm:sszzz}")
                             ))
                );
        }
    }
}
