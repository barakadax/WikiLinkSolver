using LinkSolver;
using System.Diagnostics;

namespace WikiLinkSolver;

public class Program
{
    public static async Task Main()
    {
        Stopwatch x = new();
        x.Start();
        string firstDefinition = "Börek";
        string searchedDefinition = "Midnight";

        var wikiSolver = new WikiSolver(2);

        await wikiSolver.Run(firstDefinition, searchedDefinition);
        x.Stop();
        Console.WriteLine(x.Elapsed);
    }
}
