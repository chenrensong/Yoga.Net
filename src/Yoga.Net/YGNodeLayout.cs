// Copyright (c) Meta Platforms, Inc. and affiliates.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

namespace Yoga
{
    public enum YGDirection
    {
        Inherit,
        LTR,
        RTL
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

    internal enum Edge
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

    internal enum PhysicalEdge
    {
        Left,
        Top,
        Right,
        Bottom
    }

    internal enum Dimension
    {
        Width,
        Height
    }

    internal class LayoutResults
    {
        private float[] _position = new float[4];
        private float[] _dimension = new float[2];
        private float[] _rawDimension = new float[2];
        private float[] _margin = new float[4];
        private float[] _border = new float[4];
        private float[] _padding = new float[4];
        private YGDirection _direction;
        private bool _hadOverflow;

        public float position(PhysicalEdge edge) => _position[(int)edge];
        public float dimension(Dimension dim) => _dimension[(int)dim];
        public float rawDimension(Dimension dim) => _rawDimension[(int)dim];
        public float margin(PhysicalEdge edge) => _margin[(int)edge];
        public float border(PhysicalEdge edge) => _border[(int)edge];
        public float padding(PhysicalEdge edge) => _padding[(int)edge];
        public YGDirection direction() => _direction;
        public bool hadOverflow() => _hadOverflow;

        internal void SetPosition(PhysicalEdge edge, float value) => _position[(int)edge] = value;
        internal void SetDimension(Dimension dim, float value) => _dimension[(int)dim] = value;
        internal void SetRawDimension(Dimension dim, float value) => _rawDimension[(int)dim] = value;
        internal void SetMargin(PhysicalEdge edge, float value) => _margin[(int)edge] = value;
        internal void SetBorder(PhysicalEdge edge, float value) => _border[(int)edge] = value;
        internal void SetPadding(PhysicalEdge edge, float value) => _padding[(int)edge] = value;
        internal void SetDirection(YGDirection value) => _direction = value;
        internal void SetHadOverflow(bool value) => _hadOverflow = value;
    }

    public class YogaNode
    {
        private LayoutResults _layout = new LayoutResults();

        internal LayoutResults getLayout() => _layout;

        internal static void AssertFatalWithNode(YogaNode node, bool condition, string message)
        {
            if (!condition)
            {
                throw new System.InvalidOperationException(message);
            }
        }
    }

    public static class YogaNodeLayout
    {
        public static float GetLeft(YogaNode node)
        {
            return node.getLayout().position(PhysicalEdge.Left);
        }

        public static float GetTop(YogaNode node)
        {
            return node.getLayout().position(PhysicalEdge.Top);
        }

        public static float GetRight(YogaNode node)
        {
            return node.getLayout().position(PhysicalEdge.Right);
        }

        public static float GetBottom(YogaNode node)
        {
            return node.getLayout().position(PhysicalEdge.Bottom);
        }

        public static float GetWidth(YogaNode node)
        {
            return node.getLayout().dimension(Dimension.Width);
        }

        public static float GetHeight(YogaNode node)
        {
            return node.getLayout().dimension(Dimension.Height);
        }

        public static YGDirection GetDirection(YogaNode node)
        {
            return node.getLayout().direction();
        }

        public static bool GetHadOverflow(YogaNode node)
        {
            return node.getLayout().hadOverflow();
        }

        public static float GetMargin(YogaNode node, YGEdge edge)
        {
            return GetResolvedLayoutProperty((n, e) => n.getLayout().margin(e), node, (Edge)edge);
        }

        public static float GetBorder(YogaNode node, YGEdge edge)
        {
            return GetResolvedLayoutProperty((n, e) => n.getLayout().border(e), node, (Edge)edge);
        }

        public static float GetPadding(YogaNode node, YGEdge edge)
        {
            return GetResolvedLayoutProperty((n, e) => n.getLayout().padding(e), node, (Edge)edge);
        }

        public static float GetRawHeight(YogaNode node)
        {
            return node.getLayout().rawDimension(Dimension.Height);
        }

        public static float GetRawWidth(YogaNode node)
        {
            return node.getLayout().rawDimension(Dimension.Width);
        }

        private delegate float LayoutPropertyGetter(YogaNode node, PhysicalEdge edge);

        private static float GetResolvedLayoutProperty(LayoutPropertyGetter getter, YogaNode node, Edge edge)
        {
            YogaNode.AssertFatalWithNode(
                node,
                edge <= Edge.End,
                "Cannot get layout properties of multi-edge shorthands");

            var layout = node.getLayout();

            if (edge == Edge.Start)
            {
                if (layout.direction() == YGDirection.RTL)
                {
                    return getter(node, PhysicalEdge.Right);
                }
                else
                {
                    return getter(node, PhysicalEdge.Left);
                }
            }

            if (edge == Edge.End)
            {
                if (layout.direction() == YGDirection.RTL)
                {
                    return getter(node, PhysicalEdge.Left);
                }
                else
                {
                    return getter(node, PhysicalEdge.Right);
                }
            }

            return getter(node, (PhysicalEdge)edge);
        }
    }
}

