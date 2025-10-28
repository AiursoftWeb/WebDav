# Aiursoft WebDav

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/blob/master/LICENSE)
[![Pipeline stat](https://gitlab.aiursoft.com/aiursoft/webdav/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/pipelines)
[![Test Coverage](https://gitlab.aiursoft.com/aiursoft/webdav/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/pipelines)
[![NuGet version (Aiursoft.WebDav)](https://img.shields.io/nuget/v/Aiursoft.webdav.svg)](https://www.nuget.org/packages/Aiursoft.webdav/)
[![ManHours](https://manhours.aiursoft.cn/r/gitlab.aiursoft.com/aiursoft/webdav.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/commits/master?ref_type=heads)

WebDav is an ASP.NET Core middleware that provides a WebDav server for your application.

## How to install

To install `Aiursoft.WebDav` to your project from [nuget.org](https://www.nuget.org/packages/Aiursoft.WebDav/):

```bash
dotnet add package Aiursoft.WebDav
```

## How to use

You can use this middleware in your `Startup.cs` file:

```csharp
var IsReadOnlyWebDavServer = false;
var webDavRoot = "/tmp";

var builder = WebApplication.CreateBuilder();
builder.Services
    .AddWebDav(x => x.IsReadOnly = IsReadOnlyWebDavServer)
    .AddFilesystem(options => options.SourcePath = webDavRoot);

var host = builder.Build();

// Use it as a middleware.
host.UseWebDav(new PathString("/webdav"));
```

## How to contribute

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you with push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.
