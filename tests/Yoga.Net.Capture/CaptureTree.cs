// Port of yoga/capture/CaptureTree.cpp and yoga/capture/CaptureTree.h
// Original: Copyright (c) Meta Platforms, Inc. and affiliates.
// Licensed under the MIT license.

using Facebook.Yoga;

namespace Yoga.Net.Capture;

public static class CaptureTree
{
    /// <summary>
    /// Calculate layout and capture the full tree state to a JSON file.
    /// Mirrors C++ YGNodeCalculateLayoutWithCapture.
    /// </summary>
    public static string CalculateLayoutWithCapture(
        Node node,
        float availableWidth,
        float availableHeight,
        YGDirection ownerDirection)
    {
        // Dirty the tree first (matching C++ behavior for accurate capture)
        DirtyTree(node);

        YGNodeAPI.YGNodeCalculateLayout(
            node, availableWidth, availableHeight, ownerDirection);

        return NodeSerializer.SerializeToJson(
            node, availableWidth, availableHeight, ownerDirection);
    }

    /// <summary>
    /// Calculate layout and write capture JSON to a file.
    /// </summary>
    public static void CalculateLayoutWithCaptureToFile(
        Node node,
        float availableWidth,
        float availableHeight,
        YGDirection ownerDirection,
        string path)
    {
        var json = CalculateLayoutWithCapture(
            node, availableWidth, availableHeight, ownerDirection);
        File.WriteAllText(path, json);
    }

    private static void DirtyTree(Node node)
    {
        if (YGNodeAPI.YGNodeHasMeasureFunc(node))
        {
            YGNodeAPI.YGNodeMarkDirty(node);
        }

        var childCount = YGNodeAPI.YGNodeGetChildCount(node);
        for (nuint i = 0; i < childCount; i++)
        {
            var child = YGNodeAPI.YGNodeGetChild(node, i);
            if (child != null)
            {
                DirtyTree(child);
            }
        }
    }
}
