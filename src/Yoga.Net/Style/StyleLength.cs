using System;

namespace Facebook.Yoga
{
    public struct YGValue
    {
        public float Value;
        public Unit Unit;

        public YGValue(float value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }
    }

    public class StyleLength : IEquatable<StyleLength>
    {
        public static StyleLength Undefined() => new StyleLength(FloatOptional.Undefined, Unit.Undefined);

        public static StyleLength OfAuto() => new StyleLength(FloatOptional.Undefined, Unit.Auto);

        public static StyleLength Points(float value)
        {
            return IsUndefined(value) || float.IsInfinity(value)
                ? Undefined()
                : new StyleLength(new FloatOptional(value), Unit.Point);
        }

        public static StyleLength Percent(float value)
        {
            return IsUndefined(value) || float.IsInfinity(value)
                ? Undefined()
                : new StyleLength(new FloatOptional(value), Unit.Percent);
        }

        public bool IsAuto() => _unit == Unit.Auto;

        public bool IsUndefined() => _unit == Unit.Undefined;

        public bool IsPoints() => _unit == Unit.Point;

        public bool IsPercent() => _unit == Unit.Percent;

        public bool IsDefined() => !IsUndefined();

        public FloatOptional Value() => _value;

        public FloatOptional Resolve(float referenceLength)
        {
            switch (_unit)
            {
                case Unit.Point:
                    return _value;
                case Unit.Percent:
                    return new FloatOptional(_value.Unwrap() * referenceLength * 0.01f);
                default:
                    return FloatOptional.Undefined;
            }
        }

        public static explicit operator YGValue(StyleLength length)
        {
            return new YGValue(length._value.Unwrap(), length._unit);
        }

        public override bool Equals(object? obj)
        {
            return obj is StyleLength other && Equals(other);
        }

        public bool Equals(StyleLength? other)
        {
            if (other is null) return false;
            return _value.Equals(other._value) && _unit == other._unit;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value, _unit);
        }

        public static bool operator ==(StyleLength? left, StyleLength? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(StyleLength? left, StyleLength? right)
        {
            return !(left == right);
        }

        public bool InexactEquals(StyleLength other)
        {
            return _unit == other._unit &&
                InexactEquals(_value, other._value);
        }

        private StyleLength(FloatOptional value, Unit unit)
        {
            _value = value;
            _unit = unit;
        }

        private readonly FloatOptional _value;
        private readonly Unit _unit = Unit.Undefined;

        private static bool IsUndefined(float value)
        {
            return float.IsNaN(value);
        }

        private static bool InexactEquals(FloatOptional a, FloatOptional b)
        {
            return FloatOptional.InexactEquals(a, b);
        }
    }

    internal static class StyleLengthExtensions
    {
        public static bool InexactEquals(this StyleLength a, StyleLength b)
        {
            return a.InexactEquals(b);
        }
    }
}

