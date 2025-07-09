# PublishExtension

## Build and Debug

1. Install the .NET 8 SDK.
2. Open `PublishExtension.sln` (create from `PublishExtension.csproj` if needed) in Visual Studio 2022.
3. Press **F5** to launch the experimental instance with the extension.
4. Use the `Publish` menu on the main menu bar.

### CLI

Run `dotnet run --project PublishCli -- <solution-or-project>` to invoke the shared publish logic from the command line.

### Tests

Execute `dotnet test` to run unit tests for the shared library.
