// Copyright (c) Meta Platforms, Inc. and its affiliates.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
//
// Original: yoga/YGNodeStyle.h, yoga/YGNodeStyle.cpp

namespace Yoga;

// These enums would be defined elsewhere (YGEnums.h equivalent)
public enum YGFlexDirection { Column, ColumnReverse, Row, RowReverse }
public enum YGJustify { FlexStart, Center, FlexEnd, SpaceBetween, SpaceAround, SpaceEvenly }
public enum YGAlign { Auto, FlexStart, Center, FlexEnd, Stretch, Baseline, SpaceBetween, SpaceAround }
public enum YGPositionType { Static, Relative, Absolute }
public enum YGWrap { NoWrap, Wrap, WrapReverse }
public enum YGOverflow { Visible, Hidden, Scroll }
public enum YGDisplay { Flex, None }
public enum YGGutter { Column, Row, All }
public enum YGBoxSizing { BorderBox, ContentBox }
public enum YGGridTrackType { Points, Percent, Fr, Auto, Minmax }


public class Style { /* Implementation elsewhere */ }
public class StyleSizeLength { /* Implementation elsewhere */ }
public class StyleLength { /* Implementation elsewhere */ }
public class FloatOptional { /* Implementation elsewhere */ }
public class GridLine { /* Implementation elsewhere */ }
public class GridTrackSize { /* Implementation elsewhere */ }

public static class YogaStyleAPI
{
    private static void UpdateStyle<TValue>(
        YogaNode node,
        Func<Style, TValue> getter,
        Action<Style, TValue> setter,
        TValue value)
    {
        var style = node.style();
        if (!Equals(getter(style), value))
        {
            setter(style, value);
            node.MarkDirtyAndPropagate();
        }
    }

    private static void UpdateStyle<TIdx, TValue>(
        YogaNode node,
        Func<Style, TIdx, TValue> getter,
        Action<Style, TIdx, TValue> setter,
        TIdx idx,
        TValue value)
    {
        var style = node.style();
        if (!Equals(getter(style, idx), value))
        {
            setter(style, idx, value);
            node.MarkDirtyAndPropagate();
        }
    }

    public static void CopyStyle(YogaNode dstNode, YogaNode srcNode)
    {
        if (dstNode.style() != srcNode.style())
        {
            dstNode.setStyle(srcNode.style());
            dstNode.MarkDirtyAndPropagate();
        }
    }

    // Direction
    public static void SetDirection(YogaNode node, YGDirection value)
    {
        UpdateStyle(node,
            s => s.direction,
            (s, v) => s.setDirection(v),
            value);
    }

    public static YGDirection GetDirection(YogaNode node)
    {
        return node.style().direction();
    }

    // FlexDirection
    public static void SetFlexDirection(YogaNode node, YGFlexDirection value)
    {
        UpdateStyle(node,
            s => s.flexDirection,
            (s, v) => s.setFlexDirection(v),
            value);
    }

    public static YGFlexDirection GetFlexDirection(YogaNode node)
    {
        return node.style().flexDirection();
    }

    // JustifyContent
    public static void SetJustifyContent(YogaNode node, YGJustify value)
    {
        UpdateStyle(node,
            s => s.justifyContent,
            (s, v) => s.setJustifyContent(v),
            value);
    }

    public static YGJustify GetJustifyContent(YogaNode node)
    {
        return node.style().justifyContent();
    }

    // JustifyItems
    public static void SetJustifyItems(YogaNode node, YGJustify value)
    {
        UpdateStyle(node,
            s => s.justifyItems,
            (s, v) => s.setJustifyItems(v),
            value);
    }

    public static YGJustify GetJustifyItems(YogaNode node)
    {
        return node.style().justifyItems();
    }

    // JustifySelf
    public static void SetJustifySelf(YogaNode node, YGJustify value)
    {
        UpdateStyle(node,
            s => s.justifySelf,
            (s, v) => s.setJustifySelf(v),
            value);
    }

    public static YGJustify GetJustifySelf(YogaNode node)
    {
        return node.style().justifySelf();
    }

    // AlignContent
    public static void SetAlignContent(YogaNode node, YGAlign value)
    {
        UpdateStyle(node,
            s => s.alignContent,
            (s, v) => s.setAlignContent(v),
            value);
    }

    public static YGAlign GetAlignContent(YogaNode node)
    {
        return node.style().alignContent();
    }

    // AlignItems
    public static void SetAlignItems(YogaNode node, YGAlign value)
    {
        UpdateStyle(node,
            s => s.alignItems,
            (s, v) => s.setAlignItems(v),
            value);
    }

    public static YGAlign GetAlignItems(YogaNode node)
    {
        return node.style().alignItems();
    }

    // AlignSelf
    public static void SetAlignSelf(YogaNode node, YGAlign value)
    {
        UpdateStyle(node,
            s => s.alignSelf,
            (s, v) => s.setAlignSelf(v),
            value);
    }

    public static YGAlign GetAlignSelf(YogaNode node)
    {
        return node.style().alignSelf();
    }

