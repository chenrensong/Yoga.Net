# Yoga.Net

[![NuGet](https://img.shields.io/nuget/v/Yoga.Net.svg)](https://www.nuget.org/packages/Yoga.Net)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

[English](#english) | [中文](#中文)

---

## English

### The Story

When the Claude Code source leaked, the community discovered something interesting — Claude Code uses [Ink](https://github.com/nicksrandall/ink) as its rendering engine, and Ink relies on [Meta's Yoga](https://github.com/facebook/yoga) for layout computation. This sparked a question: what if we used AI to recreate Yoga entirely in C#?

So I did. Yoga.Net is a faithful, line-by-line port of Meta's Yoga layout engine (v3.2.1) from C++ to C# — powered by AI collaboration. Every algorithm, every edge case, every test: 1:1 with the upstream implementation. The result is a production-ready Flexbox + CSS Grid layout engine for .NET, with zero external dependencies, full AOT/NativeAOT support, and 833 tests matching the original gtest suite.

Just as others have rebuilt Claude Code in Rust and Python, I rebuilt the engine that powers its UI — in C#.

### Features

- **1:1 C++ port** — faithful translation of the original C++ Yoga engine (v3.2.1)
- **Flexbox & CSS Grid** — complete Flexbox algorithm and CSS Grid layout support
- **High performance** — zero reflection, zero LINQ, `Span<T>` optimizations, `AggressiveInlining` on hot paths, struct value types for reduced allocations
- **AOT/NativeAOT compatible** — fully trimming-safe, no runtime code generation
- **Multi-target** — supports `net8.0`, `net9.0`, `net10.0`
- **833 tests** — comprehensive test suite mirroring the original C++ gtest tests 1:1 (35 skipped tests match upstream C++ `GTEST_SKIP()`)
- Measure callbacks for integrating with text measurement
- Caching for layout performance
- Deterministic layout (no undefined behavior from rounding)

### Installation

```bash
dotnet add package Yoga.Net
```

### Quick Start

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

### Building from Source

```bash
# Build
dotnet build

# Run tests (xunit.v3 standalone runner)
dotnet run --project tests/Yoga.Net.Tests/Yoga.Net.Tests.csproj

# Pack NuGet
dotnet pack --configuration Release
```

### Benchmarks

Yoga.Net includes a benchmark suite aligned with the upstream C++ `yoga/benchmark` — same test cases, same tree structures, same measure functions. Two modes are available:

#### Benchmark Modes

```bash
# Quick mode — simple stopwatch (no dependencies)
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --simple

# Full mode — BenchmarkDotNet with statistical analysis (requires capture files from yoga repo)
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release

# Filter specific benchmark class
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --filter "*SyntheticBenchmark*"
```

#### Benchmark Suite

| Class | Description | Aligns with |
|---|---|---|
| `SimpleBenchmark` | Quick stopwatch, 1000 iterations | `yoga/benchmark/YGBenchmark.c` |
| `SyntheticBenchmark` | BenchmarkDotNet, JIT + NativeAOT | `yoga/benchmark/YGBenchmark.c` |
| `CaptureBenchmark` | Real-world UI layout trees | `yoga/benchmark/Benchmark.cpp` |

All synthetic benchmarks (Stack with flex, Align stretch, Nested flex, Huge nested layout) are 1:1 ports of the C++ benchmark cases, including measure functions, node counts, and memory cleanup.

#### Results (JIT)

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

### Project Structure

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
├── tests/Yoga.Net.Benchmarks/ # Benchmarks (aligned with yoga/benchmark)
│   ├── SimpleBenchmark.cs     # Quick stopwatch (YGBenchmark.c)
│   ├── SyntheticBenchmark.cs  # BenchmarkDotNet JIT + NativeAOT (YGBenchmark.c)
│   ├── CaptureBenchmark.cs    # Real-world UI layouts (Benchmark.cpp)
│   └── TreeDeserializer.cs    # JSON capture tree deserialization with measure funcs
├── tests/Yoga.Net.Fuzz/       # Fuzz test (port of yoga/fuzz/FuzzLayout.cpp)
└── tests/Yoga.Net.Capture/    # Capture tool (port of yoga/capture/)
    ├── NodeToString.cs        # Tree serialization to JSON (NodeToString.cpp)
    └── CaptureTree.cs         # Layout + capture (CaptureTree.cpp)
```

### Verification

Three tools ported from the upstream C++ yoga project validate the C# implementation:

#### Fuzz Testing (port of `yoga/fuzz/FuzzLayout.cpp`)

Random tree structures to verify no crashes or exceptions:

```bash
dotnet run --project tests/Yoga.Net.Fuzz/Yoga.Net.Fuzz.csproj  # 10,000 rounds
dotnet run --project tests/Yoga.Net.Fuzz/Yoga.Net.Fuzz.csproj -- 100000 42  # custom rounds/seed
```

Result: **10,000/10,000 rounds passed** — zero exceptions.

#### Capture Tool (port of `yoga/capture/`)

Serialize tree state to JSON for debugging and cross-implementation comparison:

```bash
dotnet run --project tests/Yoga.Net.Capture/Yoga.Net.Capture.csproj
dotnet run --project tests/Yoga.Net.Capture/Yoga.Net.Capture.csproj -- output.json
```

Outputs style, layout inputs, children, config, and node properties — only non-default values, matching the C++ JSON structure.

#### Gentest Fixture Coverage

All 25 upstream HTML fixture files (556 individual test cases) are covered:

```bash
# Cross-validated: every fixture test ID has a corresponding C# test method
# 7 missing tests were discovered and ported from C++ generated tests
# Total: 833 tests (626 gentest-derived + 207 unit/integration tests)
```

### Performance Optimizations

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

### API Style

The library exposes two layers:

1. **C-style API** (`YGNodeAPI`, `YGNodeStyleAPI`, `YGNodeLayoutAPI`, `YGConfigAPI`, `YGPixelGridAPI`) — mirrors the original C/C++ Yoga API for 1:1 test compatibility and familiarity for developers coming from other Yoga bindings.
2. **OOP API** (`Node`, `Config`, `Style`) — idiomatic C# naming (PascalCase properties, methods, enums).

Both layers are functionally equivalent — the C-style API delegates to the OOP classes internally.

### Callbacks

#### YGMeasureFunc

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

#### Other Callbacks

```csharp
public delegate float YGBaselineFunc(Node node, float width, float height);
public delegate void YGDirtiedFunc(Node node);
public delegate void YGLogger(Config config, Node node, LogLevel logLevel, string message);
public delegate Node? YGCloneNodeFunc(Node node, Node owner, int childIndex);
```

### Enums

#### Public Enums (YG-prefixed, C-style API)

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

#### Internal Enums (OOP API, no YG prefix)

Each `YG*` enum has a corresponding internal version (`Align`, `Direction`, `Display`, etc.) with identical ordinal values. Convert via extension methods:

```csharp
Align internal = ygAlign.ToInternal();   // YGAlign -> Align
YGAlign back = internal.ToYG();          // Align -> YGAlign
string name = ygAlign.ToStringFast();    // "center" (no reflection)
```

#### Event Enums

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

### Value Types

#### YGSize — Measurement result

```csharp
public struct YGSize { public float Width; public float Height; }
```

#### YGValue — CSS value with unit

```csharp
public struct YGValue : IEquatable<YGValue> { public float Value; public Unit Unit; }

// Predefined constants
YGValue.YGValueZero       // { 0, Point }
YGValue.YGValueUndefined  // { NaN, Undefined }
YGValue.YGValueAuto       // { NaN, Auto }
```

#### FloatOptional — Optional float (NaN = undefined)

```csharp
FloatOptional.Undefined          // NaN
FloatOptional.Zero               // 0.0f
new FloatOptional(3.14f)
opt.IsDefined() / IsUndefined()
opt.Unwrap()                     // Get float (NaN = UB)
opt + other                      // Arithmetic supported
```

### Core Classes

#### Node — Layout node

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

#### Config — Layout configuration

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

#### Style — CSS properties

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

#### LayoutResults — Layout output

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

### C-style Static API

#### YGNodeAPI — Node lifecycle & tree operations

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

#### YGNodeStyleAPI — Style properties

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

#### YGNodeLayoutAPI — Read layout results

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

#### YGConfigAPI — Configuration

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

#### YGPixelGridAPI

```csharp
float YGRoundValueToPixelGrid(double value, double pointScaleFactor, bool forceCeil, bool forceFloor);
```

### CSS Grid Support

#### Grid Container

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

#### Grid Item

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

### Event System

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

### Style Value Types

#### StyleLength — CSS length (position, margin, padding, border, gap)

```csharp
StyleLength.Points(10)     // 10px
StyleLength.Percent(50)    // 50%
StyleLength.OfAuto()       // auto
StyleLength.Undefined()
length.IsAuto() / IsPoints() / IsPercent() / IsDefined()
length.Resolve(referenceLength)  // Percent: value * ref / 100
```

#### StyleSizeLength — CSS size (width, height, flex-basis)

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

#### GridLine — CSS Grid line

```csharp
GridLine.Auto()                  // auto
GridLine.FromInteger(3)          // line number 3
GridLine.Span(2)                 // span 2
line.IsAuto() / IsInteger() / IsSpan()
```

#### GridTrackSize — CSS Grid track

```csharp
GridTrackSize.Auto()
GridTrackSize.Length(100)        // 100px
GridTrackSize.Percent(50)        // 50%
GridTrackSize.Fr(1)              // 1fr
GridTrackSize.MinMax(min, max)   // minmax()
```

### Version Alignment

| Yoga.Net Version | Upstream C++ Yoga Version |
|---|---|
| 3.2.2 | [v3.2.1](https://github.com/facebook/yoga) |

### Acknowledgments

This is a C# port of [facebook/yoga](https://github.com/facebook/yoga) by Meta Platforms, Inc.

### License

[MIT](LICENSE)

---

## 中文

### 起源

Claude Code 源码泄露后，社区发现了一个有趣的细节 — Claude Code 使用 [Ink](https://github.com/nicksrandall/ink) 作为渲染引擎，而 Ink 底层依赖 Meta 的 [Yoga](https://github.com/facebook/yoga) 进行布局计算。这引发了一个想法：如果用 AI 把 Yoga 完整复刻成 C# 会怎样？

于是我做了。Yoga.Net 是 Meta Yoga 布局引擎 (v3.2.1) 从 C++ 到 C# 的逐行忠实移植 — 由 AI 协作完成。每一个算法、每一个边界情况、每一个测试用例都与上游实现 1:1 对齐。最终成果是一个可用于生产环境的 Flexbox + CSS Grid 布局引擎，零外部依赖，完整 AOT/NativeAOT 支持，833 个测试与原始 gtest 套件完全匹配。

就像有人用 Rust 和 Python 重新实现了 Claude Code 一样，我用 C# 重新实现了驱动其 UI 的核心引擎。

### 特性

- **1:1 C++ 移植** — 忠实翻译原始 C++ Yoga 引擎 (v3.2.1)
- **Flexbox 与 CSS Grid** — 完整的 Flexbox 算法和 CSS Grid 布局支持
- **高性能** — 零反射、零 LINQ、`Span<T>` 优化、热路径 `AggressiveInlining`、值类型结构体减少内存分配
- **AOT/NativeAOT 兼容** — 完全支持裁剪，无运行时代码生成
- **多目标框架** — 支持 `net8.0`、`net9.0`、`net10.0`
- **833 个测试** — 与原始 C++ gtest 测试 1:1 对齐的完整测试套件（35 个跳过的测试对应上游 C++ `GTEST_SKIP()`）
- 测量回调，可与文本测量集成
- 布局缓存提升性能
- 确定性布局（舍入无未定义行为）

### 安装

```bash
dotnet add package Yoga.Net
```

### 快速开始

```csharp
using Facebook.Yoga;
using static Facebook.Yoga.YGNodeAPI;
using static Facebook.Yoga.YGNodeStyleAPI;
using static Facebook.Yoga.YGNodeLayoutAPI;

// 创建节点
var root = YGNodeNew();
var child0 = YGNodeNew();
var child1 = YGNodeNew();

// 构建树
YGNodeInsertChild(root, child0, 0);
YGNodeInsertChild(root, child1, 1);

// 设置样式
YGNodeStyleSetWidth(root, 300);
YGNodeStyleSetHeight(root, 200);
YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

YGNodeStyleSetFlexGrow(child0, 1);
YGNodeStyleSetFlexGrow(child1, 2);

// 计算布局
YGNodeCalculateLayout(root, float.NaN, float.NaN, YGDirection.LTR);

// 读取结果
Console.WriteLine($"Root:   {YGNodeLayoutGetWidth(root)} x {YGNodeLayoutGetHeight(root)}");
Console.WriteLine($"Child0: {YGNodeLayoutGetWidth(child0)} x {YGNodeLayoutGetHeight(child0)} @ ({YGNodeLayoutGetLeft(child0)}, {YGNodeLayoutGetTop(child0)})");
Console.WriteLine($"Child1: {YGNodeLayoutGetWidth(child1)} x {YGNodeLayoutGetHeight(child1)} @ ({YGNodeLayoutGetLeft(child1)}, {YGNodeLayoutGetTop(child1)})");

// 清理
YGNodeFreeRecursive(root);
```

输出：
```
Root:   300 x 200
Child0: 100 x 200 @ (0, 0)
Child1: 200 x 200 @ (100, 0)
```

### 从源码构建

```bash
# 构建
dotnet build

# 运行测试 (xunit.v3 独立运行器)
dotnet run --project tests/Yoga.Net.Tests/Yoga.Net.Tests.csproj

# 打包 NuGet
dotnet pack --configuration Release
```

### 基准测试

Yoga.Net 包含与上游 C++ `yoga/benchmark` 对齐的基准测试套件 — 相同的测试用例、相同的树结构、相同的测量函数。提供两种模式：

#### 基准测试模式

```bash
# 快速模式 — 简单计时器（无依赖）
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --simple

# 完整模式 — BenchmarkDotNet 统计分析（需要 yoga 仓库中的 capture 文件）
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release

# 过滤特定基准测试类
dotnet run --project tests/Yoga.Net.Benchmarks/Yoga.Net.Benchmarks.csproj -c Release -- --filter "*SyntheticBenchmark*"
```

#### 基准测试套件

| 类 | 描述 | 对齐 |
|---|---|---|
| `SimpleBenchmark` | 快速计时器，1000 次迭代 | `yoga/benchmark/YGBenchmark.c` |
| `SyntheticBenchmark` | BenchmarkDotNet，JIT + NativeAOT | `yoga/benchmark/YGBenchmark.c` |
| `CaptureBenchmark` | 真实世界 UI 布局树 | `yoga/benchmark/Benchmark.cpp` |

所有合成基准测试（Stack with flex、Align stretch、Nested flex、Huge nested layout）都是 C++ 基准测试用例的 1:1 移植，包括测量函数、节点数和内存清理。

#### 性能结果 (JIT)

**环境：**
- 运行时：.NET 10.0.5 (RyuJIT x86-64-v3)
- 操作系统：Windows 11 (10.0.26200)
- CPU：13th Gen Intel Core i9-13900HX

**SyntheticBenchmark（BenchmarkDotNet，数值越低越好）：**

| 方法 | 平均值 | 内存分配 |
|---|---:|---:|
| Stack with flex | 12.37 us | 43.85 KB |
| Align stretch in undefined axis | 16.87 us | 42.73 KB |
| Nested flex (10x10) | 336.62 us | 651.3 KB |
| Huge nested layout (10,000 nodes) | 62.94 ms | 38.9 MB |

### 项目结构

项目镜像了原始 C++ 源码布局：

```
Yoga.Net/
├── src/Yoga.Net/              # 主库（命名空间：Facebook.Yoga）
│   ├── algorithm/             # 核心布局算法（FlexLine、CalculateLayout、PixelGrid...）
│   ├── config/                # 配置（Config、ExperimentalFeature）
│   ├── debug/                 # 调试/断言工具
│   ├── enums/                 # Flexbox 和 Grid 枚举
│   ├── event/                 # 事件系统
│   ├── node/                  # 节点实现（Node、LayoutResults、CachedMeasurement）
│   ├── numeric/               # 数值工具（FloatOptional、Comparison）
│   ├── style/                 # 样式属性（Style、StyleLength、StyleSizeLength）
│   ├── YGNode.cs              # 公共 C 风格 Node API
│   ├── YGNodeStyle.cs         # 公共 C 风格 Style API
│   ├── YGNodeLayout.cs        # 公共 C 风格 Layout API
│   ├── YGConfig.cs            # 公共 C 风格 Config API
│   └── YGEnums.cs             # 公共 YG 前缀枚举
├── tests/Yoga.Net.Tests/      # xUnit v3 测试（与 C++ gtest 1:1）
├── tests/Yoga.Net.Benchmarks/ # 基准测试（与 yoga/benchmark 对齐）
│   ├── SimpleBenchmark.cs     # 快速计时器（YGBenchmark.c）
│   ├── SyntheticBenchmark.cs  # BenchmarkDotNet JIT + NativeAOT（YGBenchmark.c）
│   ├── CaptureBenchmark.cs    # 真实世界 UI 布局（Benchmark.cpp）
│   └── TreeDeserializer.cs    # JSON capture 树反序列化（含测量函数）
├── tests/Yoga.Net.Fuzz/       # 模糊测试（移植自 yoga/fuzz/FuzzLayout.cpp）
└── tests/Yoga.Net.Capture/    # 捕获工具（移植自 yoga/capture/）
    ├── NodeToString.cs        # 树序列化为 JSON（NodeToString.cpp）
    └── CaptureTree.cs         # 布局 + 捕获（CaptureTree.cpp）
```

### 验证

从上游 C++ yoga 项目移植了三个验证工具来验证 C# 实现的正确性：

#### 模糊测试（移植自 `yoga/fuzz/FuzzLayout.cpp`）

随机树结构验证不会崩溃或抛出异常：

```bash
dotnet run --project tests/Yoga.Net.Fuzz/Yoga.Net.Fuzz.csproj  # 默认 10,000 轮
dotnet run --project tests/Yoga.Net.Fuzz/Yoga.Net.Fuzz.csproj -- 100000 42  # 自定义轮数/种子
```

结果：**10,000/10,000 轮通过** — 零异常。

#### 捕获工具（移植自 `yoga/capture/`）

将树状态序列化为 JSON，用于调试和跨实现对比：

```bash
dotnet run --project tests/Yoga.Net.Capture/Yoga.Net.Capture.csproj
dotnet run --project tests/Yoga.Net.Capture/Yoga.Net.Capture.csproj -- output.json
```

输出样式、布局输入、子节点、配置和节点属性 — 仅输出非默认值，与 C++ JSON 结构一致。

#### Gentest Fixture 覆盖

所有 25 个上游 HTML fixture 文件（556 个独立测试用例）均已覆盖：

```bash
# 交叉验证：每个 fixture 测试 ID 都有对应的 C# 测试方法
# 发现并移植了 7 个遗漏的测试（来自 C++ 生成测试）
# 总计：833 个测试（626 个 gentest 派生 + 207 个单元/集成测试）
```

### 性能优化

与朴素 C# 移植相比，Yoga.Net 包含以下优化：

| 优化 | 描述 |
|---|---|
| `[Flags]` 枚举位域 | `ExperimentalFeature` 使用位标志替代 `HashSet<T>` |
| 值类型结构体 | `FlexLineRunningLayout`、`FloatOptional`、`CachedMeasurement` 均为结构体 |
| `IEquatable<T>` | 避免结构体相等比较时的装箱 |
| `AggressiveInlining` | 热路径方法（`FloatOptional`、`Comparison` 等）强制内联 |
| `Span<T>` / `stackalloc` | `LayoutableChildren.Iterator` 和 `Event.PublishCore` 使用栈分配缓冲区 |
| 零 LINQ | 无 LINQ 使用 — 所有迭代均为手动循环 |
| 零反射 | 无 `System.Reflection` 使用 — 完全 AOT 兼容 |

### API 风格

库暴露两层 API：

1. **C 风格 API**（`YGNodeAPI`、`YGNodeStyleAPI`、`YGNodeLayoutAPI`、`YGConfigAPI`、`YGPixelGridAPI`）— 镜像原始 C/C++ Yoga API，实现 1:1 测试兼容，便于从其他 Yoga 绑定迁移的开发者使用。
2. **面向对象 API**（`Node`、`Config`、`Style`）— 符合 C# 惯例的命名（PascalCase 属性、方法、枚举）。

两层 API 功能完全等价 — C 风格 API 内部委托给面向对象类。

### 回调

#### YGMeasureFunc

叶节点（文本、图片）的自定义测量回调。设置后自动将 `NodeType` 改为 `Text` 并禁止添加子节点。

```csharp
public delegate YGSize YGMeasureFunc(
    Node node,
    float availableWidth,
    MeasureMode widthMode,    // Undefined / Exactly / AtMost
    float availableHeight,
    MeasureMode heightMode);  // Undefined / Exactly / AtMost
```

- `Undefined` — 父节点未约束此轴，测量固有尺寸
- `Exactly` — 父节点已确定此轴的精确值
- `AtMost` — 父节点为此轴指定了上限

#### 其他回调

```csharp
public delegate float YGBaselineFunc(Node node, float width, float height);
public delegate void YGDirtiedFunc(Node node);
public delegate void YGLogger(Config config, Node node, LogLevel logLevel, string message);
public delegate Node? YGCloneNodeFunc(Node node, Node owner, int childIndex);
```

### 枚举

#### 公共枚举（YG 前缀，C 风格 API）

| 枚举 | 值 | CSS 属性 |
|---|---|---|
| `YGAlign` | Auto, FlexStart, Center, FlexEnd, Stretch, Baseline, SpaceBetween, SpaceAround, SpaceEvenly, Start, End | align-* |
| `YGBoxSizing` | BorderBox, ContentBox | box-sizing |
| `YGDimension` | Width, Height | — |
| `YGDirection` | Inherit, LTR, RTL | direction |
| `YGDisplay` | Flex, None, Contents, Grid | display |
| `YGEdge` | Left, Top, Right, Bottom, Start, End, Horizontal, Vertical, All | margin/padding/border/position |
| `YGErrata` | [Flags] None, StretchFlexBasis, AbsolutePositionWithoutInsetsExcludesPadding, AbsolutePercentAgainstInnerSize, All, Classic | 兼容性标志 |
| `YGExperimentalFeature` | WebFlexBasis, FixFlexBasisFitContent | 实验性功能开关 |
| `YGFlexDirection` | Column, ColumnReverse, Row, RowReverse | flex-direction |
| `YGGridTrackType` | Auto, Points, Percent, Fr, Minmax | grid-template 轨道类型 |
| `YGGutter` | Column, Row, All | gap 方向 |
| `YGJustify` | Auto, FlexStart, Center, FlexEnd, SpaceBetween, SpaceAround, SpaceEvenly, Stretch, Start, End | justify-* |
| `YGLogLevel` | Error, Warn, Info, Debug, Verbose, Fatal | 日志级别 |
| `YGMeasureMode` | Undefined, Exactly, AtMost | 测量模式 |
| `YGNodeType` | Default, Text | 节点类型 |
| `YGOverflow` | Visible, Hidden, Scroll | overflow |
| `YGPositionType` | Static, Relative, Absolute | position |
| `YGUnit` | Undefined, Point, Percent, Auto, MaxContent, FitContent, Stretch | CSS 值单位 |
| `YGWrap` | NoWrap, Wrap, WrapReverse | flex-wrap |

#### 内部枚举（面向对象 API，无 YG 前缀）

每个 `YG*` 枚举都有对应的内部版本（`Align`、`Direction`、`Display` 等），序数值完全一致。通过扩展方法转换：

```csharp
Align internal = ygAlign.ToInternal();   // YGAlign -> Align
YGAlign back = internal.ToYG();          // Align -> YGAlign
string name = ygAlign.ToStringFast();    // "center"（无反射）
```

### 值类型

#### YGSize — 测量结果

```csharp
public struct YGSize { public float Width; public float Height; }
```

#### YGValue — 带 CSS 单位的值

```csharp
public struct YGValue : IEquatable<YGValue> { public float Value; public Unit Unit; }

// 预定义常量
YGValue.YGValueZero       // { 0, Point }
YGValue.YGValueUndefined  // { NaN, Undefined }
YGValue.YGValueAuto       // { NaN, Auto }
```

#### FloatOptional — 可选浮点数（NaN = 未定义）

```csharp
FloatOptional.Undefined          // NaN
FloatOptional.Zero               // 0.0f
new FloatOptional(3.14f)
opt.IsDefined() / IsUndefined()
opt.Unwrap()                     // 获取 float（NaN 为未定义行为）
opt + other                      // 支持算术运算
```

### 核心类

#### Node — 布局节点

```csharp
public class Node {
    // 构造函数
    public Node();
    public Node(Config? config);

    // 属性
    public Config Config { get; }
    public LayoutResults Layout { get; set; }
    public Style Style { get; }
    public bool HasNewLayout { get; set; }

    // 树操作
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

    // 布局
    public void CalculateLayout(float availableWidth, float availableHeight, Direction ownerDirection);
    public bool IsDirty();
    public void MarkDirtyAndPropagate();
    public LayoutResults GetLayout();

    // 回调
    public void SetMeasureFunc(YGMeasureFunc? measureFunc);  // 自动设置 NodeType
    public bool HasMeasureFunc();
    public YGSize Measure(float availableWidth, MeasureMode widthMode, float availableHeight, MeasureMode heightMode);
    public void SetBaselineFunc(YGBaselineFunc? baseLineFunc);
    public bool HasBaselineFunc();
    public void SetDirtiedFunc(YGDirtiedFunc? dirtiedFunc);

    // 配置和上下文
    public void SetConfig(Config? config);
    public Config? GetConfig();
    public void SetContext(object? context);
    public object? GetContext();

    // 节点类型
    public void SetNodeType(NodeType nodeType);
    public NodeType GetNodeType();

    // 克隆和重置
    public void MoveFrom(Node other);  // 深拷贝
    public void Reset();

    // Flex 解析
    public FloatOptional ResolveFlexGrow();
    public FloatOptional ResolveFlexShrink();
    public bool IsNodeFlexible();
}
```

#### Config — 布局配置

```csharp
public class Config {
    public static Config Default { get; }

    public void SetUseWebDefaults(bool);       // FlexDirection=Row, FlexShrink=1
    public bool UseWebDefaults();
    public void SetPointScaleFactor(float);    // 像素网格对齐
    public float GetPointScaleFactor();
    public void SetErrata(Errata);             // 兼容性标志
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
    public uint GetVersion();                  // 配置变更时递增
}
```

#### Style — CSS 属性

```csharp
public class Style {
    // 布局
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

    // 尺寸
    StyleSizeLength Dimension(Dimension dim);      // width / height
    StyleSizeLength MinDimension(Dimension dim);   // min-width / min-height
    StyleSizeLength MaxDimension(Dimension dim);   // max-width / max-height
    void SetDimension(Dimension dim, StyleSizeLength value);
    void SetMinDimension(Dimension dim, StyleSizeLength value);
    void SetMaxDimension(Dimension dim, StyleSizeLength value);

    // 间距（按 Edge）
    StyleLength Position(Edge edge), Margin(Edge edge), Padding(Edge edge), Border(Edge edge);
    void SetPosition(Edge edge, StyleLength value);
    void SetMargin(Edge edge, StyleLength value);
    void SetPadding(Edge edge, StyleLength value);
    void SetBorder(Edge edge, StyleLength value);

    // 间距（按 Gutter）
    StyleLength Gap(Gutter gutter);
    void SetGap(Gutter gutter, StyleLength value);

    // 其他
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

    // 默认值
    public const float DefaultFlexGrow = 0.0f, DefaultFlexShrink = 0.0f;
    public const float WebDefaultFlexGrow = 0.0f, WebDefaultFlexShrink = 1.0f;
}
```

#### LayoutResults — 布局输出

```csharp
public class LayoutResults {
    float Position(PhysicalEdge edge);        // Left / Top / Right / Bottom
    float Dimension(Dimension axis);           // 最终计算尺寸
    float MeasuredDimension(Dimension axis);   // 测量尺寸
    float RawDimension(Dimension axis);        // 未缩放的原始尺寸
    Direction GetDirection();
    bool HadOverflow();
    float Margin(PhysicalEdge edge);
    float Border(PhysicalEdge edge);
    float Padding(PhysicalEdge edge);
}
```

### C 风格静态 API

API 文档详见上方[英文部分](#c-style-static-api)，所有 C 风格 API 在中英文中共享相同的签名和用法。

### CSS Grid 支持

#### Grid 容器

```csharp
var grid = YGNodeAPI.YGNodeNew();
YGNodeStyleAPI.YGNodeStyleSetDisplay(grid, YGDisplay.Grid);

// 定义模板列：100px 1fr auto
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnsCount(grid, 3);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 0, YGGridTrackType.Points, 100);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 1, YGGridTrackType.Fr, 1);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 2, YGGridTrackType.Auto, 0);

// 或使用 minmax()
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnMinMax(grid, 1,
    YGGridTrackType.Points, 100, YGGridTrackType.Fr, 1);

// 定义自动行（隐式网格）
YGNodeStyleAPI.YGNodeStyleSetGridAutoRowsCount(grid, 1);
YGNodeStyleAPI.YGNodeStyleSetGridAutoRow(grid, 0, YGGridTrackType.Points, 50);
```

#### Grid 项目

```csharp
var item = YGNodeAPI.YGNodeNew();
// 放置到第 1-2 列，第 1-2 行（1 索引）
YGNodeStyleAPI.YGNodeStyleSetGridColumnStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridColumnEnd(item, 2);
YGNodeStyleAPI.YGNodeStyleSetGridRowStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridRowEnd(item, 2);
// 或使用 span
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartSpan(item, 2);  // span 2
// 或自动放置
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartAuto(item);
```

### 事件系统

```csharp
Event.Subscribe((node, eventType, data) => {
    if (eventType == EventType.LayoutPassEnd) {
        var ld = data.GetData<Event.LayoutPassEndData>();
        Console.WriteLine($"Layouts: {ld?.LayoutData?.Layouts}");
    }
});

// 事件类型：NodeAllocation、NodeDeallocation、NodeLayout、
//   LayoutPassStart、LayoutPassEnd、MeasureCallbackStart/End、
//   NodeBaselineStart/End

// 线程安全（lock + ThreadLocal 缓冲区）
Event.Unsubscribe(subscriber);
Event.Reset();
```

### 样式值类型

#### StyleLength — CSS 长度（position、margin、padding、border、gap）

```csharp
StyleLength.Points(10)     // 10px
StyleLength.Percent(50)    // 50%
StyleLength.OfAuto()       // auto
StyleLength.Undefined()
length.IsAuto() / IsPoints() / IsPercent() / IsDefined()
length.Resolve(referenceLength)  // 百分比：value * ref / 100
```

#### StyleSizeLength — CSS 尺寸（width、height、flex-basis）

扩展 StyleLength，增加 `MaxContent`、`FitContent`、`Stretch` 单位。

```csharp
StyleSizeLength.Points(100)
StyleSizeLength.Percent(50)
StyleSizeLength.OfAuto()
StyleSizeLength.OfMaxContent()
StyleSizeLength.OfFitContent()
StyleSizeLength.OfStretch(1)
size.Resolve(referenceLength)
```

#### GridLine — CSS Grid 线

```csharp
GridLine.Auto()                  // auto
GridLine.FromInteger(3)          // 线号 3
GridLine.Span(2)                 // span 2
line.IsAuto() / IsInteger() / IsSpan()
```

#### GridTrackSize — CSS Grid 轨道

```csharp
GridTrackSize.Auto()
GridTrackSize.Length(100)        // 100px
GridTrackSize.Percent(50)        // 50%
GridTrackSize.Fr(1)              // 1fr
GridTrackSize.MinMax(min, max)   // minmax()
```

### 版本对齐

| Yoga.Net 版本 | 上游 C++ Yoga 版本 |
|---|---|
| 3.2.2 | [v3.2.1](https://github.com/facebook/yoga) |

### 致谢

本项目是 [facebook/yoga](https://github.com/facebook/yoga)（Meta Platforms, Inc.）的 C# 移植版本。

### 许可证

[MIT](LICENSE)
