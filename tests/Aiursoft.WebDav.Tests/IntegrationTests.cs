using System.Net;
using System.Xml.Linq;
using Aiursoft.CSTools.Tools;

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
        await using var webServer = TestServerBuilder.BuildApp(
            Path.Combine(AppContext.BaseDirectory, "Assets"),
            availablePort,
            true);

        await webServer.StartAsync();
        using var client = new HttpClient();
        client.BaseAddress = new Uri($"http://localhost:{availablePort}/webdav/");

        static async Task MoveAsync(HttpClient client, string source, string destination)
        {
            using var request = new HttpRequestMessage(new HttpMethod("MOVE"), source);
            request.Headers.Add("Destination", new Uri(client.BaseAddress!, destination).AbsoluteUri);
            using var response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        static async Task<XDocument> PropfindAsync(HttpClient client, string path)
        {
            using var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), path);
            request.Headers.Add("Depth", "1");
            using var response = await client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.MultiStatus, response.StatusCode);
            return XDocument.Parse(await response.Content.ReadAsStringAsync());
        }

        XNamespace dav = "DAV:";

        // Put a file.
        const string newFileContent = "MyTestFile Content";
        using var newContent = new StringContent(newFileContent);
        using var putFileResult = await client.PutAsync("file.txt", newContent);
        Assert.AreEqual(HttpStatusCode.Created, putFileResult.StatusCode);

        // Edit a file.
        using var editedContent = new StringContent(newFileContent + "2");
        using var editFileResult = await client.PutAsync("file.txt", editedContent);
        Assert.AreEqual(HttpStatusCode.Created, editFileResult.StatusCode);

        // Download a file.
        using var response = await client.GetAsync("file.txt");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(newFileContent + "2", await response.Content.ReadAsStringAsync());

        // Create a folder.
        using var createFolderRequest = new HttpRequestMessage(new HttpMethod("MKCOL"), "new-folder");
        using var createFolderResult = await client.SendAsync(createFolderRequest);
        Assert.AreEqual(HttpStatusCode.Created, createFolderResult.StatusCode);

        // Move a file and list the folder content.
        await MoveAsync(client, "file.txt", "new-folder/dest.txt");
        var list = await PropfindAsync(client, "new-folder");
        Assert.IsTrue(list.Descendants(dav + "href").Any(href => href.Value.EndsWith("dest.txt")));

        // Move a folder and verify its content.
        await MoveAsync(client, "new-folder", "new-folder2");
        list = await PropfindAsync(client, "new-folder2");
        Assert.IsTrue(list.Descendants(dav + "href").Any(href => href.Value.EndsWith("dest.txt")));

        // Delete a folder.
        using var deleteResult = await client.DeleteAsync("new-folder2");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteResult.StatusCode);

        // Put and delete another file.
        using var secondContent = new StringContent(newFileContent);
        using var putFileResult2 = await client.PutAsync("file2.txt", secondContent);
        Assert.AreEqual(HttpStatusCode.Created, putFileResult2.StatusCode);
        using var deleteFileResult = await client.DeleteAsync("file2.txt");
        Assert.AreEqual(HttpStatusCode.NoContent, deleteFileResult.StatusCode);

        // List the root folder after deleting the moved collection.
        list = await PropfindAsync(client, string.Empty);
        Assert.IsFalse(list.Descendants(dav + "href").Any(href => href.Value.EndsWith("dest.txt")));
    }
}
