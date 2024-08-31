/*
 * Copyright (c) 2003, 2024, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.  Oracle designates this
 * particular file as subject to the "Classpath" exception as provided
 * by Oracle in the LICENSE file that accompanied this code.
 *
 * This code is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
 * version 2 for more details (a copy is included in the LICENSE file that
 * accompanied this code).
 *
 * You should have received a copy of the GNU General Public License version
 * 2 along with this work; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 *
 * Please contact Oracle, 500 Oracle Parkway, Redwood Shores, CA 94065 USA
 * or visit www.oracle.com if you need additional information or have any
 * questions.
 */

/*
 * Portions Copyright IBM Corporation, 1997, 2001. All Rights Reserved.
 */

using System;

namespace jdk.math;
using @boolean = bool;
/**
 * Immutable objects which encapsulate the context settings which
 * describe certain rules for numerical operators, such as those
 * implemented by the {@link BigDecimal} class.
 *
 * <p>The base-independent settings are:
 * <ol>
 * <li>{@code precision}:
 * the number of digits to be used for an operation; results are
 * rounded to this precision
 *
 * <li>{@code roundingMode}:
 * a {@link RoundingMode} object which specifies the algorithm to be
 * used for rounding.
 * </ol>
 *
 * @see     BigDecimal
 * @see     RoundingMode
 * @see <a href="https://standards.ieee.org/ieee/754/6210/">
 *      <cite>IEEE Standard for Floating-Point Arithmetic</cite></a>
 *
 * @author  Mike Cowlishaw
 * @author  Joseph D. Darcy
 * @since 1.5
 */

