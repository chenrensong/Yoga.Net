# Yoga.Net

[![NuGet](https://img.shields.io/nuget/v/Yoga.Net.svg)](https://www.nuget.org/packages/Yoga.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A high-performance C# port of [Meta's Yoga](https://github.com/facebook/yoga) layout engine — implements **Flexbox** and **CSS Grid** layout for .NET.

Yoga enables building UI layouts using Flexbox and CSS Grid on platforms that don't natively support them, including .NET desktop, mobile, and server applications.

## Features

- **1:1 C++ port** — faithful translation of the original C++ Yoga engine (v3.2.1)
- **Flexbox & CSS Grid** — complete Flexbox algorithm and CSS Grid layout support
- **High performance** — zero reflection, zero LINQ, `Span<T>` optimizations, `AggressiveInlining` on hot paths, struct value types for reduced allocations
- **AOT/NativeAOT compatible** — fully trimming-safe, no runtime code generation
- **Multi-target** — supports `net8.0`, `net9.0`, `net10.0`
- **826 tests** — comprehensive test suite mirroring the original C++ gtest tests 1:1 (35 skipped tests match upstream C++ `GTEST_SKIP()`)
- Measure callbacks for integrating with text measurement
- Caching for layout performance
- Deterministic layout (no undefined behavior from rounding)

## Installation

```bash
dotnet add package Yoga.Net
```

## Quick Start

```csharp
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;

// Create nodes
var root = YGNodeNew();
var child0 = YGNodeNew();
var child1 = YGNodeNew();

// Build tree
YGNodeInsertChild(root, child0, 0);
YGNodeInsertChild(root, child1, 1);

// Set styles
YGNodeStyleSetWidth(root, 300);
YGNodeStyleSetHeight(root, 200);
YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

YGNodeStyleSetFlexGrow(child0, 1);
YGNodeStyleSetFlexGrow(child1, 2);

// Calculate layout
YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

// Read results
Console.WriteLine($"Root:   {YGNodeLayoutGetWidth(root)} x {YGNodeLayoutGetHeight(root)}");
Console.WriteLine($"Child0: {YGNodeLayoutGetWidth(child0)} x {YGNodeLayoutGetHeight(child0)} @ ({YGNodeLayoutGetLeft(child0)}, {YGNodeLayoutGetTop(child0)})");
Console.WriteLine($"Child1: {YGNodeLayoutGetWidth(child1)} x {YGNodeLayoutGetHeight(child1)} @ ({YGNodeLayoutGetLeft(child1)}, {YGNodeLayoutGetTop(child1)})");

// Clean up
YGNodeFreeRecursive(root);
```

Output:
```
Root:   300 x 200
Child0: 100 x 200 @ (0, 0)
Child1: 200 x 200 @ (100, 0)
```

## Building from Source

```bash
# Build
dotnet build

# Run tests (xunit.v3 standalone runner)
dotnet run --project tests/Yoga.Net.Tests/Yoga.Net.Tests.csproj

# Pack NuGet
dotnet pack --configuration Release
```

## Benchmarks

Yoga.Net includes a benchmark suite aligned with the upstream C++ `yoga/benchmark` — same test cases, same tree structures, same measure functions. Two modes are available:

### Benchmark Modes

```bash
# Quick mode — simple stopwatch (no dependencies)
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --simple

# Full mode — BenchmarkDotNet with statistical analysis (requires capture files from yoga repo)
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release

# Filter specific benchmark class
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --filter "*SyntheticBenchmark*"
```

### Benchmark Suite

| Class | Description | Aligns with |
|---|---|---|
| `SimpleBenchmark` | Quick stopwatch, 1000 iterations | `yoga/benchmark/YGBenchmark.c` |
| `SyntheticBenchmark` | BenchmarkDotNet, JIT + NativeAOT | `yoga/benchmark/YGBenchmark.c` |
| `CaptureBenchmark` | Real-world UI layout trees | `yoga/benchmark/Benchmark.cpp` |

All synthetic benchmarks (Stack with flex, Align stretch, Nested flex, Huge nested layout) are 1:1 ports of the C++ benchmark cases, including measure functions, node counts, and memory cleanup.

### Results (JIT)

**Environment:**
- Runtime: .NET 10.0.5 (RyuJIT x86-64-v3)
- OS: Windows 11 (10.0.26200)
- CPU: 13th Gen Intel Core i9-13900HX

**SyntheticBenchmark (BenchmarkDotNet, lower is better):**

| Method | Mean | Allocated |
|---|---:|---:|
| Stack with flex | 12.37 us | 43.85 KB |
| Align stretch in undefined axis | 16.87 us | 42.73 KB |
| Nested flex (10x10) | 336.62 us | 651.3 KB |
| Huge nested layout (10,000 nodes) | 62.94 ms | 38.9 MB |

## Project Structure

The project mirrors the original C++ source layout:

```
Yoga.Net/
├── src/Yoga.Net/              # Main library (namespace: Facebook.Yoga)
│   ├── algorithm/             # Core layout algorithms (FlexLine, CalculateLayout, PixelGrid...)
│   ├── config/                # Configuration (Config, ExperimentalFeature)
│   ├── debug/                 # Debug/assertion utilities
│   ├── enums/                 # Flexbox & Grid enumerations
│   ├── event/                 # Event system
│   ├── node/                  # Node implementation (Node, LayoutResults, CachedMeasurement)
│   ├── numeric/               # Numeric utilities (FloatOptional, Comparison)
│   ├── style/                 # Style properties (Style, StyleLength, StyleSizeLength)
│   ├── YGNode.cs              # Public C-style Node API
│   ├── YGNodeStyle.cs         # Public C-style Style API
│   ├── YGNodeLayout.cs        # Public C-style Layout API
│   ├── YGConfig.cs            # Public C-style Config API
│   └── YGEnums.cs             # Public YG-prefixed enums
├── tests/Yoga.Net.Tests/      # xUnit v3 tests (1:1 with C++ gtest)
└── tests/Yoga.Net.Benchmarks/ # Benchmarks (aligned with yoga/benchmark)
    ├── SimpleBenchmark.cs     # Quick stopwatch (YGBenchmark.c)
    ├── SyntheticBenchmark.cs  # BenchmarkDotNet JIT + NativeAOT (YGBenchmark.c)
    ├── CaptureBenchmark.cs    # Real-world UI layouts (Benchmark.cpp)
    └── TreeDeserializer.cs    # JSON capture tree deserialization with measure funcs
```

## Performance Optimizations

Compared to a naive C# port, Yoga.Net includes the following optimizations:

| Optimization | Description |
|---|---|
| `[Flags]` enum bit fields | `ExperimentalFeature` uses bit flags instead of `HashSet<T>` |
| Value-type structs | `FlexLineRunningLayout`, `FloatOptional`, `CachedMeasurement` are structs |
| `IEquatable<T>` | Avoids boxing in equality checks for structs |
| `AggressiveInlining` | Hot-path methods (`FloatOptional`, `Comparison`, etc.) are inlined |
| `Span<T>` / `stackalloc` | Stack-allocated buffers in `LayoutableChildren.Iterator` and `Event.PublishCore` |
| Zero LINQ | No LINQ usage — all iterations are manual loops |
| Zero reflection | No `System.Reflection` usage — fully AOT compatible |

## API Style

The library exposes two layers:

1. **C-style API** (`YGNodeAPI`, `YGNodeStyleAPI`, `YGNodeLayoutAPI`, `YGConfigAPI`, `YGPixelGridAPI`) — mirrors the original C/C++ Yoga API for 1:1 test compatibility and familiarity for developers coming from other Yoga bindings.
2. **OOP API** (`Node`, `Config`, `Style`) — idiomatic C# naming (PascalCase properties, methods, enums).

Both layers are functionally equivalent — the C-style API delegates to the OOP classes internally.

## Callbacks

### YGMeasureFunc

Custom measurement callback for leaf nodes (text, images). Setting this automatically changes `NodeType` to `Text` and disallows children.

```csharp
public delegate YGSize YGMeasureFunc(
    Node node,
    float availableWidth,
    MeasureMode widthMode,    // Undefined / Exactly / AtMost
    float availableHeight,
    MeasureMode heightMode);  // Undefined / Exactly / AtMost
```

- `Undefined` — parent didn't constrain this axis, measure intrinsic size
- `Exactly` — parent determined the exact value for this axis
- `AtMost` — parent specified an upper bound for this axis

### Other Callbacks

```csharp
public delegate float YGBaselineFunc(Node node, float width, float height);
public delegate void YGDirtiedFunc(Node node);
public delegate void YGLogger(Config config, Node node, LogLevel logLevel, string message);
public delegate Node? YGCloneNodeFunc(Node node, Node owner, int childIndex);
```

## Enums

### Public Enums (YG-prefixed, C-style API)

| Enum | Values | CSS Property |
|---|---|---|
| `YGAlign` | Auto, FlexStart, Center, FlexEnd, Stretch, Baseline, SpaceBetween, SpaceAround, SpaceEvenly, Start, End | align-* |
| `YGBoxSizing` | BorderBox, ContentBox | box-sizing |
| `YGDimension` | Width, Height | — |
| `YGDirection` | Inherit, LTR, RTL | direction |
| `YGDisplay` | Flex, None, Contents, Grid | display |
| `YGEdge` | Left, Top, Right, Bottom, Start, End, Horizontal, Vertical, All | margin/padding/border/position |
| `YGErrata` | [Flags] None, StretchFlexBasis, AbsolutePositionWithoutInsetsExcludesPadding, AbsolutePercentAgainstInnerSize, All, Classic | Compatibility flags |
| `YGExperimentalFeature` | WebFlexBasis, FixFlexBasisFitContent | Experimental feature toggles |
| `YGFlexDirection` | Column, ColumnReverse, Row, RowReverse | flex-direction |
| `YGGridTrackType` | Auto, Points, Percent, Fr, Minmax | grid-template track type |
| `YGGutter` | Column, Row, All | gap direction |
| `YGJustify` | Auto, FlexStart, Center, FlexEnd, SpaceBetween, SpaceAround, SpaceEvenly, Stretch, Start, End | justify-* |
| `YGLogLevel` | Error, Warn, Info, Debug, Verbose, Fatal | Log level |
| `YGMeasureMode` | Undefined, Exactly, AtMost | Measure mode |
| `YGNodeType` | Default, Text | Node type |
| `YGOverflow` | Visible, Hidden, Scroll | overflow |
| `YGPositionType` | Static, Relative, Absolute | position |
| `YGUnit` | Undefined, Point, Percent, Auto, MaxContent, FitContent, Stretch | CSS value unit |
| `YGWrap` | NoWrap, Wrap, WrapReverse | flex-wrap |

### Internal Enums (OOP API, no YG prefix)

Each `YG*` enum has a corresponding internal version (`Align`, `Direction`, `Display`, etc.) with identical ordinal values. Convert via extension methods:

```csharp
Align internal = ygAlign.ToInternal();   // YGAlign -> Align
YGAlign back = internal.ToYG();          // Align -> YGAlign
string name = ygAlign.ToStringFast();    // "center" (no reflection)
```

### Event Enums

```csharp
enum EventType {
    NodeAllocation, NodeDeallocation, NodeLayout,
    LayoutPassStart, LayoutPassEnd,
    MeasureCallbackStart, MeasureCallbackEnd,
    NodeBaselineStart, NodeBaselineEnd
}
enum LayoutType { Layout, Measure, CachedLayout, CachedMeasure }
enum LayoutPassReason {
    Initial, AbsLayout, Stretch, MultilineStretch,
    FlexLayout, MeasureChild, AbsMeasureChild, FlexMeasure, GridLayout
}
```

## Value Types

### YGSize — Measurement result

```csharp
public struct YGSize { public float Width; public float Height; }
```

### YGValue — CSS value with unit

```csharp
public struct YGValue : IEquatable<YGValue> { public float Value; public Unit Unit; }

// Predefined constants
YGValue.YGValueZero       // { 0, Point }
YGValue.YGValueUndefined  // { NaN, Undefined }
YGValue.YGValueAuto       // { NaN, Auto }
```

### FloatOptional — Optional float (NaN = undefined)

```csharp
FloatOptional.Undefined          // NaN
FloatOptional.Zero               // 0.0f
new FloatOptional(3.14f)
opt.IsDefined() / IsUndefined()
opt.Unwrap()                     // Get float (NaN = UB)
opt + other                      // Arithmetic supported
```

## Core Classes

### Node — Layout node

```csharp
public class Node {
    // Constructors
    public Node();
    public Node(Config? config);

    // Properties
    public Config Config { get; }
    public LayoutResults Layout { get; set; }
    public Style Style { get; }
    public bool HasNewLayout { get; set; }

    // Tree operations
    public void InsertChild(Node child, nuint index);
    public void ReplaceChild(Node oldChild, Node newChild);
    public bool RemoveChild(Node child);
    public void RemoveChild(nuint index);
    public void ClearChildren();
    public void SetChildren(IReadOnlyList<Node> children);
    public Node? GetChild(nuint index);
    public nuint GetChildCount();
    public Node? GetOwner();
    public IReadOnlyList<Node> GetChildren();

    // Layout
    public void CalculateLayout(float availableWidth, float availableHeight, Direction ownerDirection);
    public bool IsDirty();
    public void MarkDirtyAndPropagate();
    public LayoutResults GetLayout();

    // Callbacks
    public void SetMeasureFunc(YGMeasureFunc? measureFunc);  // auto-sets NodeType
    public bool HasMeasureFunc();
    public YGSize Measure(float availableWidth, MeasureMode widthMode, float availableHeight, MeasureMode heightMode);
    public void SetBaselineFunc(YGBaselineFunc? baseLineFunc);
    public bool HasBaselineFunc();
    public void SetDirtiedFunc(YGDirtiedFunc? dirtiedFunc);

    // Config & context
    public void SetConfig(Config? config);
    public Config? GetConfig();
    public void SetContext(object? context);
    public object? GetContext();

    // Node type
    public void SetNodeType(NodeType nodeType);
    public NodeType GetNodeType();

    // Clone & reset
    public void MoveFrom(Node other);  // deep copy
    public void Reset();

    // Flex resolution
    public FloatOptional ResolveFlexGrow();
    public FloatOptional ResolveFlexShrink();
    public bool IsNodeFlexible();
}
```

### Config — Layout configuration

```csharp
public class Config {
    public static Config Default { get; }

    public void SetUseWebDefaults(bool);       // FlexDirection=Row, FlexShrink=1
    public bool UseWebDefaults();
    public void SetPointScaleFactor(float);    // pixel grid alignment
    public float GetPointScaleFactor();
    public void SetErrata(Errata);             // compatibility flags
    public void AddErrata(Errata);
    public void RemoveErrata(Errata);
    public Errata GetErrata();
    public bool HasErrata(Errata);
    public void SetExperimentalFeatureEnabled(ExperimentalFeature, bool);
    public bool IsExperimentalFeatureEnabled(ExperimentalFeature);
    public void SetLogger(YGLogger);
    public void SetCloneNodeCallback(YGCloneNodeFunc?);
    public Node CloneNode(Node node, Node owner, int childIndex);
    public void SetContext(object?);
    public uint GetVersion();                  // incremented on config change
}
```

### Style — CSS properties

```csharp
public class Style {
    // Layout
    public Direction Direction;
    public FlexDirection FlexDirection;
    public Justify JustifyContent, JustifyItems, JustifySelf;
    public Align AlignContent, AlignItems, AlignSelf;
    public Display Display;
    public PositionType PositionType;
    public Overflow Overflow;
    public FlexWrap FlexWrap;
    public BoxSizing BoxSizing;

    // Flex
    public FloatOptional Flex, FlexGrow, FlexShrink;
    public StyleSizeLength FlexBasis;

    // Dimensions
    StyleSizeLength Dimension(Dimension dim);      // width / height
    StyleSizeLength MinDimension(Dimension dim);   // min-width / min-height
    StyleSizeLength MaxDimension(Dimension dim);   // max-width / max-height
    void SetDimension(Dimension dim, StyleSizeLength value);
    void SetMinDimension(Dimension dim, StyleSizeLength value);
    void SetMaxDimension(Dimension dim, StyleSizeLength value);

    // Spacing (by Edge)
    StyleLength Position(Edge edge), Margin(Edge edge), Padding(Edge edge), Border(Edge edge);
    void SetPosition(Edge edge, StyleLength value);
    void SetMargin(Edge edge, StyleLength value);
    void SetPadding(Edge edge, StyleLength value);
    void SetBorder(Edge edge, StyleLength value);

    // Gap (by Gutter)
    StyleLength Gap(Gutter gutter);
    void SetGap(Gutter gutter, StyleLength value);

    // Other
    public FloatOptional AspectRatio;

    // Grid
    public GridLine GridColumnStart, GridColumnEnd, GridRowStart, GridRowEnd;
    GridTrackList GridTemplateColumns { get; }
    GridTrackList GridTemplateRows { get; }
    GridTrackList GridAutoColumns { get; }
    GridTrackList GridAutoRows { get; }
    void SetGridTemplateColumnAt(int index, GridTrackSize value);
    void SetGridTemplateRowAt(int index, GridTrackSize value);
    void SetGridAutoColumnAt(int index, GridTrackSize value);
    void SetGridAutoRowAt(int index, GridTrackSize value);
    void ResizeGridTemplateColumns(int count);
    void ResizeGridTemplateRows(int count);
    void ResizeGridAutoColumns(int count);
    void ResizeGridAutoRows(int count);

    // Defaults
    public const float DefaultFlexGrow = 0.0f, DefaultFlexShrink = 0.0f;
    public const float WebDefaultFlexGrow = 0.0f, WebDefaultFlexShrink = 1.0f;
}
```

### LayoutResults — Layout output

```csharp
public class LayoutResults {
    float Position(PhysicalEdge edge);        // Left / Top / Right / Bottom
    float Dimension(Dimension axis);           // final computed size
    float MeasuredDimension(Dimension axis);   // measured size
    float RawDimension(Dimension axis);        // unscaled raw size
    Direction GetDirection();
    bool HadOverflow();
    float Margin(PhysicalEdge edge);
    float Border(PhysicalEdge edge);
    float Padding(PhysicalEdge edge);
}
```

## C-style Static API

### YGNodeAPI — Node lifecycle & tree operations

```csharp
Node YGNodeNew();
Node YGNodeNewWithConfig(Config config);
Node YGNodeClone(Node oldNode);
void YGNodeFree(Node node);              // remove from parent, clear children
void YGNodeFreeRecursive(Node root);
void YGNodeReset(Node node);

void YGNodeCalculateLayout(Node node, float availableWidth, float availableHeight, YGDirection ownerDirection);
// Pass float.NaN for unconstrained dimensions

void YGNodeInsertChild(Node owner, Node child, nuint index);
void YGNodeSwapChild(Node owner, Node child, nuint index);
void YGNodeRemoveChild(Node owner, Node child);
void YGNodeRemoveAllChildren(Node owner);
void YGNodeSetChildren(Node owner, Node[] children);
Node? YGNodeGetChild(Node node, nuint index);
nuint YGNodeGetChildCount(Node node);
Node? YGNodeGetOwner(Node node);

void YGNodeSetConfig(Node node, Config config);
Config YGNodeGetConfig(Node node);
void YGNodeSetContext(Node node, object? context);
object? YGNodeGetContext(Node node);

void YGNodeSetMeasureFunc(Node node, YGMeasureFunc? measureFunc);
bool YGNodeHasMeasureFunc(Node node);
void YGNodeSetBaselineFunc(Node node, YGBaselineFunc? baselineFunc);
bool YGNodeHasBaselineFunc(Node node);
void YGNodeSetDirtiedFunc(Node node, YGDirtiedFunc? dirtiedFunc);

void YGNodeSetNodeType(Node node, YGNodeType nodeType);
bool YGNodeIsDirty(Node node);
bool YGNodeGetHasNewLayout(Node node);
```

### YGNodeStyleAPI — Style properties

All Set methods auto-call `node.MarkDirtyAndPropagate()` on value change.

```csharp
// Enum properties
void YGNodeStyleSetDirection(Node node, YGDirection value);
void YGNodeStyleSetFlexDirection(Node node, YGFlexDirection value);
void YGNodeStyleSetJustifyContent/Items/Self(Node node, YGJustify value);
void YGNodeStyleSetAlignContent/Items/Self(Node node, YGAlign value);
void YGNodeStyleSetPositionType(Node node, YGPositionType value);
void YGNodeStyleSetFlexWrap(Node node, YGWrap value);
void YGNodeStyleSetOverflow(Node node, YGOverflow value);
void YGNodeStyleSetDisplay(Node node, YGDisplay value);
void YGNodeStyleSetBoxSizing(Node node, YGBoxSizing value);
// Corresponding Get methods for each...

// Flex
void YGNodeStyleSetFlex(Node node, float flex);
void YGNodeStyleSetFlexGrow(Node node, float flexGrow);
void YGNodeStyleSetFlexShrink(Node node, float flexShrink);
void YGNodeStyleSetFlexBasis(Node node, float flexBasis);
void YGNodeStyleSetFlexBasisPercent(Node node, float percent);
void YGNodeStyleSetFlexBasisAuto(Node node);

// Position (by Edge)
void YGNodeStyleSetPosition(Node node, YGEdge edge, float points);
void YGNodeStyleSetPositionPercent(Node node, YGEdge edge, float percent);
void YGNodeStyleSetPositionAuto(Node node, YGEdge edge);
YGValue YGNodeStyleGetPosition(Node node, YGEdge edge);

// Margin (by Edge)
void YGNodeStyleSetMargin(Node node, YGEdge edge, float points);
void YGNodeStyleSetMarginPercent(Node node, YGEdge edge, float percent);
void YGNodeStyleSetMarginAuto(Node node, YGEdge edge);
YGValue YGNodeStyleGetMargin(Node node, YGEdge edge);

// Padding (by Edge)
void YGNodeStyleSetPadding(Node node, YGEdge edge, float points);
void YGNodeStyleSetPaddingPercent(Node node, YGEdge edge, float percent);
YGValue YGNodeStyleGetPadding(Node node, YGEdge edge);

// Border (by Edge, points only)
void YGNodeStyleSetBorder(Node node, YGEdge edge, float border);
float YGNodeStyleGetBorder(Node node, YGEdge edge);

// Gap (by Gutter)
void YGNodeStyleSetGap(Node node, YGGutter gutter, float gapLength);
void YGNodeStyleSetGapPercent(Node node, YGGutter gutter, float percent);
YGValue YGNodeStyleGetGap(Node node, YGGutter gutter);

// AspectRatio
void YGNodeStyleSetAspectRatio(Node node, float aspectRatio);
float YGNodeStyleGetAspectRatio(Node node);

// Dimensions — each has Set/Percent/Auto/MaxContent/FitContent/Stretch variants
// Width, Height: all variants
// MinWidth, MinHeight: no Auto
// MaxWidth, MaxHeight: no Auto
void YGNodeStyleSetWidth(Node node, float points);
void YGNodeStyleSetWidthPercent(Node node, float percent);
void YGNodeStyleSetWidthAuto(Node node);
YGValue YGNodeStyleGetWidth(Node node);
// ... same pattern for Height, MinWidth, MinHeight, MaxWidth, MaxHeight

// Grid Items
void YGNodeStyleSetGridColumnStart(Node node, int value);
void YGNodeStyleSetGridColumnStartAuto(Node node);
void YGNodeStyleSetGridColumnStartSpan(Node node, int span);
// GridColumnEnd, GridRowStart, GridRowEnd — same pattern

// Grid Container
void YGNodeStyleSetGridTemplateColumnsCount(Node node, int count);
void YGNodeStyleSetGridTemplateColumn(Node node, int index, YGGridTrackType type, float value);
void YGNodeStyleSetGridTemplateColumnMinMax(Node node, int index,
    YGGridTrackType minType, float minValue, YGGridTrackType maxType, float maxValue);
// GridTemplateRows, GridAutoColumns, GridAutoRows — same pattern
```

### YGNodeLayoutAPI — Read layout results

Use after calling `YGNodeCalculateLayout()`.

```csharp
float YGNodeLayoutGetLeft/Top/Right/Bottom(Node node);
float YGNodeLayoutGetWidth/Height(Node node);
float YGNodeLayoutGetRawWidth/RawHeight(Node node);
YGDirection YGNodeLayoutGetDirection(Node node);
bool YGNodeLayoutGetHadOverflow(Node node);
float YGNodeLayoutGetMargin(Node node, YGEdge edge);    // auto Start/End -> Left/Right
float YGNodeLayoutGetBorder(Node node, YGEdge edge);
float YGNodeLayoutGetPadding(Node node, YGEdge edge);
```

### YGConfigAPI — Configuration

```csharp
Config YGConfigNew();
Config YGConfigGetDefault();
void YGConfigSetUseWebDefaults(Config config, bool enabled);
void YGConfigSetPointScaleFactor(Config config, float pixelsInPoint);
void YGConfigSetErrata(Config config, YGErrata errata);
void YGConfigSetLogger(Config config, YGLogger? logger);
void YGConfigSetContext(Config config, object? context);
void YGConfigSetExperimentalFeatureEnabled(Config config, YGExperimentalFeature feature, bool enabled);
bool YGConfigIsExperimentalFeatureEnabled(Config config, YGExperimentalFeature feature);
void YGConfigSetCloneNodeFunc(Config config, YGCloneNodeFunc? callback);
```

### YGPixelGridAPI

```csharp
float YGRoundValueToPixelGrid(double value, double pointScaleFactor, bool forceCeil, bool forceFloor);
```

## CSS Grid Support

### Grid Container

```csharp
var grid = YGNodeAPI.YGNodeNew();
YGNodeStyleAPI.YGNodeStyleSetDisplay(grid, YGDisplay.Grid);

// Define template columns: 100px 1fr auto
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnsCount(grid, 3);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 0, YGGridTrackType.Points, 100);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 1, YGGridTrackType.Fr, 1);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 2, YGGridTrackType.Auto, 0);

// Or with minmax()
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnMinMax(grid, 1,
    YGGridTrackType.Points, 100, YGGridTrackType.Fr, 1);

// Define auto rows (implicit grid)
YGNodeStyleAPI.YGNodeStyleSetGridAutoRowsCount(grid, 1);
YGNodeStyleAPI.YGNodeStyleSetGridAutoRow(grid, 0, YGGridTrackType.Points, 50);
```

### Grid Item

```csharp
var item = YGNodeAPI.YGNodeNew();
// Place at column 1-2, row 1-2 (1-indexed)
YGNodeStyleAPI.YGNodeStyleSetGridColumnStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridColumnEnd(item, 2);
YGNodeStyleAPI.YGNodeStyleSetGridRowStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridRowEnd(item, 2);
// Or use span
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartSpan(item, 2);  // span 2
// Or auto placement
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartAuto(item);
```

## Event System

```csharp
Event.Subscribe((node, eventType, data) => {
    if (eventType == EventType.LayoutPassEnd) {
        var ld = data.GetData<Event.LayoutPassEndData>();
        Console.WriteLine($"Layouts: {ld?.LayoutData?.Layouts}");
    }
});

// Event types: NodeAllocation, NodeDeallocation, NodeLayout,
//   LayoutPassStart, LayoutPassEnd, MeasureCallbackStart/End,
//   NodeBaselineStart/End

// Thread-safe with lock + ThreadLocal buffer
Event.Unsubscribe(subscriber);
Event.Reset();
```

## Style Value Types

### StyleLength — CSS length (position, margin, padding, border, gap)

```csharp
StyleLength.Points(10)     // 10px
StyleLength.Percent(50)    // 50%
StyleLength.OfAuto()       // auto
StyleLength.Undefined()
length.IsAuto() / IsPoints() / IsPercent() / IsDefined()
length.Resolve(referenceLength)  // Percent: value * ref / 100
```

### StyleSizeLength — CSS size (width, height, flex-basis)

Extends StyleLength with `MaxContent`, `FitContent`, `Stretch` units.

```csharp
StyleSizeLength.Points(100)
StyleSizeLength.Percent(50)
StyleSizeLength.OfAuto()
StyleSizeLength.OfMaxContent()
StyleSizeLength.OfFitContent()
StyleSizeLength.OfStretch(1)
size.Resolve(referenceLength)
```

### GridLine — CSS Grid line

```csharp
GridLine.Auto()                  // auto
GridLine.FromInteger(3)          // line number 3
GridLine.Span(2)                 // span 2
line.IsAuto() / IsInteger() / IsSpan()
```

### GridTrackSize — CSS Grid track

```csharp
GridTrackSize.Auto()
GridTrackSize.Length(100)        // 100px
GridTrackSize.Percent(50)        // 50%
GridTrackSize.Fr(1)              // 1fr
GridTrackSize.MinMax(min, max)   // minmax()
```

## Version Alignment

| Yoga.Net Version | Upstream C++ Yoga Version |
|---|---|
| 3.2.1 | [v3.2.1](https://github.com/facebook/yoga) |

## Acknowledgments

This is a C# port of [facebook/yoga](https://github.com/facebook/yoga) by Meta Platforms, Inc.

## License

[MIT](LICENSE)
