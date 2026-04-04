// Copyright (c) Meta Platforms, Inc. and affiliates.
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// Ported from yoga/tests/YGFlexBasisFitContentTest.cpp

using Xunit;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;
using static Facebook.Yoga.YGConfigAPI;

namespace Yoga.Tests;

public class YGFlexBasisFitContentTest
{
    private static YGSize MeasureTextLike(Node node, float width, MeasureMode widthMode, float height, MeasureMode heightMode)
    {
        float measuredWidth = 200.0f;
        if (widthMode == MeasureMode.AtMost)
        {
            measuredWidth = Math.Min(measuredWidth, width);
        }
        return new YGSize { Width = measuredWidth, Height = 20.0f };
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Percentage_height_converges(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root, 300);
        YGNodeStyleSetWidth(root, 100);

        var container = YGNodeNewWithConfig(config);
        YGNodeInsertChild(root, container, 0);

        var child = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeightPercent(child, 50);
        YGNodeInsertChild(container, child, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(75f, YGNodeLayoutGetHeight(child));
        Assert.Equal(150f, YGNodeLayoutGetHeight(container));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Percentage_with_flex_grow_converges(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root, 400);
        YGNodeStyleSetWidth(root, 100);

        var containerA = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(containerA, 1);
        YGNodeInsertChild(root, containerA, 0);

        var childA = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeightPercent(childA, 25);
        YGNodeInsertChild(containerA, childA, 0);

        var containerB = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(containerB, 1);
        YGNodeInsertChild(root, containerB, 1);

        var childB = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeightPercent(childB, 50);
        YGNodeInsertChild(containerB, childB, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(150f, YGNodeLayoutGetHeight(containerA));
        Assert.Equal(250f, YGNodeLayoutGetHeight(containerB));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Flex_shrink_overflow_converges(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root, 200);
        YGNodeStyleSetWidth(root, 100);

        var container = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexShrink(container, 1);
        YGNodeInsertChild(root, container, 0);

        var child = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeightPercent(child, 100);
        YGNodeInsertChild(container, child, 0);

        var fixedNode = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(fixedNode, 150);
        YGNodeInsertChild(root, fixedNode, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(50f, YGNodeLayoutGetHeight(container));
        Assert.Equal(150f, YGNodeLayoutGetHeight(fixedNode));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Scroll_avoids_remeasure(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var measureCount = new int[] { 0 };

        YGSize measureFunc(Node node, float w, MeasureMode wm, float h, MeasureMode hm)
        {
            measureCount[0]++;
            return new YGSize { Width = 50.0f, Height = 50.0f };
        }

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetOverflow(root, YGOverflow.Scroll);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 500);

        var sibling = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(sibling, 100);
        YGNodeInsertChild(root, sibling, 0);

        var wrapper = YGNodeNewWithConfig(config);
        YGNodeInsertChild(root, wrapper, 1);

        var inner = YGNodeNewWithConfig(config);
        YGNodeInsertChild(wrapper, inner, 0);

        var leaf = YGNodeNewWithConfig(config);
        YGNodeSetMeasureFunc(leaf, measureFunc);
        YGNodeInsertChild(inner, leaf, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        int firstPassCount = measureCount[0];

        YGNodeStyleSetHeight(sibling, 200);
        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        int secondPassCount = measureCount[0] - firstPassCount;

        Assert.Equal(50f, YGNodeLayoutGetHeight(leaf));
        Assert.Equal(0, secondPassCount);

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Row_direction_unchanged(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var container = YGNodeNewWithConfig(config);
        YGNodeInsertChild(root, container, 0);

        var text = YGNodeNewWithConfig(config);
        YGNodeSetMeasureFunc(text, MeasureTextLike);
        YGNodeInsertChild(container, text, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(100f, YGNodeLayoutGetWidth(text));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Row_scroll_skips_width(bool featureEnabled)
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, featureEnabled);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);
        YGNodeStyleSetOverflow(root, YGOverflow.Scroll);
        YGNodeStyleSetWidth(root, 100);
        YGNodeStyleSetHeight(root, 100);

        var text = YGNodeNewWithConfig(config);
        YGNodeSetMeasureFunc(text, MeasureTextLike);
        YGNodeInsertChild(root, text, 0);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

        Assert.Equal(200f, YGNodeLayoutGetWidth(text));

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }

    [Fact]
    public void Flex_basis_fit_content_feature_change_invalidates_cache()
    {
        var config = YGConfigNew();
        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, false);

        var root = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(root, 300);
        YGNodeStyleSetWidth(root, 100);

        var container = YGNodeNewWithConfig(config);
        YGNodeStyleSetFlexGrow(container, 1);
        YGNodeInsertChild(root, container, 0);

        var child = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeightPercent(child, 50);
        YGNodeInsertChild(container, child, 0);

        var fixedNode = YGNodeNewWithConfig(config);
        YGNodeStyleSetHeight(fixedNode, 100);
        YGNodeInsertChild(root, fixedNode, 1);

        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        float heightBefore = YGNodeLayoutGetHeight(container);

        YGConfigSetExperimentalFeatureEnabled(config, YGExperimentalFeature.FixFlexBasisFitContent, true);
        YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);
        float heightAfter = YGNodeLayoutGetHeight(container);

        Assert.Equal(heightBefore, heightAfter);

        YGNodeFreeRecursive(root);
        YGConfigFree(config);
    }
}
