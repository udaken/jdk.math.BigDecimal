using System;
using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace jdk.math;
using @boolean = bool;

internal static class BigIntegerUtility {

    public static BigInteger Random(int numBits, Random rnd) {
        var b = new byte[(numBits + 7) / 8];
        rnd.NextBytes(b);
        return new BigInteger(b, isUnsigned: true);
    }
    public static BigInteger Create(int signum, int[] magnitude) {
        return Create(signum, (uint[]) (object) magnitude);
    }
    public static BigInteger Create(int signum, ReadOnlySpan<uint> magnitude) {
        if (Unsafe.SizeOf<BigInteger>() != Unsafe.SizeOf<BigIntegerLayout>()) {
            throw new NotSupportedException();
        }

        BigIntegerLayout layout = new BigIntegerLayout { _sign = signum, _bits = magnitude.ToArray() };
        return Unsafe.As<BigIntegerLayout, BigInteger>(ref layout);
    }
    private static ref readonly BigIntegerLayout AsAccessor(this in BigInteger bigInteger) {
        return ref Unsafe.As<BigInteger, BigIntegerLayout>(ref Unsafe.AsRef(in bigInteger));
    }
    public static bool TryGetMagnitude(this in BigInteger bigInteger, out ReadOnlySpan<uint> bits, out int compactValue) {
        BigIntegerLayout layout = AsAccessor(in bigInteger);
        bits = layout._bits;
        compactValue = layout._sign;
        return layout._bits != null;
    }

    public static int signum(this BigInteger self) => self.Sign;

    public static int hashCode(this BigInteger self) => (int) self.GetHashCode();

    public static int bitLength(this BigInteger self) => (int) self.GetBitLength();
    public static BigInteger abs(this BigInteger self) {
        return BigInteger.Abs(self);
    }
    public static BigInteger negate(this BigInteger self) => -self;

    public static BigInteger shiftLeft(this BigInteger self, int n) => self << n;

    public static BigInteger add(this BigInteger self, BigInteger x) {
        return self + x;
    }

    public static BigInteger multiply(this BigInteger self, BigInteger x) {
        return self * x;
    }
    public static BigInteger divide(this BigInteger self, BigInteger dividor) {
        return self / dividor;
    }
    public static BigInteger mod(this BigInteger self, BigInteger dividor) {
        return self % dividor;
    }
    public static BigInteger gcd(this BigInteger self, BigInteger a) {
        return BigInteger.GreatestCommonDivisor(self, a);
    }
    public static int compareTo(this BigInteger self, BigInteger b) {
        return self.CompareTo(b);
    }


    public static int compareHalf(this BigInteger self, BigInteger b) {
        //return self.CompareTo(b / 2);

        if (self.Sign < b.Sign)
            return -1;
        else if (self.Sign > b.Sign)
            return 1;

        bool bHasMag = b.TryGetMagnitude(out var bval, out var bCompactValue);
        bool hasMag = self.TryGetMagnitude(out var val, out var compactValue);
        if (!bHasMag && !hasMag) {
            uint bv = (uint) bCompactValue;
            uint hb = ((bv >>> 1));
            uint v = (uint) compactValue;
            if (v != hb)
                return v < hb ? -1 : 1;

            return (bv & 1) << 31 == 0 ? 0 : -1;
        } else if (!bHasMag) {
            bval = stackalloc[] { (uint) Math.Abs(bCompactValue) };
        } else if (!hasMag) {
            val = stackalloc[] { (uint) Math.Abs(compactValue) };
        }

        int blen = bval.Length;
        int len = val.Length;
        if (len <= 0)
            return blen <= 0 ? 0 : -1;
        if (len > blen)
            return 1;
        if (len < blen - 1)
            return -1;

        int bstart = bval.Length - 1;
        uint carry = 0;
        // Only 2 cases left:len == blen or len == blen - 1
        if (len != blen) { // len == blen - 1
            if (bval[bstart] == 1) {
                --bstart;
                carry = 0x8000_0000;
            } else
                return -1;
        }
        // compare values with right-shifted values of b,
        // carrying shifted-out bits across words
        for (int i = len - 1, j = bstart; i >= 0;) {
            uint bv = bval[j--];
            uint hb = ((bv >>> 1) + carry);
            uint v = val[i--];
            if (v != hb)
                return v < hb ? -1 : 1;
            carry = (bv & 1) << 31; // carray will be either 0x80000000 or 0
        }
        return carry == 0 ? 0 : -1;
    }

