/*
 * Copyright (c) 2003, 2015, Oracle and/or its affiliates. All rights reserved.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * This code is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License version 2 only, as
 * published by the Free Software Foundation.
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
 * @test
 * @bug 4851776 4907265 6177836 6876282 8066842
 * @summary Some tests for the divide methods.
 * @author Joseph D. Darcy
 */

using System.Diagnostics;
using System.Numerics;

namespace jdk.math.test;
using static jdk.math.BigDecimal;
using boolean = bool;

public class DivideTests
{

    // Preliminary exact divide method; could be used for comparison
    // purposes.
    BigDecimal anotherDivide(BigDecimal dividend, BigDecimal divisor)
    {
        /*
         * Handle zero cases first.
         */
        if (divisor.signum() == 0)
        {   // x/0
            if (dividend.signum() == 0)    // 0/0
                throw new ArithmeticException("Division undefined");  // NaN
            throw new ArithmeticException("Division by zero");
        }
        if (dividend.signum() == 0)        // 0/y
            return BigDecimal.ZERO;
        else
        {
            /*
             * Determine if there is a result with a terminating
             * decimal expansion.  Putting aside overflow and
             * underflow considerations, the existance of an exact
             * result only depends on the ratio of the intVal's of the
             * dividend (i.e. this) and divisor since the scales
             * of the argument just affect where the decimal point
             * lies.
             *
             * For the ratio of (a = this.intVal) and (b =
             * divisor.intVal) to have a finite decimal expansion,
             * once a/b is put in lowest terms, b must be equal to
             * (2^i)*(5^j) for some integer i,j >= 0.  Therefore, we
             * first compute to see if b_prime =(b/gcd(a,b)) is equal
             * to (2^i)*(5^j).
             */
            BigInteger TWO = new BigInteger(2);
            BigInteger FIVE = new BigInteger(5);
            BigInteger TEN = new BigInteger(10);

            BigInteger divisorIntvalue = divisor.scaleByPowerOfTen(divisor.scale()).toBigInteger().abs();
            BigInteger dividendIntvalue = dividend.scaleByPowerOfTen(dividend.scale()).toBigInteger().abs();

            BigInteger b_prime = divisorIntvalue.divide(dividendIntvalue.gcd(divisorIntvalue));

            boolean goodDivisor = false;
            int i = 0, j = 0;

            {
                while (!b_prime.equals(BigInteger.One))
                {
                    int b_primeModTen = b_prime.mod(TEN).intValue();

                    switch (b_primeModTen)
                    {
                        case 0:
                            // b_prime divisible by 10=2*5, increment i and j
                            i++;
                            j++;
                            b_prime = b_prime.divide(TEN);
                            break;

                        case 5:
                            // b_prime divisible by 5, increment j
                            j++;
                            b_prime = b_prime.divide(FIVE);
                            break;

                        case 2:
                        case 4:
                        case 6:
                        case 8:
                            // b_prime divisible by 2, increment i
                            i++;
                            b_prime = b_prime.divide(TWO);
                            break;

                        default: // hit something we shouldn't have
                            b_prime = BigInteger.One; // terminate loop
                            goto badDivisor;
                    }
                }

                goodDivisor = true;
            }
        badDivisor:
            ;
            if (!goodDivisor)
            {
                throw new ArithmeticException("Non terminating decimal expansion");
            }
            else
            {
                // What is a rule for determining how many digits are
                // needed?  Once that is determined, cons up a new
                // MathContext object and pass it on to the divide(bd,
                // mc) method; precision == ?, roundingMode is unnecessary.

                // Are we sure this is the right scale to use?  Should
                // also determine a precision-based method.
                MathContext mc = new MathContext(dividend.precision() +
                                                 (int)Math.Ceiling(
                                                      10.0 * divisor.precision() / 3.0),
                                                 RoundingMode.UNNECESSARY);
                // Should do some more work here to rescale, etc.
                return dividend.divide(divisor, mc);
            }
        }
    }
    static class StrictMath
    {
        public static double pow(double x, double y) => Math.Pow(x, y);
    }

    [Test, Combinatorial]
    public static void powersOf2and5([Range(0, 5)] int i, [Range(0, 5)] int j)
    {
        int powerOf2 = (int)StrictMath.pow(2.0, i);

        int powerOf5 = (int)StrictMath.pow(5.0, j);
        int product;

        _ = BigDecimal.ONE.divide(new BigDecimal(product = powerOf2 * powerOf5));

        _ = new BigDecimal(powerOf2).divide(new BigDecimal(powerOf5));

        _ = new BigDecimal(powerOf5).divide(new BigDecimal(powerOf2));
    }

