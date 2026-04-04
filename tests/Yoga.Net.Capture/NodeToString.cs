// Port of yoga/capture/NodeToString.cpp and yoga/capture/NodeToString.h
// Original: Copyright (c) Meta Platforms, Inc. and affiliates.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Nodes;
using Facebook.Yoga;

namespace Yoga.Net.Capture;

[Flags]
public enum PrintOptions : byte
{
    Layout = 1 << 0,
    Children = 1 << 1,
    Style = 1 << 2,
    Config = 1 << 3,
    Node = 1 << 4,
}

public static class NodeSerializer
{
    private static void AppendFloatIfNotDefault(
        JsonObject j, string key, float num, float defaultNum)
    {
        if (num != defaultNum && !float.IsNaN(num))
        {
            j[key] = num;
        }
    }

    private static void WriteFloatIfNotNaN(JsonObject j, string key, float value)
    {
        if (!float.IsNaN(value))
        {
            j[key] = value;
        }
    }

    private static void AppendYGValueIfNotDefault(
        JsonObject j, string key, YGValue value, YGValue defaultValue)
    {
        if (!YGValueEquals(value, defaultValue))
        {
            if (value.Unit == Unit.Auto)
            {
                j[key] = "auto";
            }
            else if (value.Unit == Unit.Undefined)
            {
                j[key] = "undefined";
            }
            else
            {
                var obj = new JsonObject
                {
                    ["value"] = value.Value,
                    ["unit"] = value.Unit == Unit.Point ? "px" : "pct"
                };
                j[key] = obj;
            }
        }
    }

    private static void AppendEnumValueIfNotDefault(
        JsonObject j, string key, string value, string defaultValue)
    {
        if (value != defaultValue)
        {
            j[key] = value;
        }
    }

    private static void AppendBoolIfNotDefault(
        JsonObject j, string key, bool value, bool defaultValue)
    {
        if (value != defaultValue)
        {
            j[key] = value;
        }
    }

    private delegate YGValue EdgeGetter(Node node, YGEdge edge);

    private static void AppendEdges(
        JsonObject j,
        string key,
        Node node,
        Node defaultNode,
        EdgeGetter getter)
    {
        var style = j["style"] as JsonObject ?? new JsonObject();
        j["style"] = style;

        AppendYGValueIfNotDefault(style, key + "-left",
            getter(node, YGEdge.Left), getter(defaultNode, YGEdge.Left));
        AppendYGValueIfNotDefault(style, key + "-right",
            getter(node, YGEdge.Right), getter(defaultNode, YGEdge.Right));
        AppendYGValueIfNotDefault(style, key + "-top",
            getter(node, YGEdge.Top), getter(defaultNode, YGEdge.Top));
        AppendYGValueIfNotDefault(style, key + "-bottom",
            getter(node, YGEdge.Bottom), getter(defaultNode, YGEdge.Bottom));
        AppendYGValueIfNotDefault(style, key + "-all",
            getter(node, YGEdge.All), getter(defaultNode, YGEdge.All));
        AppendYGValueIfNotDefault(style, key + "-start",
            getter(node, YGEdge.Start), getter(defaultNode, YGEdge.Start));
        AppendYGValueIfNotDefault(style, key + "-end",
            getter(node, YGEdge.End), getter(defaultNode, YGEdge.End));
        AppendYGValueIfNotDefault(style, key + "-vertical",
            getter(node, YGEdge.Vertical), getter(defaultNode, YGEdge.Vertical));
        AppendYGValueIfNotDefault(style, key + "-horizontal",
            getter(node, YGEdge.Horizontal), getter(defaultNode, YGEdge.Horizontal));
    }

    private static YGValue BorderFloatToYGValue(Node node, YGEdge edge)
    {
        float val = YGNodeStyleAPI.YGNodeStyleGetBorder(node, edge);
        var unit = float.IsNaN(val) ? Unit.Undefined : Unit.Point;
        return new YGValue(val, unit);
    }

