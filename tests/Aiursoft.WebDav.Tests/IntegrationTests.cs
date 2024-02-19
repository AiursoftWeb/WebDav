using System.Net;
using System.Text;
using Aiursoft.CSTools.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebDav;

namespace Aiursoft.WebDav.Tests;

[TestClass]
public class IntegrationTests
{
    [TestInitialize]
    public async Task InitFiles()
    {
        // Clean up
        if (Directory.Exists(Path.Combine(AppContext.BaseDirectory, "Assets")))
        {
            Directory.Delete(Path.Combine(AppContext.BaseDirectory, "Assets"), true);
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Assets"));
            var indexContentFile = Path.Combine(AppContext.BaseDirectory, "Assets", "index.html");
            await File.WriteAllTextAsync(indexContentFile, "<h2>Hello world!</h2>");
        }
    }
    
    [TestMethod]
    public async Task TestWebDav()
    {
        var availablePort = Network.GetAvailablePort();
        var webServer = TestServerBuilder.BuildApp(
            Path.Combine(AppContext.BaseDirectory, "Assets"), 
            availablePort, 
            true);
        
        await webServer.StartAsync();
        await Task.Delay(1000);
        
        var clientParams = new WebDavClientParams { BaseAddress = new Uri($"http://localhost:{availablePort}/webdav/") };
        using var client = new WebDavClient(clientParams);
        
        // Put a file
        var newFileContent = "MyTestFile Content";
        var putFileResult = await client.PutFile("file.txt", new MemoryStream(Encoding.UTF8.GetBytes(newFileContent)));
        Assert.AreEqual((int)HttpStatusCode.Created, putFileResult.StatusCode);
        
        // Edit a file
        var editFileResult = await client.PutFile("file.txt", new MemoryStream(Encoding.UTF8.GetBytes(newFileContent + "2")));
        Assert.AreEqual((int)HttpStatusCode.Created, editFileResult.StatusCode);
        
        // Download a file
        var response = await client.GetRawFile("file.txt");
        var responseString = await new StreamReader(response.Stream).ReadToEndAsync();
        Assert.AreEqual(newFileContent + "2", responseString);
        
        // Create a folder
        var createFolderResult = await client.Mkcol("new-folder");
        Assert.AreEqual((int)HttpStatusCode.Created, createFolderResult.StatusCode);
        
        // Move a file
        var moveResult = await client.Move("file.txt", "new-folder/dest.txt");
        Assert.AreEqual((int)HttpStatusCode.Created, moveResult.StatusCode);
        
        // List folder content
        var list = await client.Propfind("new-folder");
        Assert.IsTrue(list.Resources.Any(f => f.Uri.ToString().EndsWith("dest.txt")));
        
        // Move a folder
        var moveFolderResult = await client.Move("new-folder", "new-folder2");
        Assert.AreEqual((int)HttpStatusCode.Created, moveFolderResult.StatusCode);
        
        // List folder content
        list = await client.Propfind("new-folder2");
        Assert.IsTrue(list.Resources.Any(f => f.Uri.ToString().EndsWith("dest.txt")));
        
        // Delete a folder
        var deleteResult = await client.Delete("new-folder2");
        Assert.AreEqual((int)HttpStatusCode.NoContent, deleteResult.StatusCode);
        
        // Put a file
        var putFileResult2 = await client.PutFile("file2.txt", new MemoryStream(Encoding.UTF8.GetBytes(newFileContent)));
        Assert.AreEqual((int)HttpStatusCode.Created, putFileResult2.StatusCode);
        
        // Delete a file
        var deleteFileResult = await client.Delete("file2.txt");
        Assert.AreEqual((int)HttpStatusCode.NoContent, deleteFileResult.StatusCode);
        
        // List folder content
        list = await client.Propfind("");
        Assert.IsFalse(list.Resources.Any(f => f.Uri.ToString().EndsWith("dest.txt")));
    }
}