// Copyright (c) Meta Platforms, Inc. and affiliates.
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.

using System;
using System.Runtime.CompilerServices;

namespace Yoga
{
    public enum YGUnit
    {
        Undefined,
        Point,
        Percent,
        Auto,
        FitContent,
        MaxContent,
        Stretch
    }

    public readonly struct YGValue : IEquatable<YGValue>
    {
        public readonly float Value;
        public readonly YGUnit Unit;

        public static readonly float Undefined = float.NaN;

        public static readonly YGValue Zero = new YGValue(0, YGUnit.Point);
        public static readonly YGValue UndefinedValue = new YGValue(Undefined, YGUnit.Undefined);
        public static readonly YGValue Auto = new YGValue(Undefined, YGUnit.Auto);

        public YGValue(float value, YGUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUndefined(float value)
        {
            return float.IsNaN(value);
        }

        public bool Equals(YGValue other)
        {
            if (Unit != other.Unit)
            {
                return false;
            }

            switch (Unit)
            {
                case YGUnit.Undefined:
                case YGUnit.Auto:
                case YGUnit.FitContent:
                case YGUnit.MaxContent:
                case YGUnit.Stretch:
                    return true;
                case YGUnit.Point:
                case YGUnit.Percent:
                    return Value == other.Value;
                default:
                    return false;
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is YGValue other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Unit);
        }

        public static bool operator ==(YGValue left, YGValue right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(YGValue left, YGValue right)
        {
            return !left.Equals(right);
        }

        public static YGValue operator -(YGValue value)
        {
            return new YGValue(-value.Value, value.Unit);
        }
    }
}

