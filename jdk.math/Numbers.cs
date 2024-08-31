using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace jdk;

internal class Long {
    public const int SIZE = sizeof(long);
    public const long MIN_VALUE = long.MinValue;
    public const long MAX_VALUE = long.MaxValue;

    public static int numberOfLeadingZeros(long i) => BitOperations.LeadingZeroCount(unchecked((ulong) i));
    public static int signum(long i) => Math.Sign(i);
    public static int compare(long x, long y) => x.CompareTo(y);

    private static (ulong Quotient, ulong Remainder) DivRem(ulong left, ulong right) {
        ulong quotient = left / right;
        return (quotient, left - (quotient * right));
    }

    public static (long Quotient, long Remainder) divRemUnsigned(long left, long right) {
        (ulong Quotient, ulong Remainder) =
#if NET7_0_OR_GREATER
        Math.DivRem((ulong) left, (ulong) right);
#else
        DivRem((ulong) left, (ulong) right);
#endif
        return ((long) Quotient, (long) Remainder);
    }

    public static string toString(long value) => value.ToString(CultureInfo.InvariantCulture);
}

internal class Integer {
    public const int SIZE = sizeof(int);
    public const int MIN_VALUE = int.MinValue;
    public const int MAX_VALUE = int.MaxValue;

    public static int numberOfLeadingZeros(int i) => BitOperations.LeadingZeroCount(unchecked((uint) i));
    public static int signum(int i) => Math.Sign(i);
    public static int compare(int x, int y) => x.CompareTo(y);
    public static string toString(int value) => value.ToString(CultureInfo.InvariantCulture);
}

internal static class Float {
    public const float MAX_VALUE = System.Single.MaxValue;
    public const float MIN_VALUE = System.Single.Epsilon;
    public const int SIZE = 32;

    public const int PRECISION = 24;
    public const int MAX_EXPONENT = (1 << (SIZE - PRECISION - 1)) - 1;
    public const int MIN_EXPONENT = 1 - MAX_EXPONENT;

    public const float POSITIVE_INFINITY = Single.PositiveInfinity;
    public const float NEGATIVE_INFINITY = Single.NegativeInfinity;
    public static float intBitsToFloat(int value) {
#if NETCOREAPP3_1_OR_GREATER
        return BitConverter.Int32BitsToSingle(value);
#else
        return Unsafe.As<int, float>(ref value);
#endif
    }
}
internal static class Double {
    public const double MAX_VALUE = System.Double.MaxValue;
    public const double MIN_VALUE = System.Double.Epsilon;
    public const int SIZE = 64;
    public const int PRECISION = 53;
    public const int MAX_EXPONENT = (1 << (SIZE - PRECISION - 1)) - 1; // 1023
    public const int MIN_EXPONENT = 1 - MAX_EXPONENT; // -1022

    public const double POSITIVE_INFINITY = double.PositiveInfinity;
    public const double NEGATIVE_INFINITY = double.NegativeInfinity;

    public static bool isNaN(double value) => double.IsNaN(value);
    public static bool isPositiveInfinity(double value) => double.IsPositiveInfinity(value);
    public static bool isInfinite(double value) => double.IsInfinity(value);

    public static double longBitsToDouble(long l) => BitConverter.Int64BitsToDouble(l);

    public static long doubleToLongBits(double d) => BitConverter.DoubleToInt64Bits(d);
}

internal static class MathSupport {

    public static double scalb(double x, int n) {
#if NETCOREAPP3_0_OR_GREATER
        return Math.ScaleB(x, n);
#else
        const double SCALEB_C1 = 8.98846567431158E+307; // 0x1p1023
        const double SCALEB_C2 = 2.2250738585072014E-308; // 0x1p-1022
        const double SCALEB_C3 = 9007199254740992; // 0x1p53
        
        // Implementation based on https://git.musl-libc.org/cgit/musl/tree/src/math/scalbln.c
        //
        // Performs the calculation x * 2^n efficiently. It constructs a double from 2^n by building
        // the correct biased exponent. If n is greater than the maximum exponent (1023) or less than
        // the minimum exponent (-1022), adjust x and n to compute correct result.

        double y = x;
        if (n > 1023) {
            y *= SCALEB_C1;
            n -= 1023;
            if (n > 1023) {
                y *= SCALEB_C1;
                n -= 1023;
                if (n > 1023) {
                    n = 1023;
                }
            }
        } else if (n < -1022) {
            y *= SCALEB_C2 * SCALEB_C3;
            n += 1022 - 53;
            if (n < -1022) {
                y *= SCALEB_C2 * SCALEB_C3;
                n += 1022 - 53;
                if (n < -1022) {
                    n = -1022;
                }
            }
        }

        double u = BitConverter.Int64BitsToDouble(((long) (0x3ff + n) << 52));
        return y * u;
#endif
    }

    public static float scalb(float x, int n) {
#if NETCOREAPP3_0_OR_GREATER
        return MathF.ScaleB(x, n);
#else
        const float SCALEB_C1 = 1.7014118E+38f; // 0x1p127f
        const float SCALEB_C2 = 1.1754944E-38f; // 0x1p-126f
        const float SCALEB_C3 = 16777216f; // 0x1p24f

        // Implementation based on https://git.musl-libc.org/cgit/musl/tree/src/math/scalblnf.c
        //
        // Performs the calculation x * 2^n efficiently. It constructs a float from 2^n by building
        // the correct biased exponent. If n is greater than the maximum exponent (127) or less than
        // the minimum exponent (-126), adjust x and n to compute correct result.

        float y = x;
        if (n > 127) {
            y *= SCALEB_C1;
            n -= 127;
            if (n > 127) {
                y *= SCALEB_C1;
                n -= 127;
                if (n > 127) {
                    n = 127;
                }
            }
        } else if (n < -126) {
            y *= SCALEB_C2 * SCALEB_C3;
            n += 126 - 24;
            if (n < -126) {
                y *= SCALEB_C2 * SCALEB_C3;
                n += 126 - 24;
                if (n < -126) {
                    n = -126;
                }
            }
        }

        float u = BitConverter.Int32BitsToSingle(((int) (0x7f + n) << 23));
        return y * u;
#endif
    }
}