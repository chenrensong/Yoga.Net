namespace YogaSharp;

public enum YGAlign
{
    Auto,
    FlexStart,
    Center,
    FlexEnd,
    Stretch,
    Baseline,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly,
    Start,
    End
}

public enum YGBoxSizing
{
    BorderBox,
    ContentBox
}

public enum YGDimension
{
    Width,
    Height
}

public enum YGDirection
{
    Inherit,
    LTR,
    RTL
}

public enum YGDisplay
{
    Flex,
    None,
    Contents,
    Grid
}

public enum YGEdge
{
    Left,
    Top,
    Right,
    Bottom,
    Start,
    End,
    Horizontal,
    Vertical,
    All
}

[System.Flags]
public enum YGErrata
{
    None = 0,
    StretchFlexBasis = 1,
    AbsolutePositionWithoutInsetsExcludesPadding = 2,
    AbsolutePercentAgainstInnerSize = 4,
    All = 2147483647,
    Classic = 2147483646
}

public enum YGExperimentalFeature
{
    WebFlexBasis,
    FixFlexBasisFitContent
}

public enum YGFlexDirection
{
    Column,
    ColumnReverse,
    Row,
    RowReverse
}

public enum YGGridTrackType
{
    Auto,
    Points,
    Percent,
    Fr,
    Minmax
}

public enum YGGutter
{
    Column,
    Row,
    All
}

public enum YGJustify
{
    Auto,
    FlexStart,
    Center,
    FlexEnd,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly,
    Stretch,
    Start,
    End
}

public enum YGLogLevel
{
    Error,
    Warn,
    Info,
    Debug,
    Verbose,
    Fatal
}

public enum YGMeasureMode
{
    Undefined,
    Exactly,
    AtMost
}

public enum YGNodeType
{
    Default,
    Text
}

public enum YGOverflow
{
    Visible,
    Hidden,
    Scroll
}

public enum YGPositionType
{
    Static,
    Relative,
    Absolute
}

public enum YGUnit
{
    Undefined,
    Point,
    Percent,
    Auto,
    MaxContent,
    FitContent,
    Stretch
}

public enum YGWrap
{
    NoWrap,
    Wrap,
    WrapReverse
}

public static class YGEnumExtensions
{
    public static string ToStringFast(this YGAlign value)
    {
        return value switch
        {
            YGAlign.Auto => "auto",
            YGAlign.FlexStart => "flex-start",
            YGAlign.Center => "center",
            YGAlign.FlexEnd => "flex-end",
            YGAlign.Stretch => "stretch",
            YGAlign.Baseline => "baseline",
            YGAlign.SpaceBetween => "space-between",
            YGAlign.SpaceAround => "space-around",
            YGAlign.SpaceEvenly => "space-evenly",
            YGAlign.Start => "start",
            YGAlign.End => "end",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGBoxSizing value)
    {
        return value switch
        {
            YGBoxSizing.BorderBox => "border-box",
            YGBoxSizing.ContentBox => "content-box",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGDimension value)
    {
        return value switch
        {
            YGDimension.Width => "width",
            YGDimension.Height => "height",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGDirection value)
    {
        return value switch
        {
            YGDirection.Inherit => "inherit",
            YGDirection.LTR => "ltr",
            YGDirection.RTL => "rtl",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGDisplay value)
    {
        return value switch
        {
            YGDisplay.Flex => "flex",
            YGDisplay.None => "none",
            YGDisplay.Contents => "contents",
            YGDisplay.Grid => "grid",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGEdge value)
    {
        return value switch
        {
            YGEdge.Left => "left",
            YGEdge.Top => "top",
            YGEdge.Right => "right",
            YGEdge.Bottom => "bottom",
            YGEdge.Start => "start",
            YGEdge.End => "end",
            YGEdge.Horizontal => "horizontal",
            YGEdge.Vertical => "vertical",
            YGEdge.All => "all",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGErrata value)
    {
        return value switch
        {
            YGErrata.None => "none",
            YGErrata.StretchFlexBasis => "stretch-flex-basis",
            YGErrata.AbsolutePositionWithoutInsetsExcludesPadding => "absolute-position-without-insets-excludes-padding",
            YGErrata.AbsolutePercentAgainstInnerSize => "absolute-percent-against-inner-size",
            YGErrata.All => "all",
            YGErrata.Classic => "classic",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGExperimentalFeature value)
    {
        return value switch
        {
            YGExperimentalFeature.WebFlexBasis => "web-flex-basis",
            YGExperimentalFeature.FixFlexBasisFitContent => "fix-flex-basis-fit-content",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGFlexDirection value)
    {
        return value switch
        {
            YGFlexDirection.Column => "column",
            YGFlexDirection.ColumnReverse => "column-reverse",
            YGFlexDirection.Row => "row",
            YGFlexDirection.RowReverse => "row-reverse",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGGridTrackType value)
    {
        return value switch
        {
            YGGridTrackType.Auto => "auto",
            YGGridTrackType.Points => "points",
            YGGridTrackType.Percent => "percent",
            YGGridTrackType.Fr => "fr",
            YGGridTrackType.Minmax => "minmax",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGGutter value)
    {
        return value switch
        {
            YGGutter.Column => "column",
            YGGutter.Row => "row",
            YGGutter.All => "all",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGJustify value)
    {
        return value switch
        {
            YGJustify.Auto => "auto",
            YGJustify.FlexStart => "flex-start",
            YGJustify.Center => "center",
            YGJustify.FlexEnd => "flex-end",
            YGJustify.SpaceBetween => "space-between",
            YGJustify.SpaceAround => "space-around",
            YGJustify.SpaceEvenly => "space-evenly",
            YGJustify.Stretch => "stretch",
            YGJustify.Start => "start",
            YGJustify.End => "end",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGLogLevel value)
    {
        return value switch
        {
            YGLogLevel.Error => "error",
            YGLogLevel.Warn => "warn",
            YGLogLevel.Info => "info",
            YGLogLevel.Debug => "debug",
            YGLogLevel.Verbose => "verbose",
            YGLogLevel.Fatal => "fatal",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGMeasureMode value)
    {
        return value switch
        {
            YGMeasureMode.Undefined => "undefined",
            YGMeasureMode.Exactly => "exactly",
            YGMeasureMode.AtMost => "at-most",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGNodeType value)
    {
        return value switch
        {
            YGNodeType.Default => "default",
            YGNodeType.Text => "text",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGOverflow value)
    {
        return value switch
        {
            YGOverflow.Visible => "visible",
            YGOverflow.Hidden => "hidden",
            YGOverflow.Scroll => "scroll",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGPositionType value)
    {
        return value switch
        {
            YGPositionType.Static => "static",
            YGPositionType.Relative => "relative",
            YGPositionType.Absolute => "absolute",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGUnit value)
    {
        return value switch
        {
            YGUnit.Undefined => "undefined",
            YGUnit.Point => "point",
            YGUnit.Percent => "percent",
            YGUnit.Auto => "auto",
            YGUnit.MaxContent => "max-content",
            YGUnit.FitContent => "fit-content",
            YGUnit.Stretch => "stretch",
            _ => "unknown"
        };
    }

    public static string ToStringFast(this YGWrap value)
    {
        return value switch
        {
            YGWrap.NoWrap => "no-wrap",
            YGWrap.Wrap => "wrap",
            YGWrap.WrapReverse => "wrap-reverse",
            _ => "unknown"
        };
    }
}

