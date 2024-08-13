using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace jdk.math.test;

static class MathSupport
{
    /**
     * Returns a floating-point power of two in the normal range.
     */
    static double powerOfTwoD(int n)
    {
        Debug.Assert(n >= Double.MIN_EXPONENT && n <= Double.MAX_EXPONENT);
        return Double.longBitsToDouble((((long)n + (long)DoubleConsts.EXP_BIAS) <<
                                        (DoubleConsts.SIGNIFICAND_WIDTH - 1))
                                       & DoubleConsts.EXP_BIT_MASK);
    }

    /**
     * Returns a floating-point power of two in the normal range.
     */
    static float powerOfTwoF(int n)
    {
        Debug.Assert(n >= Float.MIN_EXPONENT && n <= Float.MAX_EXPONENT);
        return Float.intBitsToFloat(((n + FloatConsts.EXP_BIAS) <<
                                     (FloatConsts.SIGNIFICAND_WIDTH - 1))
                                    & FloatConsts.EXP_BIT_MASK);
    }

    public static double JNextDouble(this Random rng, double origin, double bound)
    {
        checkRange(origin, bound);
        return boundedNextDouble(rng, origin, bound);
    }
    static void checkRange(double origin, double bound)
    {
        if (!(Double.NEGATIVE_INFINITY < origin && origin < bound &&
                bound < Double.POSITIVE_INFINITY))
        {
            throw new ArgumentOutOfRangeException();
        }
    }
    static double boundedNextDouble(Random rng, double origin, double bound)
    {
        double r = rng.NextDouble();
        if (origin < bound)
        {
            if (bound - origin < Double.POSITIVE_INFINITY)
            {
                r = r * (bound - origin) + origin;
            }
            else
            {
                /* avoids overflow at the cost of 3 more multiplications */
                double halfOrigin = 0.5 * origin;
                r = (r * (0.5 * bound - halfOrigin) + halfOrigin) * 2.0;
            }
            if (r >= bound)  // may need to correct a rounding problem
                r = MathNextDown(bound);
        }
        return r;
    }

    /**
     * Returns the size of an ulp of the argument.  An ulp, unit in
     * the last place, of a {@code double} value is the positive
     * distance between this floating-point value and the {@code
     * double} value next larger in magnitude.  Note that for non-NaN
     * <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
     *
     * <p>Special Cases:
     * <ul>
     * <li> If the argument is NaN, then the result is NaN.
     * <li> If the argument is positive or negative infinity, then the
     * result is positive infinity.
     * <li> If the argument is positive or negative zero, then the result is
     * {@code Double.MIN_VALUE}.
     * <li> If the argument is &plusmn;{@code Double.MAX_VALUE}, then
     * the result is equal to 2<sup>971</sup>.
     * </ul>
     *
     * @param d the floating-point value whose ulp is to be returned
     * @return the size of an ulp of the argument
     * @author Joseph D. Darcy
     * @since 1.5
     */
    internal static double MathUlp(double d)
    {
        int getExponent(double d)
        {
            var layout = DoubleLayout.FromDouble(d);
            return layout.Exponent;
        }
        int exp = getExponent(d);
        switch (exp)
        {
            case Double.MAX_EXPONENT + 1: return Math.Abs(d);       // NaN or infinity
            case Double.MIN_EXPONENT - 1: return Double.MIN_VALUE;  // zero or subnormal
            default:
                {
                    Debug.Assert(exp <= Double.MAX_EXPONENT && exp >= Double.MIN_EXPONENT);

                    // ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
                    exp = exp - (DoubleConsts.SIGNIFICAND_WIDTH - 1);
                    if (exp >= Double.MIN_EXPONENT)
                    {
                        return powerOfTwoD(exp);
                    }
                    else
                    {
                        // return a subnormal result; left shift integer
                        // representation of Double.MIN_VALUE appropriate
                        // number of positions
                        return BitConverter.Int64BitsToDouble(1L <<
                                (exp - (Double.MIN_EXPONENT - (DoubleConsts.SIGNIFICAND_WIDTH - 1))));
                    }
                }
        };
    }