    private static void SerializeTreeImpl(
        JsonObject j,
        Node node,
        Node defaultNode,
        PrintOptions options)
    {
        if ((options & PrintOptions.Layout) == PrintOptions.Layout)
        {
            var layout = new JsonObject();
            WriteFloatIfNotNaN(layout, "width", YGNodeStyleAPI.YGNodeStyleGetWidth(node).Value);
            WriteFloatIfNotNaN(layout, "height", YGNodeStyleAPI.YGNodeStyleGetHeight(node).Value);
            WriteFloatIfNotNaN(layout, "top", YGNodeStyleAPI.YGNodeStyleGetPosition(node, YGEdge.Top).Value);
            WriteFloatIfNotNaN(layout, "left", YGNodeStyleAPI.YGNodeStyleGetPosition(node, YGEdge.Left).Value);
            if (layout.Count > 0)
            {
                j["layout"] = layout;
            }
        }

        if ((options & PrintOptions.Style) == PrintOptions.Style)
        {
            var style = new JsonObject();
            j["style"] = style;

            AppendEnumValueIfNotDefault(style, "flex-direction",
                YGNodeStyleAPI.YGNodeStyleGetFlexDirection(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetFlexDirection(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "justify-content",
                YGNodeStyleAPI.YGNodeStyleGetJustifyContent(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetJustifyContent(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "align-items",
                YGNodeStyleAPI.YGNodeStyleGetAlignItems(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetAlignItems(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "align-content",
                YGNodeStyleAPI.YGNodeStyleGetAlignContent(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetAlignContent(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "align-self",
                YGNodeStyleAPI.YGNodeStyleGetAlignSelf(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetAlignSelf(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "flex-wrap",
                YGNodeStyleAPI.YGNodeStyleGetFlexWrap(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetFlexWrap(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "overflow",
                YGNodeStyleAPI.YGNodeStyleGetOverflow(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetOverflow(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "display",
                YGNodeStyleAPI.YGNodeStyleGetDisplay(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetDisplay(defaultNode).ToStringFast());
            AppendEnumValueIfNotDefault(style, "position-type",
                YGNodeStyleAPI.YGNodeStyleGetPositionType(node).ToStringFast(),
                YGNodeStyleAPI.YGNodeStyleGetPositionType(defaultNode).ToStringFast());

            AppendFloatIfNotDefault(style, "flex-grow",
                YGNodeStyleAPI.YGNodeStyleGetFlexGrow(node),
                YGNodeStyleAPI.YGNodeStyleGetFlexGrow(defaultNode));
            AppendFloatIfNotDefault(style, "flex-shrink",
                YGNodeStyleAPI.YGNodeStyleGetFlexShrink(node),
                YGNodeStyleAPI.YGNodeStyleGetFlexShrink(defaultNode));
            AppendFloatIfNotDefault(style, "flex",
                YGNodeStyleAPI.YGNodeStyleGetFlex(node),
                YGNodeStyleAPI.YGNodeStyleGetFlex(defaultNode));
            AppendYGValueIfNotDefault(style, "flex-basis",
                YGNodeStyleAPI.YGNodeStyleGetFlexBasis(node),
                YGNodeStyleAPI.YGNodeStyleGetFlexBasis(defaultNode));

            AppendEdges(j, "margin", node, defaultNode,
                (n, e) => YGNodeStyleAPI.YGNodeStyleGetMargin(n, e));
            AppendEdges(j, "padding", node, defaultNode,
                (n, e) => YGNodeStyleAPI.YGNodeStyleGetPadding(n, e));
            AppendEdges(j, "border", node, defaultNode, BorderFloatToYGValue);
            AppendEdges(j, "position", node, defaultNode,
                (n, e) => YGNodeStyleAPI.YGNodeStyleGetPosition(n, e));

            // Ensure style reference is current after AppendEdges
            style = j["style"] as JsonObject ?? style;

            AppendYGValueIfNotDefault(style, "gap",
                YGNodeStyleAPI.YGNodeStyleGetGap(node, YGGutter.All),
                YGNodeStyleAPI.YGNodeStyleGetGap(defaultNode, YGGutter.All));
            AppendYGValueIfNotDefault(style, "column-gap",
                YGNodeStyleAPI.YGNodeStyleGetGap(node, YGGutter.Column),
                YGNodeStyleAPI.YGNodeStyleGetGap(defaultNode, YGGutter.Column));
            AppendYGValueIfNotDefault(style, "row-gap",
                YGNodeStyleAPI.YGNodeStyleGetGap(node, YGGutter.Row),
                YGNodeStyleAPI.YGNodeStyleGetGap(defaultNode, YGGutter.Row));

            AppendYGValueIfNotDefault(style, "width",
                YGNodeStyleAPI.YGNodeStyleGetWidth(node),
                YGNodeStyleAPI.YGNodeStyleGetWidth(defaultNode));
            AppendYGValueIfNotDefault(style, "height",
                YGNodeStyleAPI.YGNodeStyleGetHeight(node),
                YGNodeStyleAPI.YGNodeStyleGetHeight(defaultNode));
            AppendYGValueIfNotDefault(style, "max-width",
                YGNodeStyleAPI.YGNodeStyleGetMaxWidth(node),
                YGNodeStyleAPI.YGNodeStyleGetMaxWidth(defaultNode));
            AppendYGValueIfNotDefault(style, "max-height",
                YGNodeStyleAPI.YGNodeStyleGetMaxHeight(node),
                YGNodeStyleAPI.YGNodeStyleGetMaxHeight(defaultNode));
            AppendYGValueIfNotDefault(style, "min-width",
                YGNodeStyleAPI.YGNodeStyleGetMinWidth(node),
                YGNodeStyleAPI.YGNodeStyleGetMinWidth(defaultNode));
            AppendYGValueIfNotDefault(style, "min-height",
                YGNodeStyleAPI.YGNodeStyleGetMinHeight(node),
                YGNodeStyleAPI.YGNodeStyleGetMinHeight(defaultNode));
        }

        if ((options & PrintOptions.Config) == PrintOptions.Config)
        {
            var config = YGNodeAPI.YGNodeGetConfig(node)!;
            var defaultConfig = YGConfigAPI.YGConfigGetDefault();

            var configObj = new JsonObject();
            j["config"] = configObj;

            AppendBoolIfNotDefault(configObj, "use-web-defaults",
                YGConfigAPI.YGConfigGetUseWebDefaults(config),
                YGConfigAPI.YGConfigGetUseWebDefaults(defaultConfig));
            AppendFloatIfNotDefault(configObj, "point-scale-factor",
                YGConfigAPI.YGConfigGetPointScaleFactor(config),
                YGConfigAPI.YGConfigGetPointScaleFactor(defaultConfig));

            var errata = YGConfigAPI.YGConfigGetErrata(config);
            var defaultErrata = YGConfigAPI.YGConfigGetErrata(defaultConfig);
            if (errata == YGErrata.None || errata == YGErrata.All ||
                errata == YGErrata.Classic)
            {
                AppendEnumValueIfNotDefault(configObj, "errata",
                    errata.ToStringFast(),
                    defaultErrata.ToStringFast());
            }
        }

        if ((options & PrintOptions.Node) == PrintOptions.Node)
        {
            bool nodeAlwaysForms = YGNodeAPI.YGNodeGetAlwaysFormsContainingBlock(node);
            bool defaultAlwaysForms = YGNodeAPI.YGNodeGetAlwaysFormsContainingBlock(defaultNode);
            if (nodeAlwaysForms != defaultAlwaysForms)
            {
                var nodeObj = new JsonObject
                {
                    ["always-forms-containing-block"] = nodeAlwaysForms,
                };
                j["node"] = nodeObj;
            }
        }

        var childCount = YGNodeAPI.YGNodeGetChildCount(node);
        if ((options & PrintOptions.Children) == PrintOptions.Children &&
            childCount > 0)
        {
            var children = new JsonArray();
            for (nuint i = 0; i < childCount; i++)
            {
                var childNode = YGNodeAPI.YGNodeGetChild(node, i);
                if (childNode == null) continue;
                var childObj = new JsonObject();
                SerializeTreeImpl(childObj, childNode, defaultNode, options);
                children.Add(childObj);
            }
            j["children"] = children;
        }
    }

    public static JsonObject SerializeTree(
        Node node,
        PrintOptions options = PrintOptions.Style | PrintOptions.Children | PrintOptions.Config | PrintOptions.Node)
    {
        var defaultNode = YGNodeAPI.YGNodeNew();
        var j = new JsonObject();
        SerializeTreeImpl(j, node, defaultNode, options);
        YGNodeAPI.YGNodeFree(defaultNode);
        return j;
    }

    public static JsonObject SerializeLayoutInputs(
        float availableWidth,
        float availableHeight,
        YGDirection ownerDirection)
    {
        var j = new JsonObject
        {
            ["owner-direction"] = ownerDirection.ToStringFast(),
        };
        WriteFloatIfNotNaN(j, "available-width", availableWidth);
        WriteFloatIfNotNaN(j, "available-height", availableHeight);
        return j;
    }

    public static string SerializeToJson(
        Node node,
        float availableWidth,
        float availableHeight,
        YGDirection ownerDirection,
        PrintOptions options = PrintOptions.Style | PrintOptions.Children | PrintOptions.Config | PrintOptions.Node)
    {
        var root = new JsonObject();
        root["layout-inputs"] = SerializeLayoutInputs(availableWidth, availableHeight, ownerDirection);

        var treeObj = new JsonObject();
        var defaultNode = YGNodeAPI.YGNodeNew();
        SerializeTreeImpl(treeObj, node, defaultNode, options);
        YGNodeAPI.YGNodeFree(defaultNode);
        root["tree"] = treeObj;

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }

    private static bool YGValueEquals(YGValue a, YGValue b)
    {
        return a.Equals(b);
    }
}
