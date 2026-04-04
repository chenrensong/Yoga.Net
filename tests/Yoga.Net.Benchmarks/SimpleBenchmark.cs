using System.Diagnostics;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;

namespace Yoga.Net.Benchmarks;

public static class SimpleBenchmark
{
    public static void Run()
    {
        Console.WriteLine("Yoga.Net Simple Benchmark");
        Console.WriteLine("=========================");
        Console.WriteLine($"Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
        Console.WriteLine();

        // Warmup
        for (int i = 0; i < 100; i++)
        {
            StackWithFlex();
            SimpleLayout();
            AlignStretch();
        }

        // Benchmarks
        RunBenchmark("Stack with flex (10 children)", StackWithFlex);
        RunBenchmark("Align stretch (10 children)", AlignStretch);
        RunBenchmark("Simple layout (5 nodes)", SimpleLayout);
        RunBenchmark("Row layout (10 children)", RowLayout);

        Console.WriteLine();
        Console.WriteLine("Note: These are single-threaded results.");
    }

    private static void RunBenchmark(string name, Action action)
    {
        const int iterations = 1000;
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            action();
        }

        sw.Stop();
        var avgMs = sw.Elapsed.TotalMilliseconds / iterations;
        var opsPerSec = 1000.0 / avgMs;
        Console.WriteLine($"{name,-35} {avgMs:F4} ms/op ({opsPerSec:F0} ops/sec)");
    }

    private static void StackWithFlex()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetFlexGrow(child, 1);
            YGNodeInsertChild(root, child, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    private static void AlignStretch()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetHeight(child, 20);
            YGNodeInsertChild(root, child, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    private static void SimpleLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 800);
        YGNodeStyleSetHeight(root, 600);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        var header = YGNodeNew();
        YGNodeStyleSetHeight(header, 60);
        YGNodeInsertChild(root, header, 0);

        var content = YGNodeNew();
        YGNodeStyleSetFlexGrow(content, 1);
        YGNodeInsertChild(root, content, 1);

        var footer = YGNodeNew();
        YGNodeStyleSetHeight(footer, 40);
        YGNodeInsertChild(root, footer, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    private static void RowLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 500);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetWidth(child, 40);
            YGNodeStyleSetHeight(child, 80);
            YGNodeInsertChild(root, child, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }
}
