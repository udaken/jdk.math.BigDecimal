/*
 * Copyright (c) 2003, 2018, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 7036582
 * @summary Some new tests for the add method and constructor with MathContext.
 * @run main RangeTests
 * @run main/othervm -XX:+IgnoreUnrecognizedVMOptions -XX:+EliminateAutoBox -XX:AutoBoxCacheMax=20000 RangeTests
 * @author Sergey V. Kuksenko
 */
using NUnit.Framework;
using System.Numerics;

namespace jdk.math.test;

public class RangeTests
{

    private static void addTest(BigDecimal arg1, BigDecimal arg2, BigDecimal expectedResult)
    {
        Assert.That(arg1.add(arg2), Is.EqualTo(expectedResult));
        Assert.That(arg2.add(arg1), Is.EqualTo(expectedResult));
    }

    /*
     *  Test BigDecimal.add(BigDecimal) when values are withing different ranges:
     *  1. within 32 bits
     *  2. within 64 bits
     *  3. outside 64 bits.
     */
    [Test]
    public static void addBoundaryTest()
    {
        addTest(
                new BigDecimal("85070591730234615847396907784232501249"),
                BigDecimal.valueOf(0),
                new BigDecimal("85070591730234615847396907784232501249"));
        addTest(
                new BigDecimal("-85070591730234615847396907784232501249"),
                BigDecimal.valueOf(0),
                new BigDecimal("-85070591730234615847396907784232501249"));
        addTest(
                new BigDecimal("85070591730234615847396907784232501249"),
                BigDecimal.valueOf(1),
                new BigDecimal("85070591730234615847396907784232501250"));
        addTest(
                new BigDecimal("85070591730234615847396907784232501249"),
                BigDecimal.valueOf(-1),
                new BigDecimal("85070591730234615847396907784232501248"));
        addTest(
                new BigDecimal("-85070591730234615847396907784232501250"),
                BigDecimal.valueOf(-1),
                new BigDecimal("-85070591730234615847396907784232501251"));
        addTest(
                new BigDecimal("-85070591730234615847396907784232501249"),
                BigDecimal.valueOf(1),
                new BigDecimal("-85070591730234615847396907784232501248"));
        addTest(
                new BigDecimal("147573952589676412927"),
                BigDecimal.valueOf(int.MaxValue),
                new BigDecimal("147573952591823896574"));
        addTest(
                new BigDecimal("-147573952589676412927"),
                BigDecimal.valueOf(int.MaxValue),
                new BigDecimal("-147573952587528929280"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(999),
                new BigDecimal("79228162514264337593543951334"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(int.MaxValue / 2),
                new BigDecimal("79228162514264337594617692158"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(int.MinValue / 2),
                new BigDecimal("79228162514264337592470208511"));
        addTest(
                new BigDecimal("-79228162514264337593543950335"),
                BigDecimal.valueOf(int.MaxValue / 2),
                new BigDecimal("-79228162514264337592470208512"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(-(int.MinValue / 2)),
                new BigDecimal("79228162514264337594617692159"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(long.MaxValue / 2),
                new BigDecimal("79228162518876023611971338238"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(long.MinValue / 2),
                new BigDecimal("79228162509652651575116562431"));
        addTest(
                new BigDecimal("-79228162514264337593543950335"),
                BigDecimal.valueOf(long.MaxValue / 2),
                new BigDecimal("-79228162509652651575116562432"));
        addTest(
                new BigDecimal("79228162514264337593543950335"),
                BigDecimal.valueOf(-(long.MinValue / 2)),
                new BigDecimal("79228162518876023611971338239"));
        addTest(
                new BigDecimal("-9223372036854775808"),
                BigDecimal.valueOf(1),
                new BigDecimal("-9223372036854775807"));
        addTest(
                new BigDecimal("-9223372036854775808"),
                BigDecimal.valueOf(long.MaxValue / 2),
                new BigDecimal("-4611686018427387905"));
        addTest(
                new BigDecimal("9223372036854775808"),
                BigDecimal.valueOf(-1),
                new BigDecimal("9223372036854775807"));
        addTest(
                new BigDecimal("9223372036854775808"),
                BigDecimal.valueOf(-long.MaxValue / 2),
                new BigDecimal("4611686018427387905"));
    }

    private static void testRoundingFromBigInteger(BigInteger bi, int scale, MathContext mc)
    {
        BigDecimal bd1 = new BigDecimal(bi, scale, mc);
        BigDecimal bd2 = (new BigDecimal(bi, scale)).round(mc);
        Assert.That(bd1, Is.EqualTo(bd2));
    }

    [Test]
    public static void roundingConstructorTest()
    {
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                7, MathContext.DECIMAL64);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                0, MathContext.DECIMAL64);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                -7, MathContext.DECIMAL64);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                7, MathContext.DECIMAL128);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                177, MathContext.DECIMAL128);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                177, MathContext.DECIMAL32);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                177, MathContext.UNLIMITED);
        testRoundingFromBigInteger(
                BigInteger.Parse("85070591730234615847396907784232501249"),
                0, MathContext.UNLIMITED);
    }

    private static void minLongConstructorTest(MathContext mc)
    {
        BigDecimal bd1 = new BigDecimal(long.MinValue, mc);
        BigDecimal bd2 = new BigDecimal(long.MinValue).round(mc);
        Assert.That(bd1, Is.EqualTo(bd2));
    }

    [Test]
    public static void minLongConstructorTest()
    {
        minLongConstructorTest(MathContext.UNLIMITED);
        minLongConstructorTest(MathContext.DECIMAL32);
        minLongConstructorTest(MathContext.DECIMAL64);
        minLongConstructorTest(MathContext.DECIMAL128);
    }
}
