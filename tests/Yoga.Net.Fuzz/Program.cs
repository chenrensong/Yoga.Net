using Yoga.Net.Fuzz;

Console.WriteLine("Yoga.Net Fuzz Test");
Console.WriteLine("==================");
Console.WriteLine();

int rounds = 10_000;
int seed = 42;

if (args.Length > 0)
{
    int.TryParse(args[0], out rounds);
}
if (args.Length > 1)
{
    int.TryParse(args[1], out seed);
}

Console.WriteLine($"Rounds: {rounds}, Seed: {seed}");
Console.WriteLine();

var result = FuzzLayout.Run(rounds, seed);

Console.WriteLine($"Results: {result.SuccessCount}/{result.TotalRounds} passed");

if (result.AllPassed)
{
    Console.WriteLine("SUCCESS: No exceptions in all rounds.");
}
else
{
    Console.WriteLine($"FAILURE: {result.Exceptions.Count} exceptions encountered:");
    for (int i = 0; i < Math.Min(result.Exceptions.Count, 10); i++)
    {
        Console.WriteLine($"  [{i + 1}] {result.Exceptions[i].GetType().Name}: {result.Exceptions[i].Message}");
        if (result.Exceptions[i].StackTrace != null)
        {
            Console.WriteLine($"      {result.Exceptions[i].StackTrace!.Split('\n')[0].Trim()}");
        }
    }
    if (result.Exceptions.Count > 10)
    {
        Console.WriteLine($"  ... and {result.Exceptions.Count - 10} more");
    }
}

return result.AllPassed ? 0 : 1;
