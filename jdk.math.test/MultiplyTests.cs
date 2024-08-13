/*
 * Copyright (c) 2006, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 6850606
 * @summary Test BigDecimal.multiply(BigDecimal)
 * @author xlu
 */

using NUnit.Framework;

namespace jdk.math.test;

public class MultiplyTests
{
    private static IEnumerable<BigDecimal[]> TestData()
    {
        BigDecimal[] bd1 = {
            new BigDecimal("123456789"),
            new BigDecimal("1234567898"),
            new BigDecimal("12345678987")
        };

        BigDecimal[] bd2 = {
            new BigDecimal("987654321"),
            new BigDecimal("8987654321"),
            new BigDecimal("78987654321")
        };
        // Two dimensonal array recording bd1[i] * bd2[j] &
        // 0 <= i <= 2 && 0 <= j <= 2;
        BigDecimal[][] expectedResults = {
            new []{new BigDecimal("121932631112635269"),
             new BigDecimal("1109586943112635269"),
             new BigDecimal("9751562173112635269")
            },
            new []{ new BigDecimal("1219326319027587258"),
              new BigDecimal("11095869503027587258"),
              new BigDecimal("97515622363027587258")
            },
            new []{ new BigDecimal("12193263197189452827"),
              new BigDecimal("110958695093189452827"),
              new BigDecimal("975156224183189452827")
            }
        };

        for (int i = 0; i < bd1.Length; i++)
        {
            for (int j = 0; j < bd2.Length; j++)
            {
                yield return new[] { bd1[i], bd2[j], expectedResults[i][j] };
            }
        }
    }

    [TestCaseSource(nameof(TestData))]
    public static void multiplyTests(BigDecimal x, BigDecimal y, BigDecimal expectedResult)
    {
        Assert.That(x.multiply(y), Is.EqualTo(expectedResult));
    }
    [Test]
    public static void multiplyTests()
    {
        BigDecimal x = BigDecimal.valueOf(8L, 1);
        BigDecimal xPower = BigDecimal.valueOf(-1L);

        for (int i = 0; i < 100; i++)
        {
            xPower = xPower.multiply(x);
        }
    }
}
