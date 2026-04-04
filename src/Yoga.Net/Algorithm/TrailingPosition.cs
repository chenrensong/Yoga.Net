// Copyright (c) Meta Platforms, Inc. and affiliates.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

using Facebook.Yoga;

namespace Facebook.Yoga.Algorithm
{
    public static class TrailingPosition
    {
        // Given an offset to an edge, returns the offset to the opposite edge on the
        // same axis. This assumes that the width/height of both nodes is determined at
        // this point.
        public static float GetPositionOfOppositeEdge(
            float position,
            FlexDirection axis,
            Node containingNode,
            Node node)
        {
            return containingNode.Layout.MeasuredDimension(FlexDirectionUtil.Dimension(axis)) -
                node.Layout.MeasuredDimension(FlexDirectionUtil.Dimension(axis)) - position;
        }

        public static void SetChildTrailingPosition(
            Node node,
            Node child,
            FlexDirection axis)
        {
            child.SetLayoutPosition(
                GetPositionOfOppositeEdge(
                    child.Layout.Position(FlexDirectionUtil.FlexStartEdge(axis)),
                    axis,
                    node,
                    child),
                FlexDirectionUtil.FlexEndEdge(axis));
        }

        public static bool NeedsTrailingPosition(FlexDirection axis)
        {
            return axis == FlexDirection.RowReverse ||
                axis == FlexDirection.ColumnReverse;
        }
    }
}

