using System;
using System.Threading;

namespace Facebook.Yoga
{
    public static partial class LayoutAlgorithm
    {
        private static uint gCurrentGenerationCount = 0;

        public static void ConstrainMaxSizeForMode(
            Node node,
            Direction direction,
            FlexDirection axis,
            float ownerAxisSize,
            float ownerWidth,
            ref SizingMode mode,
            ref float size)
        {
            var maxSize = node.Style.ResolveMaxDimension(
                    direction,
                    axis.Dimension(),
                    ownerAxisSize,
                    ownerWidth) +
                new FloatOptional(node.Style.ComputeMarginForAxis(axis, ownerWidth));

            switch (mode)
            {
                case SizingMode.StretchFit:
                case SizingMode.FitContent:
                    size = (!maxSize.IsDefined() || size < maxSize.Unwrap())
                        ? size
                        : maxSize.Unwrap();
                    break;
                case SizingMode.MaxContent:
                    if (maxSize.IsDefined())
                    {
                        mode = SizingMode.FitContent;
                        size = maxSize.Unwrap();
                    }
                    break;
            }
        }

        private static void ComputeFlexBasisForChild(
            Node node,
            Node child,
            float width,
            SizingMode widthMode,
            float height,
            float ownerWidth,
            float ownerHeight,
            SizingMode heightMode,
            Direction direction,
            ref LayoutData layoutMarkerData,
            uint depth,
            uint generationCount)
        {
            var mainAxis = node.Style.FlexDirection().ResolveDirection(direction);
            bool isMainAxisRow = mainAxis.IsRow();
            float mainAxisSize = isMainAxisRow ? width : height;
            float mainAxisOwnerSize = isMainAxisRow ? ownerWidth : ownerHeight;

            float childWidth = YGConstants.YGUndefined;
            float childHeight = YGConstants.YGUndefined;
            SizingMode childWidthSizingMode;
            SizingMode childHeightSizingMode;

            var resolvedFlexBasis = child.ResolveFlexBasis(
                direction,
                mainAxis,
                mainAxisOwnerSize,
                ownerWidth);
            bool isRowStyleDimDefined =
                child.HasDefiniteLength(Dimension.Width, ownerWidth);
            bool isColumnStyleDimDefined =
                child.HasDefiniteLength(Dimension.Height, ownerHeight);

            bool fixFlexBasisFitContent =
                node.Config.IsExperimentalFeatureEnabled(
                    ExperimentalFeature.FixFlexBasisFitContent);

            bool useResolvedFlexBasis =
                resolvedFlexBasis.IsDefined() && Comparison.IsDefined(mainAxisSize);

            if (fixFlexBasisFitContent && resolvedFlexBasis.IsDefined() &&
                resolvedFlexBasis.Unwrap() > 0)
            {
                useResolvedFlexBasis = true;
            }

            if (useResolvedFlexBasis)
            {
                if (child.Layout.ComputedFlexBasis.IsUndefined ||
                    (child.Config.IsExperimentalFeatureEnabled(
                         ExperimentalFeature.WebFlexBasis) &&
                     child.Layout.ComputedFlexBasisGeneration != generationCount))
                {
                    var paddingAndBorder = new FloatOptional(
                        Utils.PaddingAndBorderForAxis(child, mainAxis, direction, ownerWidth));
                    child.SetLayoutComputedFlexBasis(
                        Comparison.MaxOrDefined(resolvedFlexBasis, paddingAndBorder));
                }
            }
            else if (isMainAxisRow && isRowStyleDimDefined)
            {
                var paddingAndBorder = new FloatOptional(
                    Utils.PaddingAndBorderForAxis(
                        child, FlexDirection.Row, direction, ownerWidth));

                child.SetLayoutComputedFlexBasis(
                    Comparison.MaxOrDefined(
                        child.GetResolvedDimension(
                            direction, Dimension.Width, ownerWidth, ownerWidth),
                        paddingAndBorder));
            }
            else if (!isMainAxisRow && isColumnStyleDimDefined)
            {
                var paddingAndBorder = new FloatOptional(
                    Utils.PaddingAndBorderForAxis(
                        child, FlexDirection.Column, direction, ownerWidth));
                child.SetLayoutComputedFlexBasis(
                    Comparison.MaxOrDefined(
                        child.GetResolvedDimension(
                            direction, Dimension.Height, ownerHeight, ownerWidth),
                        paddingAndBorder));
            }
            else
            {
                childWidthSizingMode = SizingMode.MaxContent;
                childHeightSizingMode = SizingMode.MaxContent;

                float marginRow =
                    child.Style.ComputeMarginForAxis(FlexDirection.Row, ownerWidth);
                float marginColumn =
                    child.Style.ComputeMarginForAxis(FlexDirection.Column, ownerWidth);

                if (isRowStyleDimDefined)
                {
                    childWidth = child
                        .GetResolvedDimension(
                            direction, Dimension.Width, ownerWidth, ownerWidth)
                        .Unwrap() +
                        marginRow;
                    childWidthSizingMode = SizingMode.StretchFit;
                }
                if (isColumnStyleDimDefined)
                {
                    childHeight =
                        child
                            .GetResolvedDimension(
                                direction, Dimension.Height, ownerHeight, ownerWidth)
                            .Unwrap() +
                            marginColumn;
                    childHeightSizingMode = SizingMode.StretchFit;
                }

                if ((!isMainAxisRow && node.Style.Overflow() == Overflow.Scroll) ||
                    node.Style.Overflow() != Overflow.Scroll)
                {
                    if (Comparison.IsUndefined(childWidth) && Comparison.IsDefined(width))
                    {
                        childWidth = width;
                        childWidthSizingMode = SizingMode.FitContent;
                    }
                }

                bool applyHeightFitContent =
                    isMainAxisRow || node.Style.Overflow() != Overflow.Scroll;
                if (fixFlexBasisFitContent)
                {
                    applyHeightFitContent = isMainAxisRow ||
                        (child.HasMeasureFunc() &&
                         node.Style.Overflow() != Overflow.Scroll);
                }
                if (applyHeightFitContent && Comparison.IsUndefined(childHeight) &&
                    Comparison.IsDefined(height))
                {
                    childHeight = height;
                    childHeightSizingMode = SizingMode.FitContent;
                }

                ref readonly var childStyle = ref child.Style;
                if (childStyle.AspectRatio.IsDefined())
                {
                    if (!isMainAxisRow && childWidthSizingMode == SizingMode.StretchFit)
                    {
                        childHeight = marginColumn +
                            (childWidth - marginRow) / childStyle.AspectRatio().Unwrap();
                        childHeightSizingMode = SizingMode.StretchFit;
                    }
                    else if (
                        isMainAxisRow && childHeightSizingMode == SizingMode.StretchFit)
                    {
                        childWidth = marginRow +
                            (childHeight - marginColumn) * childStyle.AspectRatio().Unwrap();
                        childWidthSizingMode = SizingMode.StretchFit;
                    }
                }

                bool hasExactWidth =
                    Comparison.IsDefined(width) && widthMode == SizingMode.StretchFit;
                bool childWidthStretch =
                    AlignUtils.ResolveChildAlignment(node, child) == Align.Stretch &&
                    childWidthSizingMode != SizingMode.StretchFit;
                if (!isMainAxisRow && !isRowStyleDimDefined && hasExactWidth &&
                    childWidthStretch)
                {
                    childWidth = width;
                    childWidthSizingMode = SizingMode.StretchFit;
                    if (childStyle.AspectRatio.IsDefined())
                    {
                        childHeight =
                            (childWidth - marginRow) / childStyle.AspectRatio().Unwrap();
                        childHeightSizingMode = SizingMode.StretchFit;
                    }
                }

                bool hasExactHeight =
                    Comparison.IsDefined(height) && heightMode == SizingMode.StretchFit;
                bool childHeightStretch =
                    AlignUtils.ResolveChildAlignment(node, child) == Align.Stretch &&
                    childHeightSizingMode != SizingMode.StretchFit;
                if (isMainAxisRow && !isColumnStyleDimDefined && hasExactHeight &&
                    childHeightStretch)
                {
                    childHeight = height;
                    childHeightSizingMode = SizingMode.StretchFit;

                    if (childStyle.AspectRatio.IsDefined())
                    {
                        childWidth =
                            (childHeight - marginColumn) * childStyle.AspectRatio().Unwrap();
                        childWidthSizingMode = SizingMode.StretchFit;
                    }
                }

                ConstrainMaxSizeForMode(
                    child,
                    direction,
                    FlexDirection.Row,
                    ownerWidth,
                    ownerWidth,
                    ref childWidthSizingMode,
                    ref childWidth);
                ConstrainMaxSizeForMode(
                    child,
                    direction,
                    FlexDirection.Column,
                    ownerHeight,
                    ownerWidth,
                    ref childHeightSizingMode,
                    ref childHeight);

                CalculateLayoutInternal(
                    child,
                    childWidth,
                    childHeight,
                    direction,
                    childWidthSizingMode,
                    childHeightSizingMode,
                    ownerWidth,
                    ownerHeight,
                    false,
                    LayoutPassReason.kMeasureChild,
                    ref layoutMarkerData,
                    depth,
                    generationCount);

                child.SetLayoutComputedFlexBasis(new FloatOptional(
                    Comparison.MaxOrDefined(
                        child.Layout.MeasuredDimension(mainAxis.Dimension()),
                        Utils.PaddingAndBorderForAxis(child, mainAxis, direction, ownerWidth))));
            }
            child.SetLayoutComputedFlexBasisGeneration(generationCount);
        }

