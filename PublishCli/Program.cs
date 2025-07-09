using System;
using PublishLib;

if (args.Length == 0)
{
    Console.WriteLine("Usage: publishcli <solution-or-project>");
    return;
}

var path = args[0];
var publisher = new Publisher();
string result = path.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)
    ? await publisher.PublishSolutionAsync(path)
    : await publisher.PublishProjectAsync(path);
Console.WriteLine(result);
