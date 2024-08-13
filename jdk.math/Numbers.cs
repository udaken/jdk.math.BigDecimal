using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

    public static (long Quotient, long Remainder) divRemUnsigned(long left, long right) {
        (ulong Quotient, ulong Remainder) = ulong.DivRem((ulong) left, (ulong) right);
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
    public static float intBitsToFloat(int value) => BitConverter.Int32BitsToSingle(value);
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