    /**
     * Returns the size of an ulp of the argument.  An ulp, unit in
     * the last place, of a {@code float} value is the positive
     * distance between this floating-point value and the {@code
     * float} value next larger in magnitude.  Note that for non-NaN
     * <i>x</i>, <code>ulp(-<i>x</i>) == ulp(<i>x</i>)</code>.
     *
     * <p>Special Cases:
     * <ul>
     * <li> If the argument is NaN, then the result is NaN.
     * <li> If the argument is positive or negative infinity, then the
     * result is positive infinity.
     * <li> If the argument is positive or negative zero, then the result is
     * {@code Float.MIN_VALUE}.
     * <li> If the argument is &plusmn;{@code Float.MAX_VALUE}, then
     * the result is equal to 2<sup>104</sup>.
     * </ul>
     *
     * @param f the floating-point value whose ulp is to be returned
     * @return the size of an ulp of the argument
     * @author Joseph D. Darcy
     * @since 1.5
     */
    public static float MathUlp(float f)
    {
        int getExponent(float d)
        {
            var layout = SingleLayout.FromSingle(d);
            return layout.Exponent;
        }
        int exp = getExponent(f);

        switch (exp)
        {
            case Float.MAX_EXPONENT + 1: return Math.Abs(f);     // NaN or infinity
            case Float.MIN_EXPONENT - 1: return Single.Epsilon; // zero or subnormal
            default:
                {
                    Debug.Assert(exp <= Float.MAX_EXPONENT && exp >= Float.MIN_EXPONENT);

                    // ulp(x) is usually 2^(SIGNIFICAND_WIDTH-1)*(2^ilogb(x))
                    exp = exp - (FloatConsts.SIGNIFICAND_WIDTH - 1);
                    if (exp >= Float.MIN_EXPONENT)
                    {
                        return powerOfTwoF(exp);
                    }
                    else
                    {
                        // return a subnormal result; left shift integer
                        // representation of FloatConsts.MIN_VALUE appropriate
                        // number of positions
                        return Float.intBitsToFloat(1 <<
                                (exp - (Float.MIN_EXPONENT - (FloatConsts.SIGNIFICAND_WIDTH - 1))));
                    }
                }
        };
    }
    public static double MathNextUp(double d)
    {
        if (d < Double.POSITIVE_INFINITY)
        {
            long transducer = BitConverter.DoubleToInt64Bits(d + 0.0D);
            return BitConverter.Int64BitsToDouble(transducer + ((transducer >= 0L) ? 1L : -1L));
        }
        else
        {
            return d;
        }
    }
    public static float MathNextUp(float f)
    {
        if (f < Single.PositiveInfinity)
        {
            int transducer = BitConverter.SingleToInt32Bits(f + 0.0F);
            return BitConverter.Int32BitsToSingle(transducer + ((transducer >= 0) ? 1 : -1));
        }
        else
        {
            return f;
        }
    }

    public static double MathNextDown(double d)
    {
        if (Double.isNaN(d) || d == Double.NEGATIVE_INFINITY)
            return d;
        else
        {
            if (d == 0.0)
                return -Double.MIN_VALUE;
            else
                return BitConverter.Int64BitsToDouble(BitConverter.DoubleToInt64Bits(d) +
                                               ((d > 0.0d) ? -1L : +1L));
        }
    }

    public static float MathNextDown(float f)
    {
        if (Single.IsNaN(f) || f == Single.NegativeInfinity)
            return f;
        else
        {
            if (f == 0.0f)
                return -Single.Epsilon;
            else
                return BitConverter.Int32BitsToSingle(BitConverter.SingleToInt32Bits(f) +
                                            ((f > 0.0f) ? -1 : +1));
        }
    }
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public readonly struct DoubleLayout(ulong value)
    {
        public static DoubleLayout FromDouble(double value) => new(BitConverter.DoubleToUInt64Bits(value));
        public const ulong SignMask = 0x8000_0000_0000_0000UL;
        public const ulong ExponentMask = 0x7FF0_0000_0000_0000UL;
        public const ulong FractionMask = 0x000F_FFFF_FFFF_FFFFUL;

        public bool SignBit => ((value & SignMask) >> 63) != 0;
        public uint ExponentBits => (uint)((value & ExponentMask) >> 52);
        public ulong FractionBits => (value & FractionMask);

        public int Sign => SignBit ? -1 :
            ExponentBits == 0 ? 0 : 1;

        public bool ExponentIsNanOrInfinity => ExponentBits == 0x7FF;
        public bool ExponentIsZero => ExponentBits == 0;
        public short Exponent => (short)(((int)ExponentBits) - 1023);
    }
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public readonly struct SingleLayout(uint value)
    {
        public static SingleLayout FromSingle(float value) => new(BitConverter.SingleToUInt32Bits(value));
        public const uint SignMask = 0x8000_0000U;
        public const uint ExponentMask = 0x7F80_0000U;
        public const uint FractionMask = 0x007F_FFFFU;

        public bool SignBit => ((value & SignMask) >> 31) != 0;
        public byte ExponentBits => (byte)((value & ExponentMask) >> 23);
        public uint FractionBits => (value & FractionMask);

        public int Sign => SignBit ? -1 :
            ExponentBits == 0 ? 0 : 1;

        public bool ExponentIsNanOrInfinity => ExponentBits == 0xFF;
        public bool ExponentIsZero => ExponentBits == 0;
        public sbyte Exponent => (sbyte)((ExponentBits) - 127);

    }

}
