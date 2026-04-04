using BenchmarkDotNet.Attributes;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;

namespace Yoga.Net.Benchmarks;

[SimpleJob(launchCount: 3, warmupCount: 5, iterationCount: 100)]
[MemoryDiagnoser]
public class SyntheticBenchmark
{
    [Benchmark(Description = "Stack with flex")]
    public void StackWithFlex()
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

    [Benchmark(Description = "Align stretch in undefined axis")]
    public void AlignStretchInUndefinedAxis()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetHeight(child, 20);
            YGNodeInsertChild(root, child, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    [Benchmark(Description = "Nested flex (10x10)")]
    public void NestedFlex()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        for (int i = 0; i < 10; i++)
        {
            var row = YGNodeNew();
            YGNodeStyleSetFlexDirection(row, YGFlexDirection.Row);
            YGNodeStyleSetFlexGrow(row, 1);

            for (int j = 0; j < 10; j++)
            {
                var cell = YGNodeNew();
                YGNodeStyleSetFlexGrow(cell, 1);
                YGNodeInsertChild(row, cell, (nuint)j);
            }

            YGNodeInsertChild(root, row, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    [Benchmark(Description = "Huge nested layout (1000 nodes)")]
    public void HugeNestedLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        // Create 10x10x10 nested structure = 1000 nodes
        for (int i = 0; i < 10; i++)
        {
            var level1 = YGNodeNew();
            YGNodeStyleSetFlexDirection(level1, YGFlexDirection.Row);
            YGNodeStyleSetFlexGrow(level1, 1);

            for (int j = 0; j < 10; j++)
            {
                var level2 = YGNodeNew();
                YGNodeStyleSetFlexDirection(level2, YGFlexDirection.Column);
                YGNodeStyleSetFlexGrow(level2, 1);

                for (int k = 0; k < 10; k++)
                {
                    var leaf = YGNodeNew();
                    YGNodeStyleSetFlexGrow(leaf, 1);
                    YGNodeInsertChild(level2, leaf, (nuint)k);
                }

                YGNodeInsertChild(level1, level2, (nuint)j);
            }

            YGNodeInsertChild(root, level1, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    [Benchmark(Description = "Deep nested layout (4 levels)")]
    public void DeepNestedLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);

        CreateDeepTree(root, 4, 10);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    private void CreateDeepTree(Node parent, int depth, int childrenPerLevel)
    {
        if (depth <= 0) return;

        var flexDir = depth % 2 == 0 ? YGFlexDirection.Row : YGFlexDirection.Column;

        for (int i = 0; i < childrenPerLevel; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetFlexDirection(child, flexDir);
            YGNodeStyleSetFlexGrow(child, 1);
            YGNodeInsertChild(parent, child, (nuint)i);

            CreateDeepTree(child, depth - 1, childrenPerLevel);
        }
    }

    [Benchmark(Description = "Grid-like layout with gaps")]
    public void GridLikeLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 300);
        YGNodeStyleSetHeight(root, 300);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);
        YGNodeStyleSetGap(root, YGGutter.All, 10);

        for (int i = 0; i < 5; i++)
        {
            var row = YGNodeNew();
            YGNodeStyleSetFlexDirection(row, YGFlexDirection.Row);
            YGNodeStyleSetFlexGrow(row, 1);
            YGNodeStyleSetGap(row, YGGutter.All, 10);

            for (int j = 0; j < 5; j++)
            {
                var cell = YGNodeNew();
                YGNodeStyleSetFlexGrow(cell, 1);
                YGNodeInsertChild(row, cell, (nuint)j);
            }

            YGNodeInsertChild(root, row, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    [Benchmark(Description = "Absolute positioning")]
    public void AbsolutePositioning()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        for (int i = 0; i < 10; i++)
        {
            var child = YGNodeNew();
            YGNodeStyleSetPositionType(child, YGPositionType.Absolute);
            YGNodeStyleSetPosition(child, YGEdge.Left, i * 5);
            YGNodeStyleSetPosition(child, YGEdge.Top, i * 5);
            YGNodeStyleSetWidth(child, 20);
            YGNodeStyleSetHeight(child, 20);
            YGNodeInsertChild(root, child, (nuint)i);
        }

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }

    [Benchmark(Description = "Complex mixed layout")]
    public void ComplexMixedLayout()
    {
        var root = YGNodeNew();
        YGNodeStyleSetWidth(root, 800);
        YGNodeStyleSetHeight(root, 600);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Column);
        YGNodeStyleSetPadding(root, YGEdge.All, 20);
        YGNodeStyleSetGap(root, YGGutter.All, 10);

        // Header
        var header = YGNodeNew();
        YGNodeStyleSetHeight(header, 60);
        YGNodeInsertChild(root, header, 0);

        // Content area
        var content = YGNodeNew();
        YGNodeStyleSetFlexDirection(content, YGFlexDirection.Row);
        YGNodeStyleSetFlexGrow(content, 1);
        YGNodeStyleSetGap(content, YGGutter.All, 10);

        // Sidebar
        var sidebar = YGNodeNew();
        YGNodeStyleSetWidth(sidebar, 200);
        YGNodeInsertChild(content, sidebar, 0);

        // Main area with nested content
        var main = YGNodeNew();
        YGNodeStyleSetFlexGrow(main, 1);
        YGNodeStyleSetFlexDirection(main, YGFlexDirection.Column);
        YGNodeStyleSetGap(main, YGGutter.All, 5);

        for (int i = 0; i < 5; i++)
        {
            var row = YGNodeNew();
            YGNodeStyleSetFlexDirection(row, YGFlexDirection.Row);
            YGNodeStyleSetFlexGrow(row, 1);
            YGNodeStyleSetGap(row, YGGutter.All, 5);

            for (int j = 0; j < 3; j++)
            {
                var item = YGNodeNew();
                YGNodeStyleSetFlexGrow(item, 1);
                YGNodeInsertChild(row, item, (nuint)j);
            }

            YGNodeInsertChild(main, row, (nuint)i);
        }

        YGNodeInsertChild(content, main, 1);
        YGNodeInsertChild(root, content, 1);

        // Footer
        var footer = YGNodeNew();
        YGNodeStyleSetHeight(footer, 40);
        YGNodeInsertChild(root, footer, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
    }
}