    public static int compareMagnitude(this BigInteger self, BigInteger val) {
        if (!self.TryGetMagnitude(out var m1, out var compactValue1)) {
            m1 = [self.Sign >= 0 ? (uint) compactValue1 : (uint) -compactValue1];
        }
        if (!val.TryGetMagnitude(out var m2, out var compactValue2)) {
            m2 = [val.Sign >= 0 ? (uint) compactValue2 : (uint) -compactValue2];
        }

        int len1 = m1.Length;
        int len2 = m2.Length;
        if (len1 < len2)
            return -1;
        if (len1 > len2)
            return 1;
        int i = LastMismatchIndex(m1, m2);
        if (i != -1)
            return m1[i].CompareTo(m2[i]);
        return 0;

        static int LastMismatchIndex(ReadOnlySpan<uint> self, ReadOnlySpan<uint> other) {
            if (self.Length != other.Length) {
                throw new ArgumentOutOfRangeException(nameof(other), $"Argument <{nameof(other)}> must be equals <{nameof(self)}>");
            }

            for (int i = self.Length - 1; i >= 0; i--) {
                if (self[i] != other[i])
                    return i;
            }
            return -1;
        }
    }
    public static boolean testBit(this BigInteger self, int position) {
        return !(self & (1 << position)).IsZero;
    }
    public static BigDecimal toBigDecimal(this BigInteger bigInteger, int sign, int scale) {
        Debug.Assert(bigInteger.Sign >= 0);
        long compactValue = toCompactValue(bigInteger, sign);
        if (compactValue != BigDecimal.INFLATED) {
            return new BigDecimal(default, compactValue, scale, prec: 0);
        }

        return new BigDecimal(sign == -1 ? -bigInteger : bigInteger, BigDecimal.INFLATED, scale, prec: 0);
    }
    public static long toCompactValue(this BigInteger bigInteger, int sign) {
        if (sign == 0)
            return 0;

        Debug.Assert(bigInteger.Sign >= 0);

        if (long.MinValue < bigInteger && bigInteger <= long.MaxValue)
            return ((long) bigInteger) * sign;

        return BigDecimal.INFLATED;
    }
    public static BigInteger toBigInteger(this BigInteger bigInteger, int sign) {
        if (bigInteger.Sign != sign) return -bigInteger;
        return bigInteger;
    }
    public static int intValue(this BigInteger bigInteger) {
        return (int) bigInteger;
    }
    public static long longValue(this BigInteger bigInteger) {
        if (bigInteger.TryGetMagnitude(out var bits, out var compactValueOrSign)) {
            ulong ul = bits.Length > 1 ?
                ((ulong) bits[1]) << 32 | bits[0] :
                bits[0];
            long l = compactValueOrSign >= 0 ?
                unchecked((long) ul) :
                unchecked(-(long) ul);
            return l;
        } else {
            return compactValueOrSign;
        }
    }

    public static string toString(this BigInteger bigInteger) {
        return bigInteger.ToString(CultureInfo.InvariantCulture);
    }

    public static bool equals(this BigInteger bigInteger, BigInteger other) {
        return bigInteger.Equals(other);
    }

    public static BigInteger pow(this BigInteger bigInteger, int exponent) {
        return BigInteger.Pow(bigInteger, exponent);
    }

    public static bool isOdd(this BigInteger bigInteger) {
        return !bigInteger.IsEven;
    }

    public static bool isZero(this BigInteger bigInteger) {
        return bigInteger.IsZero;
    }

