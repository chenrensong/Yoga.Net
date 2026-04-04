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
            AlignStretch();
            NestedFlex();
            HugeNestedLayout();
        }

        // Benchmarks
        RunBenchmark("Stack with flex", StackWithFlex);
        RunBenchmark("Align stretch in undefined axis", AlignStretch);
        RunBenchmark("Nested flex", NestedFlex);
        RunBenchmark("Huge nested layout", HugeNestedLayout);

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

    // Matches YGBenchmark.c:78-89
    private static YGSize _measure(
        Node node,
        float width,
        MeasureMode widthMode,
        float height,
        MeasureMode heightMode)
    {
        return new YGSize
        {
            Width = widthMode == MeasureMode.Undefined ? 10 : width,
            Height = heightMode == MeasureMode.Undefined ? 10 : height,
        };
    }

    // YGBenchmark.c:92-106
    private static void StackWithFlex()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeSetMeasureFunc(child, _measure);
            YGNodeStyleSetFlex(child, 1);
            YGNodeInsertChild(root, child, 0);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        YGNodeFreeRecursive(root);
    }

    // YGBenchmark.c:108-120
    private static void AlignStretch()
    {
        var root = YGNodeNew();

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetHeight(child, 20);
            YGNodeSetMeasureFunc(child, _measure);
            YGNodeInsertChild(root, child, 0);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        YGNodeFreeRecursive(root);
    }

    // YGBenchmark.c:122-140
    private static void NestedFlex()
    {
        var root = YGNodeNew();

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetFlex(child, 1);
            YGNodeInsertChild(root, child, 0);

            for (int ii = 0; ii < 10; ii++)
            {
                var grandChild = YGNodeNew();
                YGNodeSetMeasureFunc(grandChild, _measure);
                YGNodeStyleSetFlex(grandChild, 1);
                YGNodeInsertChild(child, grandChild, 0);
            }
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        YGNodeFreeRecursive(root);
    }

    // YGBenchmark.c:142-182
    private static void HugeNestedLayout()
    {
        var root = YGNodeNew();

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetFlexGrow(child, 1);
            YGNodeStyleSetWidth(child, 10);
            YGNodeStyleSetHeight(child, 10);
            YGNodeInsertChild(root, child, 0);

            for (int ii = 0; ii < 10; ii++)
            {
                var grandChild = YGNodeNew();
                YGNodeStyleSetFlexDirection(grandChild, YGFlexDirection.Row);
                YGNodeStyleSetFlexGrow(grandChild, 1);
                YGNodeStyleSetWidth(grandChild, 10);
                YGNodeStyleSetHeight(grandChild, 10);
                YGNodeInsertChild(child, grandChild, 0);

                for (int iii = 0; iii < 10; iii++)
                {
                    var grandGrandChild = YGNodeNew();
                    YGNodeStyleSetFlexGrow(grandGrandChild, 1);
                    YGNodeStyleSetWidth(grandGrandChild, 10);
                    YGNodeStyleSetHeight(grandGrandChild, 10);
                    YGNodeInsertChild(grandChild, grandGrandChild, 0);

                    for (int iiii = 0; iiii < 10; iiii++)
                    {
                        var grandGrandGrandChild = YGNodeNew();
                        YGNodeStyleSetFlexDirection(grandGrandGrandChild, YGFlexDirection.Row);
                        YGNodeStyleSetFlexGrow(grandGrandGrandChild, 1);
                        YGNodeStyleSetWidth(grandGrandGrandChild, 10);
                        YGNodeStyleSetHeight(grandGrandGrandChild, 10);
                        YGNodeInsertChild(grandGrandChild, grandGrandGrandChild, 0);
                    }
                }
            }
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        YGNodeFreeRecursive(root);
    }
}
