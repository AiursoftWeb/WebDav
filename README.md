# Aiursoft WebDav

<a name="aiursoft-webdav"></a>

[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/AiursoftWeb/webdav/blob/master/LICENSE)
[![Pipeline stat](https://badge-proxy.aiursoft.com/aiursoft/webdav/badges/master/pipeline.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/pipelines)
[![Test Coverage](https://badge-proxy.aiursoft.com/aiursoft/webdav/badges/master/coverage.svg)](https://gitlab.aiursoft.com/aiursoft/webdav/-/pipelines)
[![S360 compliance](https://s360.aiursoft.com/badge.svg?project=aiursoft%2Fwebdav)](https://s360.aiursoft.com/SupplyChain?tab=projects&project=aiursoft%2Fwebdav&scope=baseline&ecosystem=all)
[![NuGet version (Aiursoft.WebDav)](https://img.shields.io/nuget/v/Aiursoft.webdav.svg)](https://www.nuget.org/packages/Aiursoft.webdav/)
[![Man hours](https://manhours.aiursoft.com/r/github.com/aiursoftweb/webdav.svg)](https://manhours.aiursoft.com/r/github.com/aiursoftweb/webdav.html)

WebDav is an ASP.NET Core middleware that provides a WebDav server for your application.

## Overview

### What is WebDav?

Host a WebDAV endpoint backed by a configured local filesystem inside an ASP.NET Core application.

## Installation

Add the package to your .NET project:

<a name="how-to-install"></a>

To install `Aiursoft.WebDav` to your project from [nuget.org](https://www.nuget.org/packages/Aiursoft.WebDav/):

```bash
dotnet add package Aiursoft.WebDav
```

Requires .NET 10 or later.

## Usage

<a name="how-to-use"></a>

Use this complete ASP.NET Core `Program.cs`:

```csharp
using Aiursoft.WebDav;

var IsReadOnlyWebDavServer = false;
var webDavRoot = "/tmp";

var builder = WebApplication.CreateBuilder();
builder.Services
    .AddWebDav(x => x.IsReadOnly = IsReadOnlyWebDavServer)
    .AddFilesystem(options => options.SourcePath = webDavRoot);

var host = builder.Build();

// Use it as a middleware.
host.UseWebDav(new PathString("/webdav"));
host.Run();
```

## Configuration

**Filesystem and route**

Set `LocalFilesystemOptions.SourcePath` to the directory the application will expose; its default is an empty string, so configure it explicitly. `WebDavOptions.IsReadOnly` defaults to `false`. Set it to `true` for read-only access, and choose the middleware base path passed to `UseWebDav`.

The sample uses `/tmp` as in the original example. Replace it with the intended dedicated directory before deployment. Authentication and authorization are the hosting application's responsibility; mounting the middleware does not create a login policy.

## Development

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

From the repository root, restore, build and test:

```bash
dotnet restore Aiursoft.WebDav.sln
dotnet build Aiursoft.WebDav.sln --no-restore
dotnet test Aiursoft.WebDav.sln --no-restore
```

Run `./lint.sh` for the existing static checks.

## How to contribute

<a name="how-to-contribute"></a>

There are many ways to contribute to the project: logging bugs, submitting pull requests, reporting issues, and creating suggestions.

Even if you have push rights on the repository, you should create a personal fork and create feature branches there when you need them. This keeps the main repository clean and your workflow cruft out of sight.

We're also interested in your feedback on the future of this project. You can submit a suggestion or feature request through the issue tracker. To make this process more effective, we're asking that these include more information to help define them more clearly.

## License

Licensed under the [MIT License](https://github.com/AiursoftWeb/webdav/blob/master/LICENSE).
