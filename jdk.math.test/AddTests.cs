/*
 * Copyright (c) 2006, 2018, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 6362557 8200698
 * @summary Some tests of add(BigDecimal, mc)
 * @author Joseph D. Darcy
 */
using System;
using System.Numerics;
using NUnit.Framework;

namespace jdk.math.test;

[TestFixture]
public class AddTests
{
    private static HashSet<RoundingMode> nonExactRoundingModes = new HashSet<RoundingMode>(
        Enum.GetValues<RoundingMode>().Where(e => e != RoundingMode.UNNECESSARY));

    [SetUp]
    public void Setup()
    {
        nonExactRoundingModes.Remove(RoundingMode.UNNECESSARY);
    }

    [Test]
    public void SimpleTests()
    {
        BigDecimal[] bd1 = {
            new BigDecimal(BigInteger.Parse("7812404666936930160"), 11),
            new BigDecimal(BigInteger.Parse("7812404666936930160"), 12),
            new BigDecimal(BigInteger.Parse("7812404666936930160"), 13),
        };
        BigDecimal bd2 = new BigDecimal(BigInteger.Parse("2790000"), 1);
        BigDecimal[] expectedResult = {
            new BigDecimal("78403046.66936930160"),
            new BigDecimal("8091404.666936930160"),
            new BigDecimal("1060240.4666936930160"),
        };

        for (int i = 0; i < bd1.Length; i++)
        {
            Assert.AreEqual(expectedResult[i], bd1[i].add(bd2));
        }
    }

    [Test]
    public void ExtremaTests()
    {
        Assert.DoesNotThrow(() => BigDecimal.valueOf(1, -int.MaxValue).add(BigDecimal.valueOf(2, int.MaxValue), new MathContext(2, RoundingMode.DOWN)));
        Assert.DoesNotThrow(() => BigDecimal.valueOf(1, -int.MaxValue).add(BigDecimal.valueOf(-2, int.MaxValue), new MathContext(2, RoundingMode.DOWN)));
    }

    public static IEnumerable<object[]> RoundingGradationTests()
    {
        (string, string)[] data = [
            ( "1234e100", "1234e97" ),
            ( "1234e100", "1234e96" ),
            ( "1234e100", "1234e95" ),
            ( "1234e100", "1234e94" ),
            ( "1234e100", "1234e93" ),
            ( "1234e100", "1234e92" ),
            ( "1234e100", "1234e50" ),

            ( "1000e100", "1234e97" ),
            ( "1000e100", "1234e96" ),
            ( "1000e100", "1234e95" ),
            ( "1000e100", "1234e94" ),
            ( "1000e100", "1234e93" ),
            ( "1000e100", "1234e92" ),
            ( "1000e100", "1234e50" ),

            ( "1999e100", "1234e97" ),
            ( "1999e100", "1234e96" ),
            ( "1999e100", "1234e95" ),
            ( "1999e100", "1234e94" ),
            ( "1999e100", "1234e93" ),
            ( "1999e100", "1234e92" ),
            ( "1999e100", "1234e50" ),

            ( "9999e100", "1234e97" ),
            ( "9999e100", "1234e96" ),
            ( "9999e100", "1234e95" ),
            ( "9999e100", "1234e94" ),
            ( "9999e100", "1234e93" ),
            ( "9999e100", "1234e92" ),
            ( "9999e100", "1234e50" ),
        ];

        return
            (from x in data
             from rm in nonExactRoundingModes
             select new object[] { x.Item1, x.Item2, rm })
             .Union(
            from x in data
            from rm in nonExactRoundingModes
            select new object[] { x.Item2, x.Item1, rm }
            );
    }

    [Test]
    public void PrecisionConsistencyTest()
    {
        MathContext mc = new MathContext(1, RoundingMode.DOWN);
        BigDecimal a = BigDecimal.valueOf(1999, -1); // value is equivalent to 19990

        BigDecimal sum1 = a.add(BigDecimal.One, mc);
        _ = a.precision();
        BigDecimal sum2 = a.add(BigDecimal.One, mc);

        Assert.AreEqual(sum1, sum2, "Unequal sums after calling precision!");
    }

    [Test]
    public void ArithmeticExceptionTest()
    {
        var val = new BigDecimal("1e2147483647");
        Assert.Catch<ArithmeticException>(() =>
        {
            BigDecimal x = val.add(new BigDecimal(1));
        });
    }

    [TestCaseSource(nameof(RoundingGradationTests))]
    public void TestRoundAway(string s1, string s2, RoundingMode rm)
    {
        BigDecimal b1 = new BigDecimal(s1);
        BigDecimal b2 = new BigDecimal(s2);

        _ = b1.precision();
        _ = b2.precision();

        BigDecimal b1_negate = b1.negate();
        BigDecimal b2_negate = b2.negate();

        _ = b1_negate.precision();
        _ = b2_negate.precision();

        TestRoundAway0(b1, b2, rm);
        TestRoundAway0(b1, b2_negate, rm);
        TestRoundAway0(b1_negate, b2, rm);
        TestRoundAway0(b1_negate, b2_negate, rm);
    }

    private void TestRoundAway0(BigDecimal b1, BigDecimal b2, RoundingMode rm)
    {
        BigDecimal exactSum = b1.add(b2);

        for (int precision = 1; precision < exactSum.precision() + 2; precision++)
        {
            MathContext mc = new MathContext(precision, rm);
            BigDecimal roundedExactSum = exactSum.round(mc);

            BigDecimal sum = b1.add(b2, mc);
            Assert.AreEqual(roundedExactSum, sum,
                $"Exact sum {exactSum} [new BigDecimal(\"{b1}\").add(new BigDecimal(\"{b2}\"))]rounded by {mc} expected: {roundedExactSum} got: {sum}");
        }
    }
}