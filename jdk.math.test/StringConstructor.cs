/*
 * Copyright (c) 1999, 2022, Oracle and/or its affiliates. All rights reserved.
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
 * @library /test/lib
 * @build jdk.test.lib.RandomFactory
 * @run main StringConstructor
 * @bug 4103117 4331084 4488017 4490929 6255285 6268365 8074460 8078672 8233760
 * @summary Tests the BigDecimal string constructor (use -Dseed=X to set PRNG seed).
 * @key randomness
 */
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Numerics;

namespace jdk.math.test;

public class StringConstructor
{
    [Test]
    public static void main()
    {
        constructWithError("");
        constructWithError("+");
        constructWithError("-");
        constructWithError("+e");
        constructWithError("-e");
        constructWithError("e+");
        constructWithError("1.-0");
        constructWithError(".-123");
        constructWithError("-");
        constructWithError("--1.1");
        constructWithError("-+1.1");
        constructWithError("+-1.1");
        constructWithError("1-.1");
        constructWithError("1+.1");
        constructWithError("1.111+1");
        constructWithError("1.111-1");
        constructWithError("11.e+");
        constructWithError("11.e-");
        constructWithError("11.e+-");
        constructWithError("11.e-+");
        constructWithError("11.e-+1");
        constructWithError("11.e+-1");

        // Range checks
        constructWithError("1e" + int.MinValue);
        constructWithError("10e" + int.MinValue);
        constructWithError("0.01e" + int.MinValue);
        constructWithError("1e" + ((long)int.MinValue - 1));

        /* These BigDecimals produce a string with an exponent > Integer.MAX_VALUE */
        roundtripWithAbnormalExponent(BigDecimal.valueOf(10, int.MinValue));
        roundtripWithAbnormalExponent(BigDecimal.valueOf(long.MinValue, int.MinValue));
        roundtripWithAbnormalExponent(new BigDecimal(BigInteger.Parse("1" + new string('0', 100)), int.MinValue));

        /* These Strings have an exponent > Integer.MAX_VALUE */
        roundtripWithAbnormalExponent("1.0E+2147483649");
        roundtripWithAbnormalExponent("-9.223372036854775808E+2147483666");
        roundtripWithAbnormalExponent("1.0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000E+2147483748");

        // Roundtrip tests
        var random = Randomizer.CreateRandomizer();
        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(100) + 1;
            BigInteger bi = BigIntegerUtility.Random(size, random);
            if (random.NextBool())
                bi = -bi;
            int decimalLength = bi.toString().Length;
            int scale = random.Next(decimalLength);
            BigDecimal bd = new BigDecimal(bi, scale);
            String bdString = bd.toString();
            // System.err.println("bi" + bi.toString() + "\tscale " + scale);
            // System.err.println("bd string: " + bdString);
            BigDecimal bdDoppel = new BigDecimal(bdString);
            Assert.That(bdDoppel, Is.EqualTo(bd), "String constructor failure.");
        }
    }

    private static void roundtripWithAbnormalExponent(BigDecimal bd)
    {
        Assert.That(new BigDecimal(bd.toString()), Is.EqualTo(bd), "Abnormal exponent roundtrip failure");
    }

    private static void roundtripWithAbnormalExponent(String s)
    {
        Assert.That(new BigDecimal(s).toString(), Is.EqualTo(s), "Abnormal exponent roundtrip failure");
    }

    /*
     * Verify precision is set properly if the significand has
     * non-ASCII leading zeros.
     */
    [Test]
    public static void nonAsciiZeroTest()
    {
        String[] values = {
            "00004e5",
            "\u0660\u0660\u0660\u06604e5",
        };

        BigDecimal expected = new BigDecimal("4e5");

        foreach (String s in values)
        {
            BigDecimal tmp = new BigDecimal(s);
            Assert.That(tmp, Is.EqualTo(expected));
            Assert.That(tmp.precision(), Is.EqualTo(1));
        }
    }

    [Test]
    public static void leadingExponentZeroTest()
    {
        BigDecimal twelve = new BigDecimal("12");
        BigDecimal onePointTwo = new BigDecimal("1.2");

        String start = "1.2e0";
        String end = "1";
        String middle = "";

        // Test with more excess zeros than the largest number of
        // decimal digits needed to represent a long
        int limit = ((int)Math.Log10(long.MaxValue)) + 6;
        for (int i = 0; i < limit; i++, middle += "0")
        {
            String t1 = start + middle;
            String t2 = t1 + end;

            testString(t1, onePointTwo);
            testString(t2, twelve);
        }
    }

    private static void testString(String s, BigDecimal expected)
    {
        Assert.That(new BigDecimal(s), Is.EqualTo(expected));
        Assert.That(new BigDecimal(switchZero(s)), Is.EqualTo(expected));
    }

    private static String switchZero(String s)
    {
        return s.Replace('0', '\u0660'); // Arabic-Indic zero
    }

    private static void constructWithError(String badString)
    {
        Assert.Catch<FormatException>(() => new BigDecimal(badString));
    }
}
