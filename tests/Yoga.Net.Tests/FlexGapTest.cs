// Copyright (c) Meta Platforms, Inc. and affiliates.
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// Ported from yoga/tests/FlexGapTest.cpp

using Xunit;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;
using static Facebook.Yoga.YGConfigAPI;

namespace Yoga.Tests;

public class FlexGapTest
{
    [Fact]
    public void Gap_negative_value()
    {
        var config = YGConfigNew();

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetGap(root, YGGutter.All, -20);
        YGNodeStyleSetHeight(root, 200);

        var root_child0 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child0, 20);
        YGNodeInsertChild(root, root_child0, 0);

        var root_child1 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child1, 20);
        YGNodeInsertChild(root, root_child1, 1);

        var root_child2 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child2, 20);
        YGNodeInsertChild(root, root_child2, 2);

        var root_child3 = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root_child3, 20);
        YGNodeInsertChild(root, root_child3, 3);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(0f, YGNodeLayoutGetLeft(root));
        Assert.Equal(0f, YGNodeLayoutGetTop(root));
        Assert.Equal(80f, YGNodeLayoutGetWidth(root));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root));

        Assert.Equal(0f, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(20f, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(40f, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child2));

        Assert.Equal(60f, YGNodeLayoutGetLeft(root_child3));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child3));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child3));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child3));

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.RTL);

        Assert.Equal(0f, YGNodeLayoutGetLeft(root));
        Assert.Equal(0f, YGNodeLayoutGetTop(root));
        Assert.Equal(80f, YGNodeLayoutGetWidth(root));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root));

        Assert.Equal(60f, YGNodeLayoutGetLeft(root_child0));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child0));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child0));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child0));

        Assert.Equal(40f, YGNodeLayoutGetLeft(root_child1));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child1));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child1));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child1));

        Assert.Equal(20f, YGNodeLayoutGetLeft(root_child2));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child2));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child2));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child2));

        Assert.Equal(0f, YGNodeLayoutGetLeft(root_child3));
        Assert.Equal(0f, YGNodeLayoutGetTop(root_child3));
        Assert.Equal(20f, YGNodeLayoutGetWidth(root_child3));
        Assert.Equal(200f, YGNodeLayoutGetHeight(root_child3));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }
}
