using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoga
{
    public enum MeasureMode
    {
        Undefined = 0,
        AtMost = 1,
        Exactly = 2
    }

    public enum Direction
    {
        Inherit = 0,
        LTR = 1,
        RTL = 2
    }

    public enum NodeType
    {
        Default = 0,
        Text = 1,
        Virtual = 2
    }

    public enum Alignment
    {
        Auto = 0,
        FlexStart = 1,
        Center = 2,
        FlexEnd = 3,
        Stretch = 4,
        Baseline = 5,
        SpaceBetween = 6,
        SpaceAround = 7,
        SpaceEvenly = 8
    }

    public enum FlexDirection
    {
        Column = 0,
        ColumnReverse = 1,
        Row = 2,
        RowReverse = 3
    }

    public enum Wrap
    {
        NoWrap = 0,
        Wrap = 1,
        WrapReverse = 2
    }

    public enum PositionType
    {
        Relative = 0,
        Absolute = 1
    }

    public enum Unit
    {
        Undefined = 0,
        Point = 1,
        Percent = 2,
        Auto = 3
    }

    public enum Display
    {
        Flex = 0,
        None = 1
    }

    public enum Overflow
    {
        Visible = 0,
        Hidden = 1,
        Scroll = 2
    }

    public struct Value
    {
        public Unit Unit;
        public float Value_;

        public static Value Undefined() => new Value { Unit = Unit.Undefined, Value_ = 0 };
        public static Value Point(float value) => new Value { Unit = Unit.Point, Value_ = value };
        public static Value Percent(float value) => new Value { Unit = Unit.Percent, Value_ = value };
        public static Value Auto() => new Value { Unit = Unit.Auto, Value_ = 0 };

        public bool IsUndefined() => Unit == Unit.Undefined;
        public bool IsAuto() => Unit == Unit.Auto;
        public bool IsPercent() => Unit == Unit.Percent;
        public bool IsPoint() => Unit == Unit.Point;
    }

    public struct YogaSize
    {
        public float Width;
        public float Height;
    }

    public delegate YogaSize MeasureFunc(YogaNode node, float width, MeasureMode widthMode, float height, MeasureMode heightMode);
    public delegate float BaselineFunc(YogaNode node, float width, float height);
    public delegate void DirtiedFunc(YogaNode node);

    public class YogaNode : IDisposable
    {
        private YogaConfig? _config;
        private YogaNode? _owner;
        private List<YogaNode> _children = new List<YogaNode>();
        private object? _context;
        private bool _hasNewLayout = true;
        private bool _isDirty = false;
        private DirtiedFunc? _dirtiedFunc;
        private MeasureFunc? _measureFunc;
        private BaselineFunc? _baselineFunc;
        private bool _isReferenceBaseline = false;
        private NodeType _nodeType = NodeType.Default;
        private bool _alwaysFormsContainingBlock = false;

        // Layout properties
        private float _positionX = 0;
        private float _positionY = 0;
        private float _computedWidth = 0;
        private float _computedHeight = 0;
        private float _computedLeft = 0;
        private float _computedTop = 0;
        private float _marginLeft = 0;
        private float _marginTop = 0;
        private float _marginRight = 0;
        private float _marginBottom = 0;
        private float _borderLeft = 0;
        private float _borderTop = 0;
        private float _borderRight = 0;
        private float _borderBottom = 0;
        private float _paddingLeft = 0;
        private float _paddingTop = 0;
        private float _paddingRight = 0;
        private float _paddingBottom = 0;

        // Style properties
        private FlexDirection _flexDirection = FlexDirection.Column;
        private Direction _direction = Direction.Inherit;
        private Wrap _wrap = Wrap.NoWrap;
        private Alignment _alignItems = Alignment.Stretch;
        private Alignment _alignSelf = Alignment.Auto;
        private Alignment _alignContent = Alignment.FlexStart;
        private Justify _justifyContent = Justify.FlexStart;
        private Display _display = Display.Flex;
        private Overflow _overflow = Overflow.Visible;
        private PositionType _positionType = PositionType.Relative;
        private float _flexGrow = 0;
        private float _flexShrink = 0;
        private Value _flexBasis = Value.Auto();
        private float _width = float.NaN;
        private float _height = float.NaN;
        private float _minWidth = 0;
        private float _minHeight = 0;
        private float _maxWidth = float.PositiveInfinity;
        private float _maxHeight = float.PositiveInfinity;
        private Value _marginStart = Value.Undefined();
        private Value _marginEnd = Value.Undefined();
        private Value _paddingStart = Value.Undefined();
        private Value _paddingEnd = Value.Undefined();
        private Value _widthPercent = Value.Undefined();
        private Value _heightPercent = Value.Undefined();

        public YogaNode() : this(YogaConfig.Default) { }

        public YogaNode(YogaConfig config)
        {
            _config = config;
        }

        public YogaNode(YogaConfig config, bool isClone) : this(config)
        {
        }

        public YogaConfig? Config
        {
            get => _config;
            set => _config = value;
        }

        public YogaNode? Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public List<YogaNode> Children => _children;

        public object? Context
        {
            get => _context;
            set => _context = value;
        }

        public bool HasNewLayout
        {
            get => _hasNewLayout;
            set => _hasNewLayout = value;
        }

        public bool IsDirty => _isDirty;

        public DirtiedFunc? DirtiedFunc
        {
            get => _dirtiedFunc;
            set => _dirtiedFunc = value;
        }

        public bool HasMeasureFunc => _measureFunc != null;

        public MeasureFunc? MeasureFunc
        {
            get => _measureFunc;
            set => _measureFunc = value;
        }

        public BaselineFunc? BaselineFunc
        {
            get => _baselineFunc;
            set => _baselineFunc = value;
        }

        public bool HasBaselineFunc => _baselineFunc != null;

        public bool IsReferenceBaseline
        {
            get => _isReferenceBaseline;
            set => _isReferenceBaseline = value;
        }

        public NodeType NodeType
        {
            get => _nodeType;
            set => _nodeType = value;
        }

        public bool AlwaysFormsContainingBlock
        {
            get => _alwaysFormsContainingBlock;
            set => _alwaysFormsContainingBlock = value;
        }

        // Style properties accessors
        public FlexDirection FlexDirection
        {
            get => _flexDirection;
            set => _flexDirection = value;
        }

        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public Wrap Wrap
        {
            get => _wrap;
            set => _wrap = value;
        }

        public Alignment AlignItems
        {
            get => _alignItems;
            set => _alignItems = value;
        }

        public Alignment AlignSelf
        {
            get => _alignSelf;
            set => _alignSelf = value;
        }

        public Alignment AlignContent
        {
            get => _alignContent;
            set => _alignContent = value;
        }

        public Justify JustifyContent
        {
            get => _justifyContent;
            set => _justifyContent = value;
        }

        public Display Display
        {
            get => _display;
            set => _display = value;
        }

        public Overflow Overflow
        {
            get => _overflow;
            set => _overflow = value;
        }

        public PositionType PositionType
        {
            get => _positionType;
            set => _positionType = value;
        }

        public float FlexGrow
        {
            get => _flexGrow;
            set => _flexGrow = value;
        }

        public float FlexShrink
        {
            get => _flexShrink;
            set => _flexShrink = value;
        }

        public Value FlexBasis
        {
            get => _flexBasis;
            set => _flexBasis = value;
        }

        public float Width
        {
            get => _width;
            set => _width = value;
        }

        public float Height
        {
            get => _height;
            set => _height = value;
        }

        public float MinWidth
        {
            get => _minWidth;
            set => _minWidth = value;
        }

        public float MinHeight
        {
            get => _minHeight;
            set => _minHeight = value;
        }

        public float MaxWidth
        {
            get => _maxWidth;
            set => _maxWidth = value;
        }

        public float MaxHeight
        {
            get => _maxHeight;
            set => _maxHeight = value;
        }

        // Layout results
        public float PositionX { get => _positionX; set => _positionX = value; }
        public float PositionY { get => _positionY; set => _positionY = value; }
        public float ComputedWidth { get => _computedWidth; set => _computedWidth = value; }
        public float ComputedHeight { get => _computedHeight; set => _computedHeight = value; }
        public float ComputedLeft { get => _computedLeft; set => _computedLeft = value; }
        public float ComputedTop { get => _computedTop; set => _computedTop = value; }
        public float MarginLeft { get => _marginLeft; set => _marginLeft = value; }
        public float MarginTop { get => _marginTop; set => _marginTop = value; }
        public float MarginRight { get => _marginRight; set => _marginRight = value; }
        public float MarginBottom { get => _marginBottom; set => _marginBottom = value; }
        public float BorderLeft { get => _borderLeft; set => _borderLeft = value; }
        public float BorderTop { get => _borderTop; set => _borderTop = value; }
        public float BorderRight { get => _borderRight; set => _borderRight = value; }
        public float BorderBottom { get => _borderBottom; set => _borderBottom = value; }
        public float PaddingLeft { get => _paddingLeft; set => _paddingLeft = value; }
        public float PaddingTop { get => _paddingTop; set => _paddingTop = value; }
        public float PaddingRight { get => _paddingRight; set => _paddingRight = value; }
        public float PaddingBottom { get => _paddingBottom; set => _paddingBottom = value; }

        public void Reset()
        {
            _hasNewLayout = true;
            _isDirty = false;
            _context = null;
            _dirtiedFunc = null;
            _measureFunc = null;
            _baselineFunc = null;
            _isReferenceBaseline = false;
            _nodeType = NodeType.Default;
            _alwaysFormsContainingBlock = false;

            _children.Clear();
            _owner = null;

            ResetLayoutRecursive();
        }

        private void ResetLayoutRecursive()
        {
            _positionX = 0;
            _positionY = 0;
            _computedWidth = 0;
            _computedHeight = 0;
            _computedLeft = 0;
            _computedTop = 0;
            _marginLeft = 0;
            _marginTop = 0;
            _marginRight = 0;
            _marginBottom = 0;
            _borderLeft = 0;
            _borderTop = 0;
            _borderRight = 0;
            _borderBottom = 0;
            _paddingLeft = 0;
            _paddingTop = 0;
            _paddingRight = 0;
            _paddingBottom = 0;
        }

        public void InsertChild(YogaNode child, size_t index)
        {
            _children.Insert((int)index, child);
        }

        public bool RemoveChild(YogaNode child)
        {
            return _children.Remove(child);
        }

        public void ClearChildren()
        {
            _children.Clear();
        }

        public void SetChildren(IList<YogaNode> children)
        {
            _children.Clear();
            foreach (var child in children)
            {
                _children.Add(child);
            }
        }

        public YogaNode? GetChild(size_t index)
        {
            if (index < _children.Count)
                return _children[(int)index];
            return null;
        }

        public size_t GetChildCount()
        {
            return (size_t)_children.Count;
        }

        public void SetOwner(YogaNode? owner)
        {
            _owner = owner;
        }

        public YogaNode? GetOwner()
        {
            return _owner;
        }

        public void MarkDirtyAndPropagate()
        {
            if (!_isDirty)
            {
                _isDirty = true;
                if (_owner != null)
                {
                    _owner.MarkDirtyAndPropagate();
                }
            }
        }

        public void SetDirty(bool dirty)
        {
            _isDirty = dirty;
        }

        public void SetLayout(float left, float top, float width, float height)
        {
            _computedLeft = left;
            _computedTop = top;
            _computedWidth = width;
            _computedHeight = height;
            _hasNewLayout = false;
        }

        public void ClearLayout()
        {
            _computedLeft = 0;
            _computedTop = 0;
            _computedWidth = 0;
            _computedHeight = 0;
        }

        public YogaSize Measure(float width, MeasureMode widthMode, float height, MeasureMode heightMode)
        {
            if (_measureFunc != null)
            {
                return _measureFunc(this, width, widthMode, height, heightMode);
            }
            return new YogaSize { Width = 0, Height = 0 };
        }

        public float Baseline(float width, float height)
        {
            if (_baselineFunc != null)
            {
                return _baselineFunc(this, width, height);
            }
            return height;
        }

        public void SetDirtiedFunc(DirtiedFunc? func)
        {
            _dirtiedFunc = func;
        }

        public DirtiedFunc? GetDirtiedFunc()
        {
            return _dirtiedFunc;
        }

        public void SetContext(object? context)
        {
            _context = context;
        }

        public object? GetContext()
        {
            return _context;
        }

        public void SetMeasureFunc(MeasureFunc? func)
        {
            _measureFunc = func;
        }

        public void SetBaselineFunc(BaselineFunc? func)
        {
            _baselineFunc = func;
        }

        public void SetIsReferenceBaseline(bool isReference)
        {
            _isReferenceBaseline = isReference;
        }

        public void SetNodeType(NodeType type)
        {
            _nodeType = type;
        }

        public NodeType GetNodeType()
        {
            return _nodeType;
        }

        public void SetAlwaysFormsContainingBlock(bool always)
        {
            _alwaysFormsContainingBlock = always;
        }

        public bool AlwaysFormsContainingBlock()
        {
            return _alwaysFormsContainingBlock;
        }

        public void SetConfig(YogaConfig config)
        {
            _config = config;
        }

        public YogaConfig? GetConfig()
        {
            return _config;
        }

        public void ReplaceChild(YogaNode child, size_t index)
        {
            if (index < _children.Count)
            {
                _children[(int)index] = child;
            }
        }

        public void Dispose()
        {
            foreach (var child in _children)
            {
                child.Dispose();
            }
            _children.Clear();
        }
    }

    public enum Justify
    {
        FlexStart = 0,
        Center = 1,
        FlexEnd = 2,
        SpaceBetween = 3,
        SpaceAround = 4,
        SpaceEvenly = 5
    }

    public class YogaConfig
    {
        private static YogaConfig? _default;
        private bool _useWebDefaults = false;
        private float _pointScaleFactor = 1.0f;
        private DirtiedFunc? _cloneNodeFunc;

        public static YogaConfig Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new YogaConfig();
                }
                return _default;
            }
        }

        public bool UseWebDefaults
        {
            get => _useWebDefaults;
            set => _useWebDefaults = value;
        }

        public float PointScaleFactor
        {
            get => _pointScaleFactor;
            set => _pointScaleFactor = value;
        }

        public DirtiedFunc? CloneNodeFunc
        {
            get => _cloneNodeFunc;
            set => _cloneNodeFunc = value;
        }
    }

    public class Yoga
    {
        public static YogaNode NodeNew()
        {
            return NodeNewWithConfig(YogaConfig.Default);
        }

        public static YogaNode NodeNewWithConfig(YogaConfig config)
        {
            if (config == null)
                throw new ArgumentNullException("config", "Tried to construct YGNode with null config");

            var node = new YogaNode(config);
            return node;
        }

        public static YogaNode NodeClone(YogaNode oldNode)
        {
            var node = new YogaNode(oldNode.Config ?? YogaConfig.Default, true);
            node.SetContext(oldNode.Context);
            node.SetOwner(null);
            foreach (var child in oldNode.Children)
            {
                node.Children.Add(child);
            }
            return node;
        }

        public static void NodeFree(YogaNode node)
        {
            if (node == null) return;

            var owner = node.Owner;
            if (owner != null)
            {
                owner.RemoveChild(node);
                node.Owner = null;
            }

            foreach (var child in node.Children)
            {
                child.Owner = null;
            }

            node.ClearChildren();
            node.Dispose();
        }

        public static void NodeFreeRecursive(YogaNode root)
        {
            if (root == null) return;

            size_t skipped = 0;
            while (root.GetChildCount() > skipped)
            {
                var child = root.GetChild(skipped);
                if (child?.Owner != root)
                {
                    skipped += 1;
                }
                else
                {
                    NodeRemoveChild(root, child);
                    NodeFreeRecursive(child);
                }
            }
            NodeFree(root);
        }

        public static void NodeFinalize(YogaNode node)
        {
            if (node == null) return;
            node.Dispose();
        }

        public static void NodeReset(YogaNode node)
        {
            node?.Reset();
        }

        public static void NodeCalculateLayout(YogaNode node, float ownerWidth, float ownerHeight, Direction ownerDirection)
        {
            if (node == null) return;

            CalculateLayout(node, ownerWidth, ownerHeight, ownerDirection);
        }

        private static void CalculateLayout(YogaNode node, float ownerWidth, float ownerHeight, Direction ownerDirection)
        {
            if (node == null) return;

            if (node.Display == Display.None)
            {
                node.ComputedWidth = 0;
                node.ComputedHeight = 0;
                return;
            }

            var children = node.Children;
            float usedWidth = 0;
            float usedHeight = 0;

            if (node.Width.IsPoint())
            {
                usedWidth = node.Width;
            }
            else if (ownerWidth > 0 && node.Width.IsPercent())
            {
                usedWidth = ownerWidth * node.Width.Value_;
            }
            else
            {
                usedWidth = ownerWidth > 0 ? ownerWidth : 0;
            }

            if (node.Height.IsPoint())
            {
                usedHeight = node.Height;
            }
            else if (ownerHeight > 0 && node.Height.IsPercent())
            {
                usedHeight = ownerHeight * node.Height.Value_;
            }
            else
            {
                usedHeight = ownerHeight > 0 ? ownerHeight : 0;
            }

            if (node.HasMeasureFunc)
            {
                var size = node.Measure(usedWidth, usedWidth > 0 ? MeasureMode.AtMost : MeasureMode.Undefined,
                                         usedHeight, usedHeight > 0 ? MeasureMode.AtMost : MeasureMode.Undefined);
                node.ComputedWidth = size.Width;
                node.ComputedHeight = size.Height;
            }
            else
            {
                foreach (var child in children)
                {
                    CalculateLayout(child, usedWidth, usedHeight, ownerDirection);
                    usedHeight += child.ComputedHeight;
                    usedWidth = Math.Max(usedWidth, child.ComputedWidth);
                }

                node.ComputedWidth = usedWidth;
                node.ComputedHeight = usedHeight;
            }

            node.HasNewLayout = false;
        }

        public static bool NodeGetHasNewLayout(YogaNode node)
        {
            return node?.HasNewLayout ?? false;
        }

        public static void NodeSetHasNewLayout(YogaNode node, bool hasNewLayout)
        {
            if (node != null)
                node.HasNewLayout = hasNewLayout;
        }

        public static bool NodeIsDirty(YogaNode node)
        {
            return node?.IsDirty ?? false;
        }

        public static void NodeMarkDirty(YogaNode node)
        {
            if (node == null) return;

            if (!node.HasMeasureFunc)
            {
                throw new InvalidOperationException("Only leaf nodes with custom measure functions should manually mark themselves as dirty");
            }

            node.MarkDirtyAndPropagate();
        }

        public static void NodeSetDirtiedFunc(YogaNode node, DirtiedFunc dirtiedFunc)
        {
            if (node != null)
                node.DirtiedFunc = dirtiedFunc;
        }

        public static DirtiedFunc? NodeGetDirtiedFunc(YogaNode node)
        {
            return node?.DirtiedFunc;
        }

        public static void NodeInsertChild(YogaNode owner, YogaNode child, size_t index)
        {
            if (owner == null || child == null) return;

            if (child.Owner != null)
            {
                throw new InvalidOperationException("Child already has an owner, it must be removed first.");
            }

            if (owner.HasMeasureFunc)
            {
                throw new InvalidOperationException("Cannot add child: Nodes with measure functions cannot have children.");
            }

            owner.InsertChild(child, index);
            child.Owner = owner;
            owner.MarkDirtyAndPropagate();
        }

        public static void NodeSwapChild(YogaNode owner, YogaNode child, size_t index)
        {
            if (owner == null || child == null) return;

            owner.ReplaceChild(child, index);
            child.Owner = owner;
        }

        public static void NodeRemoveChild(YogaNode owner, YogaNode excludedChild)
        {
            if (owner == null || excludedChild == null) return;

            if (owner.GetChildCount() == 0)
            {
                return;
            }

            var childOwner = excludedChild.Owner;
            if (owner.RemoveChild(excludedChild))
            {
                if (owner == childOwner)
                {
                    excludedChild.ClearLayout();
                    excludedChild.Owner = null;

                    var dirtiedFunc = excludedChild.DirtiedFunc;
                    excludedChild.DirtiedFunc = null;
                    excludedChild.SetDirty(true);
                    excludedChild.DirtiedFunc = dirtiedFunc;
                }
                owner.MarkDirtyAndPropagate();
            }
        }

        public static void NodeRemoveAllChildren(YogaNode owner)
        {
            if (owner == null) return;

            var childCount = owner.GetChildCount();
            if (childCount == 0)
            {
                return;
            }

            var firstChild = owner.GetChild(0);
            if (firstChild?.Owner == owner)
            {
                for (size_t i = 0; i < childCount; i++)
                {
                    var oldChild = owner.GetChild(i);
                    if (oldChild != null)
                    {
                        oldChild.ClearLayout();
                        oldChild.Owner = null;

                        var dirtiedFunc = oldChild.DirtiedFunc;
                        oldChild.DirtiedFunc = null;
                        oldChild.SetDirty(true);
                        oldChild.DirtiedFunc = dirtiedFunc;
                    }
                }
                owner.ClearChildren();
                owner.MarkDirtyAndPropagate();
                return;
            }

            owner.SetChildren(new List<YogaNode>());
            owner.MarkDirtyAndPropagate();
        }

        public static void NodeSetChildren(YogaNode owner, YogaNode[] children)
        {
            if (owner == null) return;

            if (children == null || children.Length == 0)
            {
                if (owner.GetChildCount() > 0)
                {
                    foreach (var child in owner.Children)
                    {
                        child.ClearLayout();
                        child.Owner = null;
                    }
                    owner.SetChildren(new List<YogaNode>());
                    owner.MarkDirtyAndPropagate();
                }
            }
            else
            {
                if (owner.GetChildCount() > 0)
                {
                    foreach (var oldChild in owner.Children)
                    {
                        if (!children.Contains(oldChild))
                        {
                            oldChild.ClearLayout();
                            oldChild.Owner = null;
                        }
                    }
                }

                var newChildren = children.ToList();
                owner.SetChildren(newChildren);
                foreach (var child in newChildren)
                {
                    child.Owner = owner;
                }
                owner.MarkDirtyAndPropagate();
            }
        }

        public static YogaNode? NodeGetChild(YogaNode node, size_t index)
        {
            return node?.GetChild(index);
        }

        public static size_t NodeGetChildCount(YogaNode node)
        {
            return node?.GetChildCount() ?? 0;
        }

        public static YogaNode? NodeGetOwner(YogaNode node)
        {
            return node?.Owner;
        }

        public static YogaNode? NodeGetParent(YogaNode node)
        {
            return node?.Owner;
        }

        public static void NodeSetConfig(YogaNode node, YogaConfig config)
        {
            if (node != null && config != null)
                node.SetConfig(config);
        }

        public static YogaConfig? NodeGetConfig(YogaNode node)
        {
            return node?.Config;
        }

        public static void NodeSetContext(YogaNode node, object? context)
        {
            if (node != null)
                node.SetContext(context);
        }

        public static object? NodeGetContext(YogaNode node)
        {
            return node?.GetContext();
        }

        public static void NodeSetMeasureFunc(YogaNode node, MeasureFunc? measureFunc)
        {
            if (node != null)
                node.MeasureFunc = measureFunc;
        }

        public static bool NodeHasMeasureFunc(YogaNode node)
        {
            return node?.HasMeasureFunc ?? false;
        }

        public static void NodeSetBaselineFunc(YogaNode node, BaselineFunc? baselineFunc)
        {
            if (node != null)
                node.BaselineFunc = baselineFunc;
        }

        public static bool NodeHasBaselineFunc(YogaNode node)
        {
            return node?.HasBaselineFunc ?? false;
        }

        public static void NodeSetIsReferenceBaseline(YogaNode node, bool isReferenceBaseline)
        {
            if (node != null && node.IsReferenceBaseline != isReferenceBaseline)
            {
                node.IsReferenceBaseline = isReferenceBaseline;
                node.MarkDirtyAndPropagate();
            }
        }

        public static bool NodeIsReferenceBaseline(YogaNode node)
        {
            return node?.IsReferenceBaseline ?? false;
        }

        public static void NodeSetNodeType(YogaNode node, NodeType nodeType)
        {
            if (node != null)
                node.NodeType = nodeType;
        }

        public static NodeType NodeGetNodeType(YogaNode node)
        {
            return node?.NodeType ?? NodeType.Default;
        }

        public static void NodeSetAlwaysFormsContainingBlock(YogaNode node, bool alwaysFormsContainingBlock)
        {
            if (node != null)
                node.AlwaysFormsContainingBlock = alwaysFormsContainingBlock;
        }

        public static bool NodeGetAlwaysFormsContainingBlock(YogaNode node)
        {
            return node?.AlwaysFormsContainingBlock ?? false;
        }

        public static bool NodeCanUseCachedMeasurement(
            MeasureMode widthMode,
            float availableWidth,
            MeasureMode heightMode,
            float availableHeight,
            MeasureMode lastWidthMode,
            float lastAvailableWidth,
            MeasureMode lastHeightMode,
            float lastAvailableHeight,
            float lastComputedWidth,
            float lastComputedHeight,
            float marginRow,
            float marginColumn,
            YogaConfig config)
        {
            return false;
        }
    }

    public enum Edge
    {
        Left = 0,
        Top = 0,
        Right = 2,
        Bottom = 3,
        Start = 4,
        End = 5,
        Horizontal = 6,
        Vertical = 7,
        All = 8
    }

    public enum LayoutUnit
    {
        Undefined = 0,
        Point = 1,
        Percent = 2,
        Auto = 3
    }

    public class LayoutAnchor
    {
        public float Value { get; set; }
        public LayoutUnit Unit { get; set; }

        public static LayoutAnchor Point(float value) => new LayoutAnchor { Value = value, Unit = LayoutUnit.Point };
        public static LayoutAnchor Percent(float value) => new LayoutAnchor { Value = value, Unit = LayoutUnit.Percent };
        public static LayoutAnchor Auto() => new LayoutAnchor { Value = 0, Unit = LayoutUnit.Auto };
        public static LayoutAnchor Undefined() => new LayoutAnchor { Value = 0, Unit = LayoutUnit.Undefined };
    }

    public class YogaNodeExtensions
    {
        public static void SetWidth(YogaNode node, float width) => node.Width = width;
        public static void SetHeight(YogaNode node, float height) => node.Height = height;
        public static void SetWidthPercent(YogaNode node, float percent) => node.Width = Value.Percent(percent);
        public static void SetHeightPercent(YogaNode node, float percent) => node.Height = Value.Percent(percent);
        public static void SetMinWidth(YogaNode node, float width) => node.MinWidth = width;
        public static void SetMinHeight(YogaNode node, float height) => node.MinHeight = height;
        public static void SetMaxWidth(YogaNode node, float width) => node.MaxWidth = width;
        public static void SetMaxHeight(YogaNode node, float height) => node.MaxHeight = height;
        public static void SetFlexDirection(YogaNode node, FlexDirection direction) => node.FlexDirection = direction;
        public static void SetDirection(YogaNode node, Direction direction) => node.Direction = direction;
        public static void SetWrap(YogaNode node, Wrap wrap) => node.Wrap = wrap;
        public static void SetAlignItems(YogaNode node, Alignment alignment) => node.AlignItems = alignment;
        public static void SetAlignSelf(YogaNode node, Alignment alignment) => node.AlignSelf = alignment;
        public static void SetAlignContent(YogaNode node, Alignment alignment) => node.AlignContent = alignment;
        public static void SetJustifyContent(YogaNode node, Justify justify) => node.JustifyContent = justify;
        public static void SetDisplay(YogaNode node, Display display) => node.Display = display;
        public static void SetOverflow(YogaNode node, Overflow overflow) => node.Overflow = overflow;
        public static void SetPositionType(YogaNode node, PositionType positionType) => node.PositionType = positionType;
        public static void SetFlexGrow(YogaNode node, float grow) => node.FlexGrow = grow;
        public static void SetFlexShrink(YogaNode node, float shrink) => node.FlexShrink = shrink;
        public static void SetFlexBasis(YogaNode node, float basis) => node.FlexBasis = Value.Point(basis);
        public static void SetFlexBasisPercent(YogaNode node, float percent) => node.FlexBasis = Value.Percent(percent);
        public static void SetFlexBasisAuto(YogaNode node) => node.FlexBasis = Value.Auto();

        public static float GetLeft(YogaNode node) => node.ComputedLeft;
        public static float GetTop(YogaNode node) => node.ComputedTop;
        public static float GetRight(YogaNode node) => node.ComputedLeft + node.ComputedWidth;
        public static float GetBottom(YogaNode node) => node.ComputedTop + node.ComputedHeight;
        public static float GetWidth(YogaNode node) => node.ComputedWidth;
        public static float GetHeight(YogaNode node) => node.ComputedHeight;
        public static float GetMarginLeft(YogaNode node) => node.MarginLeft;
        public static float GetMarginTop(YogaNode node) => node.MarginTop;
        public static float GetMarginRight(YogaNode node) => node.MarginRight;
        public static float GetMarginBottom(YogaNode node) => node.MarginBottom;
        public static float GetBorderLeft(YogaNode node) => node.BorderLeft;
        public static float GetBorderTop(YogaNode node) => node.BorderTop;
        public static float GetBorderRight(YogaNode node) => node.BorderRight;
        public static float GetBorderBottom(YogaNode node) => node.BorderBottom;
        public static float GetPaddingLeft(YogaNode node) => node.PaddingLeft;
        public static float GetPaddingTop(YogaNode node) => node.PaddingTop;
        public static float GetPaddingRight(YogaNode node) => node.PaddingRight;
        public static float GetPaddingBottom(YogaNode node) => node.PaddingBottom;
    }

    public enum LayoutUnitType
    {
        Undefined = 0,
        Point = 1,
        Percent = 2,
        Auto = 3
    }

    public struct LayoutMetrics
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
        public float Width;
        public float Height;
    }
}
</Program>

