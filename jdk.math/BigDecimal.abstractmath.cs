using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace jdk.math;
partial struct BigDecimal :
#if NET7_0_OR_GREATER
          INumber<BigDecimal>,
#endif
          IConvertible,
          IComparable<BigDecimal>,
          IEquatable<BigDecimal>,
          IFormattable {

    public int Sign => signum();
    public override String ToString() {
        return toString();
    }

    public readonly override int GetHashCode() {
        return hashCode();
    }
    public override bool Equals(Object? x) {
        return x is BigDecimal xDec && equals(xDec);
    }

    public bool Equals(BigDecimal xDec) {
        return equals(xDec);
    }

    public static readonly BigDecimal MinusOne = new BigDecimal(-1);
    public static BigDecimal Zero => BigDecimal.ZERO;
    public static BigDecimal One => BigDecimal.ONE;

#if NET7_0_OR_GREATER
    static int INumberBase<BigDecimal>.Radix => 10;
    static BigDecimal IAdditiveIdentity<BigDecimal, BigDecimal>.AdditiveIdentity => BigDecimal.ZERO;
    static BigDecimal IMultiplicativeIdentity<BigDecimal, BigDecimal>.MultiplicativeIdentity => BigDecimal.ONE;

    static BigDecimal INumberBase<BigDecimal>.Abs(BigDecimal value) {
        return value.abs();
    }

    static bool INumberBase<BigDecimal>.IsCanonical(BigDecimal value) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.IsComplexNumber(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsEvenInteger(BigDecimal value) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.IsFinite(BigDecimal value) {
        return true;
    }

    static bool INumberBase<BigDecimal>.IsImaginaryNumber(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsInfinity(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsInteger(BigDecimal value) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.IsNaN(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsNegative(BigDecimal value) {
        return value.signum() < 0;
    }

    static bool INumberBase<BigDecimal>.IsNegativeInfinity(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsNormal(BigDecimal value) {
        return !value.Equals(ZERO);
    }

    static bool INumberBase<BigDecimal>.IsOddInteger(BigDecimal value) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.IsPositive(BigDecimal value) {
        return value.signum() > 0;
    }

    static bool INumberBase<BigDecimal>.IsPositiveInfinity(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsRealNumber(BigDecimal value) {
        return true;
    }

    static bool INumberBase<BigDecimal>.IsSubnormal(BigDecimal value) {
        return false;
    }

    static bool INumberBase<BigDecimal>.IsZero(BigDecimal value) {
        return value.signum() == 0;
    }

    static BigDecimal INumberBase<BigDecimal>.MaxMagnitude(BigDecimal x, BigDecimal y) {
        throw new NotImplementedException();
    }

    static BigDecimal INumberBase<BigDecimal>.MaxMagnitudeNumber(BigDecimal x, BigDecimal y) {
        throw new NotImplementedException();
    }

    static BigDecimal INumberBase<BigDecimal>.MinMagnitude(BigDecimal x, BigDecimal y) {
        throw new NotImplementedException();
    }

    static BigDecimal INumberBase<BigDecimal>.MinMagnitudeNumber(BigDecimal x, BigDecimal y) {
        throw new NotImplementedException();
    }

    static BigDecimal INumberBase<BigDecimal>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    static BigDecimal INumberBase<BigDecimal>.Parse(string s, NumberStyles style, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    static BigDecimal ISpanParsable<BigDecimal>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    static BigDecimal IParsable<BigDecimal>.Parse(string s, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertFromChecked<TOther>(TOther value, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertFromSaturating<TOther>(TOther value, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertFromTruncating<TOther>(TOther value, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertToChecked<TOther>(BigDecimal value, out TOther result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertToSaturating<TOther>(BigDecimal value, out TOther result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryConvertToTruncating<TOther>(BigDecimal value, out TOther result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool INumberBase<BigDecimal>.TryParse(string? s, NumberStyles style, IFormatProvider? provider, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool ISpanParsable<BigDecimal>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out BigDecimal result) {
        throw new NotImplementedException();
    }

    static bool IParsable<BigDecimal>.TryParse(string? s, IFormatProvider? provider, out BigDecimal result) {
        throw new NotImplementedException();
    }
#endif

    public int CompareTo(object? obj) {
        return obj switch {
            null => 1,
            BigDecimal bigDecimal => this.compareTo(bigDecimal),
            _ => throw new ArgumentException(null, nameof(obj)),
        };
    }

    public int CompareTo(BigDecimal other) {
        return compareTo(other);
    }

    TypeCode IConvertible.GetTypeCode() {
        return TypeCode.Object;
    }

    bool IConvertible.ToBoolean(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    byte IConvertible.ToByte(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    char IConvertible.ToChar(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    DateTime IConvertible.ToDateTime(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    decimal IConvertible.ToDecimal(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    double IConvertible.ToDouble(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    short IConvertible.ToInt16(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    int IConvertible.ToInt32(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    long IConvertible.ToInt64(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    sbyte IConvertible.ToSByte(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    float IConvertible.ToSingle(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    string IFormattable.ToString(string? format, IFormatProvider? formatProvider) {
        return ToString(); // TODO
    }

    string IConvertible.ToString(IFormatProvider? provider) {
        return ToString();
    }

    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    ushort IConvertible.ToUInt16(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    uint IConvertible.ToUInt32(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

    ulong IConvertible.ToUInt64(IFormatProvider? provider) {
        throw new NotImplementedException();
    }

#if NET7_0_OR_GREATER
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        //FIXME
        var s = ToString();
        charsWritten = s.Length;
        return s.AsSpan().TryCopyTo(destination);
    }
#endif

    public static BigDecimal operator +(BigDecimal value) {
        return value;
    }

    public static BigDecimal operator +(BigDecimal left, BigDecimal right) {
        return left.add(right);
    }

    public static BigDecimal operator -(BigDecimal value) {
        return value.negate();
    }

    public static BigDecimal operator -(BigDecimal left, BigDecimal right) {
        return left.subtract(right);
    }

    public static BigDecimal operator ++(BigDecimal value) {
        return value.add(BigDecimal.ONE);
    }

    public static BigDecimal operator --(BigDecimal value) {
        return value.subtract(BigDecimal.ONE);
    }

    public static BigDecimal operator *(BigDecimal left, BigDecimal right) {
        return left.multiply(right);
    }

    public static BigDecimal operator /(BigDecimal left, BigDecimal right) {
        var (div, _) = left.divideAndRemainder(right);
        return div;
    }

    public static BigDecimal operator %(BigDecimal left, BigDecimal right) {
        var (_, rem) = left.divideAndRemainder(right);
        return rem;
    }

    public static bool operator ==(BigDecimal left, BigDecimal right) {
        return left.Equals(right);
    }

    public static bool operator !=(BigDecimal left, BigDecimal right) {
        return !left.Equals(right);
    }

    public static bool operator <(BigDecimal left, BigDecimal right) {
        return left.compareTo(right) < 0;
    }

    public static bool operator >(BigDecimal left, BigDecimal right) {
        return left.compareTo(right) > 0;
    }

    public static bool operator <=(BigDecimal left, BigDecimal right) {
        return left.compareTo(right) <= 0;
    }

    public static bool operator >=(BigDecimal left, BigDecimal right) {
        return left.compareTo(right) >= 0;
    }

    public static (BigDecimal, BigDecimal) DivRem(BigDecimal left, BigDecimal right) {
        return left.divideAndRemainder(right);
    }

    public static implicit operator BigDecimal(decimal value) {
        var layout = Unsafe.As<decimal, DecimalLayout>(ref value);
        var sign = layout.Sign;
        if (layout.High == 0) {
            if (sign >= 0 && layout.Low64 <= long.MaxValue) {
                return new BigDecimal(default, (long) layout.Low64, layout.Scale, 0);
            } else if (layout.Low64 < 0x8000000000000000L) {
                Debug.Assert(sign == -1);
                Debug.Assert(unchecked((long) layout.Low64) != INFLATED);
                return new BigDecimal(default, (-(long) layout.Low64), layout.Scale, 0);
            }
        }

        var bigInteger = new BigIntegerUtility.BigIntegerLayout([layout.Low, layout.Mid, layout.High], sign < 0);

        var intVal = Unsafe.As<BigIntegerUtility.BigIntegerLayout, BigInteger>(ref bigInteger);
        return new BigDecimal(intVal, layout.Scale);
    }

    [StructLayout(LayoutKind.Sequential)]
    private readonly struct DecimalLayout {
        private const int ScaleShift = 16;

        public readonly int _flags;
        public readonly uint _hi32;
        public readonly ulong _lo64;

        public byte Scale => (byte) (_flags >> ScaleShift);
        internal uint High => _hi32;
        internal uint Low => (uint) _lo64;
        internal uint Mid => (uint) (_lo64 >> 32);

        internal ulong Low64 => _lo64;
        internal int Sign => (_lo64 | High) == 0 ? 0 : (_flags >> 31) | 1;
    }
}
