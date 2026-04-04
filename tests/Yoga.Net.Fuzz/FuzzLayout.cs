// Port of yoga/fuzz/FuzzLayout.cpp for Yoga.Net verification
// Original: Copyright (c) Meta Platforms, Inc. and affiliates.
// Licensed under the MIT license.

using Facebook.Yoga;

namespace Yoga.Net.Fuzz;

/// <summary>
/// Fuzz test for Yoga.Net layout engine.
/// Creates random tree structures and verifies no crashes/exceptions occur.
/// </summary>
public static class FuzzLayout
{
    private const int MaxDepth = 20;
    private const int MaxChildren = 20;
    private const int MaxNodesPerTree = 50;

    private static readonly YGFlexDirection[] FlexDirections =
    {
        YGFlexDirection.Column,
        YGFlexDirection.ColumnReverse,
        YGFlexDirection.Row,
        YGFlexDirection.RowReverse,
    };

    /// <summary>
    /// Run N rounds of fuzz testing with a given random seed.
    /// </summary>
    public static FuzzResult Run(int rounds, int seed = 42)
    {
        var random = new Random(seed);
        int successCount = 0;
        var exceptions = new List<Exception>();

        for (int round = 0; round < rounds; round++)
        {
            try
            {
                var config = YGConfigAPI.YGConfigNew();
                var root = YGNodeAPI.YGNodeNewWithConfig(config);
                int nodeCount = 1; // count root
                FillFuzzedTree(random, config, root, depth: 0, ref nodeCount);

                YGNodeAPI.YGNodeCalculateLayout(
                    root,
                    float.NaN, // YGUndefined
                    float.NaN, // YGUndefined
                    YGDirection.LTR);

                YGNodeAPI.YGNodeFreeRecursive(root);
                YGConfigAPI.YGConfigFree(config);
                successCount++;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        return new FuzzResult(rounds, successCount, exceptions);
    }

    private static void FillFuzzedTree(
        Random random,
        Config config,
        Node root,
        int depth,
        ref int nodeCount)
    {
        if (depth > MaxDepth || nodeCount >= MaxNodesPerTree)
        {
            return;
        }

        int children = random.Next(MaxChildren + 1);
        for (int i = 0; i < children; i++)
        {
            if (nodeCount >= MaxNodesPerTree)
            {
                break;
            }

            var child = YGNodeAPI.YGNodeNewWithConfig(config);
            nodeCount++;

            // Set random flex direction on root (matching C++ behavior)
            YGNodeStyleAPI.YGNodeStyleSetFlexDirection(
                root,
                FlexDirections[random.Next(FlexDirections.Length)]);

            // Set random width and height
            YGNodeStyleAPI.YGNodeStyleSetWidth(child, random.NextSingle() * 1000);
            YGNodeStyleAPI.YGNodeStyleSetGap(
                child,
                YGGutter.All,
                random.NextSingle() * 100);
            YGNodeStyleAPI.YGNodeStyleSetHeight(child, random.NextSingle() * 1000);

            YGNodeAPI.YGNodeInsertChild(root, child, (nuint)i);
            FillFuzzedTree(random, config, child, depth + 1, ref nodeCount);
        }
    }
}

public record FuzzResult(int TotalRounds, int SuccessCount, List<Exception> Exceptions)
{
    public bool AllPassed => Exceptions.Count == 0;
}