    // PositionType
    public static void SetPositionType(YogaNode node, YGPositionType value)
    {
        UpdateStyle(node,
            s => s.positionType,
            (s, v) => s.setPositionType(v),
            value);
    }

    public static YGPositionType GetPositionType(YogaNode node)
    {
        return node.style().positionType();
    }

    // FlexWrap
    public static void SetFlexWrap(YogaNode node, YGWrap value)
    {
        UpdateStyle(node,
            s => s.flexWrap,
            (s, v) => s.setFlexWrap(v),
            value);
    }

    public static YGWrap GetFlexWrap(YogaNode node)
    {
        return node.style().flexWrap();
    }

    // Overflow
    public static void SetOverflow(YogaNode node, YGOverflow value)
    {
        UpdateStyle(node,
            s => s.overflow,
            (s, v) => s.setOverflow(v),
            value);
    }

    public static YGOverflow GetOverflow(YogaNode node)
    {
        return node.style().overflow();
    }

    // Display
    public static void SetDisplay(YogaNode node, YGDisplay value)
    {
        UpdateStyle(node,
            s => s.display,
            (s, v) => s.setDisplay(v),
            value);
    }

    public static YGDisplay GetDisplay(YogaNode node)
    {
        return node.style().display();
    }

    // Flex
    public static void SetFlex(YogaNode node, float value)
    {
        UpdateStyle(node,
            s => s.flex,
            (s, v) => s.setFlex(new FloatOptional(v)),
            new FloatOptional(value));
    }

    public static float GetFlex(YogaNode node)
    {
        var flex = node.style().flex();
        return flex.isUndefined() ? float.NaN : flex.unwrap();
    }

    // FlexGrow
    public static void SetFlexGrow(YogaNode node, float value)
    {
        UpdateStyle(node,
            s => s.flexGrow,
            (s, v) => s.setFlexGrow(v),
            new FloatOptional(value));
    }

    public static float GetFlexGrow(YogaNode node)
    {
        var flexGrow = node.style().flexGrow();
        return flexGrow.isUndefined() ? 0.0f : flexGrow.unwrap();
    }

    // FlexShrink
    public static void SetFlexShrink(YogaNode node, float value)
    {
        UpdateStyle(node,
            s => s.flexShrink,
            (s, v) => s.setFlexShrink(v),
            new FloatOptional(value));
    }

    public static float GetFlexShrink(YogaNode node)
    {
        var flexShrink = node.style().flexShrink();
        return flexShrink.isUndefined()
            ? (node.getConfig().useWebDefaults() ? 1.0f : 0.0f)
            : flexShrink.unwrap();
    }

