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

/*
 * @test
 * @bug 8211936
 * @summary Tests of BigDecimal.intValue() and BigDecimal.longValue()
 */
using System;
using System.Diagnostics;
using System.Numerics;
using NUnit.Framework;

namespace jdk.math.test;

public class IntegralValueTests
{
    private static IEnumerable<object[]> testCaseSource()
    {
        yield return [INT_VALUES, true];
        yield return [LONG_VALUES, false];
    }

    private static readonly (BigDecimal, long)[] INT_VALUES =
        [
         // 2**31 - 1
         (new BigDecimal("2147483647"), int.MaxValue),
         (new BigDecimal("2147483647.0"), int.MaxValue),
         (new BigDecimal("2147483647.00"), int.MaxValue),

         (new BigDecimal("-2147483647"), -int.MaxValue),
         (new BigDecimal("-2147483647.0"), -int.MaxValue),

         // -2**31
         (new BigDecimal("-2147483648"), int.MinValue),
         (new BigDecimal("-2147483648.1"), int.MinValue),
         (new BigDecimal("-2147483648.01"), int.MinValue),

         // -2**31 + 1 truncation to 2**31 - 1
         (new BigDecimal("-2147483649"), int.MaxValue),

         // 2**64 - 1 truncation to 1
         (new BigDecimal("4294967295"), -1),

         // 2**64 truncation to 0
         (new BigDecimal("4294967296"), 0),

         // Fast path truncation to 0
         (new BigDecimal("1e32"), 0),

         // Slow path truncation to -2**31
         (new BigDecimal("1e31"), int.MinValue),

         // Slow path
         (new BigDecimal("1e0"), 1),

         // Fast path round to 0
         (new BigDecimal("9e-1"), 0),

         // Some random values
         (new BigDecimal("900e-1"), 90), // Increasing negative exponents
         (new BigDecimal("900e-2"), 9),
         (new BigDecimal("900e-3"), 0),

         // Fast path round to 0
         (new BigDecimal("123456789e-9"), 0),

         // Slow path round to 1
         (new BigDecimal("123456789e-8"), 1),

         // Increasing positive exponents
         (new BigDecimal("10000001e1"), 100000010),
         (new BigDecimal("10000001e10"), -1315576832),
         (new BigDecimal("10000001e100"), 0),
         (new BigDecimal("10000001e1000"), 0),
         (new BigDecimal("10000001e10000"), 0),
         (new BigDecimal("10000001e100000"), 0),
         (new BigDecimal("10000001e1000000"), 0),
         (new BigDecimal("10000001e10000000"), 0),
         (new BigDecimal("10000001e100000000"), 0),
         (new BigDecimal("10000001e1000000000"), 0),

         // Increasing negative exponents
         (new BigDecimal("10000001e-1"), 1000000),
         (new BigDecimal("10000001e-10"), 0),
         (new BigDecimal("10000001e-100"), 0),
         (new BigDecimal("10000001e-1000"), 0),
         (new BigDecimal("10000001e-10000"), 0),
         (new BigDecimal("10000001e-100000"), 0),
         (new BigDecimal("10000001e-1000000"), 0),
         (new BigDecimal("10000001e-10000000"), 0),
         (new BigDecimal("10000001e-100000000"), 0),
         (new BigDecimal("10000001e-1000000000"), 0),

         // Currency calculation to 4 places
         (new BigDecimal("12345.0001"), 12345),
         (new BigDecimal("12345.9999"), 12345),
         (new BigDecimal("-12345.0001"), -12345),
         (new BigDecimal("-12345.9999"), -12345),
        ];

    private static readonly (BigDecimal, long)[] LONG_VALUES =
        [
         // 2**63 - 1
         (new BigDecimal("9223372036854775807"), long.MaxValue),
         (new BigDecimal("9223372036854775807.0"), long.MaxValue),
         (new BigDecimal("9223372036854775807.00"), long.MaxValue),

         // 2**63 truncation to -2**63
         (new BigDecimal("-9223372036854775808"), long.MinValue),
         (new BigDecimal("-9223372036854775808.1"), long.MinValue),
         (new BigDecimal("-9223372036854775808.01"), long.MinValue),

         // -2**63 + 1 truncation to 2**63 - 1
         (new BigDecimal("-9223372036854775809"), 9223372036854775807L),

         // 2**64 - 1 truncation to -1
         (new BigDecimal("18446744073709551615"), -1L),

         // 2**64 truncation to 0
         (new BigDecimal("18446744073709551616"), 0L),

         // Slow path truncation to -2**63
         (new BigDecimal("1e63"),  -9223372036854775808L),
         (new BigDecimal("-1e63"), -9223372036854775808L),
         // Fast path with larger magnitude scale
         (new BigDecimal("1e64"), 0L),
         (new BigDecimal("-1e64"), 0L),
         (new BigDecimal("1e65"), 0L),
         (new BigDecimal("-1e65"), 0L),

         // Slow path
         (new BigDecimal("1e0"), 1L),

         // Fast path round to 0
         (new BigDecimal("9e-1"), 0L),

         // Some random values
         (new BigDecimal("900e-1"), 90L), // Increasing negative exponents
         (new BigDecimal("900e-2"), 9L),
         (new BigDecimal("900e-3"), 0L),

         // Fast path round to 0
         (new BigDecimal("123456789e-9"), 0L),

         // Slow path round to 1
         (new BigDecimal("123456789e-8"), 1L),

         // Increasing positive exponents
         (new BigDecimal("10000001e1"), 100000010L),
         (new BigDecimal("10000001e10"), 100000010000000000L),
         (new BigDecimal("10000001e100"), 0L),
         (new BigDecimal("10000001e1000"), 0L),
         (new BigDecimal("10000001e10000"), 0L),
         (new BigDecimal("10000001e100000"), 0L),
         (new BigDecimal("10000001e1000000"), 0L),
         (new BigDecimal("10000001e10000000"), 0L),
         (new BigDecimal("10000001e100000000"), 0L),
         (new BigDecimal("10000001e1000000000"), 0L),

         // Increasing negative exponents
         (new BigDecimal("10000001e-1"), 1000000L),
         (new BigDecimal("10000001e-10"), 0L),
         (new BigDecimal("10000001e-100"), 0L),
         (new BigDecimal("10000001e-1000"), 0L),
         (new BigDecimal("10000001e-10000"), 0L),
         (new BigDecimal("10000001e-100000"), 0L),
         (new BigDecimal("10000001e-1000000"), 0L),
         (new BigDecimal("10000001e-10000000"), 0L),
         (new BigDecimal("10000001e-100000000"), 0L),
         (new BigDecimal("10000001e-1000000000"), 0L),

         // Currency calculation to 4 places
         (new BigDecimal("12345.0001"), 12345L),
         (new BigDecimal("12345.9999"), 12345L),
         (new BigDecimal("-12345.0001"), -12345L),
         (new BigDecimal("-12345.9999"), -12345L),
        ];

    [Test]
    [TestCaseSource(nameof(testCaseSource))]
    public static void integralValuesTest((BigDecimal, long)[] v, bool isInt)
    {
        foreach ((BigDecimal bd, long expected) in v)
        {
            if (isInt)
            {
                Debug.Assert(expected is >= int.MinValue and <= int.MaxValue);
                int intValue = bd.intValue();
                Assert.That(intValue, Is.EqualTo((int)expected));
            }
            else
            {
                long longValue = bd.longValue();
                Assert.That(longValue, Is.EqualTo(expected));
            }
        }
    }
}