    [Test]
    public static void nonTerminating()
    {
        int[] primes = [1, 3, 7, 13, 17];

        // For each pair of prime products, verify the ratio of
        // non-equal products has a non-terminating expansion.

        for (int i = 0; i < primes.Length; i++)
        {
            for (int j = i + 1; j < primes.Length; j++)
            {

                for (int m = 0; m < primes.Length; m++)
                {
                    for (int n = m + 1; n < primes.Length; n++)
                    {
                        int dividend = primes[i] * primes[j];
                        int divisor = primes[m] * primes[n];

                        if (((dividend / divisor) * divisor) != dividend)
                        {
                            var bdividend = new BigDecimal(dividend);
                            var bdivisor = new BigDecimal(divisor);
                            Assert.Catch<ArithmeticException>(() =>
                            {
                                bdividend.divide(bdivisor);
                            });
                        }

                    }
                }
            }
        }
    }

    [Test]
    public static void properScaleTests()
    {
        BigDecimal[][] testCases = [
            [new BigDecimal("1"),       new BigDecimal("5"),            new BigDecimal("2e-1")],
            [new BigDecimal("1"),       new BigDecimal("50e-1"),        new BigDecimal("2e-1")],
            [new BigDecimal("10e-1"),   new BigDecimal("5"),            new BigDecimal("2e-1")],
            [new BigDecimal("1"),       new BigDecimal("500e-2"),       new BigDecimal("2e-1")],
            [new BigDecimal("100e-2"),  new BigDecimal("5"),            new BigDecimal("20e-2")],
            [new BigDecimal("1"),       new BigDecimal("32"),           new BigDecimal("3125e-5")],
            [new BigDecimal("1"),       new BigDecimal("64"),           new BigDecimal("15625e-6")],
            [new BigDecimal("1.0000000"),       new BigDecimal("64"),   new BigDecimal("156250e-7")],
        ];

        foreach (BigDecimal[] tc in testCases)
        {
            BigDecimal quotient;
            Assert.That(quotient = tc[0].divide(tc[1]), Is.EqualTo(tc[2]));
        }
    }

    [Test]
    public static void trailingZeroTests()
    {
        MathContext mc = new MathContext(3, RoundingMode.FLOOR);
        BigDecimal[][] testCases = [
            [new BigDecimal("19"),      new BigDecimal("100"),          new BigDecimal("0.19")],
            [new BigDecimal("21"),      new BigDecimal("110"),          new BigDecimal("0.190")],
        ];

        foreach (BigDecimal[] tc in testCases)
        {
            BigDecimal quotient;
            Assert.That(quotient = tc[0].divide(tc[1], mc), Is.EqualTo(tc[2]));
        }
    }


