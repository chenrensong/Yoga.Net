// Copyright (c) Meta Platforms, Inc. and affiliates.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
//
// C# unit tests mimicking yoga/tests (generated test patterns)

using Xunit;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;

namespace Yoga.Tests;

/// <summary>
/// Tests ported from yoga/tests/generated/YGAbsolutePositionTest.cpp
/// </summary>
public class AbsolutePositionTests
{
    [Fact]
    public void Absolute_layout_width_height_start_top()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeStyleSetPositionType(root_child0, YGPositionType.Absolute);
        YGNodeStyleSetPosition(root_child0, YGEdge.Start, 10);
        YGNodeStyleSetPosition(root_child0, YGEdge.Top, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.RTL);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(80, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Absolute_layout_width_height_end_bottom()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeStyleSetPositionType(root_child0, YGPositionType.Absolute);
        YGNodeStyleSetPosition(root_child0, YGEdge.End, 10);
        YGNodeStyleSetPosition(root_child0, YGEdge.Bottom, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(80, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(80, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.RTL);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(80, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Absolute_layout_start_top_end_bottom()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root_child0, YGPositionType.Absolute);
        YGNodeStyleSetPosition(root_child0, YGEdge.Start, 10);
        YGNodeStyleSetPosition(root_child0, YGEdge.Top, 10);
        YGNodeStyleSetPosition(root_child0, YGEdge.End, 10);
        YGNodeStyleSetPosition(root_child0, YGEdge.Bottom, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(80, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(80, YGNodeLayoutGetHeight(root_child0));

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.RTL);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(80, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(80, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGFlexDirectionTest.cpp
/// </summary>
public class FlexDirectionTests
{
    [Fact]
    public void Flex_direction_column_no_height()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(30, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(20, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Flex_direction_row_no_width()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(30, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(20, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Flex_direction_column()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Flex_direction_row()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGAlignItemsTest.cpp
/// </summary>
public class AlignItemsTests
{
    [Fact]
    public void Align_items_stretch()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Align_items_center()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetAlignItems(root, YGAlign.Center);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(45, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Align_items_flex_start()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetAlignItems(root, YGAlign.FlexStart);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Align_items_flex_end()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetAlignItems(root, YGAlign.FlexEnd);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(90, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGFlexTest.cpp
/// </summary>
public class FlexTests
{
    [Fact]
    public void Flex_basis_flex_grow_column()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child0, 1);
        YGNodeStyleSetFlexBasis(root_child0, 50);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child1, 1);
        YGNodeInsertChild(root, root_child1, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(75, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(75, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(25, YGNodeLayoutGetHeight(root_child1));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Flex_basis_flex_grow_row()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child0, 1);
        YGNodeStyleSetFlexBasis(root_child0, 50);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child1, 1);
        YGNodeInsertChild(root, root_child1, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(75, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(75, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(25, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child1));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Flex_shrink_column()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexShrink(root_child0, 1);
        YGNodeStyleSetHeight(root_child0, 100);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child1, 50);
        YGNodeInsertChild(root, root_child1, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(50, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(50, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(50, YGNodeLayoutGetHeight(root_child1));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGJustifyContentTest.cpp
/// </summary>
public class JustifyContentTests
{
    [Fact]
    public void Justify_content_row_flex_start()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 102);
        YGNodeStyleSetHeight(root, 102);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(102, YGNodeLayoutGetWidth(root));
        Assert.Equal(102, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(20, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Justify_content_row_flex_end()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetJustifyContent(root, YGJustify.FlexEnd);
        YGNodeStyleSetWidth(root, 102);
        YGNodeStyleSetHeight(root, 102);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(102, YGNodeLayoutGetWidth(root));
        Assert.Equal(102, YGNodeLayoutGetHeight(root));

        Assert.Equal(72, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(82, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(92, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Justify_content_row_center()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetJustifyContent(root, YGJustify.Center);
        YGNodeStyleSetWidth(root, 102);
        YGNodeStyleSetHeight(root, 102);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(102, YGNodeLayoutGetWidth(root));
        Assert.Equal(102, YGNodeLayoutGetHeight(root));

        Assert.Equal(36, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(46, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(56, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Justify_content_row_space_between()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetJustifyContent(root, YGJustify.SpaceBetween);
        YGNodeStyleSetWidth(root, 102);
        YGNodeStyleSetHeight(root, 102);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(102, YGNodeLayoutGetWidth(root));
        Assert.Equal(102, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(46, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(92, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(102, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Justify_content_column_flex_start()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 102);
        YGNodeStyleSetHeight(root, 102);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child1, 10);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root_child2, 10);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(102, YGNodeLayoutGetWidth(root));
        Assert.Equal(102, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(102, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(102, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(20, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(102, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child2));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGMarginTest.cpp and YGPaddingTest.cpp
/// </summary>
public class MarginPaddingTests
{
    [Fact]
    public void Margin_start()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetMargin(root_child0, YGEdge.Start, 10);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.RTL);

        Assert.Equal(80, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Padding_no_size()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetPadding(root, YGEdge.Left, 10);
        YGNodeStyleSetPadding(root, YGEdge.Top, 10);
        YGNodeStyleSetPadding(root, YGEdge.Right, 10);
        YGNodeStyleSetPadding(root, YGEdge.Bottom, 10);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(20, YGNodeLayoutGetWidth(root));
        Assert.Equal(20, YGNodeLayoutGetHeight(root));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Padding_container_match_child()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetPadding(root, YGEdge.Left, 10);
        YGNodeStyleSetPadding(root, YGEdge.Top, 10);
        YGNodeStyleSetPadding(root, YGEdge.Right, 10);
        YGNodeStyleSetPadding(root, YGEdge.Bottom, 10);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(30, YGNodeLayoutGetWidth(root));
        Assert.Equal(30, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGBorderTest.cpp
/// </summary>
public class BorderTests
{
    [Fact]
    public void Border_no_size()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetBorder(root, YGEdge.Left, 10);
        YGNodeStyleSetBorder(root, YGEdge.Top, 10);
        YGNodeStyleSetBorder(root, YGEdge.Right, 10);
        YGNodeStyleSetBorder(root, YGEdge.Bottom, 10);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(20, YGNodeLayoutGetWidth(root));
        Assert.Equal(20, YGNodeLayoutGetHeight(root));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Border_container_match_child()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetBorder(root, YGEdge.Left, 10);
        YGNodeStyleSetBorder(root, YGEdge.Top, 10);
        YGNodeStyleSetBorder(root, YGEdge.Right, 10);
        YGNodeStyleSetBorder(root, YGEdge.Bottom, 10);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 10);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(30, YGNodeLayoutGetWidth(root));
        Assert.Equal(30, YGNodeLayoutGetHeight(root));

        Assert.Equal(10, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(10, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGDimensionTest.cpp
/// </summary>
public class DimensionTests
{
    [Fact]
    public void Wrap_child()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 100);
        YGNodeStyleSetHeight(root_child0, 100);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGDisplayTest.cpp
/// </summary>
public class DisplayTests
{
    [Fact]
    public void Display_none()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child0, 1);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child1, 1);
        YGNodeStyleSetDisplay(root_child1, YGDisplay.None);
        YGNodeInsertChild(root, root_child1, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(0, YGNodeLayoutGetHeight(root_child1));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGGapTest.cpp
/// </summary>
public class GapTests
{
    [Fact]
    public void Column_gap()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);
        YGNodeStyleSetGap(root, YGGutter.Column, 10);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child0, 1);
        YGNodeStyleSetFlexShrink(root_child0, 1);
        YGNodeStyleSetFlexBasis(root_child0, 0);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child1, 1);
        YGNodeStyleSetFlexShrink(root_child1, 1);
        YGNodeStyleSetFlexBasis(root_child1, 0);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child2, 1);
        YGNodeStyleSetFlexShrink(root_child2, 1);
        YGNodeStyleSetFlexBasis(root_child2, 0);
        YGNodeInsertChild(root, root_child2, 2);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        // Each child gets (100 - 2*10) / 3 = 80/3 ≈ 26.67
        var child0Width = YGNodeLayoutGetWidth(root_child0);
        Assert.True(child0Width > 26 && child0Width < 28, $"Expected ~26.67 but got {child0Width}");
        Assert.Equal(100, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGPercentageTest.cpp
/// </summary>
public class PercentageTests
{
    [Fact]
    public void Percentage_width_height()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetWidth(root, 200);
        YGNodeStyleSetHeight(root, 200);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidthPercent(root_child0, 30);
        YGNodeStyleSetHeightPercent(root_child0, 30);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(200, YGNodeLayoutGetWidth(root));
        Assert.Equal(200, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(60, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(60, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGFlexWrapTest.cpp
/// </summary>
public class FlexWrapTests
{
    [Fact]
    public void Wrap_row()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetFlexWrap(root, YGWrap.Wrap);
        YGNodeStyleSetWidth(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 31);
        YGNodeStyleSetHeight(root_child0, 30);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 32);
        YGNodeStyleSetHeight(root_child1, 30);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 33);
        YGNodeStyleSetHeight(root_child2, 30);
        YGNodeInsertChild(root, root_child2, 2);

        var root_child3 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child3, 34);
        YGNodeStyleSetHeight(root_child3, 30);
        YGNodeInsertChild(root, root_child3, 3);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(60, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(31, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(30, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(31, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(32, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(30, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(63, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(33, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(30, YGNodeLayoutGetHeight(root_child2));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child3));
        Assert.Equal(30, YGNodeLayoutGetTop(root_child3));
        Assert.Equal(34, YGNodeLayoutGetWidth(root_child3));
        Assert.Equal(30, YGNodeLayoutGetHeight(root_child3));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests for the FloatOptional type
/// Ported from yoga/tests/FloatOptionalTest.cpp
/// </summary>
public class FloatOptionalTests
{
    [Fact]
    public void Undefined_is_undefined()
    {
        var opt = FloatOptional.Undefined;
        Assert.True(opt.IsUndefined());
        Assert.False(opt.IsDefined());
    }

    [Fact]
    public void Value_is_defined()
    {
        var opt = new FloatOptional(42.0f);
        Assert.False(opt.IsUndefined());
        Assert.True(opt.IsDefined());
        Assert.Equal(42.0f, opt.Unwrap());
    }

    [Fact]
    public void Zero_is_defined()
    {
        var opt = FloatOptional.Zero;
        Assert.False(opt.IsUndefined());
        Assert.True(opt.IsDefined());
        Assert.Equal(0.0f, opt.Unwrap());
    }

    [Fact]
    public void Undefined_equals_undefined()
    {
        Assert.True(FloatOptional.Undefined == FloatOptional.Undefined);
    }

    [Fact]
    public void Undefined_does_not_equal_value()
    {
        Assert.True(FloatOptional.Undefined != new FloatOptional(1.0f));
    }

    [Fact]
    public void Unwrap_or_default_uses_default_for_undefined()
    {
        var opt = FloatOptional.Undefined;
        Assert.Equal(99.0f, opt.UnwrapOrDefault(99.0f));
    }

    [Fact]
    public void Unwrap_or_default_uses_value_when_defined()
    {
        var opt = new FloatOptional(42.0f);
        Assert.Equal(42.0f, opt.UnwrapOrDefault(99.0f));
    }
}

/// <summary>
/// Tests ported from yoga/tests/YGMeasureTest.cpp
/// </summary>
public class MeasureTests
{
    private static YGSize MeasureFunction(
        Node node, float width, MeasureMode widthMode, float height, MeasureMode heightMode)
    {
        return new YGSize { Width = 10, Height = 10 };
    }

    [Fact]
    public void Measure_once_single_child()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeSetMeasureFunc(root_child0, MeasureFunction);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(10, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/YGDefaultValuesTest.cpp
/// </summary>
public class DefaultValuesTests
{
    [Fact]
    public void Assert_default_values()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);

        Assert.Equal(YGFlexDirection.Column, YGNodeStyleGetFlexDirection(root));
        Assert.Equal(YGJustify.FlexStart, YGNodeStyleGetJustifyContent(root));
        Assert.Equal(YGAlign.FlexStart, YGNodeStyleGetAlignContent(root));
        Assert.Equal(YGAlign.Stretch, YGNodeStyleGetAlignItems(root));
        Assert.Equal(YGAlign.Auto, YGNodeStyleGetAlignSelf(root));
        Assert.Equal(YGPositionType.Relative, YGNodeStyleGetPositionType(root));
        Assert.Equal(YGWrap.NoWrap, YGNodeStyleGetFlexWrap(root));
        Assert.Equal(YGOverflow.Visible, YGNodeStyleGetOverflow(root));

        YGNodeFreeRecursive(root);
    }
}

/// <summary>
/// Tests ported from yoga/tests/generated/YGMinMaxDimensionTest.cpp
/// </summary>
public class MinMaxDimensionTests
{
    [Fact]
    public void Max_width()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetMaxWidth(root_child0, 50);
        YGNodeStyleSetHeight(root_child0, 10);
        YGNodeInsertChild(root, root_child0, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(50, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(10, YGNodeLayoutGetHeight(root_child0));

        YGNodeFreeRecursive(root);
    }

    [Fact]
    public void Min_height()
    {
        var config = new Config();
        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetPositionType(root, YGPositionType.Absolute);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child0, 1);
        YGNodeStyleSetMinHeight(root_child0, 60);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(root_child1, 1);
        YGNodeInsertChild(root, root_child1, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0, YGNodeLayoutGetLeft(root));
        Assert.Equal(0, YGNodeLayoutGetTop(root));
        Assert.Equal(100, YGNodeLayoutGetWidth(root));
        Assert.Equal(100, YGNodeLayoutGetHeight(root));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(80, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(0, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(80, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(100, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(20, YGNodeLayoutGetHeight(root_child1));

        YGNodeFreeRecursive(root);
    }
}
