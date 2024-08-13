/*
 * Copyright (c) 2003, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 4916097
 * @summary Some exponent over/undeflow tests for the pow method
 * @author Joseph D. Darcy
 */

using NUnit.Framework;

namespace jdk.math.test;

public class PowTests
{
    static readonly object?[][] testCases = {
            [BigDecimal.valueOf(0, int.MaxValue),  (0),              BigDecimal.valueOf(1, 0)],
            [BigDecimal.valueOf(0, int.MaxValue),  (1),              BigDecimal.valueOf(0, int.MaxValue)],
            [BigDecimal.valueOf(0, int.MaxValue),  (2),              BigDecimal.valueOf(0, int.MaxValue)],
            [BigDecimal.valueOf(0, int.MaxValue),  (999999999),      BigDecimal.valueOf(0, int.MaxValue)],

            [BigDecimal.valueOf(0, int.MinValue),  (0),              BigDecimal.valueOf(1, 0)],
            [BigDecimal.valueOf(0, int.MinValue),  (1),              BigDecimal.valueOf(0, int.MinValue)],
            [BigDecimal.valueOf(0, int.MinValue),  (2),              BigDecimal.valueOf(0, int.MinValue)],
            [BigDecimal.valueOf(0, int.MinValue),  (999999999),      BigDecimal.valueOf(0, int.MinValue)],

            [BigDecimal.valueOf(1, int.MaxValue),  (0),              BigDecimal.valueOf(1, 0)],
            [BigDecimal.valueOf(1, int.MaxValue),  (1),              BigDecimal.valueOf(1, int.MaxValue)],
            [BigDecimal.valueOf(1, int.MaxValue),  (2),              null], // overflow
            [BigDecimal.valueOf(1, int.MaxValue),  (999999999),      null], // overflow

            [BigDecimal.valueOf(1, int.MinValue),  (0),              BigDecimal.valueOf(1, 0)],
            [BigDecimal.valueOf(1, int.MinValue),  (1),              BigDecimal.valueOf(1, int.MinValue)],
            [BigDecimal.valueOf(1, int.MinValue),  (2),              null], // underflow
            [BigDecimal.valueOf(1, int.MinValue),  (999999999),      null], // underflow
        };


    [TestCaseSource(nameof(testCases))]
    public static void zeroAndOneTests(BigDecimal num, int exponent, BigDecimal? expect)
    {
        if (expect != null)
        {
            var result = num.pow(exponent);
            Assert.That(result, Is.EqualTo(expect));
        }
        else
        {
            Assert.Catch<ArithmeticException>(() => num.pow(exponent));
        }
    }

}
