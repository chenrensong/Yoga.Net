using System;

namespace Facebook.Yoga
{
    public class LayoutResults
    {
        public const int MaxCachedMeasurements = 8;

        public uint ComputedFlexBasisGeneration = 0;
        public FloatOptional ComputedFlexBasis = FloatOptional.Undefined;

        public uint GenerationCount = 0;
        public uint ConfigVersion = 0;
        public Direction LastOwnerDirection = Direction.Inherit;

        public uint NextCachedMeasurementsIndex = 0;
        public CachedMeasurement[] CachedMeasurements = new CachedMeasurement[MaxCachedMeasurements];

        public CachedMeasurement CachedLayout = new CachedMeasurement();

        private Direction _direction = Direction.Inherit;
        private bool _hadOverflow = false;

        private float[] _dimensions = { YogaConstants.Undefined, YogaConstants.Undefined };
        private float[] _measuredDimensions = { YogaConstants.Undefined, YogaConstants.Undefined };
        private float[] _rawDimensions = { YogaConstants.Undefined, YogaConstants.Undefined };
        private float[] _position = new float[4];
        private float[] _margin = new float[4];
        private float[] _border = new float[4];
        private float[] _padding = new float[4];

        public LayoutResults()
        {
            for (int i = 0; i < MaxCachedMeasurements; i++)
            {
                CachedMeasurements[i] = new CachedMeasurement();
            }
        }

        public Direction GetDirection()
        {
            return _direction;
        }

        public void SetDirection(Direction direction)
        {
            _direction = direction;
        }

        public bool HadOverflow()
        {
            return _hadOverflow;
        }

        public void SetHadOverflow(bool hadOverflow)
        {
            _hadOverflow = hadOverflow;
        }

        public float Dimension(Dimension axis)
        {
            return _dimensions[(int)axis];
        }

        public void SetDimension(Dimension axis, float dimension)
        {
            _dimensions[(int)axis] = dimension;
        }

        public float MeasuredDimension(Dimension axis)
        {
            return _measuredDimensions[(int)axis];
        }

        public float RawDimension(Dimension axis)
        {
            return _rawDimensions[(int)axis];
        }

        public void SetMeasuredDimension(Dimension axis, float dimension)
        {
            _measuredDimensions[(int)axis] = dimension;
        }

        public void SetRawDimension(Dimension axis, float dimension)
        {
            _rawDimensions[(int)axis] = dimension;
        }

        public float Position(PhysicalEdge physicalEdge)
        {
            return _position[(int)physicalEdge];
        }

        public void SetPosition(PhysicalEdge physicalEdge, float dimension)
        {
            _position[(int)physicalEdge] = dimension;
        }

        public float Margin(PhysicalEdge physicalEdge)
        {
            return _margin[(int)physicalEdge];
        }

        public void SetMargin(PhysicalEdge physicalEdge, float dimension)
        {
            _margin[(int)physicalEdge] = dimension;
        }

        public float Border(PhysicalEdge physicalEdge)
        {
            return _border[(int)physicalEdge];
        }

        public void SetBorder(PhysicalEdge physicalEdge, float dimension)
        {
            _border[(int)physicalEdge] = dimension;
        }

        public float Padding(PhysicalEdge physicalEdge)
        {
            return _padding[(int)physicalEdge];
        }

        public void SetPadding(PhysicalEdge physicalEdge, float dimension)
        {
            _padding[(int)physicalEdge] = dimension;
        }

        public bool Equals(LayoutResults layout)
        {
            if (layout == null) return false;

            bool isEqual = Comparison.InexactEquals(_position, layout._position) &&
                           Comparison.InexactEquals(_dimensions, layout._dimensions) &&
                           Comparison.InexactEquals(_margin, layout._margin) &&
                           Comparison.InexactEquals(_border, layout._border) &&
                           Comparison.InexactEquals(_padding, layout._padding) &&
                           GetDirection() == layout.GetDirection() &&
                           HadOverflow() == layout.HadOverflow() &&
                           LastOwnerDirection == layout.LastOwnerDirection &&
                           ConfigVersion == layout.ConfigVersion &&
                           NextCachedMeasurementsIndex == layout.NextCachedMeasurementsIndex &&
                           CachedLayout.Equals(layout.CachedLayout) &&
                           ComputedFlexBasis.Equals(layout.ComputedFlexBasis);

            for (int i = 0; i < MaxCachedMeasurements && isEqual; ++i)
            {
                isEqual = isEqual && CachedMeasurements[i].Equals(layout.CachedMeasurements[i]);
            }

            if (!float.IsNaN(_measuredDimensions[0]) || !float.IsNaN(layout._measuredDimensions[0]))
            {
                isEqual = isEqual && (_measuredDimensions[0] == layout._measuredDimensions[0]);
            }

            if (!float.IsNaN(_measuredDimensions[1]) || !float.IsNaN(layout._measuredDimensions[1]))
            {
                isEqual = isEqual && (_measuredDimensions[1] == layout._measuredDimensions[1]);
            }

            return isEqual;
        }
    }
}

