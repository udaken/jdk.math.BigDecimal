/*
 * Copyright (c) 2022, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 8205592
 * @summary Verify {double, float}Value methods work
 * @library /test/lib
 * @key randomness
 * @build jdk.test.lib.RandomFactory
 * @run main DoubleSingleValueTests
 */

using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace jdk.math.test;

using static jdk.math.test.MathSupport;

public class DoubleSingleValueTests
{
    const float SingleMinNormal = 1.17549435E-38f;
    const double DoubleMinNormal = 2.2250738585072014E-308;
    private static readonly BigDecimal HALF = BigDecimal.valueOf(5, 1);
    private static readonly BigDecimal EPS = BigDecimal.valueOf(1, 10_000);


    [SetUp]
    public void Setup()
    {
        Assert.That(SingleMinNormal, Is.EqualTo(BitConverter.Int32BitsToSingle(0x00800000)));
        Assert.That(DoubleMinNormal, Is.EqualTo(BitConverter.Int64BitsToDouble(0x0010000000000000L)));
    }



    private static BigDecimal nextHalfUp(double v)
    {
        BigDecimal bv = new BigDecimal(v);
        BigDecimal ulp = new BigDecimal(MathUlp(v));
        return bv.add(ulp.multiply(HALF));
    }

    private static BigDecimal nextHalfDown(double v)
    {
        BigDecimal bv = new BigDecimal(v);
        BigDecimal ulp = new BigDecimal(v - MathNextDown(v));
        return bv.subtract(ulp.multiply(HALF));
    }

    private static BigDecimal nextHalfUp(float v)
    {
        BigDecimal bv = new BigDecimal(v);
        BigDecimal ulp = new BigDecimal(MathUlp(v));
        return bv.add(ulp.multiply(HALF));
    }

    private static BigDecimal nextHalfDown(float v)
    {
        BigDecimal bv = new BigDecimal(v);
        BigDecimal ulp = new BigDecimal(v - MathNextDown(v));
        return bv.subtract(ulp.multiply(HALF));
    }

    private static String toDecHexString(double v)
    {
        return $"{v} (0x{v:x})";
    }

    private static String toDecHexString(float v)
    {
        return $"{v} (0x{v:x})";
    }

    [StackTraceHidden]
    private static void checkDouble(BigDecimal bd, double exp)
    {
        Assert.That(bd.doubleValue(), Is.EqualTo(exp));
    }

    [StackTraceHidden]
    private static void checkSingle(BigDecimal bv, float exp)
    {
        Assert.That(bv.floatValue(), Is.EqualTo(exp));
    }

    private static bool isOdd(int n)
    {
        return (n & 0x1) != 0;
    }

    [Test]
    public void DoubleValueNearMinValue([Range(0, 100 - 1)] int n)
    {
        BigDecimal b = nextHalfUp(n * Double.MIN_VALUE);
        checkDouble(b, ((n + 1) / 2 * 2) * Double.MIN_VALUE);
        checkDouble(b.subtract(EPS), n * Double.MIN_VALUE);
        checkDouble(b.add(EPS), (n + 1) * Double.MIN_VALUE);
    }

    [Test]
    public void SingleValueNearMinValue([Range(0, 100 - 1)] int n)
    {
        BigDecimal b = nextHalfUp(n * Float.MIN_VALUE);
        checkSingle(b, ((n + 1) / 2 * 2) * Float.MIN_VALUE);
        checkSingle(b.subtract(EPS), n * Float.MIN_VALUE);
        checkSingle(b.add(EPS), (n + 1) * Float.MIN_VALUE);
    }

