using System.Text.Json;
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;

namespace Yoga.Net.Benchmarks;

public static class TreeDeserializer
{
    public static Node BuildTree(JsonElement treeElement)
    {
        return BuildNode(treeElement, null, 0);
    }

    private static Node BuildNode(JsonElement element, Node? parent, int index)
    {
        var config = BuildConfig(element.GetProperty("config"));
        var node = YGNodeNewWithConfig(config);

        if (parent != null)
        {
            YGNodeInsertChild(parent, node, (nuint)index);
        }

        BuildNodeState(element, node);
        if (element.TryGetProperty("style", out var styleElement) && styleElement.ValueKind == JsonValueKind.Object)
        {
            SetStyles(styleElement, node);
        }

        if (element.TryGetProperty("children", out var children))
        {
            int childIndex = 0;
            foreach (var child in children.EnumerateArray())
            {
                BuildNode(child, node, childIndex++);
            }
        }

        return node;
    }

    private static Config BuildConfig(JsonElement configElement)
    {
        var config = new Config();

        if (configElement.ValueKind != JsonValueKind.Object)
        {
            return config;
        }

        foreach (var property in configElement.EnumerateObject())
        {
            switch (property.Name)
            {
                case "use-web-defaults":
                    config.SetUseWebDefaults(property.Value.GetBoolean());
                    break;
                case "point-scale-factor":
                    config.SetPointScaleFactor(property.Value.GetSingle());
                    break;
                case "errata":
                    config.SetErrata(ParseErrata(property.Value.GetString()!));
                    break;
                case "experimental-features":
                    foreach (var feature in property.Value.EnumerateArray())
                    {
                        config.SetExperimentalFeatureEnabled(
                            ParseExperimentalFeature(feature.GetString()!), true);
                    }
                    break;
            }
        }

        return config;
    }

    private static void BuildNodeState(JsonElement element, Node node)
    {
        if (!element.TryGetProperty("node", out var nodeState) || nodeState.ValueKind == JsonValueKind.Null)
        {
            return;
        }

        foreach (var property in nodeState.EnumerateObject())
        {
            switch (property.Name)
            {
                case "always-forms-containing-block":
                    node.SetAlwaysFormsContainingBlock(property.Value.GetBoolean());
                    break;
                case "measure-funcs":
                    var entries = new List<SerializedMeasureFunc>();
                    foreach (var mf in property.Value.EnumerateArray())
                    {
                        entries.Add(ParseMeasureFunc(mf));
                    }

                    if (entries.Count > 0)
                    {
                        var captured = entries.ToArray();
                        node.SetMeasureFunc((n, w, wm, h, hm) =>
                            MockMeasureFunc(captured, w, wm, h, hm));
                    }
                    break;
            }
        }
    }

    // Matches Benchmark.cpp:54-89 mockMeasureFunc
    private static YGSize MockMeasureFunc(
        SerializedMeasureFunc[] entries,
        float width,
        MeasureMode widthMode,
        float height,
        MeasureMode heightMode)
    {
        foreach (var entry in entries)
        {
            if (InputsMatch(
                    width, entry.InputWidth,
                    height, entry.InputHeight,
                    widthMode, entry.WidthMode,
                    heightMode, entry.HeightMode))
            {
                if (entry.DurationNs >= 1_000_000)
                {
                    Thread.Sleep((int)(entry.DurationNs / 1_000_000));
                }

                return new YGSize { Width = entry.OutputWidth, Height = entry.OutputHeight };
            }
        }

        // Matches Benchmark.cpp:49-52 defaultMeasureFunctionResult
        return new YGSize { Width = 10, Height = 10 };
    }

    // Matches Benchmark.cpp:28-47 inputsMatch
    private static bool InputsMatch(
        float actualWidth,
        float expectedWidth,
        float actualHeight,
        float expectedHeight,
        MeasureMode actualWidthMode,
        MeasureMode expectedWidthMode,
        MeasureMode actualHeightMode,
        MeasureMode expectedHeightMode)
    {
        bool widthsMatch =
            (float.IsNaN(actualWidth) && float.IsNaN(expectedWidth)) ||
            actualWidth == expectedWidth;
        bool heightsMatch =
            (float.IsNaN(actualHeight) && float.IsNaN(expectedHeight)) ||
            actualHeight == expectedHeight;

        return widthsMatch &&
               heightsMatch &&
               actualWidthMode == expectedWidthMode &&
               actualHeightMode == expectedHeightMode;
    }

