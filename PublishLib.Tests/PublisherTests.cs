using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublishLib;
using System.Threading.Tasks;

namespace PublishLib.Tests;

[TestClass]
public class PublisherTests
{
    [TestMethod]
    public async Task PublishSolution_ReturnsMessage()
    {
        var publisher = new Publisher();
        var result = await publisher.PublishSolutionAsync("test.sln");
        Assert.AreEqual("Published solution: test.sln", result);
    }

    [TestMethod]
    public async Task PublishProject_ReturnsMessage()
    {
        var publisher = new Publisher();
        var result = await publisher.PublishProjectAsync("project.csproj");
        Assert.AreEqual("Published project: project.csproj", result);
    }
}
