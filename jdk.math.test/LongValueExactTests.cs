/*
 * Copyright (c) 2005, 2023, Oracle and/or its affiliates. All rights reserved.
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

/**
 * @test
 * @bug 6806261 8211936 8305343
 * @summary Tests of BigDecimal.longValueExact
 */
using System;
using System.Numerics;
using NUnit.Framework;

namespace jdk.math.test;

public class LongValueExactTests
{
    private static long simpleLongValueExact(BigDecimal bd)
    {
        return bd.toBigIntegerExact().longValue();
    }

    [Test]
    public static void longValueExactSuccessful()
    {

        // Strings used to create BigDecimal instances on which invoking
        // longValueExact() will succeed.
        (BigDecimal, long)[] successCases = [

                (new BigDecimal("9223372036854775807"),    long.MaxValue), // 2^63 -1
                (new BigDecimal("9223372036854775807.0"),  long.MaxValue),
                (new BigDecimal("9223372036854775807.00"), long.MaxValue),

                (new BigDecimal("-9223372036854775808"),   long.MinValue), // -2^63
                (new BigDecimal("-9223372036854775808.0"), long.MinValue),
                (new BigDecimal("-9223372036854775808.00"),long.MinValue),

                (new BigDecimal("1e0"),    1L),
                (new BigDecimal(BigInteger.One, -18),   1_000_000_000_000_000_000L),

                (new BigDecimal("0e13"),   0L), // Fast path zero
                (new BigDecimal("0e64"),   0L),
                (new BigDecimal("0e1024"), 0L),

                (new BigDecimal("10.000000000000000000000000000000000"), 10L),
                ];

        foreach ((BigDecimal bd, long expected) in successCases)
        {
            long longValueExact = bd.longValueExact();
            Assert.That(longValueExact, Is.EqualTo(expected));
            Assert.That(simpleLongValueExact(bd), Is.EqualTo(longValueExact));
        }
    }

    [Test]
    public static void longValueExactExceptional()
    {
        BigDecimal[] exceptionalCases = [
                new BigDecimal("9223372036854775808"), // long.MaxValue + 1
                new BigDecimal("9223372036854775808.0"),
                new BigDecimal("9223372036854775808.00"),
                new BigDecimal("-9223372036854775809"), // long.MinValue - 1
                new BigDecimal("-9223372036854775808.1"),
                new BigDecimal("-9223372036854775808.01"),

                new BigDecimal("9999999999999999999"),
                new BigDecimal("10000000000000000000"),

                new BigDecimal("0.99"),
                new BigDecimal("0.999999999999999999999"),
                ];

        foreach (BigDecimal bd in exceptionalCases)
        {
            Assert.Catch<ArithmeticException>(() => bd.longValueExact());
        }
    }

    [Test]
    public static void longValueExactExceptional8305343()
    {
        BigDecimal[] exceptionalCases = [
                new BigDecimal("1e" + (int.MaxValue - 1)),
                new BigDecimal("1e" + (int.MaxValue)),
                ];

        foreach (BigDecimal bd in exceptionalCases)
        {
            Assert.Throws<OverflowException>(() => bd.longValueExact());
        }
    }

}