    // Matches TreeDeserialization.cpp:238-247 serializedMeasureFuncFromJson
    private static SerializedMeasureFunc ParseMeasureFunc(JsonElement j)
    {
        float inputWidth = j.TryGetProperty("width", out var w) && w.ValueKind != JsonValueKind.Null
            ? w.GetSingle()
            : float.NaN;
        var widthMode = ParseMeasureMode(j.GetProperty("width-mode").GetString()!);
        float inputHeight = j.TryGetProperty("height", out var h) && h.ValueKind != JsonValueKind.Null
            ? h.GetSingle()
            : float.NaN;
        var heightMode = ParseMeasureMode(j.GetProperty("height-mode").GetString()!);
        float outputWidth = j.GetProperty("output-width").GetSingle();
        float outputHeight = j.GetProperty("output-height").GetSingle();
        long durationNs = j.GetProperty("duration-ns").GetInt64();

        return new SerializedMeasureFunc(
            inputWidth, widthMode,
            inputHeight, heightMode,
            outputWidth, outputHeight,
            durationNs);
    }

    private static void SetStyles(JsonElement styleElement, Node node)
    {
        if (styleElement.ValueKind != JsonValueKind.Object)
        {
            return;
        }
        foreach (var property in styleElement.EnumerateObject())
        {
            ApplyStyle(node, property.Name, property.Value);
        }
    }

    private static void ApplyStyle(Node node, string key, JsonElement value)
    {
        switch (key)
        {
            case "flex-direction":
                YGNodeStyleSetFlexDirection(node, ParseFlexDirection(value.GetString()!));
                break;
            case "justify-content":
                YGNodeStyleSetJustifyContent(node, ParseJustifyContent(value.GetString()!));
                break;
            case "align-items":
                YGNodeStyleSetAlignItems(node, ParseAlign(value.GetString()!));
                break;
            case "align-content":
                YGNodeStyleSetAlignContent(node, ParseAlign(value.GetString()!));
                break;
            case "align-self":
                YGNodeStyleSetAlignSelf(node, ParseAlign(value.GetString()!));
                break;
            case "flex-wrap":
                YGNodeStyleSetFlexWrap(node, ParseWrap(value.GetString()!));
                break;
            case "overflow":
                YGNodeStyleSetOverflow(node, ParseOverflow(value.GetString()!));
                break;
            case "display":
                YGNodeStyleSetDisplay(node, ParseDisplay(value.GetString()!));
                break;
            case "position-type":
                YGNodeStyleSetPositionType(node, ParsePositionType(value.GetString()!));
                break;
            case "flex-grow":
                YGNodeStyleSetFlexGrow(node, value.GetSingle());
                break;
            case "flex-shrink":
                YGNodeStyleSetFlexShrink(node, value.GetSingle());
                break;
            case "flex":
                YGNodeStyleSetFlex(node, value.GetSingle());
                break;
            case "flex-basis":
                ApplyFlexBasis(node, value);
                break;
            case "gap":
                ApplyGap(node, YGGutter.All, value);
                break;
            case "column-gap":
                ApplyGap(node, YGGutter.Column, value);
                break;
            case "row-gap":
                ApplyGap(node, YGGutter.Row, value);
                break;
            case "aspect-ratio":
                YGNodeStyleSetAspectRatio(node, value.GetSingle());
                break;
            default:
                if (key.StartsWith("position-"))
                {
                    var edge = ParseEdge(key["position-".Length..]);
                    ApplyPosition(node, edge, value);
                }
                else if (key.StartsWith("padding-"))
                {
                    var edge = ParseEdge(key["padding-".Length..]);
                    ApplyPadding(node, edge, value);
                }
                else if (key.StartsWith("border-"))
                {
                    var edge = ParseEdge(key["border-".Length..]);
                    ApplyBorder(node, edge, value);
                }
                else if (key.StartsWith("margin-"))
                {
                    var edge = ParseEdge(key["margin-".Length..]);
                    ApplyMargin(node, edge, value);
                }
                else if (key == "width")
                {
                    ApplyDimension(node, Dimension.Width, value);
                }
                else if (key == "height")
                {
                    ApplyDimension(node, Dimension.Height, value);
                }
                else if (key == "min-width")
                {
                    ApplyMinDimension(node, Dimension.Width, value);
                }
                else if (key == "min-height")
                {
                    ApplyMinDimension(node, Dimension.Height, value);
                }
                else if (key == "max-width")
                {
                    ApplyMaxDimension(node, Dimension.Width, value);
                }
                else if (key == "max-height")
                {
                    ApplyMaxDimension(node, Dimension.Height, value);
                }
                break;
        }
    }

