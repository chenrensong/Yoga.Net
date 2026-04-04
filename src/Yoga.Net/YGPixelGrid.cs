using System;

namespace Facebook.Yoga
{
    internal static class PixelGrid
    {
        public static float RoundValueToPixelGrid(
            double value,
            double pointScaleFactor,
            bool forceCeil,
            bool forceFloor)
        {
            double scaledValue = value * pointScaleFactor;
            double roundedValue;

            if (forceCeil)
            {
                roundedValue = Math.Ceiling(scaledValue);
            }
            else if (forceFloor)
            {
                roundedValue = Math.Floor(scaledValue);
            }
            else
            {
                roundedValue = Math.Round(scaledValue);
            }

            return (float)(roundedValue / pointScaleFactor);
        }
    }

    public static partial class YogaNative
    {
        public static float YGRoundValueToPixelGrid(
            double value,
            double pointScaleFactor,
            bool forceCeil,
            bool forceFloor)
        {
            return PixelGrid.RoundValueToPixelGrid(
                value, pointScaleFactor, forceCeil, forceFloor);
        }
    }
}
</br>