    static IEnumerable<object[]> scaledRoundedDivideTestCases()
    {
        // Tests of the traditional scaled divide under different
        // rounding modes.

        // Encode rounding mode and scale for the divide in a
        // BigDecimal with the significand equal to the rounding mode
        // and the scale equal to the number's scale.

        // {dividend, dividisor, rounding, quotient}
        BigDecimal a = new BigDecimal("31415");
        BigDecimal a_minus = a.negate();
        BigDecimal b = new BigDecimal("10000");

        BigDecimal c = new BigDecimal("31425");
        BigDecimal c_minus = c.negate();

        // Ad hoc tests
        BigDecimal d = new BigDecimal(BigInteger.Parse("-37361671119238118911893939591735"), 10);
        BigDecimal e = new BigDecimal(BigInteger.Parse("74723342238476237823787879183470"), 15);

        object[][] testCases = [
            [a,         b,      (ROUND_UP, 3),        new BigDecimal("3.142")],
            [a_minus,   b,      (ROUND_UP, 3),        new BigDecimal("-3.142")],

            [a,         b,      (ROUND_DOWN, 3),      new BigDecimal("3.141")],
            [a_minus,   b,      (ROUND_DOWN, 3),      new BigDecimal("-3.141")],

            [a,         b,      (ROUND_CEILING, 3),   new BigDecimal("3.142")],
            [a_minus,   b,      (ROUND_CEILING, 3),   new BigDecimal("-3.141")],

            [a,         b,      (ROUND_FLOOR, 3),     new BigDecimal("3.141")],
            [a_minus,   b,      (ROUND_FLOOR, 3),     new BigDecimal("-3.142")],

            [a,         b,      (ROUND_HALF_UP, 3),   new BigDecimal("3.142")],
            [a_minus,   b,      (ROUND_HALF_UP, 3),   new BigDecimal("-3.142")],

            [a,         b,      (ROUND_DOWN, 3),      new BigDecimal("3.141")],
            [a_minus,   b,      (ROUND_DOWN, 3),      new BigDecimal("-3.141")],

            [a,         b,      (ROUND_HALF_EVEN, 3), new BigDecimal("3.142")],
            [a_minus,   b,      (ROUND_HALF_EVEN, 3), new BigDecimal("-3.142")],

            [c,         b,      (ROUND_HALF_EVEN, 3), new BigDecimal("3.142")],
            [c_minus,   b,      (ROUND_HALF_EVEN, 3), new BigDecimal("-3.142")],

            [d,         e,      (ROUND_HALF_UP, -5),   BigDecimal.valueOf(-1, -5)],
            [d,         e,      (ROUND_HALF_DOWN, -5), BigDecimal.valueOf(0, -5)],
            [d,         e,      (ROUND_HALF_EVEN, -5), BigDecimal.valueOf(0, -5)],
        ];
        return testCases;
    }
    [TestCaseSource(nameof(scaledRoundedDivideTestCases))]
    public static void scaledRoundedDivideTests(BigDecimal a, BigDecimal b, (int rm, int scale) c, BigDecimal expected)
    {
        int scale = c.scale;
        int rm = c.rm;

        BigDecimal quotient = a.divide(b, scale, rm);
        Assert.That(quotient, Is.EqualTo(expected));
    }

    public static IEnumerable<BigDecimal[]> scaledRoundedDivideTests6876282TestCases()
    {
        // 6876282
        BigDecimal[][] testCases2 = [
            // { dividend, divisor, expected quotient }
            [new BigDecimal(3090), new BigDecimal(7), new BigDecimal(441)],
            [ new BigDecimal("309000000000000000000000"), new BigDecimal("700000000000000000000"),
              new BigDecimal(441) ],
            [ new BigDecimal("962.430000000000"), new BigDecimal("8346463.460000000000"),
              new BigDecimal("0.000115309916") ],
            [ new BigDecimal("18446744073709551631"), new BigDecimal("4611686018427387909"),
              new BigDecimal(4) ],
            [ new BigDecimal("18446744073709551630"), new BigDecimal("4611686018427387909"),
              new BigDecimal(4) ],
            [ new BigDecimal("23058430092136939523"), new BigDecimal("4611686018427387905"),
              new BigDecimal(5) ],
            [ new BigDecimal("-18446744073709551661"), new BigDecimal("-4611686018427387919"),
              new BigDecimal(4) ],
            [ new BigDecimal("-18446744073709551660"), new BigDecimal("-4611686018427387919"),
              new BigDecimal(4) ],
        ];
        return testCases2;
    }

    [TestCaseSource(nameof(scaledRoundedDivideTests6876282TestCases))]
    public static void scaledRoundedDivideTests6876282(BigDecimal a, BigDecimal b, BigDecimal expected)
    {
        BigDecimal quo = a.divide(b, RoundingMode.HALF_UP);
        Assert.That(quo, Is.EqualTo(expected));
    }

    private static IEnumerable<object[]> divideByOneTestCases()
    {
        object[][] unscaledAndScale = [
            [ Long.MAX_VALUE,  17],
            [-Long.MAX_VALUE,  17],
            [ Long.MAX_VALUE,   0],
            [-Long.MAX_VALUE,   0],
            [ Long.MAX_VALUE, 100],
            [ -Long.MAX_VALUE, 100],
        ];
        return unscaledAndScale;
    }
    [TestCaseSource(nameof(divideByOneTestCases))]
    public static void divideByOneTests(long unscaled, int scale)
    {
        //problematic divisor: one with scale 17
        BigDecimal one = BigDecimal.ONE.setScale(17);

        BigDecimal noRound = BigDecimal.valueOf(unscaled, scale).
                divide(one, RoundingMode.UNNECESSARY);

        BigDecimal roundDown = BigDecimal.valueOf(unscaled, scale).
                divide(one, RoundingMode.DOWN);

        Assert.That(noRound.compareTo(roundDown), Is.EqualTo(0));
    }
}
