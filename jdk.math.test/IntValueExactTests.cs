/*
 * Copyright (c) 2019, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 8211936
 * @summary Tests of BigDecimal.intValueExact
 */
using System;
using System.Numerics;
using NUnit.Framework;

namespace jdk.math.test;

public class IntValueExactTests
{

    private static int simpleIntValueExact(BigDecimal bd)
    {
        return bd.toBigIntegerExact().intValue();
    }

    [Test]
    public static void intValueExactSuccessful()
    {
        // Strings used to create BigDecimal instances on which invoking
        // intValueExact() will succeed.
        (BigDecimal, int)[] successCases = [
                        (new BigDecimal("2147483647"),    int.MaxValue), // 2^31 -1
                        (new BigDecimal("2147483647.0"),  int.MaxValue),
                        (new BigDecimal("2147483647.00"), int.MaxValue),

                        (new BigDecimal("-2147483648"),   int.MinValue), // -2^31
                        (new BigDecimal("-2147483648.0"), int.MinValue),
                        (new BigDecimal("-2147483648.00"),int.MinValue),

                        (new BigDecimal("1e0"),    1),
                        (new BigDecimal(BigInteger.One, -9),   1_000_000_000),

                        (new BigDecimal("0e13"),   0), // Fast path zero
                        (new BigDecimal("0e32"),   0),
                        (new BigDecimal("0e512"), 0),

                        (new BigDecimal("10.000000000000000000000000000000000"), 10),
        ];

        foreach ((BigDecimal bd, int expected) in successCases)
        {
            int intValueExact = bd.intValueExact();

            Assert.That(intValueExact, Is.EqualTo(expected));
            Assert.That(simpleIntValueExact(bd), Is.EqualTo(intValueExact));
        }
    }

    [Test]
    public static void intValueExactExceptional()
    {
        BigDecimal[] exceptionalCases = [
                    new BigDecimal("2147483648"), // int.MaxValue + 1
                    new BigDecimal("2147483648.0"),
                    new BigDecimal("2147483648.00"),
                    new BigDecimal("-2147483649"), // int.MinValue - 1
                    new BigDecimal("-2147483649.1"),
                    new BigDecimal("-2147483649.01"),

                    new BigDecimal("9999999999999999999999999999999"),
                    new BigDecimal("10000000000000000000000000000000"),

                    new BigDecimal("0.99"),
                    new BigDecimal("0.999999999999999999999"),
        ];

        foreach (BigDecimal bd in exceptionalCases)
        {
            Assert.Catch<ArithmeticException>(() => bd.intValueExact());
        }
    }
}