public readonly struct MathContext : IEquatable<MathContext> {

    /* ----- Constants ----- */

    // defaults for constructors
    private static readonly RoundingMode DEFAULT_ROUNDINGMODE = RoundingMode.HALF_UP;
    // Smallest values for digits (Maximum is Integer.MAX_VALUE)
    private static readonly int MIN_DIGITS = 0;

    /* ----- Public Properties ----- */
    /**
     * A {@code MathContext} object whose settings have the values
     * required for unlimited precision arithmetic.
     * The values of the settings are: {@code precision=0 roundingMode=HALF_UP}
     */
    public static readonly MathContext UNLIMITED =
        new MathContext(0, RoundingMode.HALF_UP);

    /**
     * A {@code MathContext} object with a precision setting
     * matching the precision of the IEEE 754-2019 decimal32 format, 7 digits, and a
     * rounding mode of {@link RoundingMode#HALF_EVEN HALF_EVEN}.
     * Note the exponent range of decimal32 (min exponent of -95, max
     * exponent of 96) is <em>not</em> used for rounding.
     */
    public static readonly MathContext DECIMAL32 =
        new MathContext(7, RoundingMode.HALF_EVEN);

    /**
     * A {@code MathContext} object with a precision setting
     * matching the precision of the IEEE 754-2019 decimal64 format, 16 digits, and a
     * rounding mode of {@link RoundingMode#HALF_EVEN HALF_EVEN}.
     * Note the exponent range of decimal64 (min exponent of -383, max
     * exponent of 384) is <em>not</em> used for rounding.
     */
    public static readonly MathContext DECIMAL64 =
        new MathContext(16, RoundingMode.HALF_EVEN);

    /**
     * A {@code MathContext} object with a precision setting
     * matching the precision of the IEEE 754-2019 decimal128 format, 34 digits, and a
     * rounding mode of {@link RoundingMode#HALF_EVEN HALF_EVEN}.
     * Note the exponent range of decimal128 (min exponent of -6143,
     * max exponent of 6144) is <em>not</em> used for rounding.
     */
    public static readonly MathContext DECIMAL128 =
        new MathContext(34, RoundingMode.HALF_EVEN);

    /* ----- Shared Properties ----- */
    /**
     * The number of digits to be used for an operation.  A value of 0
     * indicates that unlimited precision (as many digits as are
     * required) will be used.  Note that leading zeros (in the
     * coefficient of a number) are never significant.
     *
     * <p>{@code precision} will always be non-negative.
     *
     * @serial
     */
    internal readonly int precision;

    /**
     * The rounding algorithm to be used for an operation.
     *
     * @see RoundingMode
     * @serial
     */
    internal readonly RoundingMode roundingMode;

    /* ----- Constructors ----- */

    /**
     * Constructs a new {@code MathContext} with the specified
     * precision and the {@link RoundingMode#HALF_UP HALF_UP} rounding
     * mode.
     *
     * @param setPrecision The non-negative {@code int} precision setting.
     * @throws IllegalArgumentException if the {@code setPrecision} parameter is less
     *         than zero.
     */
    public MathContext(int setPrecision) : this(setPrecision, DEFAULT_ROUNDINGMODE) {
    }

    /**
     * Constructs a new {@code MathContext} with a specified
     * precision and rounding mode.
     *
     * @param setPrecision The non-negative {@code int} precision setting.
     * @param setRoundingMode The rounding mode to use.
     * @throws IllegalArgumentException if the {@code setPrecision} parameter is less
     *         than zero.
     * @throws NullPointerException if the rounding mode argument is {@code null}
     */
    public MathContext(int setPrecision,
                       RoundingMode setRoundingMode) {
        if (setPrecision < MIN_DIGITS)
            throw new ArgumentOutOfRangeException("Digits < 0");
        ArgumentNullExceptionSupport.ThrowIfNull(setPrecision);

        precision = setPrecision;
        roundingMode = setRoundingMode;
    }

    /**
     * Constructs a new {@code MathContext} from a string.
     *
     * The string must be in the same format as that produced by the
     * {@link #toString} method.
     *
     * <p>An {@code IllegalArgumentException} is thrown if the precision
     * section of the string is out of range ({@code < 0}) or the string is
     * not in the format created by the {@link #toString} method.
     *
     * @param val The string to be parsed
     * @throws IllegalArgumentException if the precision section is out of range
     * or of incorrect format
     * @throws NullPointerException if the argument is {@code null}
     */
    public MathContext(String val) {
        int setPrecision;
        ArgumentNullExceptionSupport.ThrowIfNull(val);

        // any error here is a string format problem
        if (!val.StartsWith("precision=", StringComparison.Ordinal))
            throw new ArgumentException();
        int fence = val.IndexOf(' ');    // could be -1
        int off = 10;                     // where value starts
        setPrecision = int.Parse(val.AsSpan()[10..fence]);

        if (!val.AsSpan(fence + 1).StartsWith("roundingMode="))
            throw new ArgumentException();
        off = fence + 1 + 13;
        var str = val.AsSpan(off, val.Length);
        roundingMode = RoundingModeUtil.valueOf(str);


        if (setPrecision < MIN_DIGITS)
            throw new ArgumentOutOfRangeException("Digits < 0");
        // the other parameters cannot be invalid if we got here
        precision = setPrecision;
    }

    /**
     * Returns the {@code precision} setting.
     * This value is always non-negative.
     *
     * @return an {@code int} which is the value of the {@code precision}
     *         setting
     */
    public int getPrecision() {
        return precision;
    }

    /**
     * Returns the roundingMode setting.
     * This will be one of
     * {@link  RoundingMode#CEILING},
     * {@link  RoundingMode#DOWN},
     * {@link  RoundingMode#FLOOR},
     * {@link  RoundingMode#HALF_DOWN},
     * {@link  RoundingMode#HALF_EVEN},
     * {@link  RoundingMode#HALF_UP},
     * {@link  RoundingMode#UNNECESSARY}, or
     * {@link  RoundingMode#UP}.
     *
     * @return a {@code RoundingMode} object which is the value of the
     *         {@code roundingMode} setting
     */
    public RoundingMode getRoundingMode() {
        return roundingMode;
    }

    /**
     * Compares this {@code MathContext} with the specified
     * {@code Object} for equality.
     *
     * @param  x {@code Object} to which this {@code MathContext} is to
     *         be compared.
     * @return {@code true} if and only if the specified {@code Object} is
     *         a {@code MathContext} object which has exactly the same
     *         settings as this object
     */
    public override boolean Equals(Object? x) {
        if (x is not MathContext mc)
            return false;
        return mc.precision == this.precision
            && mc.roundingMode == this.roundingMode; // no need for .equals()
    }

    public boolean Equals(MathContext mc) {
        return mc.precision == this.precision
            && mc.roundingMode == this.roundingMode;
    }
    /**
     * Returns the hash code for this {@code MathContext}.
     *
     * @return hash code for this {@code MathContext}
     */
    public override int GetHashCode() {
        return this.precision + roundingMode.GetHashCode() * 59;
    }

    /**
     * Returns the string representation of this {@code MathContext}.
     * The {@code String} returned represents the settings of the
     * {@code MathContext} object as two space-delimited words
     * (separated by a single space character, <code>'&#92;u0020'</code>,
     * and with no leading or trailing white space), as follows:
     * <ol>
     * <li>
     * The string {@code "precision="}, immediately followed
     * by the value of the precision setting as a numeric string as if
     * generated by the {@link Integer#toString(int) Integer.toString}
     * method.
     *
     * <li>
     * The string {@code "roundingMode="}, immediately
     * followed by the value of the {@code roundingMode} setting as a
     * word.  This word will be the same as the name of the
     * corresponding public constant in the {@link RoundingMode}
     * enum.
     * </ol>
     * <p>
     * For example:
     * <pre>
     * precision=9 roundingMode=HALF_UP
     * </pre>
     *
     * Additional words may be appended to the result of
     * {@code toString} in the future if more properties are added to
     * this class.
     *
     * @return a {@code String} representing the context settings
     */
    public override string ToString() {
        return "precision=" + precision + " " +
               "roundingMode=" + roundingMode.ToString();
    }

    public static bool operator ==(MathContext left, MathContext right) {
        return left.Equals(right);
    }

    public static bool operator !=(MathContext left, MathContext right) {
        return !(left == right);
    }
}