    private static void ApplyFlexBasis(Node node, JsonElement value)
    {
        var unit = ParseUnit(value);
        switch (unit)
        {
            case YGUnit.Auto:
                YGNodeStyleSetFlexBasisAuto(node);
                break;
            case YGUnit.Point:
                YGNodeStyleSetFlexBasis(node, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Percent:
                YGNodeStyleSetFlexBasisPercent(node, value.GetProperty("value").GetSingle());
                break;
        }
    }

    private static void ApplyPosition(Node node, YGEdge edge, JsonElement value)
    {
        var unit = ParseUnit(value);
        switch (unit)
        {
            case YGUnit.Point:
                YGNodeStyleSetPosition(node, edge, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Percent:
                YGNodeStyleSetPositionPercent(node, edge, value.GetProperty("value").GetSingle());
                break;
        }
    }

    private static void ApplyPadding(Node node, YGEdge edge, JsonElement value)
    {
        var unit = ParseUnit(value);
        switch (unit)
        {
            case YGUnit.Point:
                YGNodeStyleSetPadding(node, edge, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Percent:
                YGNodeStyleSetPaddingPercent(node, edge, value.GetProperty("value").GetSingle());
                break;
        }
    }

    private static void ApplyBorder(Node node, YGEdge edge, JsonElement value)
    {
        var unit = ParseUnit(value);
        if (unit == YGUnit.Point)
        {
            YGNodeStyleSetBorder(node, edge, value.GetProperty("value").GetSingle());
        }
    }

    private static void ApplyMargin(Node node, YGEdge edge, JsonElement value)
    {
        var unit = ParseUnit(value);
        switch (unit)
        {
            case YGUnit.Point:
                YGNodeStyleSetMargin(node, edge, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Percent:
                YGNodeStyleSetMarginPercent(node, edge, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Auto:
                YGNodeStyleSetMarginAuto(node, edge);
                break;
        }
    }

    private static void ApplyGap(Node node, YGGutter gutter, JsonElement value)
    {
        var unit = ParseUnit(value);
        if (unit == YGUnit.Point)
        {
            YGNodeStyleSetGap(node, gutter, value.GetProperty("value").GetSingle());
        }
    }

    private static void ApplyDimension(Node node, Dimension dimension, JsonElement value)
    {
        var unit = ParseUnit(value);
        switch (unit)
        {
            case YGUnit.Auto:
                if (dimension == Dimension.Width)
                    YGNodeStyleSetWidthAuto(node);
                else
                    YGNodeStyleSetHeightAuto(node);
                break;
            case YGUnit.Point:
                if (dimension == Dimension.Width)
                    YGNodeStyleSetWidth(node, value.GetProperty("value").GetSingle());
                else
                    YGNodeStyleSetHeight(node, value.GetProperty("value").GetSingle());
                break;
            case YGUnit.Percent:
                if (dimension == Dimension.Width)
                    YGNodeStyleSetWidthPercent(node, value.GetProperty("value").GetSingle());
                else
                    YGNodeStyleSetHeightPercent(node, value.GetProperty("value").GetSingle());
                break;
        }
    }

    private static void ApplyMinDimension(Node node, Dimension dimension, JsonElement value)
    {
        var unit = ParseUnit(value);
        if (unit == YGUnit.Point)
        {
            if (dimension == Dimension.Width)
                YGNodeStyleSetMinWidth(node, value.GetProperty("value").GetSingle());
            else
                YGNodeStyleSetMinHeight(node, value.GetProperty("value").GetSingle());
        }
        else if (unit == YGUnit.Percent)
        {
            if (dimension == Dimension.Width)
                YGNodeStyleSetMinWidthPercent(node, value.GetProperty("value").GetSingle());
            else
                YGNodeStyleSetMinHeightPercent(node, value.GetProperty("value").GetSingle());
        }
    }

    private static void ApplyMaxDimension(Node node, Dimension dimension, JsonElement value)
    {
        var unit = ParseUnit(value);
        if (unit == YGUnit.Point)
        {
            if (dimension == Dimension.Width)
                YGNodeStyleSetMaxWidth(node, value.GetProperty("value").GetSingle());
            else
                YGNodeStyleSetMaxHeight(node, value.GetProperty("value").GetSingle());
        }
        else if (unit == YGUnit.Percent)
        {
            if (dimension == Dimension.Width)
                YGNodeStyleSetMaxWidthPercent(node, value.GetProperty("value").GetSingle());
            else
                YGNodeStyleSetMaxHeightPercent(node, value.GetProperty("value").GetSingle());
        }
    }

    // Parsers

    // Matches TreeDeserialization.cpp:226-236 measureModeFromString
    private static MeasureMode ParseMeasureMode(string str) => str switch
    {
        "undefined" => MeasureMode.Undefined,
        "exactly" => MeasureMode.Exactly,
        "at-most" => MeasureMode.AtMost,
        _ => throw new ArgumentException($"Invalid measure mode: {str}")
    };

    // Matches TreeDeserialization.cpp:214-224 directionFromString
    internal static YGDirection ParseDirection(string str) => str switch
    {
        "ltr" => YGDirection.LTR,
        "rtl" => YGDirection.RTL,
        "inherit" => YGDirection.Inherit,
        _ => throw new ArgumentException($"Invalid direction: {str}")
    };

    private static YGFlexDirection ParseFlexDirection(string str) => str switch
    {
        "row" => YGFlexDirection.Row,
        "row-reverse" => YGFlexDirection.RowReverse,
        "column" => YGFlexDirection.Column,
        "column-reverse" => YGFlexDirection.ColumnReverse,
        _ => throw new ArgumentException($"Invalid flex-direction: {str}")
    };

    private static YGJustify ParseJustifyContent(string str) => str switch
    {
        "flex-start" => YGJustify.FlexStart,
        "center" => YGJustify.Center,
        "flex-end" => YGJustify.FlexEnd,
        "space-between" => YGJustify.SpaceBetween,
        "space-around" => YGJustify.SpaceAround,
        "space-evenly" => YGJustify.SpaceEvenly,
        _ => throw new ArgumentException($"Invalid justify-content: {str}")
    };

    private static YGAlign ParseAlign(string str) => str switch
    {
        "auto" => YGAlign.Auto,
        "flex-start" => YGAlign.FlexStart,
        "center" => YGAlign.Center,
        "flex-end" => YGAlign.FlexEnd,
        "stretch" => YGAlign.Stretch,
        "baseline" => YGAlign.Baseline,
        "space-between" => YGAlign.SpaceBetween,
        "space-around" => YGAlign.SpaceAround,
        "space-evenly" => YGAlign.SpaceEvenly,
        _ => throw new ArgumentException($"Invalid align: {str}")
    };

    private static YGWrap ParseWrap(string str) => str switch
    {
        "no-wrap" => YGWrap.NoWrap,
        "wrap" => YGWrap.Wrap,
        "wrap-reverse" => YGWrap.WrapReverse,
        _ => throw new ArgumentException($"Invalid flex-wrap: {str}")
    };

    private static YGOverflow ParseOverflow(string str) => str switch
    {
        "visible" => YGOverflow.Visible,
        "hidden" => YGOverflow.Hidden,
        "scroll" => YGOverflow.Scroll,
        _ => throw new ArgumentException($"Invalid overflow: {str}")
    };

    private static YGDisplay ParseDisplay(string str) => str switch
    {
        "flex" => YGDisplay.Flex,
        "none" => YGDisplay.None,
        _ => throw new ArgumentException($"Invalid display: {str}")
    };

    private static YGPositionType ParsePositionType(string str) => str switch
    {
        "static" => YGPositionType.Static,
        "relative" => YGPositionType.Relative,
        "absolute" => YGPositionType.Absolute,
        _ => throw new ArgumentException($"Invalid position-type: {str}")
    };

    private static YGEdge ParseEdge(string str) => str switch
    {
        "left" => YGEdge.Left,
        "top" => YGEdge.Top,
        "right" => YGEdge.Right,
        "bottom" => YGEdge.Bottom,
        "start" => YGEdge.Start,
        "end" => YGEdge.End,
        "horizontal" => YGEdge.Horizontal,
        "vertical" => YGEdge.Vertical,
        "all" => YGEdge.All,
        _ => throw new ArgumentException($"Invalid edge: {str}")
    };

    private static YGUnit ParseUnit(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.String)
        {
            var str = value.GetString();
            return str switch
            {
                "auto" => YGUnit.Auto,
                "undefined" => YGUnit.Undefined,
                _ => throw new ArgumentException($"Invalid unit string: {str}")
            };
        }

        if (value.TryGetProperty("unit", out var unitElement))
        {
            var unit = unitElement.GetString();
            return unit switch
            {
                "px" => YGUnit.Point,
                "pct" => YGUnit.Percent,
                _ => throw new ArgumentException($"Invalid unit: {unit}")
            };
        }

        return YGUnit.Undefined;
    }

    private static Errata ParseErrata(string str) => str switch
    {
        "none" => Errata.None,
        "all" => Errata.All,
        "classic" => Errata.Classic,
        _ => Errata.None
    };

    private static ExperimentalFeature ParseExperimentalFeature(string str) => str switch
    {
        "web-flex-basis" => ExperimentalFeature.WebFlexBasis,
        _ => throw new ArgumentException($"Invalid experimental feature: {str}")
    };
}

// Matches capture/CaptureTree.h SerializedMeasureFunc
public record struct SerializedMeasureFunc(
    float InputWidth,
    MeasureMode WidthMode,
    float InputHeight,
    MeasureMode HeightMode,
    float OutputWidth,
    float OutputHeight,
    long DurationNs);
