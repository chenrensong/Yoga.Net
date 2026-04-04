using BenchmarkDotNet.Attributes;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;

namespace Yoga.Net.Benchmarks;

[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 100)]
[MemoryDiagnoser]
public class SyntheticBenchmark
{
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
    [Benchmark(Description = "Stack with flex")]
    public void StackWithFlex()
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
    [Benchmark(Description = "Align stretch in undefined axis")]
    public void AlignStretchInUndefinedAxis()
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
    [Benchmark(Description = "Nested flex")]
    public void NestedFlex()
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
    [Benchmark(Description = "Huge nested layout")]
    public void HugeNestedLayout()
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
