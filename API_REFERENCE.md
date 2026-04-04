# Yoga.Net API Reference

> Meta Yoga 布局引擎的 .NET 移植版（v3.2.1），实现 Flexbox + CSS Grid 布局算法。
> 命名空间：`Facebook.Yoga`，NuGet 包：`Yoga.Net`
> 目标框架：net8.0 / net9.0 / net10.0，零外部依赖，支持 AOT/NativeAOT。

---

## 目录

1. [项目概览](#1-项目概览)
2. [快速上手](#2-快速上手)
3. [委托类型（回调）](#3-委托类型回调)
4. [枚举类型](#4-枚举类型)
5. [值类型（Struct）](#5-值类型struct)
6. [核心类](#6-核心类)
7. [C 风格静态 API（YG 前缀）](#7-c-风格静态-apiyg-前缀)
8. [事件系统](#8-事件系统)
9. [样式值类型详解](#9-样式值类型详解)
10. [CSS Grid 支持](#10-css-grid-支持)
11. [内部架构](#11-内部架构)
12. [设计决策与注意事项](#12-设计决策与注意事项)

---

## 1. 项目概览

Yoga.Net 是 Meta [Yoga](https://github.com/facebook/yoga) C++ 布局引擎的 1:1 C# 移植。用于计算 UI 元素的布局（位置、尺寸），支持 Flexbox 和 CSS Grid。

### 解决方案结构

```
Yoga.Net.sln
├── src/Yoga.Net/              # 主库（类库，零依赖）
├── tests/Yoga.Net.Tests/      # xUnit v3 测试（826 个测试）
└── tests/Yoga.Net.Benchmarks/ # BenchmarkDotNet 性能测试
```

### 源码结构

```
src/Yoga.Net/
├── YGNode.cs          # C 风格节点 API（YGNodeAPI）
├── YGNodeStyle.cs     # C 风格样式 API（YGNodeStyleAPI）
├── YGNodeLayout.cs    # C 风格布局结果 API（YGNodeLayoutAPI）
├── YGConfig.cs        # C 风格配置 API（YGConfigAPI）
├── YGPixelGrid.cs     # C 风格像素网格 API（YGPixelGridAPI）
├── YGValue.cs         # YGValue 常量与辅助方法
├── YGEnums.cs         # YG 前缀公共枚举 + 转换扩展方法
├── YGMacros.cs        # YogaDeprecatedAttribute
├── YogaConstants.cs   # 内部常量（Undefined = NaN）
├── config/Config.cs   # Config 类 + ExperimentalFeatureSet
├── node/Node.cs       # Node 核心类
├── node/LayoutResults.cs  # 布局结果类
├── node/CachedMeasurement.cs # 缓存测量
├── style/Style.cs     # Style 样式类（所有 CSS 属性）
├── style/StyleLength.cs # StyleLength + YGValue struct
├── style/StyleSizeLength.cs # StyleSizeLength（宽度/高度/弹性基准）
├── style/GridLine.cs  # GridLine（CSS 网格线）
├── style/GridTrack.cs # GridTrackSize + GridTrackList
├── numeric/FloatOptional.cs # 可选浮点数（NaN = undefined）
├── event/event.cs     # Event 事件系统 + EventType/MeasureMode 等枚举
├── enums/             # 内部枚举定义（Align, Direction, Display 等）
└── algorithm/         # 核心布局算法（内部使用）
    ├── CalculateLayout.cs   # 主布局入口（91KB）
    ├── AbsoluteLayout.cs    # 绝对定位布局
    ├── Cache.cs             # 布局/测量缓存
    ├── PixelGrid.cs         # 像素网格舍入
    └── ...
```

### 双层 API 设计

| 层 | 风格 | 命名 | 用途 |
|---|---|---|---|
| C 风格 API | 静态方法 | `YGNodeStyleSetWidth()` | 兼容 C++ Yoga，1:1 测试移植 |
| OOP API | 实例方法 | `node.Style.SetDimension()` | 地道 C# 风格 |

两层 API 功能完全等价，C 风格 API 内部委托给 OOP 类。

---

## 2. 快速上手

### 最小示例（Flexbox）

```csharp
using Facebook.Yoga;

// 1. 创建节点
var root = YGNodeAPI.YGNodeNew();
var child0 = YGNodeAPI.YGNodeNew();
var child1 = YGNodeAPI.YGNodeNew();

// 2. 设置根节点样式
YGNodeStyleAPI.YGNodeStyleSetWidth(root, 100);
YGNodeStyleAPI.YGNodeStyleSetHeight(root, 100);
YGNodeStyleAPI.YGNodeStyleSetFlexDirection(root, YGFlexDirection.Row);

// 3. 设置子节点样式
YGNodeStyleAPI.YGNodeStyleSetFlexGrow(child0, 1);
YGNodeStyleAPI.YGNodeStyleSetFlexGrow(child1, 1);

// 4. 构建树
YGNodeAPI.YGNodeInsertChild(root, child0, 0);
YGNodeAPI.YGNodeInsertChild(root, child1, 1);

// 5. 计算布局
YGNodeAPI.YGNodeCalculateLayout(root, YogaConstants.Undefined, YogaConstants.Undefined, YGDirection.LTR);

// 6. 读取结果
float child0Width = YGNodeLayoutAPI.YGNodeLayoutGetWidth(child0);   // 50
float child0Height = YGNodeLayoutAPI.YGNodeLayoutGetHeight(child0);  // 100
float child0Left = YGNodeLayoutAPI.YGNodeLayoutGetLeft(child0);      // 0
```

### 自定义测量（叶子节点，如文本）

```csharp
var textNode = YGNodeAPI.YGNodeNew();

YGNodeAPI.YGNodeSetMeasureFunc(textNode, (node, availableWidth, widthMode, availableHeight, heightMode) =>
{
    // 模拟文本测量
    float measuredWidth = 100;
    float measuredHeight = 20;
    return new YGSize { Width = measuredWidth, Height = measuredHeight };
});

YGNodeAPI.YGNodeSetNodeType(textNode, YGNodeType.Text); // 设置 MeasureFunc 时自动设置
```

### CSS Grid 示例

```csharp
var grid = YGNodeAPI.YGNodeNew();
YGNodeStyleAPI.YGNodeStyleSetDisplay(grid, YGDisplay.Grid);

// 定义 3 列：100px 1fr auto
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnsCount(grid, 3);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 0, YGGridTrackType.Points, 100);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 1, YGGridTrackType.Fr, 1);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(grid, 2, YGGridTrackType.Auto, 0);

// 定义 2 行：50px 1fr
YGNodeStyleAPI.YGNodeStyleSetGridTemplateRowsCount(grid, 2);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateRow(grid, 0, YGGridTrackType.Points, 50);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateRow(grid, 1, YGGridTrackType.Fr, 1);

// 网格项：跨越第一列第一行
var item = YGNodeAPI.YGNodeNew();
YGNodeStyleAPI.YGNodeStyleSetGridColumnStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridColumnEnd(item, 2);
YGNodeStyleAPI.YGNodeStyleSetGridRowStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridRowEnd(item, 2);

YGNodeAPI.YGNodeInsertChild(grid, item, 0);
YGNodeAPI.YGNodeCalculateLayout(grid, 500, 300, YGDirection.LTR);
```

### OOP 风格示例

```csharp
var root = new Node();
root.Style.FlexDirection = FlexDirection.Row;
root.Style.SetDimension(Dimension.Width, StyleSizeLength.Points(100));
root.Style.SetDimension(Dimension.Height, StyleSizeLength.Points(100));

var child = new Node();
child.Style.FlexGrow = new FloatOptional(1);

root.InsertChild(child, 0);
root.CalculateLayout(YogaConstants.Undefined, YogaConstants.Undefined, Direction.LTR);

float w = child.Layout.Dimension(Dimension.Width);
```

---

## 3. 委托类型（回调）

### YGMeasureFunc
自定义测量回调，用于叶子节点（如文本、图片）。设置后节点自动变为 `NodeType.Text`，且不能有子节点。

```csharp
public delegate YGSize YGMeasureFunc(
    Node node,
    float availableWidth,
    MeasureMode widthMode,    // Undefined / Exactly / AtMost
    float availableHeight,
    MeasureMode heightMode);  // Undefined / Exactly / AtMost
```

返回值 `YGSize` 包含测量后的宽高。

**MeasureMode 含义：**
- `Undefined` — 父容器未约束此维度，测量内容固有尺寸
- `Exactly` — 父容器已确定此维度的精确值
- `AtMost` — 父容器给出此维度的上限

### YGBaselineFunc
自定义基线对齐回调。

```csharp
public delegate float YGBaselineFunc(Node node, float width, float height);
```

### YGDirtiedFunc
节点被标记为脏（需要重新布局）时调用。

```csharp
public delegate void YGDirtiedFunc(Node node);
```

### YGLogger
日志回调。

```csharp
public delegate void YGLogger(Config config, Node node, LogLevel logLevel, string message);
```

### YGCloneNodeFunc
节点克隆回调，用于 `display: contents` 场景。

```csharp
public delegate Node? YGCloneNodeFunc(Node node, Node owner, int childIndex);
```

返回 `null` 时使用默认的 `YGNodeClone` 行为。

### Event.Subscriber
事件系统订阅回调。

```csharp
public delegate void Subscriber(Node? node, EventType eventType, Event.Data eventData);
```

---

## 4. 枚举类型

### 公共枚举（YG 前缀，C 风格 API 使用）

| 枚举 | 值 | 对应 CSS |
|---|---|---|
| `YGAlign` | Auto, FlexStart, Center, FlexEnd, Stretch, Baseline, SpaceBetween, SpaceAround, SpaceEvenly, Start, End | align-items/align-self/align-content |
| `YGBoxSizing` | BorderBox, ContentBox | box-sizing |
| `YGDimension` | Width, Height | — |
| `YGDirection` | Inherit, LTR, RTL | direction |
| `YGDisplay` | Flex, None, Contents, Grid | display |
| `YGEdge` | Left, Top, Right, Bottom, Start, End, Horizontal, Vertical, All | margin/padding/border/position 的边 |
| `YGErrata` | [Flags] None, StretchFlexBasis, AbsolutePositionWithoutInsetsExcludesPadding, AbsolutePercentAgainstInnerSize, All, Classic | 兼容性错误修复标志 |
| `YGExperimentalFeature` | WebFlexBasis, FixFlexBasisFitContent | 实验特性开关 |
| `YGFlexDirection` | Column, ColumnReverse, Row, RowReverse | flex-direction |
| `YGGridTrackType` | Auto, Points, Percent, Fr, Minmax | grid-template 轨道类型 |
| `YGGutter` | Column, Row, All | gap 方向 |
| `YGJustify` | Auto, FlexStart, Center, FlexEnd, SpaceBetween, SpaceAround, SpaceEvenly, Stretch, Start, End | justify-content |
| `YGLogLevel` | Error, Warn, Info, Debug, Verbose, Fatal | 日志级别 |
| `YGMeasureMode` | Undefined, Exactly, AtMost | 测量模式 |
| `YGNodeType` | Default, Text | 节点类型（Text = 有 MeasureFunc） |
| `YGOverflow` | Visible, Hidden, Scroll | overflow |
| `YGPositionType` | Static, Relative, Absolute | position |
| `YGUnit` | Undefined, Point, Percent, Auto, MaxContent, FitContent, Stretch | CSS 值单位 |
| `YGWrap` | NoWrap, Wrap, WrapReverse | flex-wrap |

### 内部枚举（OOP API 使用，去掉 YG 前缀）

每个 `YG*` 枚举都有对应的内部版本（如 `Align`, `Direction`, `Display`），枚举值完全相同。通过扩展方法互转：

```csharp
YGAlign ygAlign = YGAlign.Center;
Align internal = ygAlign.ToInternal();   // Align.Center
YGAlign back = internal.ToYG();          // YGAlign.Center
string name = ygAlign.ToStringFast();    // "center"（快速，无反射）
```

### 事件与布局枚举

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

---

## 5. 值类型（Struct）

### YGSize
```csharp
public struct YGSize {
    public float Width;
    public float Height;
}
```
测量回调的返回值。

### YGValue
```csharp
public struct YGValue : IEquatable<YGValue> {
    public float Value;
    public Unit Unit;  // Unit.Point, Unit.Percent, Unit.Auto 等
}
```
CSS 值 + 单位。预定义常量：

```csharp
YGValue.YGValueZero       // { 0, Point }
YGValue.YGValueUndefined  // { NaN, Undefined }
YGValue.YGValueAuto       // { NaN, Auto }
```

### FloatOptional
```csharp
public struct FloatOptional  // NaN 表示 undefined
```
可选浮点数，避免 nullable float 的开销。关键 API：

```csharp
FloatOptional.Undefined  // NaN，表示未定义
FloatOptional.Zero       // 0.0f
new FloatOptional(3.14f)
opt.IsDefined()          // 非 NaN
opt.IsUndefined()        // 是 NaN
opt.Unwrap()             // 获取 float 值（NaN 时行为未定义）
opt + other              // 支持算术运算
```

### LayoutData
```csharp
public struct LayoutData {
    public int Layouts;          // 布局次数
    public int Measures;         // 测量次数
    public int CachedLayouts;    // 缓存命中（布局）
    public int CachedMeasures;   // 缓存命中（测量）
    public int MeasureCallbacks; // 测量回调调用次数
    public int[] MeasureCallbackReasonsCount; // 按原因分类的回调次数
}
```

---

## 6. 核心类

### Node — 布局节点

树的节点，包含样式、布局结果、子节点列表和回调。

```csharp
public class Node {
    // 构造
    public Node();
    public Node(Config? config);

    // 属性
    public Config Config { get; }
    public LayoutResults Layout { get; set; }
    public Style Style { get; }
    public bool HasNewLayout { get; set; }
    public bool AlwaysFormsContainingBlock { get; set; }

    // 树操作
    public void InsertChild(Node child, nuint index);
    public void ReplaceChild(Node oldChild, Node newChild);
    public void ReplaceChild(Node child, nuint index);
    public bool RemoveChild(Node child);
    public void RemoveChild(nuint index);
    public void ClearChildren();
    public void SetChildren(IReadOnlyList<Node> children);
    public Node? GetChild(nuint index);
    public nuint GetChildCount();
    public Node? GetOwner();
    public IReadOnlyList<Node> GetChildren();
    public LayoutableChildren<Node> LayoutChildren { get; } // 迭代可布局子节点
    public nuint GetLayoutChildCount();

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
    public float Baseline(float width, float height);
    public void SetDirtiedFunc(YGDirtiedFunc? dirtiedFunc);

    // 配置与上下文
    public void SetConfig(Config? config);
    public Config? GetConfig();
    public void SetContext(object? context);
    public object? GetContext();

    // 节点类型
    public void SetNodeType(NodeType nodeType);
    public NodeType GetNodeType();
    public void SetIsReferenceBaseline(bool isReferenceBaseline);
    public bool IsReferenceBaseline();

    // 克隆与重置
    public void MoveFrom(Node other);    // 深拷贝（不含子节点的 owner 转移）
    public void Reset();

    // 其他
    public void CloneChildrenIfNeeded();
    public void ProcessDimensions();
    public FloatOptional ResolveFlexGrow();
    public FloatOptional ResolveFlexShrink();
    public bool IsNodeFlexible();
}
```

### Config — 布局配置

```csharp
public class Config {
    // 构造
    public Config();
    public Config(YGLogger logger);

    // 静态默认实例（懒加载单例）
    public static Config Default { get; }
    public static Config GetDefault();

    // Web 默认值（FlexDirection=Row, AlignContent=Stretch）
    public void SetUseWebDefaults(bool useWebDefaults);
    public bool UseWebDefaults();

    // 像素缩放因子（用于像素网格对齐）
    public void SetPointScaleFactor(float pointScaleFactor);
    public float GetPointScaleFactor();

    // 兼容性错误修复
    public void SetErrata(Errata errata);
    public void AddErrata(Errata errata);
    public void RemoveErrata(Errata errata);
    public Errata GetErrata();
    public bool HasErrata(Errata errata);

    // 实验特性
    public void SetExperimentalFeatureEnabled(ExperimentalFeature feature, bool enabled);
    public bool IsExperimentalFeatureEnabled(ExperimentalFeature feature);
    public ExperimentalFeatureSet GetEnabledExperiments();

    // 日志
    public void SetLogger(YGLogger logger);
    public void Log(Node node, LogLevel logLevel, string format);

    // 克隆回调
    public void SetCloneNodeCallback(YGCloneNodeFunc? cloneNode);
    public Node CloneNode(Node node, Node owner, int childIndex);

    // 上下文
    public void SetContext(object? context);
    public object? GetContext();

    // 版本号（配置变更时递增，用于布局缓存失效）
    public uint GetVersion();
}
```

### Style — CSS 样式属性

```csharp
public class Style {
    // 布局方向
    public Direction Direction;
    public FlexDirection FlexDirection;
    public Justify JustifyContent;
    public Align AlignContent;
    public Align AlignItems;
    public Align AlignSelf;
    public Justify JustifyItems;
    public Justify JustifySelf;

    // 显示与定位
    public Display Display;
    public PositionType PositionType;
    public Overflow Overflow;
    public FlexWrap FlexWrap;
    public BoxSizing BoxSizing;

    // Flex 属性
    public FloatOptional Flex;           // flex 简写
    public FloatOptional FlexGrow;
    public FloatOptional FlexShrink;
    public StyleSizeLength FlexBasis;

    // 尺寸
    StyleSizeLength Dimension(Dimension dim);      // width / height
    StyleSizeLength MinDimension(Dimension dim);   // min-width / min-height
    StyleSizeLength MaxDimension(Dimension dim);   // max-width / max-height
    void SetDimension(Dimension dim, StyleSizeLength value);
    void SetMinDimension(Dimension dim, StyleSizeLength value);
    void SetMaxDimension(Dimension dim, StyleSizeLength value);

    // 间距（按 Edge 索引）
    StyleLength Position(Edge edge);
    StyleLength Margin(Edge edge);
    StyleLength Padding(Edge edge);
    StyleLength Border(Edge edge);
    void SetPosition(Edge edge, StyleLength value);
    void SetMargin(Edge edge, StyleLength value);
    void SetPadding(Edge edge, StyleLength value);
    void SetBorder(Edge edge, StyleLength value);

    // 间隙（按 Gutter 索引）
    StyleLength Gap(Gutter gutter);
    void SetGap(Gutter gutter, StyleLength value);

    // 其他
    public FloatOptional AspectRatio;

    // Grid 属性
    public GridLine GridColumnStart;
    public GridLine GridColumnEnd;
    public GridLine GridRowStart;
    public GridLine GridRowEnd;
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

    // 默认常量
    public const float DefaultFlexGrow = 0.0f;
    public const float DefaultFlexShrink = 0.0f;
    public const float WebDefaultFlexGrow = 0.0f;
    public const float WebDefaultFlexShrink = 1.0f;
}
```

### LayoutResults — 布局输出结果

```csharp
public class LayoutResults {
    // 位置
    float Position(PhysicalEdge edge);  // Left / Top / Right / Bottom
    void SetPosition(PhysicalEdge edge, float value);

    // 尺寸
    float Dimension(Dimension axis);       // 最终计算尺寸
    float MeasuredDimension(Dimension axis); // 测量尺寸
    float RawDimension(Dimension axis);     // 未缩放的原始尺寸

    // 方向与溢出
    Direction GetDirection();
    bool HadOverflow();

    // 间距（布局后计算值）
    float Margin(PhysicalEdge edge);
    float Border(PhysicalEdge edge);
    float Padding(PhysicalEdge edge);

    // 缓存
    public const int MaxCachedMeasurements = 8;
    public CachedMeasurement[] CachedMeasurements;
    public CachedMeasurement CachedLayout;

    // 内部状态（一般不需要直接使用）
    public uint ComputedFlexBasisGeneration;
    public FloatOptional ComputedFlexBasis;
    public uint GenerationCount;
    public uint ConfigVersion;
    public Direction LastOwnerDirection;
}
```

---

## 7. C 风格静态 API（YG 前缀）

### YGNodeAPI — 节点生命周期与树操作

**文件：** `YGNode.cs`

```csharp
public static class YGNodeAPI {
    // 创建与销毁
    Node YGNodeNew();
    Node YGNodeNewWithConfig(Config config);
    Node YGNodeClone(Node oldNode);
    void YGNodeFree(Node node);               // 从父节点移除并清理
    void YGNodeFreeRecursive(Node root);      // 递归释放整棵树
    void YGNodeFinalize(Node node);           // 仅清理，不移除
    void YGNodeReset(Node node);

    // 布局计算
    void YGNodeCalculateLayout(Node node, float availableWidth, float availableHeight, YGDirection ownerDirection);
    // availableWidth/Height 传 YogaConstants.Undefined (NaN) 表示未约束

    // 脏标记
    bool YGNodeIsDirty(Node node);
    bool YGNodeGetHasNewLayout(Node node);
    void YGNodeSetHasNewLayout(Node node, bool hasNewLayout);

    // 树操作
    void YGNodeInsertChild(Node owner, Node child, nuint index);
    void YGNodeSwapChild(Node owner, Node child, nuint index);
    void YGNodeRemoveChild(Node owner, Node child);
    void YGNodeRemoveAllChildren(Node owner);
    void YGNodeSetChildren(Node owner, Node[] children);
    Node? YGNodeGetChild(Node node, nuint index);
    nuint YGNodeGetChildCount(Node node);
    Node? YGNodeGetOwner(Node node);
    Node? YGNodeGetParent(Node node);  // YGNodeGetOwner 的别名

    // 配置与上下文
    void YGNodeSetConfig(Node node, Config config);
    Config YGNodeGetConfig(Node node);
    void YGNodeSetContext(Node node, object? context);
    object? YGNodeGetContext(Node node);

    // 回调
    void YGNodeSetMeasureFunc(Node node, YGMeasureFunc? measureFunc);
    bool YGNodeHasMeasureFunc(Node node);
    void YGNodeSetBaselineFunc(Node node, YGBaselineFunc? baselineFunc);
    bool YGNodeHasBaselineFunc(Node node);
    void YGNodeSetDirtiedFunc(Node node, YGDirtiedFunc? dirtiedFunc);
    YGDirtiedFunc? YGNodeGetDirtiedFunc(Node node);

    // 节点类型
    void YGNodeSetNodeType(Node node, YGNodeType nodeType);
    YGNodeType YGNodeGetNodeType(Node node);
    void YGNodeSetIsReferenceBaseline(Node node, bool isReferenceBaseline);
    bool YGNodeIsReferenceBaseline(Node node);
    void YGNodeSetAlwaysFormsContainingBlock(Node node, bool value);
    bool YGNodeGetAlwaysFormsContainingBlock(Node node);

    // 缓存查询（一般内部使用）
    bool YGNodeCanUseCachedMeasurement(float availableWidth, float availableHeight, MeasureMode widthMode, MeasureMode heightMode, float computedWidth, float computedHeight, MeasureMode lastWidthMode, MeasureMode lastHeightMode, float lastComputedWidth, float lastComputedHeight);
}
```

### YGNodeStyleAPI — 样式属性设置

**文件：** `YGNodeStyle.cs`

所有 Set 方法在值变更时自动调用 `node.MarkDirtyAndPropagate()`。

#### 样式复制

```csharp
void YGNodeCopyStyle(Node dstNode, Node srcNode);
```

#### 枚举样式属性

```csharp
// Direction
void YGNodeStyleSetDirection(Node node, YGDirection value);
YGDirection YGNodeStyleGetDirection(Node node);

// FlexDirection
void YGNodeStyleSetFlexDirection(Node node, YGFlexDirection value);
YGFlexDirection YGNodeStyleGetFlexDirection(Node node);

// JustifyContent / JustifyItems / JustifySelf
void YGNodeStyleSetJustifyContent(Node node, YGJustify value);
YGJustify YGNodeStyleGetJustifyContent(Node node);
void YGNodeStyleSetJustifyItems(Node node, YGJustify value);
YGJustify YGNodeStyleGetJustifyItems(Node node);
void YGNodeStyleSetJustifySelf(Node node, YGJustify value);
YGJustify YGNodeStyleGetJustifySelf(Node node);

// AlignContent / AlignItems / AlignSelf
void YGNodeStyleSetAlignContent(Node node, YGAlign value);
YGAlign YGNodeStyleGetAlignContent(Node node);
void YGNodeStyleSetAlignItems(Node node, YGAlign value);
YGAlign YGNodeStyleGetAlignItems(Node node);
void YGNodeStyleSetAlignSelf(Node node, YGAlign value);
YGAlign YGNodeStyleGetAlignSelf(Node node);

// PositionType
void YGNodeStyleSetPositionType(Node node, YGPositionType value);
YGPositionType YGNodeStyleGetPositionType(Node node);

// FlexWrap
void YGNodeStyleSetFlexWrap(Node node, YGWrap value);
YGWrap YGNodeStyleGetFlexWrap(Node node);

// Overflow
void YGNodeStyleSetOverflow(Node node, YGOverflow value);
YGOverflow YGNodeStyleGetOverflow(Node node);

// Display
void YGNodeStyleSetDisplay(Node node, YGDisplay value);
YGDisplay YGNodeStyleGetDisplay(Node node);

// BoxSizing
void YGNodeStyleSetBoxSizing(Node node, YGBoxSizing value);
YGBoxSizing YGNodeStyleGetBoxSizing(Node node);
```

#### Flex 属性

```csharp
void YGNodeStyleSetFlex(Node node, float flex);
float YGNodeStyleGetFlex(Node node);

void YGNodeStyleSetFlexGrow(Node node, float flexGrow);
float YGNodeStyleGetFlexGrow(Node node);

void YGNodeStyleSetFlexShrink(Node node, float flexShrink);
float YGNodeStyleGetFlexShrink(Node node);

void YGNodeStyleSetFlexBasis(Node node, float flexBasis);            // 点值
void YGNodeStyleSetFlexBasisPercent(Node node, float percent);       // 百分比
void YGNodeStyleSetFlexBasisAuto(Node node);                        // auto
void YGNodeStyleSetFlexBasisMaxContent(Node node);                  // max-content
void YGNodeStyleSetFlexBasisFitContent(Node node);                  // fit-content
void YGNodeStyleSetFlexBasisStretch(Node node);                     // stretch
YGValue YGNodeStyleGetFlexBasis(Node node);
```

#### Position（按 Edge）

```csharp
void YGNodeStyleSetPosition(Node node, YGEdge edge, float points);
void YGNodeStyleSetPositionPercent(Node node, YGEdge edge, float percent);
void YGNodeStyleSetPositionAuto(Node node, YGEdge edge);
YGValue YGNodeStyleGetPosition(Node node, YGEdge edge);
```

#### Margin（按 Edge）

```csharp
void YGNodeStyleSetMargin(Node node, YGEdge edge, float points);
void YGNodeStyleSetMarginPercent(Node node, YGEdge edge, float percent);
void YGNodeStyleSetMarginAuto(Node node, YGEdge edge);
YGValue YGNodeStyleGetMargin(Node node, YGEdge edge);
```

#### Padding（按 Edge）

```csharp
void YGNodeStyleSetPadding(Node node, YGEdge edge, float points);
void YGNodeStyleSetPaddingPercent(Node node, YGEdge edge, float percent);
YGValue YGNodeStyleGetPadding(Node node, YGEdge edge);
```

#### Border（按 Edge，仅点值）

```csharp
void YGNodeStyleSetBorder(Node node, YGEdge edge, float border);
float YGNodeStyleGetBorder(Node node, YGEdge edge);
```

#### Gap（按 Gutter）

```csharp
void YGNodeStyleSetGap(Node node, YGGutter gutter, float gapLength);
void YGNodeStyleSetGapPercent(Node node, YGGutter gutter, float percent);
YGValue YGNodeStyleGetGap(Node node, YGGutter gutter);
```

#### AspectRatio

```csharp
void YGNodeStyleSetAspectRatio(Node node, float aspectRatio);
float YGNodeStyleGetAspectRatio(Node node);
```

#### 尺寸（Width / Height / MinWidth / MinHeight / MaxWidth / MaxHeight）

每个维度都有以下变体：

```csharp
// 以 Width 为例，Height/Min*/Max* 结构相同
void YGNodeStyleSetWidth(Node node, float points);
void YGNodeStyleSetWidthPercent(Node node, float percent);
void YGNodeStyleSetWidthAuto(Node node);
void YGNodeStyleSetWidthMaxContent(Node node);
void YGNodeStyleSetWidthFitContent(Node node);
void YGNodeStyleSetWidthStretch(Node node);
YGValue YGNodeStyleGetWidth(Node node);

// Min 尺寸（无 Auto 变体）
void YGNodeStyleSetMinWidth(Node node, float points);
void YGNodeStyleSetMinWidthPercent(Node node, float percent);
void YGNodeStyleSetMinWidthMaxContent(Node node);
void YGNodeStyleSetMinWidthFitContent(Node node);
void YGNodeStyleSetMinWidthStretch(Node node);
YGValue YGNodeStyleGetMinWidth(Node node);

// Max 尺寸（无 Auto 变体）
void YGNodeStyleSetMaxWidth(Node node, float points);
void YGNodeStyleSetMaxWidthPercent(Node node, float percent);
void YGNodeStyleSetMaxWidthMaxContent(Node node);
void YGNodeStyleSetMaxWidthFitContent(Node node);
void YGNodeStyleSetMaxWidthStretch(Node node);
YGValue YGNodeStyleGetMaxWidth(Node node);
```

#### Grid Item 属性

```csharp
// GridColumnStart / GridColumnEnd / GridRowStart / GridRowEnd
void YGNodeStyleSetGridColumnStart(Node node, int value);
void YGNodeStyleSetGridColumnStartAuto(Node node);
void YGNodeStyleSetGridColumnStartSpan(Node node, int span);
int YGNodeStyleGetGridColumnStart(Node node);
// GridColumnEnd, GridRowStart, GridRowEnd 结构相同
```

#### Grid Container 属性

```csharp
// GridTemplateColumns
void YGNodeStyleSetGridTemplateColumnsCount(Node node, int count);
void YGNodeStyleSetGridTemplateColumn(Node node, int index, YGGridTrackType type, float value);
void YGNodeStyleSetGridTemplateColumnMinMax(Node node, int index,
    YGGridTrackType minType, float minValue, YGGridTrackType maxType, float maxValue);
// GridTemplateRows, GridAutoColumns, GridAutoRows 结构相同
```

### YGNodeLayoutAPI — 读取布局结果

**文件：** `YGNodeLayout.cs`

在调用 `YGNodeCalculateLayout()` 之后使用。

```csharp
public static class YGNodeLayoutAPI {
    // 位置
    float YGNodeLayoutGetLeft(Node node);
    float YGNodeLayoutGetTop(Node node);
    float YGNodeLayoutGetRight(Node node);
    float YGNodeLayoutGetBottom(Node node);

    // 尺寸
    float YGNodeLayoutGetWidth(Node node);
    float YGNodeLayoutGetHeight(Node node);

    // 原始尺寸（未缩放）
    float YGNodeLayoutGetRawWidth(Node node);
    float YGNodeLayoutGetRawHeight(Node node);

    // 方向与溢出
    YGDirection YGNodeLayoutGetDirection(Node node);
    bool YGNodeLayoutGetHadOverflow(Node node);

    // 间距（布局后计算值，自动处理 Start/End -> 物理 Edge 映射）
    float YGNodeLayoutGetMargin(Node node, YGEdge edge);
    float YGNodeLayoutGetBorder(Node node, YGEdge edge);
    float YGNodeLayoutGetPadding(Node node, YGEdge edge);
}
```

注意：传入 `YGEdge.Start` / `YGEdge.End` 时，会根据布局方向自动映射到 `Left` / `Right`。

### YGConfigAPI — 配置

**文件：** `YGConfig.cs`

```csharp
public static class YGConfigAPI {
    Config YGConfigNew();
    void YGConfigFree(Config config);  // C# 中为空操作（GC 管理）
    Config YGConfigGetDefault();

    void YGConfigSetUseWebDefaults(Config config, bool enabled);
    bool YGConfigGetUseWebDefaults(Config config);

    void YGConfigSetPointScaleFactor(Config config, float pixelsInPoint);
    float YGConfigGetPointScaleFactor(Config config);

    void YGConfigSetErrata(Config config, YGErrata errata);
    YGErrata YGConfigGetErrata(Config config);

    void YGConfigSetLogger(Config config, YGLogger? logger);
    void YGConfigSetContext(Config config, object? context);
    object? YGConfigGetContext(Config config);

    void YGConfigSetExperimentalFeatureEnabled(Config config, YGExperimentalFeature feature, bool enabled);
    bool YGConfigIsExperimentalFeatureEnabled(Config config, YGExperimentalFeature feature);

    void YGConfigSetCloneNodeFunc(Config config, YGCloneNodeFunc? callback);
}
```

### YGPixelGridAPI — 像素网格对齐

**文件：** `YGPixelGrid.cs`

```csharp
public static class YGPixelGridAPI {
    float YGRoundValueToPixelGrid(double value, double pointScaleFactor, bool forceCeil, bool forceFloor);
}
```

### YGValue 辅助

**文件：** `YGValue.cs`

```csharp
bool YGFloatIsUndefined(float value);  // 检查是否为 NaN
```

---

## 8. 事件系统

```csharp
public static class Event {
    // 订阅/取消
    void Subscribe(Subscriber? subscriber);
    void Unsubscribe(Subscriber? subscriber);
    void Reset();  // 清除所有订阅者

    // 发布
    void Publish(Node? node, EventType eventType, object? data = null);
    void Publish<T>(Node? node, in T? eventData = default) where T : EventTypedDataBase, new();
}
```

### 事件类型与数据

| EventType | 数据类 | 说明 |
|---|---|---|
| `NodeAllocation` | `NodeAllocationData { Config }` | 节点创建 |
| `NodeDeallocation` | `NodeDeallocationData { Config }` | 节点释放 |
| `NodeLayout` | `NodeLayoutData { LayoutType }` | 节点布局完成 |
| `LayoutPassStart` | — | 布局遍历开始 |
| `LayoutPassEnd` | `LayoutPassEndData { LayoutData }` | 布局遍历结束，含统计 |
| `MeasureCallbackStart` | — | 测量回调开始 |
| `MeasureCallbackEnd` | `MeasureCallbackEndData { Width, WidthMeasureMode, Height, HeightMeasureMode, MeasuredWidth, MeasuredHeight, Reason }` | 测量回调结束 |
| `NodeBaselineStart` | — | 基线测量开始 |
| `NodeBaselineEnd` | — | 基线测量结束 |

订阅示例：

```csharp
Event.Subscribe((node, eventType, data) =>
{
    if (eventType == EventType.LayoutPassEnd)
    {
        var layoutData = data.GetData<Event.LayoutPassEndData>();
        Console.WriteLine($"Layouts: {layoutData?.LayoutData?.Layouts}");
    }
});
```

**线程安全：** 使用 lock + ThreadLocal 缓冲区，可在多线程中使用。

---

## 9. 样式值类型详解

### StyleLength

表示 CSS 长度值（用于 position, margin, padding, border, gap）。

```csharp
// 工厂方法
StyleLength Undefined()     // 未定义
StyleLength OfAuto()        // auto
StyleLength Points(float)   // 点值（如 10px）
StyleLength Percent(float)  // 百分比（如 50%）

// 查询
bool IsAuto() / IsUndefined() / IsPoints() / IsPercent() / IsDefined()
FloatOptional Value()

// 解析
FloatOptional Resolve(float referenceLength)  // 百分比 * referenceLength / 100

// 转换
explicit operator YGValue(StyleLength length)
```

### StyleSizeLength

表示尺寸值（用于 width, height, min-width, max-height, flex-basis）。
比 StyleLength 多支持 `MaxContent`, `FitContent`, `Stretch` 单位。

```csharp
// 工厂方法
StyleSizeLength Undefined()           // 未定义
StyleSizeLength OfAuto()              // auto
StyleSizeLength Points(float)         // 点值
StyleSizeLength Percent(float)        // 百分比
StyleSizeLength OfMaxContent()        // max-content
StyleSizeLength OfFitContent()        // fit-content
StyleSizeLength OfStretch(float = 1)  // stretch

// 查询
bool IsAuto() / IsUndefined() / IsPoints() / IsPercent()
bool IsMaxContent() / IsFitContent() / IsStretch()

// 解析
FloatOptional Resolve(float referenceLength)

// 转换
YGValue ToYGValue()
explicit operator YGValue(StyleSizeLength length)
```

### GridLine

表示 CSS Grid 网格线（用于 grid-column-start/end, grid-row-start/end）。

```csharp
// 工厂方法
GridLine Auto()                         // auto
GridLine FromInteger(int line)          // 整数行号（如 1, 2, 3）
GridLine Span(int spanCount)            // span N

// 查询
bool IsAuto() / IsInteger() / IsSpan()
int Integer { get; }
```

### GridTrackSize

表示 CSS Grid 轨道尺寸（用于 grid-template-columns/rows, grid-auto-columns/rows）。

```csharp
// 工厂方法
GridTrackSize Auto()                                  // auto
GridTrackSize Length(float points)                    // 固定点值
GridTrackSize Percent(float percent)                  // 百分比
GridTrackSize Fr(float fraction)                      // fr 单位
GridTrackSize MinMax(StyleSizeLength min, StyleSizeLength max)  // minmax()

// 查询
YGGridTrackType TrackType  // Auto / Points / Percent / Fr / Minmax
```

### ExperimentalFeatureSet

位标志集合，用于管理实验特性开关。

```csharp
void Set(ExperimentalFeature feature, bool enabled);
bool Test(ExperimentalFeature feature);
```

---

## 10. CSS Grid 支持

### Grid 容器

```csharp
// 1. 设置 display 为 Grid
YGNodeStyleAPI.YGNodeStyleSetDisplay(node, YGDisplay.Grid);

// 2. 定义模板列（先设数量，再逐个设置）
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnsCount(node, 3);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(node, 0, YGGridTrackType.Points, 100);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(node, 1, YGGridTrackType.Fr, 1);
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumn(node, 2, YGGridTrackType.Auto, 0);

// 3. 或使用 minmax()
YGNodeStyleAPI.YGNodeStyleSetGridTemplateColumnMinMax(node, 1,
    YGGridTrackType.Points, 100,    // min: 100px
    YGGridTrackType.Fr, 1);         // max: 1fr

// 4. 定义自动行（隐式网格）
YGNodeStyleAPI.YGNodeStyleSetGridAutoRowsCount(node, 1);
YGNodeStyleAPI.YGNodeStyleSetGridAutoRow(node, 0, YGGridTrackType.Points, 50);
```

### Grid 项

```csharp
// 放置到第 1 列，第 1 行（1-indexed）
YGNodeStyleAPI.YGNodeStyleSetGridColumnStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridColumnEnd(item, 2);
YGNodeStyleAPI.YGNodeStyleSetGridRowStart(item, 1);
YGNodeStyleAPI.YGNodeStyleSetGridRowEnd(item, 2);

// 使用 span
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartSpan(item, 2);  // span 2
YGNodeStyleAPI.YGNodeStyleSetGridRowEndSpan(item, 3);       // span 3

// 自动放置
YGNodeStyleAPI.YGNodeStyleSetGridColumnStartAuto(item);
```

### OOP 风格 Grid

```csharp
var grid = new Node();
grid.Style.Display = Display.Grid;
grid.Style.SetGridTemplateColumnAt(0, GridTrackSize.Length(100));
grid.Style.SetGridTemplateColumnAt(1, GridTrackSize.Fr(1));
grid.Style.SetGridTemplateColumnAt(2, GridTrackSize.Auto());

var item = new Node();
item.Style.GridColumnStart = GridLine.FromInteger(1);
item.Style.GridColumnEnd = GridLine.Span(2);
grid.InsertChild(item, 0);
```

---

## 11. 内部架构

### 布局算法（algorithm/ 目录，内部使用）

| 文件 | 说明 |
|---|---|
| `CalculateLayout.cs` | 主入口，Flexbox + Grid 完整算法实现（~91KB） |
| `AbsoluteLayout.cs` | 绝对定位元素布局 |
| `Cache.cs` | 布局/测量缓存，`CanUseCachedMeasurement()` |
| `PixelGrid.cs` | 像素网格对齐，子像素值舍入 |
| `Align.cs` | 对齐辅助 |
| `Baseline.cs` | 基线对齐 |
| `BoundAxis.cs` | 轴约束计算 |
| `FlexLine.cs` | Flex 行数据结构 |
| `FlexDirection.cs` | FlexDirection 扩展方法（IsRow, ResolveDirection 等） |
| `TrailingPosition.cs` | 尾部位置计算 |
| `SizingMode.cs` | 尺寸模式转换 |

### 枚举映射系统

```
YGAlign (public)  <--ToInternal()/ToYG()-->  Align (internal)
YGDirection       <--------same---------->  Direction
YGDisplay         <--------same---------->  Display
... 每个枚举都如此
```

两者序数值完全相同，转换为零开销。

---

## 12. 设计决策与注意事项

1. **零外部依赖** — 主库仅使用 BCL 类型，无 NuGet 包引用
2. **AOT/NativeAOT 兼容** — `IsAotCompatible=true`, `IsTrimmable=true`, 无反射/LINQ
3. **性能优化** — struct 值类型、`[MethodImpl(AggressiveInlining)]`、ThreadLocal 事件缓冲、NaN 表示 undefined
4. **YGNodeFree 是空操作** — C# 由 GC 管理内存，`YGNodeFree` 仅解除父子关系
5. **Undefined = NaN** — `YogaConstants.Undefined` 是 `float.NaN`，传给 `CalculateLayout` 表示不约束
6. **Web 默认值** — `UseWebDefaults=true` 时：FlexDirection=Row（非 Column），FlexShrink=1（非 0）
7. **Errata** — 兼容性标志，控制 Yoga 历史行为差异（`Classic` = 启用所有旧行为）
8. **display: contents** — 需要 `YGCloneNodeFunc` 回调支持，否则使用默认克隆行为
9. **Grid 阵列式 API** — Grid 模板需先 `SetCount` 再逐个 `Set`，是 C++ vector 的 1:1 翻译
10. **Edge 映射** — 布局结果 API 中，`YGEdge.Start/End` 自动根据方向映射为 Left/Right
