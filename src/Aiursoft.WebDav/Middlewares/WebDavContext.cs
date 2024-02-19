using Aiursoft.WebDav.Helpers;

namespace Aiursoft.WebDav.Middlewares
{
    /// <summary>
    /// WebDavContext
    /// </summary>
    public class WebDavContext(string baseUrl, string path, DepthMode depth)
    {
        /// <summary>
        /// BaseUrl
        /// </summary>
        public string BaseUrl { get; set; } = baseUrl;

        /// <summary>
        /// Path
        /// </summary>
        public string Path { get; set; } = path.UrlDecode();

        /// <summary>
        /// Depth
        /// </summary>
        public DepthMode Depth { get; set; } = depth;
    }
}
