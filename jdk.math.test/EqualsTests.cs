/*
 * Copyright (c) 2009, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 1234567
 * @summary Test BigDecimal.equals() method.
 * @author xlu
 */

using System;
using System.Numerics;
using NUnit.Framework;
using static jdk.math.BigDecimal;

namespace jdk.math.test;
[TestFixture]
public class EqualsTests
{
    [Test]
    public static void main()
    {

        BigDecimal[][] testValues = [
            // The even index is supposed to return true for equals call and
            // the odd index is supposed to return false, i.e. not equal.
            [ZERO, ZERO],
            [ONE, TEN],

            [valueOf(int.MaxValue), valueOf(int.MaxValue)],
            [valueOf(long.MaxValue), valueOf(-long.MaxValue)],

            [valueOf(12345678), valueOf(12345678)],
            [valueOf(123456789), valueOf(123456788)],

            [new BigDecimal("123456789123456789123"),
             new BigDecimal(BigInteger.Parse("123456789123456789123"))],
            [new BigDecimal("123456789123456789123"),
             new BigDecimal(BigInteger.Parse("123456789123456789124"))],

            [valueOf(long.MinValue), new BigDecimal("-9223372036854775808")],
            [new BigDecimal("9223372036854775808"), valueOf(long.MaxValue)],

            [valueOf(Math.Round(Math.Pow(2, 10))), new BigDecimal("1024")],
            [new BigDecimal("1020"), valueOf(Math.Pow(2, 11))],

            [new BigDecimal(new BigInteger(2).pow(65)),
             new BigDecimal("36893488147419103232")],
            [new BigDecimal("36893488147419103231.81"),
             new BigDecimal("36893488147419103231.811"),
            ]
        ];

        bool expected = true;
        foreach (BigDecimal[] testValuePair in testValues)
        {
            bool result = testValuePair[0].equals(testValuePair[1]);
            Assert.That(result, Is.EqualTo(expected));
            expected = !expected;
        }

    }
}
