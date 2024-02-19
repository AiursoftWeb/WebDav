using Microsoft.Extensions.DependencyInjection;

namespace Aiursoft.WebDav.Builders
{
    class WebDavBuilder(IServiceCollection services) : IWebDavBuilder
    {
        public IServiceCollection Services { get; } = services;
    }
}