    // FlexBasis
    public static void SetFlexBasis(YogaNode node, float value)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.points(value));
    }

    public static void SetFlexBasisPercent(YogaNode node, float value)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.percent(value));
    }

    public static void SetFlexBasisAuto(YogaNode node)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.ofAuto());
    }

    public static void SetFlexBasisMaxContent(YogaNode node)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.ofMaxContent());
    }

    public static void SetFlexBasisFitContent(YogaNode node)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.ofFitContent());
    }

    public static void SetFlexBasisStretch(YogaNode node)
    {
        UpdateStyle(node,
            s => s.flexBasis,
            (s, v) => s.setFlexBasis(v),
            StyleSizeLength.ofStretch());
    }

    public static YGValue GetFlexBasis(YogaNode node)
    {
        return (YGValue)node.style().flexBasis();
    }

    // Position
    public static void SetPosition(YogaNode node, YGEdge edge, float points)
    {
        UpdateStyle(node,
            (s, e) => s.position(e),
            (s, e, v) => s.setPosition(e, v),
            edge,
            StyleLength.points(points));
    }

    public static void SetPositionPercent(YogaNode node, YGEdge edge, float percent)
    {
        UpdateStyle(node,
            (s, e) => s.position(e),
            (s, e, v) => s.setPosition(e, v),
            edge,
            StyleLength.percent(percent));
    }

    public static void SetPositionAuto(YogaNode node, YGEdge edge)
    {
        UpdateStyle(node,
            (s, e) => s.position(e),
            (s, e, v) => s.setPosition(e, v),
            edge,
            StyleLength.ofAuto());
    }

    public static YGValue GetPosition(YogaNode node, YGEdge edge)
    {
        return (YGValue)node.style().position(edge);
    }

    // Margin
    public static void SetMargin(YogaNode node, YGEdge edge, float points)
    {
        UpdateStyle(node,
            (s, e) => s.margin(e),
            (s, e, v) => s.setMargin(e, v),
            edge,
            StyleLength.points(points));
    }

    public static void SetMarginPercent(YogaNode node, YGEdge edge, float percent)
    {
        UpdateStyle(node,
            (s, e) => s.margin(e),
            (s, e, v) => s.setMargin(e, v),
            edge,
            StyleLength.percent(percent));
    }

    public static void SetMarginAuto(YogaNode node, YGEdge edge)
    {
        UpdateStyle(node,
            (s, e) => s.margin(e),
            (s, e, v) => s.setMargin(e, v),
            edge,
            StyleLength.ofAuto());
    }

    public static YGValue GetMargin(YogaNode node, YGEdge edge)
    {
        return (YGValue)node.style().margin(edge);
    }

    // Padding
    public static void SetPadding(YogaNode node, YGEdge edge, float points)
    {
        UpdateStyle(node,
            (s, e) => s.padding(e),
            (s, e, v) => s.setPadding(e, v),
            edge,
            StyleLength.points(points));
    }

    public static void SetPaddingPercent(YogaNode node, YGEdge edge, float percent)
    {
        UpdateStyle(node,
            (s, e) => s.padding(e),
            (s, e, v) => s.setPadding(e, v),
            edge,
            StyleLength.percent(percent));
    }

    public static YGValue GetPadding(YogaNode node, YGEdge edge)
    {
        return (YGValue)node.style().padding(edge);
    }

    // Border
    public static void SetBorder(YogaNode node, YGEdge edge, float border)
    {
        UpdateStyle(node,
            (s, e) => s.border(e),
            (s, e, v) => s.setBorder(e, v),
            edge,
            StyleLength.points(border));
    }

    public static float GetBorder(YogaNode node, YGEdge edge)
    {
        var border = node.style().border(edge);
        if (border.isUndefined() || border.isAuto())
            return float.NaN;

        return ((YGValue)border).Value;
    }

    // Gap
    public static void SetGap(YogaNode node, YGGutter gutter, float gapLength)
    {
        UpdateStyle(node,
            (s, g) => s.gap(g),
            (s, g, v) => s.setGap(g, v),
            gutter,
            StyleLength.points(gapLength));
    }

    public static void SetGapPercent(YogaNode node, YGGutter gutter, float percent)
    {
        UpdateStyle(node,
            (s, g) => s.gap(g),
            (s, g, v) => s.setGap(g, v),
            gutter,
            StyleLength.percent(percent));
    }

    public static YGValue GetGap(YogaNode node, YGGutter gutter)
    {
        return (YGValue)node.style().gap(gutter);
    }

    // AspectRatio
    public static void SetAspectRatio(YogaNode node, float aspectRatio)
    {
        UpdateStyle(node,
            s => s.aspectRatio,
            (s, v) => s.setAspectRatio(v),
            new FloatOptional(aspectRatio));
    }

    public static float GetAspectRatio(YogaNode node)
    {
        var op = node.style().aspectRatio();
        return op.isUndefined() ? float.NaN : op.unwrap();
    }

    // BoxSizing
    public static void SetBoxSizing(YogaNode node, YGBoxSizing boxSizing)
    {
        UpdateStyle(node,
            s => s.boxSizing,
            (s, v) => s.setBoxSizing(v),
            boxSizing);
    }

    public static YGBoxSizing GetBoxSizing(YogaNode node)
    {
        return node.style().boxSizing();
    }

    // Width & Height (Dimension setters)
    public static void SetWidth(YogaNode node, float points)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.points(points));
    }

    public static void SetWidthPercent(YogaNode node, float percent)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.percent(percent));
    }

    public static void SetWidthAuto(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.ofAuto());
    }

    public static void SetWidthMaxContent(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.ofMaxContent());
    }

    public static void SetWidthFitContent(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.ofFitContent());
    }

    public static void SetWidthStretch(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Width,
            StyleSizeLength.ofStretch());
    }

    public static YGValue GetWidth(YogaNode node)
    {
        return (YGValue)node.style().dimension(Dimension.Width);
    }

    public static void SetHeight(YogaNode node, float points)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.points(points));
    }

    public static void SetHeightPercent(YogaNode node, float percent)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.percent(percent));
    }

    public static void SetHeightAuto(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.ofAuto());
    }

    public static void SetHeightMaxContent(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.ofMaxContent());
    }

    public static void SetHeightFitContent(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.ofFitContent());
    }

    public static void SetHeightStretch(YogaNode node)
    {
        UpdateStyle(node,
            (s, d) => s.dimension(d),
            (s, d, v) => s.setDimension(d, v),
            Dimension.Height,
            StyleSizeLength.ofStretch());
    }

    public static YGValue GetHeight(YogaNode node)
    {
        return (YGValue)node.style().dimension(Dimension.Height);
    }

    // Min/Max Dimensions (simplified - similar pattern for all)
    public static void SetMinWidth(YogaNode node, float minWidth)
    {
        UpdateStyle(node,
            (s, d) => s.minDimension(d),
            (s, d, v) => s.setMinDimension(d, v),
            Dimension.Width,
            StyleSizeLength.points(minWidth));
    }

    public static void SetMinWidthPercent(YogaNode node, float minWidth)
    {
        UpdateStyle(node,
            (s, d) => s.minDimension(d),
            (s, d, v) => s.setMinDimension(d, v),
            Dimension.Width,
            StyleSizeLength.percent(minWidth));
    }

    public static YGValue GetMinWidth(YogaNode node)
    {
        return (YGValue)node.style().minDimension(Dimension.Width);
    }

    // Grid properties would follow the same pattern...
}
