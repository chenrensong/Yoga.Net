using System;
using System.Collections.Generic;

namespace Facebook.Yoga
{

    public class FlexLineRunningLayout
    {
        public float TotalFlexGrowFactors { get; set; } = 0.0f;
        public float TotalFlexShrinkScaledFactors { get; set; } = 0.0f;
        public float RemainingFreeSpace { get; set; } = 0.0f;
        public float MainDim { get; set; } = 0.0f;
        public float CrossDim { get; set; } = 0.0f;

        public FlexLineRunningLayout() { }

        public FlexLineRunningLayout(float totalFlexGrowFactors, float totalFlexShrinkScaledFactors)
        {
            TotalFlexGrowFactors = totalFlexGrowFactors;
            TotalFlexShrinkScaledFactors = totalFlexShrinkScaledFactors;
        }
    }

    public class FlexLine
    {
        public readonly IReadOnlyList<Node> ItemsInFlow;
        public readonly float SizeConsumed;
        public readonly int NumberOfAutoMargins;
        public FlexLineRunningLayout Layout { get; set; }

        public FlexLine(IReadOnlyList<Node> itemsInFlow, float sizeConsumed, int numberOfAutoMargins, FlexLineRunningLayout layout)
        {
            ItemsInFlow = itemsInFlow;
            SizeConsumed = sizeConsumed;
            NumberOfAutoMargins = numberOfAutoMargins;
            Layout = layout;
        }
    }

    public static partial class YogaAlgorithm
    {
        public static FlexLine CalculateFlexLine(
            Node node,
            Direction ownerDirection,
            float ownerWidth,
            float mainAxisOwnerSize,
            float availableInnerWidth,
            float availableInnerMainDim,
            IEnumerator<Node> iterator,
            int lineCount)
        {
            var itemsInFlow = new List<Node>((int)node.GetChildCount());

            float sizeConsumed = 0.0f;
            float totalFlexGrowFactors = 0.0f;
            float totalFlexShrinkScaledFactors = 0.0f;
            int numberOfAutoMargins = 0;
            Node? firstElementInLine = null;

            float sizeConsumedIncludingMinConstraint = 0;
            Direction direction = node.ResolveDirection(ownerDirection);
            FlexDirection mainAxis = node.Style.FlexDirection.ResolveDirection(direction);
            bool isNodeFlexWrap = node.Style.FlexWrap != Wrap.NoWrap;
            float gap = node.Style.ComputeGapForAxis(mainAxis, availableInnerMainDim);

            while (iterator.MoveNext())
            {
                var child = iterator.Current;
                if (child.Style.Display == Display.None ||
                    child.Style.PositionType == PositionType.Absolute)
                {
                    continue;
                }

                if (firstElementInLine == null)
                {
                    firstElementInLine = child;
                }

                if (child.Style.FlexStartMarginIsAuto(mainAxis, ownerDirection))
                {
                    numberOfAutoMargins++;
                }
                if (child.Style.FlexEndMarginIsAuto(mainAxis, ownerDirection))
                {
                    numberOfAutoMargins++;
                }

                child.SetLineIndex((nuint)lineCount);
                float childMarginMainAxis = child.Style.ComputeMarginForAxis(mainAxis, availableInnerWidth);
                float childLeadingGapMainAxis = child == firstElementInLine ? 0.0f : gap;
                
                float flexBasisWithMinAndMaxConstraints = BoundAxis.BoundAxisWithinMinAndMax(
                    child,
                    direction,
                    mainAxis,
                    child.Layout.ComputedFlexBasis,
                    mainAxisOwnerSize,
                    ownerWidth).Unwrap();

                if (sizeConsumedIncludingMinConstraint + flexBasisWithMinAndMaxConstraints +
                            childMarginMainAxis + childLeadingGapMainAxis >
                        availableInnerMainDim &&
                    isNodeFlexWrap && itemsInFlow.Count > 0)
                {
                    break;
                }

                sizeConsumedIncludingMinConstraint += flexBasisWithMinAndMaxConstraints +
                    childMarginMainAxis + childLeadingGapMainAxis;
                sizeConsumed += flexBasisWithMinAndMaxConstraints + childMarginMainAxis +
                    childLeadingGapMainAxis;

                if (child.IsNodeFlexible())
                {
                    totalFlexGrowFactors += child.ResolveFlexGrow();
                    totalFlexShrinkScaledFactors += -child.ResolveFlexShrink() *
                        child.Layout.ComputedFlexBasis.Unwrap();
                }

                itemsInFlow.Add(child);
            }

            if (totalFlexGrowFactors > 0 && totalFlexGrowFactors < 1)
            {
                totalFlexGrowFactors = 1;
            }

            if (totalFlexShrinkScaledFactors > 0 && totalFlexShrinkScaledFactors < 1)
            {
                totalFlexShrinkScaledFactors = 1;
            }

            return new FlexLine(
                itemsInFlow,
                sizeConsumed,
                numberOfAutoMargins,
                new FlexLineRunningLayout(totalFlexGrowFactors, totalFlexShrinkScaledFactors)
            );
        }
    }
}