    public static float floatValue(this BigInteger bigInteger) {
        return (float) bigInteger.doubleValue();
    }
    public static double doubleValue(this BigInteger bigInteger) {
#if NET6_0_OR_GREATER
        //return (double) bigInteger;

        if (bigInteger.IsZero) return 0.0;
        else if (bigInteger.IsOne) return 1;

        long bitLength = bigInteger.GetBitLength();
        Debug.Assert(bitLength > 0);
        if (bitLength <= Double.PRECISION)
            return (double) bigInteger;

        var len = getDigitLength(bitLength);
        Debug.Assert(len > 0);
        char[]? array = null;
        Span<char> buf = len <= 128 ? stackalloc char[len] : array = ArrayPool<char>.Shared.Rent(len);
        try {
            if (bigInteger.TryFormat(buf, out int used, "R")) {
                ReadOnlySpan<char> digits = buf.Slice(0, used);
                return double.Parse(digits);
            } else {
                throw new InvalidOperationException();
            }
        } finally {
            if (array != null) {
                ArrayPool<char>.Shared.Return(array);
            }
        }

        int getDigitLength(long n) {
            if (n <= 256)
                return 78;

            return (int) (Math.Floor(n * Math.Log10(2.0))) + 1;
            //return (int) ((n - 1) / 3) + 1;
        }
#else
        if (bigInteger.IsZero) return 0.0;
        else if (bigInteger.IsOne) return 1;
        
        var layout = bigInteger.AsAccessor();
        long bitLength = layout.GetBitLength();
        Debug.Assert(bitLength > 0);
        if (bitLength <= Double.PRECISION)
            return (double) bigInteger;

        string digits =  bigInteger.ToString("R");
        return double.Parse(digits);
#endif
    }

    internal struct BigIntegerLayout {
        internal const uint MaskHighBit = unchecked((uint) int.MinValue);

        private static readonly BigIntegerLayout s_bnMinInt = new BigIntegerLayout(-1, new uint[] { MaskHighBit });

        internal int _sign;
        internal uint[]? _bits;

        private BigIntegerLayout(int sign, uint[]? bits) {
            if (bits != null) {
                switch (sign) {
                    case 0:
                    case 1:
                    case -1:
                        break;
                    default:
                        throw new ArgumentException(null, nameof(sign));
                }
            }
            _sign = sign;
            _bits = bits;
        }
        internal BigIntegerLayout(ReadOnlySpan<uint> value, bool negative) {

            int length = 0;
            for (int i = value.Length - 1; i >= 0; i--) {
                if (value[0] != 0) {
                    length = i + 1;
                    break;
                }
            }

            value = value.Slice(0, length);

            if (value.Length == 0) {
                this = default;
                return;
            }

            if (value.Length == 1 && value[0] < MaskHighBit) {
                _sign = negative ? -(int) value[0] : (int) value[0];
                _bits = null;
                if (_sign == int.MinValue) {
                    this = s_bnMinInt;
                    return;
                }
            } else {
                _sign = negative ? -1 : 1;
                _bits = value.ToArray();
            }
        }
        public long GetBitLength() {
            uint highValue;
            int bitsArrayLength;
            int sign = _sign;
            uint[]? bits = _bits;

            if (bits == null) {
                bitsArrayLength = 1;
                highValue = (uint) (sign < 0 ? -sign : sign);
            } else {
                bitsArrayLength = bits.Length;
                highValue = bits[bitsArrayLength - 1];
            }

            long bitLength = bitsArrayLength * 32L - BitOperations.LeadingZeroCount(highValue);

            if (sign >= 0)
                return bitLength;

            if ((highValue & (highValue - 1)) != 0)
                return bitLength;

            for (int i = bitsArrayLength - 2; i >= 0; i--) {
                if (bits![i] == 0)
                    continue;

                return bitLength;
            }

            return bitLength - 1;
        }

    }
}

internal static class Arrays {
    public static T[] copyOf<T>(T[] source, int length) {
        var newArray = new T[length];
        Array.Copy(source, newArray, Math.Min(length, source.Length));
        return newArray;
    }
}