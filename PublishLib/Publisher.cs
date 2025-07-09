using System;
using System.Threading.Tasks;

namespace PublishLib;

public class Publisher
{
    public Task<string> PublishSolutionAsync(string solutionPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(solutionPath);
        // placeholder - actual publish implementation would go here
        return Task.FromResult($"Published solution: {solutionPath}");
    }

    public Task<string> PublishProjectAsync(string projectPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(projectPath);
        // placeholder - actual publish implementation would go here
        return Task.FromResult($"Published project: {projectPath}");
    }
}