public enum RoundingMode {
    UP = (BigDecimal.ROUND_UP),
    DOWN = (BigDecimal.ROUND_DOWN),
    CEILING = (BigDecimal.ROUND_CEILING),
    FLOOR = (BigDecimal.ROUND_FLOOR),
    HALF_UP = (BigDecimal.ROUND_HALF_UP),
    HALF_DOWN = (BigDecimal.ROUND_HALF_DOWN),
    HALF_EVEN = (BigDecimal.ROUND_HALF_EVEN),
    UNNECESSARY = (BigDecimal.ROUND_UNNECESSARY),
}
public static class RoundingModeUtil {
    public static RoundingMode valueOf(int value) {
        return value switch {
            BigDecimal.ROUND_UP => RoundingMode.UP,
            BigDecimal.ROUND_DOWN => RoundingMode.DOWN,
            BigDecimal.ROUND_CEILING => RoundingMode.CEILING,
            BigDecimal.ROUND_FLOOR => RoundingMode.FLOOR,
            BigDecimal.ROUND_HALF_UP => RoundingMode.HALF_UP,
            BigDecimal.ROUND_HALF_DOWN => RoundingMode.HALF_DOWN,
            BigDecimal.ROUND_HALF_EVEN => RoundingMode.HALF_EVEN,
            BigDecimal.ROUND_UNNECESSARY => RoundingMode.UNNECESSARY,
            _ => throw new ArgumentException(),
        };
    }
    public static RoundingMode valueOf(string str) {
        return valueOf(str.AsSpan());
    }

    public static RoundingMode valueOf(ReadOnlySpan<char> str) {
        return str switch {
            nameof(RoundingMode.UP) => RoundingMode.UP,
            nameof(RoundingMode.DOWN) => RoundingMode.DOWN,
            nameof(RoundingMode.CEILING) => RoundingMode.CEILING,
            nameof(RoundingMode.FLOOR) => RoundingMode.FLOOR,
            nameof(RoundingMode.HALF_UP) => RoundingMode.HALF_UP,
            nameof(RoundingMode.HALF_DOWN) => RoundingMode.HALF_DOWN,
            nameof(RoundingMode.HALF_EVEN) => RoundingMode.HALF_EVEN,
            nameof(RoundingMode.UNNECESSARY) => RoundingMode.UNNECESSARY,
            _ => throw new ArgumentException(),
        };
    }

    public static MidpointRounding AsMidpointRounding(this RoundingMode roundingMode) {
        return roundingMode.oldMode() switch {
#if NETCOREAPP3_0_OR_GREATER
            BigDecimal.ROUND_DOWN => (MidpointRounding.ToZero),
            BigDecimal.ROUND_CEILING => (MidpointRounding.ToPositiveInfinity),
            BigDecimal.ROUND_FLOOR => (MidpointRounding.ToNegativeInfinity),
#endif
            BigDecimal.ROUND_HALF_UP => (MidpointRounding.AwayFromZero),
            BigDecimal.ROUND_HALF_EVEN => (MidpointRounding.ToEven),
            _ => throw new NotSupportedException(),
        };
    }

    public static int oldMode(this RoundingMode roundingMode) => (int) roundingMode;

}