    [Test]
    public void DoubleValueNearMinNormal()
    {
        double v = DoubleMinNormal;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfDown(v);
            checkDouble(bv, isOdd(n) ? MathNextDown(v) : v);
            checkDouble(bv.subtract(EPS), MathNextDown(v));
            checkDouble(bv.add(EPS), v);
            v = MathNextDown(v);
        }
        v = DoubleMinNormal;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfUp(v);
            checkDouble(bv, isOdd(n) ? MathNextUp(v) : v);
            checkDouble(bv.subtract(EPS), v);
            checkDouble(bv.add(EPS), MathNextUp(v));
            v = MathNextUp(v);
        }
    }

    [Test]
    public void SingleValueNearMinNormal()
    {
        float v = SingleMinNormal;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfDown(v);
            checkSingle(bv, isOdd(n) ? MathNextDown(v) : v);
            checkSingle(bv.subtract(EPS), MathNextDown(v));
            checkSingle(bv.add(EPS), v);
            v = MathNextDown(v);
        }
        v = SingleMinNormal;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfUp(v);
            checkSingle(bv, isOdd(n) ? MathNextUp(v) : v);
            checkSingle(bv.subtract(EPS), v);
            checkSingle(bv.add(EPS), MathNextUp(v));
            v = MathNextUp(v);
        }
    }

    [Test]
    public void DoubleValueNearMaxValue()
    {
        double v = Double.MAX_VALUE;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfDown(v);
            checkDouble(bv, isOdd(n) ? v : MathNextDown(v));
            checkDouble(bv.subtract(EPS), MathNextDown(v));
            checkDouble(bv.add(EPS), v);
            v = MathNextDown(v);
        }
        {
            BigDecimal bv = nextHalfUp(Double.MAX_VALUE);
            checkDouble(bv, Double.POSITIVE_INFINITY);
            checkDouble(bv.subtract(EPS), Double.MAX_VALUE);
            checkDouble(bv.add(EPS), Double.POSITIVE_INFINITY);
        }
    }

    [Test]
    public void SingleValueNearMaxValue()
    {
        float v = Single.MaxValue;
        for (int n = 0; n < 100; ++n)
        {
            BigDecimal bv = nextHalfDown(v);
            checkSingle(bv, isOdd(n) ? v : MathNextDown(v));
            checkSingle(bv.subtract(EPS), MathNextDown(v));
            checkSingle(bv.add(EPS), v);
            v = MathNextDown(v);
        }
        {
            BigDecimal bv = nextHalfUp(Single.MaxValue);
            checkSingle(bv, Single.PositiveInfinity);
            checkSingle(bv.subtract(EPS), Single.MaxValue);
            checkSingle(bv.add(EPS), Single.PositiveInfinity);
        }
    }

    [Test]
    public void DoubleValueRandom()
    {
        var r = Random.Shared;
        for (int i = 0; i < 5_000; ++i)
        {
            double v = r.NextDouble() * Double.MAX_VALUE;
            checkDouble(new BigDecimal(v), v);
        }
        for (int i = 0; i < 5_000; ++i)
        {
            double v = r.NextDouble() * -Double.MAX_VALUE;
            checkDouble(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            double v = r.NextDouble(-1e9, 1e9);
            checkDouble(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            double v = r.NextDouble(-1e6, 1e6);
            checkDouble(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            double v = r.NextDouble(-1e-6, 1e-6);
            checkDouble(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            double v = r.NextDouble(-1e-9, 1e-9);
            checkDouble(new BigDecimal(v), v);
        }
    }

    [Test]
    public void SingleValueRandom()
    {
        var r = Randomizer.CreateRandomizer();
        for (int i = 0; i < 10_000; ++i)
        {
            float v = r.NextFloat(-Single.MaxValue, Single.MaxValue);
            Debug.Assert(!Single.IsNaN(v));
            Debug.Assert(!Single.IsInfinity(v));
            checkSingle(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            float v = r.NextFloat(-1e9f, 1e9f);
            checkSingle(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            float v = r.NextFloat(-1e6f, 1e6f);
            checkSingle(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            float v = r.NextFloat(-1e-6f, 1e-6f);
            checkSingle(new BigDecimal(v), v);
        }
        for (int i = 0; i < 10_000; ++i)
        {
            float v = r.NextFloat(-1e-9f, 1e-9f);
            checkSingle(new BigDecimal(v), v);
        }
    }

    [Test]
    public void DoubleValueExtremes()
    {
        checkDouble(BigDecimal.valueOf(1, 1000), 0.0);
        checkDouble(BigDecimal.valueOf(-1, 1000), -0.0);
        checkDouble(BigDecimal.valueOf(1, -1000), Double.POSITIVE_INFINITY);
        checkDouble(BigDecimal.valueOf(-1, -1000), -Double.POSITIVE_INFINITY);
    }

    [Test]
    public void SingleValueExtremes()
    {
        checkSingle(BigDecimal.valueOf(1, 1000), 0.0f);
        checkSingle(BigDecimal.valueOf(-1, 1000), -0.0f);
        checkSingle(BigDecimal.valueOf(1, -1000), Single.PositiveInfinity);
        checkSingle(BigDecimal.valueOf(-1, -1000), -Single.PositiveInfinity);
    }


}

file static class RandomExtention
{
    public static double NextDouble(this Random r, double min, double max)
    {
        if (max <= min)
            throw new ArgumentOutOfRangeException(nameof(max), "Maximum value must be greater than or equal to minimum.");

        if (max == min)
            return min;

        double range = max - min;
        return r.NextDouble() * range + min;
    }
}