        private static void MeasureNodeWithMeasureFunc(
            Node node,
            Direction direction,
            float availableWidth,
            float availableHeight,
            SizingMode widthSizingMode,
            SizingMode heightSizingMode,
            float ownerWidth,
            float ownerHeight,
            ref LayoutData layoutMarkerData,
            LayoutPassReason reason)
        {
            Debug.AssertFatalWithNode(
                node,
                node.HasMeasureFunc(),
                "Expected node to have custom measure function");

            if (widthSizingMode == SizingMode.MaxContent)
            {
                availableWidth = YGConstants.YGUndefined;
            }
            if (heightSizingMode == SizingMode.MaxContent)
            {
                availableHeight = YGConstants.YGUndefined;
            }

            ref readonly var layout = ref node.Layout;
            float paddingAndBorderAxisRow = layout.Padding(PhysicalEdge.Left) +
                layout.Padding(PhysicalEdge.Right) + layout.Border(PhysicalEdge.Left) +
                layout.Border(PhysicalEdge.Right);
            float paddingAndBorderAxisColumn = layout.Padding(PhysicalEdge.Top) +
                layout.Padding(PhysicalEdge.Bottom) + layout.Border(PhysicalEdge.Top) +
                layout.Border(PhysicalEdge.Bottom);

            float innerWidth = Comparison.IsUndefined(availableWidth)
                ? availableWidth
                : Comparison.MaxOrDefined(0.0f, availableWidth - paddingAndBorderAxisRow);
            float innerHeight = Comparison.IsUndefined(availableHeight)
                ? availableHeight
                : Comparison.MaxOrDefined(0.0f, availableHeight - paddingAndBorderAxisColumn);

            if (widthSizingMode == SizingMode.StretchFit &&
                heightSizingMode == SizingMode.StretchFit)
            {
                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Row,
                        direction,
                        availableWidth,
                        ownerWidth,
                        ownerWidth),
                    Dimension.Width);
                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Column,
                        direction,
                        availableHeight,
                        ownerHeight,
                        ownerWidth),
                    Dimension.Height);
            }
            else
            {
                // TODO: Event.Publish for MeasureCallbackStart

                var measuredSize = node.Measure(
                    innerWidth,
                    widthSizingMode.ToMeasureMode(),
                    innerHeight,
                    heightSizingMode.ToMeasureMode());

                layoutMarkerData.MeasureCallbacks += 1;
                layoutMarkerData.MeasureCallbackReasonsCount[(size_t)reason] +=
                    1;

                // TODO: Event.Publish for MeasureCallbackEnd

                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Row,
                        direction,
                        (widthSizingMode == SizingMode.MaxContent ||
                         widthSizingMode == SizingMode.FitContent)
                            ? measuredSize.Width + paddingAndBorderAxisRow
                            : availableWidth,
                        ownerWidth,
                        ownerWidth),
                    Dimension.Width);

                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Column,
                        direction,
                        (heightSizingMode == SizingMode.MaxContent ||
                         heightSizingMode == SizingMode.FitContent)
                            ? measuredSize.Height + paddingAndBorderAxisColumn
                            : availableHeight,
                        ownerHeight,
                        ownerWidth),
                    Dimension.Height);
            }
        }

        private static void MeasureNodeWithoutChildren(
            Node node,
            Direction direction,
            float availableWidth,
            float availableHeight,
            SizingMode widthSizingMode,
            SizingMode heightSizingMode,
            float ownerWidth,
            float ownerHeight)
        {
            ref readonly var layout = ref node.Layout;

            float width = availableWidth;
            if (widthSizingMode == SizingMode.MaxContent ||
                widthSizingMode == SizingMode.FitContent)
            {
                width = layout.Padding(PhysicalEdge.Left) +
                    layout.Padding(PhysicalEdge.Right) +
                    layout.Border(PhysicalEdge.Left) + layout.Border(PhysicalEdge.Right);
            }
            node.SetLayoutMeasuredDimension(
                BoundAxis.BoundAxis(
                    node, FlexDirection.Row, direction, width, ownerWidth, ownerWidth),
                Dimension.Width);

            float height = availableHeight;
            if (heightSizingMode == SizingMode.MaxContent ||
                heightSizingMode == SizingMode.FitContent)
            {
                height = layout.Padding(PhysicalEdge.Top) +
                    layout.Padding(PhysicalEdge.Bottom) +
                    layout.Border(PhysicalEdge.Top) + layout.Border(PhysicalEdge.Bottom);
            }
            node.SetLayoutMeasuredDimension(
                BoundAxis.BoundAxis(
                    node,
                    FlexDirection.Column,
                    direction,
                    height,
                    ownerHeight,
                    ownerWidth),
                Dimension.Height);
        }

        internal static bool IsFixedSize(float dim, SizingMode sizingMode)
        {
            return sizingMode == SizingMode.StretchFit ||
                (Comparison.IsDefined(dim) && sizingMode == SizingMode.FitContent &&
                 dim <= 0.0);
        }

        private static bool MeasureNodeWithFixedSize(
            Node node,
            Direction direction,
            float availableWidth,
            float availableHeight,
            SizingMode widthSizingMode,
            SizingMode heightSizingMode,
            float ownerWidth,
            float ownerHeight)
        {
            if (IsFixedSize(availableWidth, widthSizingMode) &&
                IsFixedSize(availableHeight, heightSizingMode))
            {
                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Row,
                        direction,
                        Comparison.IsUndefined(availableWidth) ||
                                (widthSizingMode == SizingMode.FitContent &&
                                 availableWidth < 0.0f)
                            ? 0.0f
                            : availableWidth,
                        ownerWidth,
                        ownerWidth),
                    Dimension.Width);

                node.SetLayoutMeasuredDimension(
                    BoundAxis.BoundAxis(
                        node,
                        FlexDirection.Column,
                        direction,
                        Comparison.IsUndefined(availableHeight) ||
                                (heightSizingMode == SizingMode.FitContent &&
                                 availableHeight < 0.0f)
                            ? 0.0f
                            : availableHeight,
                        ownerHeight,
                        ownerWidth),
                    Dimension.Height);
                return true;
            }

            return false;
        }

        public static void ZeroOutLayoutRecursively(Node node)
        {
            node.Layout = new LayoutResults();
            node.SetLayoutDimension(0, Dimension.Width);
            node.SetLayoutDimension(0, Dimension.Height);
            node.SetHasNewLayout(true);

            node.CloneChildrenIfNeeded();
            foreach (var child in node.GetChildren())
            {
                ZeroOutLayoutRecursively(child);
            }
        }

        public static void CleanupContentsNodesRecursively(Node node)
        {
            if (node.HasContentsChildren())
            {
                node.CloneContentsChildrenIfNeeded();
                foreach (var child in node.GetChildren())
                {
                    if (child.Style.Display() == Display.Contents)
                    {
                        child.Layout = new LayoutResults();
                        child.SetLayoutDimension(0, Dimension.Width);
                        child.SetLayoutDimension(0, Dimension.Height);
                        child.SetHasNewLayout(true);
                        child.SetDirty(false);
                        child.CloneChildrenIfNeeded();

                        CleanupContentsNodesRecursively(child);
                    }
                }
            }
        }

        public static float CalculateAvailableInnerDimension(
            Node node,
            Direction direction,
            Dimension dimension,
            float availableDim,
            float paddingAndBorder,
            float ownerDim,
            float ownerWidth)
        {
            float availableInnerDim = availableDim - paddingAndBorder;
            if (Comparison.IsDefined(availableInnerDim))
            {
                var minDimensionOptional =
                    node.Style.ResolvedMinDimension(
                        direction, dimension, ownerDim, ownerWidth);
                float minInnerDim = minDimensionOptional.IsUndefined
                    ? 0.0f
                    : minDimensionOptional.Unwrap() - paddingAndBorder;

                var maxDimensionOptional =
                    node.Style.ResolvedMaxDimension(
                        direction, dimension, ownerDim, ownerWidth);

                float maxInnerDim = maxDimensionOptional.IsUndefined
                    ? float.MaxValue
                    : maxDimensionOptional.Unwrap() - paddingAndBorder;
                availableInnerDim = Comparison.MaxOrDefined(
                    Comparison.MinOrDefined(availableInnerDim, maxInnerDim), minInnerDim);
            }

            return availableInnerDim;
        }

        private static float ComputeFlexBasisForChildren(
            Node node,
            float availableInnerWidth,
            float availableInnerHeight,
            float ownerWidth,
            float ownerHeight,
            SizingMode widthSizingMode,
            SizingMode heightSizingMode,
            Direction direction,
            FlexDirection mainAxis,
            bool performLayout,
            ref LayoutData layoutMarkerData,
            uint depth,
            uint generationCount)
        {
            float totalOuterFlexBasis = 0.0f;
            Node singleFlexChild = null;
            var children = node.LayoutChildren;
            SizingMode sizingModeMainDim =
                mainAxis.IsRow() ? widthSizingMode : heightSizingMode;
            if (sizingModeMainDim == SizingMode.StretchFit)
            {
                foreach (var child in children)
                {
                    if (child.IsNodeFlexible())
                    {
                        if (singleFlexChild != null ||
                            Comparison.InexactEquals(child.ResolveFlexGrow(), 0.0f) ||
                            Comparison.InexactEquals(child.ResolveFlexShrink(), 0.0f))
                        {
                            singleFlexChild = null;
                            break;
                        }
                        else
                        {
                            singleFlexChild = child;
                        }
                    }
                }
            }

            foreach (var child in children)
            {
                child.ProcessDimensions();
                if (child.Style.Display() == Display.None)
                {
                    ZeroOutLayoutRecursively(child);
                    child.SetHasNewLayout(true);
                    child.SetDirty(false);
                    continue;
                }
                if (performLayout)
                {
                    Direction childDirection = child.ResolveDirection(direction);
                    child.SetPosition(
                        childDirection, availableInnerWidth, availableInnerHeight);
                }

                if (child.Style.PositionType() == PositionType.Absolute)
                {
                    continue;
                }
                if (child == singleFlexChild)
                {
                    child.SetLayoutComputedFlexBasisGeneration(generationCount);
                    child.SetLayoutComputedFlexBasis(new FloatOptional(0));
                }
                else
                {
                    ComputeFlexBasisForChild(
                        node,
                        child,
                        availableInnerWidth,
                        widthSizingMode,
                        availableInnerHeight,
                        ownerWidth,
                        ownerHeight,
                        heightSizingMode,
                        direction,
                        ref layoutMarkerData,
                        depth,
                        generationCount);
                }

                totalOuterFlexBasis +=
                    (child.Layout.ComputedFlexBasis.Unwrap() +
                     child.Style.ComputeMarginForAxis(mainAxis, availableInnerWidth));
            }

            return totalOuterFlexBasis;
        }

        private static float DistributeFreeSpaceSecondPass(
            FlexLine flexLine,
            Node node,
            FlexDirection mainAxis,
            FlexDirection crossAxis,
            Direction direction,
            float ownerWidth,
            float mainAxisOwnerSize,
            float availableInnerMainDim,
            float availableInnerCrossDim,
            float availableInnerWidth,
            float availableInnerHeight,
            bool mainAxisOverflows,
            SizingMode sizingModeCrossDim,
            bool performLayout,
            ref LayoutData layoutMarkerData,
            uint depth,
            uint generationCount)
        {
            float childFlexBasis = 0;
            float flexShrinkScaledFactor = 0;
            float flexGrowFactor = 0;
            float deltaFreeSpace = 0;
            bool isMainAxisRow = mainAxis.IsRow();
            bool isNodeFlexWrap = node.Style.FlexWrap() != Wrap.NoWrap;

            foreach (var currentLineChild in flexLine.ItemsInFlow)
            {
                childFlexBasis = BoundAxis.BoundAxisWithinMinAndMax(
                         currentLineChild,
                         direction,
                         mainAxis,
                         currentLineChild.Layout.ComputedFlexBasis,
                         mainAxisOwnerSize,
                         ownerWidth)
                         .Unwrap();
                float updatedMainSize = childFlexBasis;

                if (Comparison.IsDefined(flexLine.Layout.RemainingFreeSpace) &&
                    flexLine.Layout.RemainingFreeSpace < 0)
                {
                    flexShrinkScaledFactor =
                        -currentLineChild.ResolveFlexShrink() * childFlexBasis;
                    if (flexShrinkScaledFactor != 0)
                    {
                        float childSize = YGConstants.YGUndefined;

                        if (Comparison.IsDefined(flexLine.Layout.TotalFlexShrinkScaledFactors) &&
                            flexLine.Layout.TotalFlexShrinkScaledFactors == 0)
                        {
                            childSize = childFlexBasis + flexShrinkScaledFactor;
                        }
                        else
                        {
                            childSize = childFlexBasis +
                                (flexLine.Layout.RemainingFreeSpace /
                                 flexLine.Layout.TotalFlexShrinkScaledFactors) *
                                    flexShrinkScaledFactor;
                        }

                        updatedMainSize = BoundAxis.BoundAxis(
                            currentLineChild,
                            mainAxis,
                            direction,
                            childSize,
                            availableInnerMainDim,
                            availableInnerWidth);
                    }
                }
                else if (
                    Comparison.IsDefined(flexLine.Layout.RemainingFreeSpace) &&
                    flexLine.Layout.RemainingFreeSpace > 0)
                {
                    flexGrowFactor = currentLineChild.ResolveFlexGrow();

                    if (!float.IsNaN(flexGrowFactor) && flexGrowFactor != 0)
                    {
                        updatedMainSize = BoundAxis.BoundAxis(
                            currentLineChild,
                            mainAxis,
                            direction,
                            childFlexBasis +
                                flexLine.Layout.RemainingFreeSpace /
                                    flexLine.Layout.TotalFlexGrowFactors * flexGrowFactor,
                            availableInnerMainDim,
                            availableInnerWidth);
                    }
                }

                deltaFreeSpace += updatedMainSize - childFlexBasis;

                float marginMain = currentLineChild.Style.ComputeMarginForAxis(
                    mainAxis, availableInnerWidth);
                float marginCross = currentLineChild.Style.ComputeMarginForAxis(
                    crossAxis, availableInnerWidth);

                float childCrossSize = YGConstants.YGUndefined;
                float childMainSize = updatedMainSize + marginMain;
                SizingMode childCrossSizingMode;
                SizingMode childMainSizingMode = SizingMode.StretchFit;

                ref readonly var childStyle = ref currentLineChild.Style;
                if (childStyle.AspectRatio.IsDefined())
                {
                    childCrossSize = isMainAxisRow
                        ? (childMainSize - marginMain) / childStyle.AspectRatio().Unwrap()
                        : (childMainSize - marginMain) * childStyle.AspectRatio().Unwrap();
                    childCrossSizingMode = SizingMode.StretchFit;

                    childCrossSize += marginCross;
                }
                else if (
                    !float.IsNaN(availableInnerCrossDim) &&
                    !currentLineChild.HasDefiniteLength(
                        crossAxis.Dimension(), availableInnerCrossDim) &&
                    sizingModeCrossDim == SizingMode.StretchFit &&
                    !(isNodeFlexWrap && mainAxisOverflows) &&
                    AlignUtils.ResolveChildAlignment(node, currentLineChild) == Align.Stretch &&
                    !currentLineChild.Style.FlexStartMarginIsAuto(
                        crossAxis, direction) &&
                    !currentLineChild.Style.FlexEndMarginIsAuto(crossAxis, direction))
                {
                    childCrossSize = availableInnerCrossDim;
                    childCrossSizingMode = SizingMode.StretchFit;
                }
                else if (!currentLineChild.HasDefiniteLength(
                           crossAxis.Dimension(), availableInnerCrossDim))
                {
                    childCrossSize = availableInnerCrossDim;
                    childCrossSizingMode = Comparison.IsUndefined(childCrossSize)
                        ? SizingMode.MaxContent
                        : SizingMode.FitContent;
                }
                else
                {
                    childCrossSize = currentLineChild
                             .GetResolvedDimension(
                                 direction,
                                 crossAxis.Dimension(),
                                 availableInnerCrossDim,
                                 availableInnerWidth)
                             .Unwrap() +
                        marginCross;
                    bool isLoosePercentageMeasurement =
                        currentLineChild.ProcessedDimension(crossAxis.Dimension())
                            .IsPercent() &&
                        sizingModeCrossDim != SizingMode.StretchFit;
                    childCrossSizingMode =
                        Comparison.IsUndefined(childCrossSize) || isLoosePercentageMeasurement
                        ? SizingMode.MaxContent
                        : SizingMode.StretchFit;
                }

                ConstrainMaxSizeForMode(
                    currentLineChild,
                    direction,
                    mainAxis,
                    availableInnerMainDim,
                    availableInnerWidth,
                    ref childMainSizingMode,
                    ref childMainSize);
                ConstrainMaxSizeForMode(
                    currentLineChild,
                    direction,
                    crossAxis,
                    availableInnerCrossDim,
                    availableInnerWidth,
                    ref childCrossSizingMode,
                    ref childCrossSize);

                bool requiresStretchLayout =
                    !currentLineChild.HasDefiniteLength(
                        crossAxis.Dimension(), availableInnerCrossDim) &&
                    AlignUtils.ResolveChildAlignment(node, currentLineChild) == Align.Stretch &&
                    !currentLineChild.Style.FlexStartMarginIsAuto(
                        crossAxis, direction) &&
                    !currentLineChild.Style.FlexEndMarginIsAuto(crossAxis, direction);

                float childWidth = isMainAxisRow ? childMainSize : childCrossSize;
                float childHeight = !isMainAxisRow ? childMainSize : childCrossSize;

                SizingMode childWidthSizingMode =
                    isMainAxisRow ? childMainSizingMode : childCrossSizingMode;
                SizingMode childHeightSizingMode =
                    !isMainAxisRow ? childMainSizingMode : childCrossSizingMode;

                bool isLayoutPass = performLayout && !requiresStretchLayout;
                CalculateLayoutInternal(
                    currentLineChild,
                    childWidth,
                    childHeight,
                    node.Layout.Direction,
                    childWidthSizingMode,
                    childHeightSizingMode,
                    availableInnerWidth,
                    availableInnerHeight,
                    isLayoutPass,
                    isLayoutPass ? LayoutPassReason.kFlexLayout
                                 : LayoutPassReason.kFlexMeasure,
                    ref layoutMarkerData,
                    depth,
                    generationCount);
                node.SetLayoutHadOverflow(
                    node.Layout.HadOverflow() ||
                    currentLineChild.Layout.HadOverflow());
            }
            return deltaFreeSpace;
        }

        private static void DistributeFreeSpaceFirstPass(
            FlexLine flexLine,
            Direction direction,
            FlexDirection mainAxis,
            float ownerWidth,
            float mainAxisOwnerSize,
            float availableInnerMainDim,
            float availableInnerWidth)
        {
            float flexShrinkScaledFactor = 0;
            float flexGrowFactor = 0;
            float baseMainSize = 0;
            float boundMainSize = 0;
            float deltaFreeSpace = 0;

            foreach (var currentLineChild in flexLine.ItemsInFlow)
            {
                float childFlexBasis = BoundAxis.BoundAxisWithinMinAndMax(
                               currentLineChild,
                               direction,
                               mainAxis,
                               currentLineChild.Layout.ComputedFlexBasis,
                               mainAxisOwnerSize,
                               ownerWidth)
                               .Unwrap();

                if (flexLine.Layout.RemainingFreeSpace < 0)
                {
                    flexShrinkScaledFactor =
                        -currentLineChild.ResolveFlexShrink() * childFlexBasis;

                    if (Comparison.IsDefined(flexShrinkScaledFactor) &&
                        flexShrinkScaledFactor != 0)
                    {
                        baseMainSize = childFlexBasis +
                            flexLine.Layout.RemainingFreeSpace /
                                flexLine.Layout.TotalFlexShrinkScaledFactors *
                                flexShrinkScaledFactor;
                        boundMainSize = BoundAxis.BoundAxis(
                            currentLineChild,
                            mainAxis,
                            direction,
                            baseMainSize,
                            availableInnerMainDim,
                            availableInnerWidth);
                        if (Comparison.IsDefined(baseMainSize) && Comparison.IsDefined(boundMainSize) &&
                            baseMainSize != boundMainSize)
                        {
                            deltaFreeSpace += boundMainSize - childFlexBasis;
                            flexLine.Layout.TotalFlexShrinkScaledFactors -=
                                (-currentLineChild.ResolveFlexShrink() *
                                 currentLineChild.Layout.ComputedFlexBasis.Unwrap());
                        }
                    }
                }
                else if (
                    Comparison.IsDefined(flexLine.Layout.RemainingFreeSpace) &&
                    flexLine.Layout.RemainingFreeSpace > 0)
                {
                    flexGrowFactor = currentLineChild.ResolveFlexGrow();

                    if (Comparison.IsDefined(flexGrowFactor) && flexGrowFactor != 0)
                    {
                        baseMainSize = childFlexBasis +
                            flexLine.Layout.RemainingFreeSpace /
                                flexLine.Layout.TotalFlexGrowFactors * flexGrowFactor;
                        boundMainSize = BoundAxis.BoundAxis(
                            currentLineChild,
                            mainAxis,
                            direction,
                            baseMainSize,
                            availableInnerMainDim,
                            availableInnerWidth);

                        if (Comparison.IsDefined(baseMainSize) && Comparison.IsDefined(boundMainSize) &&
                            baseMainSize != boundMainSize)
                        {
                            deltaFreeSpace += boundMainSize - childFlexBasis;
                            flexLine.Layout.TotalFlexGrowFactors -= flexGrowFactor;
                        }
                    }
                }
            }
            flexLine.Layout.RemainingFreeSpace -= deltaFreeSpace;
        }

        private static void ResolveFlexibleLength(
            Node node,
            FlexLine flexLine,
            FlexDirection mainAxis,
            FlexDirection crossAxis,
            Direction direction,
            float ownerWidth,
            float mainAxisOwnerSize,
            float availableInnerMainDim,
            float availableInnerCrossDim,
            float availableInnerWidth,
            float availableInnerHeight,
            bool mainAxisOverflows,
            SizingMode sizingModeCrossDim,
            bool performLayout,
            ref LayoutData layoutMarkerData,
            uint depth,
            uint generationCount)
        {
            float originalFreeSpace = flexLine.Layout.RemainingFreeSpace;
            DistributeFreeSpaceFirstPass(
                flexLine,
                direction,
                mainAxis,
                ownerWidth,
                mainAxisOwnerSize,
                availableInnerMainDim,
                availableInnerWidth);

            float distributedFreeSpace = DistributeFreeSpaceSecondPass(
                flexLine,
                node,
                mainAxis,
                crossAxis,
                direction,
                ownerWidth,
                mainAxisOwnerSize,
                availableInnerMainDim,
                availableInnerCrossDim,
                availableInnerWidth,
                availableInnerHeight,
                mainAxisOverflows,
                sizingModeCrossDim,
                performLayout,
                ref layoutMarkerData,
                depth,
                generationCount);

            flexLine.Layout.RemainingFreeSpace = originalFreeSpace - distributedFreeSpace;
        }

        private static void JustifyMainAxis(
            Node node,
            FlexLine flexLine,
            FlexDirection mainAxis,
            FlexDirection crossAxis,
            Direction direction,
            SizingMode sizingModeMainDim,
            SizingMode sizingModeCrossDim,
            float mainAxisOwnerSize,
            float ownerWidth,
            float availableInnerMainDim,
            float availableInnerCrossDim,
            float availableInnerWidth,
            bool performLayout)
        {
            ref readonly var style = ref node.Style;

            float leadingPaddingAndBorderMain =
                node.Style.ComputeFlexStartPaddingAndBorder(
                    mainAxis, direction, ownerWidth);
            float trailingPaddingAndBorderMain =
                node.Style.ComputeFlexEndPaddingAndBorder(
                    mainAxis, direction, ownerWidth);

            float gap =
                node.Style.ComputeGapForAxis(mainAxis, availableInnerMainDim);
            if (sizingModeMainDim == SizingMode.FitContent &&
                flexLine.Layout.RemainingFreeSpace > 0)
            {
                if (style.MinDimension(mainAxis.Dimension()).IsDefined() &&
                    style
                        .ResolvedMinDimension(
                            direction, mainAxis.Dimension(), mainAxisOwnerSize, ownerWidth)
                        .IsDefined())
                {
                    float minAvailableMainDim =
                        style
                            .ResolvedMinDimension(
                                direction, mainAxis.Dimension(), mainAxisOwnerSize, ownerWidth)
                            .Unwrap() -
                            leadingPaddingAndBorderMain - trailingPaddingAndBorderMain;
                    float occupiedSpaceByChildNodes =
                        availableInnerMainDim - flexLine.Layout.RemainingFreeSpace;
                    flexLine.Layout.RemainingFreeSpace = Comparison.MaxOrDefined(
                        0.0f, minAvailableMainDim - occupiedSpaceByChildNodes);
                }
                else
                {
                    flexLine.Layout.RemainingFreeSpace = 0;
                }
            }

            float leadingMainDim = 0;
            float betweenMainDim = gap;
            Justify justifyContent = flexLine.Layout.RemainingFreeSpace >= 0
                ? node.Style.JustifyContent()
                : AlignUtils.FallbackAlignment(node.Style.JustifyContent());

            if (flexLine.NumberOfAutoMargins == 0)
            {
                switch (justifyContent)
                {
                    case Justify.Start:
                    case Justify.End:
                    case Justify.Auto:
                        break;
                    case Justify.Stretch:
                        break;
                    case Justify.Center:
                        leadingMainDim = flexLine.Layout.RemainingFreeSpace / 2;
                        break;
                    case Justify.FlexEnd:
                        leadingMainDim = flexLine.Layout.RemainingFreeSpace;
                        break;
                    case Justify.SpaceBetween:
                        if (flexLine.ItemsInFlow.Count > 1)
                        {
                            betweenMainDim += flexLine.Layout.RemainingFreeSpace /
                                (float)(flexLine.ItemsInFlow.Count - 1);
                        }
                        break;
                    case Justify.SpaceEvenly:
                        leadingMainDim = flexLine.Layout.RemainingFreeSpace /
                            (float)(flexLine.ItemsInFlow.Count + 1);
                        betweenMainDim += leadingMainDim;
                        break;
                    case Justify.SpaceAround:
                        leadingMainDim = 0.5f * flexLine.Layout.RemainingFreeSpace /
                            (float)(flexLine.ItemsInFlow.Count);
                        betweenMainDim += leadingMainDim * 2;
                        break;
                    case Justify.FlexStart:
                        break;
                }
            }

            flexLine.Layout.MainDim = leadingPaddingAndBorderMain + leadingMainDim;
            flexLine.Layout.CrossDim = 0;

            float maxAscentForCurrentLine = 0;
            float maxDescentForCurrentLine = 0;
            bool isNodeBaselineLayout = BaselineUtils.IsBaselineLayout(node);
            foreach (var child in flexLine.ItemsInFlow)
            {
                ref readonly var childLayout = ref child.Layout;
                if (child.Style.FlexStartMarginIsAuto(mainAxis, direction) &&
                    flexLine.Layout.RemainingFreeSpace > 0.0f)
                {
                    flexLine.Layout.MainDim += flexLine.Layout.RemainingFreeSpace /
                        (float)(flexLine.NumberOfAutoMargins);
                }

                if (performLayout)
                {
                    child.SetLayoutPosition(
                        childLayout.Position(mainAxis.FlexStartEdge()) +
                            flexLine.Layout.MainDim,
                        mainAxis.FlexStartEdge());
                }

                if (child != flexLine.ItemsInFlow[flexLine.ItemsInFlow.Count - 1])
                {
                    flexLine.Layout.MainDim += betweenMainDim;
                }

                if (child.Style.FlexEndMarginIsAuto(mainAxis, direction) &&
                    flexLine.Layout.RemainingFreeSpace > 0.0f)
                {
                    flexLine.Layout.MainDim += flexLine.Layout.RemainingFreeSpace /
                        (float)(flexLine.NumberOfAutoMargins);
                }
                bool canSkipFlex =
                    !performLayout && sizingModeCrossDim == SizingMode.StretchFit;
                if (canSkipFlex)
                {
                    flexLine.Layout.MainDim +=
                        child.Style.ComputeMarginForAxis(mainAxis, availableInnerWidth) +
                        BoundAxis.BoundAxisWithinMinAndMax(
                            child,
                            direction,
                            mainAxis,
                            childLayout.ComputedFlexBasis,
                            mainAxisOwnerSize,
                            ownerWidth)
                            .Unwrap();
                    flexLine.Layout.CrossDim = availableInnerCrossDim;
                }
                else
                {
                    flexLine.Layout.MainDim +=
                        child.DimensionWithMargin(mainAxis, availableInnerWidth);

                    if (isNodeBaselineLayout)
                    {
                        float ascent = BaselineUtils.CalculateBaseline(child) +
                            child.Style.ComputeFlexStartPaddingAndBorder(
                                FlexDirection.Column, direction, availableInnerWidth);
                        float descent =
                            child.Layout.MeasuredDimension(Dimension.Height) +
                            child.Style.ComputeMarginForAxis(
                                FlexDirection.Column, availableInnerWidth) -
                            ascent;

                        maxAscentForCurrentLine =
                            Comparison.MaxOrDefined(maxAscentForCurrentLine, ascent);
                        maxDescentForCurrentLine =
                            Comparison.MaxOrDefined(maxDescentForCurrentLine, descent);
                    }
                    else
                    {
                        flexLine.Layout.CrossDim = Comparison.MaxOrDefined(
                            flexLine.Layout.CrossDim,
                            child.DimensionWithMargin(crossAxis, availableInnerWidth));
                    }
                }
            }
            flexLine.Layout.MainDim += trailingPaddingAndBorderMain;

            if (isNodeBaselineLayout)
            {
                flexLine.Layout.CrossDim =
                    maxAscentForCurrentLine + maxDescentForCurrentLine;
            }
        }

        private static void CalculateLayoutImpl(
            Node node,
            float availableWidth,
            float availableHeight,
            Direction ownerDirection,
            SizingMode widthSizingMode,
            SizingMode heightSizingMode,
            float ownerWidth,
            float ownerHeight,
            bool performLayout,
            LayoutPassReason reason,
            ref LayoutData layoutMarkerData,
            uint depth,
            uint generationCount)
        {
            uint kDefaultGenerationCount = 0;
            FlexDirection mainAxis = node.Style.FlexDirection().ResolveDirection(ownerDirection);
            FlexDirection crossAxis = mainAxis.ToOpposite();
            bool isMainAxisRow = mainAxis.IsRow();
            SizingMode sizingModeMainDim =
                isMainAxisRow ? widthSizingMode : heightSizingMode;
            SizingMode sizingModeCrossDim =
                isMainAxisRow ? heightSizingMode : widthSizingMode;

            bool isNodeFlexWrap = node.Style.FlexWrap() != Wrap.NoWrap;

            float leadingPaddingAndBorderMain =
                node.Style.ComputeFlexStartPaddingAndBorder(mainAxis, ownerDirection, ownerWidth);
            float trailingPaddingAndBorderMain =
                node.Style.ComputeFlexEndPaddingAndBorder(mainAxis, ownerDirection, ownerWidth);
            float leadingPaddingAndBorderCross =
                node.Style.ComputeFlexStartPaddingAndBorder(crossAxis, ownerDirection, ownerWidth);
            float trailingPaddingAndBorderCross =
                node.Style.ComputeFlexEndPaddingAndBorder(crossAxis, ownerDirection, ownerWidth);

            float innerMainDim = isMainAxisRow
                ? availableWidth - leadingPaddingAndBorderMain - trailingPaddingAndBorderMain
                : availableHeight - leadingPaddingAndBorderMain - trailingPaddingAndBorderMain;
            float innerCrossDim = isMainAxisRow
                ? availableHeight - leadingPaddingAndBorderCross - trailingPaddingAndBorderCross
                : availableWidth - leadingPaddingAndBorderCross - trailingPaddingAndBorderCross;

            if (isMainAxisRow)
            {
                innerCrossDim = CalculateAvailableInnerDimension(
                    node,
                    ownerDirection,
                    Dimension.Height,
                    availableHeight - leadingPaddingAndBorderCross - trailingPaddingAndBorderCross,
                    leadingPaddingAndBorderCross + trailingPaddingAndBorderCross,
                    ownerHeight,
                    ownerWidth);
            }
            else
            {
                innerMainDim = CalculateAvailableInnerDimension(
                    node,
                    ownerDirection,
                    Dimension.Width,
                    availableWidth - leadingPaddingAndBorderMain - trailingPaddingAndBorderMain,
                    leadingPaddingAndBorderMain + trailingPaddingAndBorderMain,
                    ownerWidth,
                    ownerWidth);
            }

            if (node.HasMeasureFunc())
            {
                MeasureNodeWithMeasureFunc(
                    node,
                    ownerDirection,
                    availableWidth,
                    availableHeight,
                    widthSizingMode,
                    heightSizingMode,
                    ownerWidth,
                    ownerHeight,
                    ref layoutMarkerData,
                    reason);
                if (depth > 0)
                {
                    node.SetLayoutDimension(0, Dimension.Width);
                    node.SetLayoutDimension(0, Dimension.Height);
                }
                return;
            }

            if (node.GetChildren().Count == 0)
            {
                MeasureNodeWithoutChildren(
                    node,
                    ownerDirection,
                    availableWidth,
                    availableHeight,
                    widthSizingMode,
                    heightSizingMode,
                    ownerWidth,
                    ownerHeight);
                return;
            }

            if (MeasureNodeWithFixedSize(
                node,
                ownerDirection,
                availableWidth,
                availableHeight,
                widthSizingMode,
                heightSizingMode,
                ownerWidth,
                ownerHeight))
            {
                return;
            }

            node.CloneChildrenIfNeeded();
            node.MarkChildrenWithDisplayNone();

            float availableInnerWidth = !Comparison.IsUndefined(availableWidth)
                ? availableWidth - leadingPaddingAndBorderMain - trailingPaddingAndBorderMain
                : YGConstants.YGUndefined;
            float availableInnerHeight = !Comparison.IsUndefined(availableHeight)
                ? availableHeight - leadingPaddingAndBorderCross - trailingPaddingAndBorderCross
                : YGConstants.YGUndefined;

            float availableInnerMainDim = isMainAxisRow
                ? availableInnerWidth
                : availableInnerHeight;
            float availableInnerCrossDim = isMainAxisRow
                ? availableInnerHeight
                : availableInnerWidth;

            float maxContentMainDim = isMainAxisRow
                ? node.Style.MaxDimension(Dimension.Width).IsDefined
                    ? node.Style.ResolvedMaxDimension(ownerDirection, Dimension.Width, ownerWidth, ownerWidth).Unwrap()
                    : float.MaxValue
                : node.Style.MaxDimension(Dimension.Height).IsDefined
                    ? node.Style.ResolvedMaxDimension(ownerDirection, Dimension.Height, ownerHeight, ownerWidth).Unwrap()
                    : float.MaxValue;
            float maxContentCrossDim = !isMainAxisRow
                ? node.Style.MaxDimension(Dimension.Width).IsDefined
                    ? node.Style.ResolvedMaxDimension(ownerDirection, Dimension.Width, ownerWidth, ownerWidth).Unwrap()
                    : float.MaxValue
                : node.Style.MaxDimension(Dimension.Height).IsDefined
                    ? node.Style.ResolvedMaxDimension(ownerDirection, Dimension.Height, ownerHeight, ownerWidth).Unwrap()
                    : float.MaxValue;

            if (Comparison.IsUndefined(availableInnerMainDim))
            {
                availableInnerMainDim = maxContentMainDim;
                if (Comparison.IsUndefined(availableInnerCrossDim))
                {
                    availableInnerCrossDim = maxContentCrossDim;
                }
            }
            else if (Comparison.IsUndefined(availableInnerCrossDim))
            {
                availableInnerCrossDim = maxContentCrossDim;
            }

            float flexBasisOuter = ComputeFlexBasisForChildren(
                node,
                availableInnerWidth,
                availableInnerHeight,
                ownerWidth,
                ownerHeight,
                widthSizingMode,
                heightSizingMode,
                ownerDirection,
                mainAxis,
                performLayout,
                ref layoutMarkerData,
                depth,
                generationCount);

            float minInnerMainDim = node.Style.ResolvedMinDimension(
                    ownerDirection, mainAxis.Dimension(), ownerWidth, ownerWidth)
                .IsDefined
                ? node.Style.ResolvedMinDimension(
                      ownerDirection, mainAxis.Dimension(), ownerWidth, ownerWidth)
                    .Unwrap()
                : 0.0f;
            float maxInnerMainDim = node.Style.ResolvedMaxDimension(
                    ownerDirection, mainAxis.Dimension(), ownerWidth, ownerWidth)
                .IsDefined
                ? node.Style.ResolvedMaxDimension(
                      ownerDirection, mainAxis.Dimension(), ownerWidth, ownerWidth)
                    .Unwrap()
                : float.MaxValue;

            flexBasisOuter = Comparison.MaxOrDefined(flexBasisOuter, minInnerMainDim);
            flexBasisOuter = Comparison.MinOrDefined(flexBasisOuter, maxInnerMainDim);

            bool mainAxisOverflows = false;
            if (Comparison.IsDefined(availableInnerMainDim))
            {
                mainAxisOverflows = flexBasisOuter > availableInnerMainDim;
            }
            else if (sizingModeMainDim == SizingMode.MaxContent &&
                     node.Style.Overflow() != Overflow.Visible)
            {
                mainAxisOverflows = maxContentMainDim < flexBasisOuter;
            }

            float initialMainDim = flexBasisOuter + leadingPaddingAndBorderMain +
                trailingPaddingAndBorderMain;
            node.SetLayoutComputedMainDimension(initialMainDim, mainAxis);
            node.SetLayoutComputedCrossDimension(
                isMainAxisRow ? availableHeight : availableWidth, crossAxis);

            var flexLines = FlexLine.FlexLineCalculate(
                node,
                availableInnerMainDim,
                availableInnerCrossDim,
                availableInnerWidth,
                availableInnerHeight,
                ownerDirection,
                mainAxis,
                flexBasisOuter,
                ref layoutMarkerData,
                depth,
                generationCount);

            if (flexLines == null || flexLines.Count == 0)
            {
                return;
            }

            bool isMainAxisOverflowing = false;
            bool isCrossAxisOverflowing = false;

            if (Comparison.IsDefined(availableInnerMainDim))
            {
                isMainAxisOverflowing = flexBasisOuter > availableInnerMainDim;
            }
            else if (sizingModeMainDim == SizingMode.MaxContent &&
                     node.Style.Overflow() != Overflow.Visible)
            {
                isMainAxisOverflowing = maxContentMainDim < flexBasisOuter;
            }

            for (int i = 0; i < flexLines.Count; i++)
            {
                var flexLine = flexLines[i];

                float flexLineInnerMainDim = availableInnerMainDim;
                if (Comparison.IsDefined(availableInnerMainDim))
                {
                    flexLine.Layout.RemainingFreeSpace = availableInnerMainDim;
                    for (int j = 0; j < flexLine.ItemsInFlow.Count; j++)
                    {
                        var child = flexLine.ItemsInFlow[j];
                        child.SetLayoutHadOverflow(false);
                        flexLine.Layout.RemainingFreeSpace -=
                            (child.Layout.ComputedFlexBasis.Unwrap() +
                             child.Style.ComputeMarginForAxis(mainAxis, availableInnerWidth));
                    }
                    flexLine.Layout.RemainingFreeSpace -=
                        node.Style.ComputeGapForAxis(mainAxis, flexLineInnerMainDim) *
                        (flexLine.ItemsInFlow.Count - 1);
                }
                else
                {
                    flexLine.Layout.RemainingFreeSpace = Comparison.IsDefined(maxContentMainDim)
                        ? maxContentMainDim - flexBasisOuter
                        : float.MaxValue;
                }

                ResolveFlexibleLength(
                    node,
                    flexLine,
                    mainAxis,
                    crossAxis,
                    ownerDirection,
                    ownerWidth,
                    mainAxisOwnerSize: 0.0f,
                    availableInnerMainDim,
                    availableInnerCrossDim,
                    availableInnerWidth,
                    availableInnerHeight,
                    mainAxisOverflows,
                    sizingModeCrossDim,
                    performLayout,
                    ref layoutMarkerData,
                    depth,
                    generationCount);

                if (performLayout)
                {
                    bool nodeIsOverflowing = flexLine.Layout.RemainingFreeSpace < 0;
                    node.SetLayoutHadOverflow(node.Layout.HadOverflow() || nodeIsOverflowing);
                }

                JustifyMainAxis(
                    node,
                    flexLine,
                    mainAxis,
                    crossAxis,
                    ownerDirection,
                    sizingModeMainDim,
                    sizingModeCrossDim,
                    flexLine.Layout.MainDim,
                    ownerWidth,
                    availableInnerMainDim,
                    availableInnerCrossDim,
                    availableInnerWidth,
                    performLayout);
            }

            float totalInnerCrossDim = 0;
            float maxBaseline = 0;
            foreach (var flexLine in flexLines)
            {
                totalInnerCrossDim += flexLine.Layout.CrossDim +
                    flexLine.Layout.MainDim + flexLine.Layout.Gap;
                maxBaseline = Comparison.MaxOrDefined(maxBaseline, flexLine.Layout.MaxBaseline);
            }

            if (Comparison.IsUndefined(availableInnerCrossDim))
            {
                availableInnerCrossDim = node.Style.ResolvedMaxDimension(
                        ownerDirection, crossAxis.Dimension(), ownerHeight, ownerWidth)
                    .IsDefined
                    ? node.Style.ResolvedMaxDimension(
                          ownerDirection, crossAxis.Dimension(), ownerHeight, ownerWidth)
                        .Unwrap()
                    : maxContentCrossDim;
            }

            totalInnerCrossDim += leadingPaddingAndBorderCross + trailingPaddingAndBorderCross;

            if (performLayout)
            {
                node.SetLayoutDimension(totalInnerCrossDim, crossAxis.Dimension());
            }
            else
            {
                float dimension = BoundAxis.BoundAxis(
                    node,
                    crossAxis,
                    ownerDirection,
                    totalInnerCrossDim,
                    availableInnerCrossDim,
                    availableInnerWidth);
                node.SetLayoutMeasuredDimension(dimension, crossAxis.Dimension());
            }

            for (int i = 0; i < flexLines.Count; i++)
            {
                var flexLine = flexLines[i];
                if (performLayout)
                {
                    AbsoluteLayout.ResolveFlexibleLength(
                        node,
                        flexLine,
                        mainAxis,
                        crossAxis,
                        ownerDirection,
                        ownerWidth,
                        ownerHeight,
                        availableInnerMainDim,
                        availableInnerCrossDim,
                        availableInnerWidth,
                        availableInnerHeight,
                        sizingModeMainDim,
                        sizingModeCrossDim,
                        availableInnerMainDim,
                        availableInnerCrossDim,
                        ref layoutMarkerData,
                        depth,
                        generationCount,
                        i,
                        flexLines.Count);
                }
                else
                {
                    AbsoluteLayout.ResolveFlexibleLength(
                        node,
                        flexLine,
                        mainAxis,
                        crossAxis,
                        ownerDirection,
                        ownerWidth,
                        ownerHeight,
                        availableInnerMainDim,
                        availableInnerCrossDim,
                        availableInnerWidth,
                        availableInnerHeight,
                        sizingModeMainDim,
                        sizingModeCrossDim,
                        availableInnerMainDim,
                        availableInnerCrossDim,
                        ref layoutMarkerData,
                        depth,
                        generationCount,
                        i,
                        flexLines.Count);
                }
            }

            if (performLayout)
            {
                bool needsReset = true;
                for (int i = 0; i < flexLines.Count && needsReset; i++)
                {
                    var flexLine = flexLines[i];
                    if (flexLine.Layout.RemainingFreeSpace != 0 ||
                        flexLine.Layout.MainDim != flexLine.Layout.MainDim)
                    {
                        needsReset = false;
                        break;
                    }
                }

                if (needsReset)
                {
                    for (int i = 0; i < node.GetChildren().Count; i++)
                    {
                        var child = node.GetChildren()[i];
                        child.SetLayoutPosition(0, mainAxis.FlexStartEdge());
                        child.SetLayoutPosition(0, crossAxis.FlexStartEdge());
                    }
                }
            }
        }

    //
    // This is a wrapper around the calculateLayoutImpl function. It determines
    // whether the layout request is redundant and can be skipped.
    //
    // Parameters:
    //  Input parameters are the same as calculateLayoutImpl (see above)
    //  Return parameter is true if layout was performed, false if skipped
    //
    public static bool calculateLayoutInternal(
        Node node,
        float availableWidth,
        float availableHeight,
        Direction ownerDirection,
        SizingMode widthSizingMode,
        SizingMode heightSizingMode,
        float ownerWidth,
        float ownerHeight,
        bool performLayout,
        LayoutPassReason reason,
        LayoutData layoutMarkerData,
        uint depth,
        uint generationCount)
    {
        LayoutResults layout = node.GetLayout();

        depth++;

        bool needToVisitNode =
            (node.IsDirty() && layout.GenerationCount != generationCount) ||
            layout.ConfigVersion != node.GetConfig().getVersion() ||
            layout.LastOwnerDirection != ownerDirection;

        if (needToVisitNode)
        {
            // Invalidate the cached results.
            layout.NextCachedMeasurementsIndex = 0;
            layout.CachedLayout.AvailableWidth = -1;
            layout.CachedLayout.AvailableHeight = -1;
            layout.CachedLayout.WidthSizingMode = SizingMode.MaxContent;
            layout.CachedLayout.HeightSizingMode = SizingMode.MaxContent;
            layout.CachedLayout.ComputedWidth = -1;
            layout.CachedLayout.ComputedHeight = -1;
        }

        CachedMeasurement cachedResults = null;

        // Determine whether the results are already cached. We maintain a separate
        // cache for layouts and measurements. A layout operation modifies the
        // positions and dimensions for nodes in the subtree. The algorithm assumes
        // that each node gets laid out a maximum of one time per tree layout, but
        // multiple measurements may be required to resolve all of the flex
        // dimensions. We handle nodes with measure functions specially here because
        // they are the most expensive to measure, so it's worth avoiding redundant
        // measurements if at all possible.
        if (node.HasMeasureFunc())
        {
            float marginAxisRow = node.Style.computeMarginForAxis(FlexDirection.Row, ownerWidth);
            float marginAxisColumn = node.Style.computeMarginForAxis(FlexDirection.Column, ownerWidth);

            // First, try to use the layout cache.
            if (Cache.CanUseCachedMeasurement(
                    widthSizingMode,
                    availableWidth,
                    heightSizingMode,
                    availableHeight,
                    layout.CachedLayout.WidthSizingMode,
                    layout.CachedLayout.AvailableWidth,
                    layout.CachedLayout.HeightSizingMode,
                    layout.CachedLayout.AvailableHeight,
                    layout.CachedLayout.ComputedWidth,
                    layout.CachedLayout.ComputedHeight,
                    marginAxisRow,
                    marginAxisColumn,
                    node.GetConfig()))
            {
                cachedResults = layout.CachedLayout;
            }
            else
            {
                // Try to use the measurement cache.
                for (int i = 0; i < layout.NextCachedMeasurementsIndex; i++)
                {
                    if (Cache.CanUseCachedMeasurement(
                            widthSizingMode,
                            availableWidth,
                            heightSizingMode,
                            availableHeight,
                            layout.CachedMeasurements[i].WidthSizingMode,
                            layout.CachedMeasurements[i].AvailableWidth,
                            layout.CachedMeasurements[i].HeightSizingMode,
                            layout.CachedMeasurements[i].AvailableHeight,
                            layout.CachedMeasurements[i].ComputedWidth,
                            layout.CachedMeasurements[i].ComputedHeight,
                            marginAxisRow,
                            marginAxisColumn,
                            node.GetConfig()))
                    {
                        cachedResults = layout.CachedMeasurements[i];
                        break;
                    }
                }
            }
        }
        else if (performLayout)
        {
            if (Comparison.InexactEquals(layout.CachedLayout.AvailableWidth, availableWidth) &&
                Comparison.InexactEquals(layout.CachedLayout.AvailableHeight, availableHeight) &&
                layout.CachedLayout.WidthSizingMode == widthSizingMode &&
                layout.CachedLayout.HeightSizingMode == heightSizingMode)
            {
                cachedResults = layout.CachedLayout;
            }
        }
        else
        {
            for (uint i = 0; i < layout.NextCachedMeasurementsIndex; i++)
            {
                if (Comparison.InexactEquals(layout.CachedMeasurements[i].AvailableWidth, availableWidth) &&
                    Comparison.InexactEquals(layout.CachedMeasurements[i].AvailableHeight, availableHeight) &&
                    layout.CachedMeasurements[i].WidthSizingMode == widthSizingMode &&
                    layout.CachedMeasurements[i].HeightSizingMode == heightSizingMode)
                {
                    cachedResults = layout.CachedMeasurements[i];
                    break;
                }
            }
        }

        if (!needToVisitNode && cachedResults != null)
        {
            layout.SetMeasuredDimension(Dimension.Width, cachedResults.ComputedWidth);
            layout.SetMeasuredDimension(Dimension.Height, cachedResults.ComputedHeight);

            if (performLayout)
                layoutMarkerData.CachedLayouts += 1;
            else
                layoutMarkerData.CachedMeasures += 1;
        }
        else
        {
            CalculateLayoutImpl(
                node,
                availableWidth,
                availableHeight,
                ownerDirection,
                widthSizingMode,
                heightSizingMode,
                ownerWidth,
                ownerHeight,
                performLayout,
                reason,
                layoutMarkerData,
                depth,
                generationCount);

            layout.LastOwnerDirection = ownerDirection;
            layout.ConfigVersion = node.GetConfig().getVersion();

            if (cachedResults == null)
            {
                layoutMarkerData.MaxMeasureCache = Math.Max(
                    layoutMarkerData.MaxMeasureCache,
                    layout.NextCachedMeasurementsIndex + 1u);

                if (layout.NextCachedMeasurementsIndex == LayoutResults.MaxCachedMeasurements)
                {
                    layout.NextCachedMeasurementsIndex = 0;
                }

                CachedMeasurement newCacheEntry = null;
                if (performLayout)
                {
                    // Use the single layout cache entry.
                    newCacheEntry = layout.CachedLayout;
                }
                else
                {
                    // Allocate a new measurement cache entry.
                    newCacheEntry = layout.CachedMeasurements[layout.NextCachedMeasurementsIndex];
                    layout.NextCachedMeasurementsIndex++;
                }

                newCacheEntry.AvailableWidth = availableWidth;
                newCacheEntry.AvailableHeight = availableHeight;
                newCacheEntry.WidthSizingMode = widthSizingMode;
                newCacheEntry.HeightSizingMode = heightSizingMode;
                newCacheEntry.ComputedWidth = layout.MeasuredDimension(Dimension.Width);
                newCacheEntry.ComputedHeight = layout.MeasuredDimension(Dimension.Height);
            }
        }

        if (performLayout)
        {
            node.SetLayoutDimension(
                node.GetLayout().MeasuredDimension(Dimension.Width),
                Dimension.Width);
            node.SetLayoutDimension(
                node.GetLayout().MeasuredDimension(Dimension.Height),
                Dimension.Height);

            node.SetHasNewLayout(true);
            node.SetDirty(false);
        }

        layout.GenerationCount = generationCount;

        LayoutType layoutType;
        if (performLayout)
        {
            layoutType = !needToVisitNode && cachedResults == layout.CachedLayout
                ? LayoutType.kCachedLayout
                : LayoutType.kLayout;
        }
        else
        {
            layoutType = cachedResults != null ? LayoutType.kCachedMeasure : LayoutType.kMeasure;
        }
        // TODO: Event.Publish for NodeLayout

        return (needToVisitNode || cachedResults == null);
    }

    public static void calculateLayout(
        Node node,
        float ownerWidth,
        float ownerHeight,
        Direction ownerDirection)
    {
        // TODO: Event.Publish for LayoutPassStart
        LayoutData markerData = new LayoutData();

        // Increment the generation count. This will force the recursive routine to
        // visit all dirty nodes at least once. Subsequent visits will be skipped if
        // the input parameters don't change.
        Interlocked.Increment(ref gCurrentGenerationCount);
        node.processDimensions();
        Direction direction = node.resolveDirection(ownerDirection);
        float width = YogaGlobal.YGUndefined;
        SizingMode widthSizingMode = SizingMode.MaxContent;
        YogaStyle style = node.Style;
        if (node.hasDefiniteLength(Dimension.Width, ownerWidth))
        {
            width =
                (node.getResolvedDimension(
                         direction,
                         YogaGlobal.dimension(FlexDirection.Row),
                         ownerWidth,
                         ownerWidth)
                     .unwrap() +
                 node.Style.computeMarginForAxis(FlexDirection.Row, ownerWidth));
            widthSizingMode = SizingMode.StretchFit;
        }
        else if (style.resolvedMaxDimension(direction, Dimension.Width, ownerWidth, ownerWidth).isDefined())
        {
            width = style.resolvedMaxDimension(direction, Dimension.Width, ownerWidth, ownerWidth).unwrap();
            widthSizingMode = SizingMode.FitContent;
        }
        else
        {
            width = ownerWidth;
            widthSizingMode = Comparison.IsUndefined(width) ? SizingMode.MaxContent : SizingMode.StretchFit;
        }

        float height = YogaGlobal.YGUndefined;
        SizingMode heightSizingMode = SizingMode.MaxContent;
        if (node.hasDefiniteLength(Dimension.Height, ownerHeight))
        {
            height =
                (node.getResolvedDimension(
                         direction,
                         YogaGlobal.dimension(FlexDirection.Column),
                         ownerHeight,
                         ownerWidth)
                     .unwrap() +
                 node.Style.computeMarginForAxis(FlexDirection.Column, ownerWidth));
            heightSizingMode = SizingMode.StretchFit;
        }
        else if (style.resolvedMaxDimension(direction, Dimension.Height, ownerHeight, ownerWidth).isDefined())
        {
            height = style.resolvedMaxDimension(direction, Dimension.Height, ownerHeight, ownerWidth).unwrap();
            heightSizingMode = SizingMode.FitContent;
        }
        else
        {
            height = ownerHeight;
            heightSizingMode = Comparison.IsUndefined(height) ? SizingMode.MaxContent : SizingMode.StretchFit;
        }
        if (calculateLayoutInternal(
                node,
                width,
                height,
                ownerDirection,
                widthSizingMode,
                heightSizingMode,
                ownerWidth,
                ownerHeight,
                true,
                LayoutPassReason.kInitial,
                markerData,
                0, // tree root
                (uint)gCurrentGenerationCount))
        {
            node.setPosition(node.GetLayout().direction(), ownerWidth, ownerHeight);
            roundLayoutResultsToPixelGrid(node, 0.0f, 0.0f);
        }

        // TODO: Event.Publish for LayoutPassEnd
    }
    }
}

