/*
 * Copyright (c) 2006, 2013, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 6473768
 * @summary Tests of BigDecimal.compareTo
 * @author Joseph D. Darcy
 */
using System;
using NUnit.Framework;

using static jdk.math.BigDecimal;
namespace jdk.math.test;

[TestFixture]
public class CompareToTests
{
    [Test]
    public void TestCompareTo()
    {
        // First operand, second operand, expected compareTo result
        (BigDecimal, BigDecimal, int)[] testCases = {
            // Šî–{
            (valueOf(0),        valueOf(0),     0),
            (valueOf(0),        valueOf(1),     -1),
            (valueOf(1),        valueOf(2),     -1),
            (valueOf(2),        valueOf(1),     1),
            (valueOf(10),       valueOf(10),    0),
            // Significands would compare differently than scaled value
            (valueOf(2,1),      valueOf(2),     -1),
            (valueOf(2,-1),     valueOf(2),     1),
            (valueOf(1,1),      valueOf(2),     -1),
            (valueOf(1,-1),     valueOf(2),     1),
            (valueOf(5,-1),     valueOf(2),     1),
            // Boundary and near boundary values
            (valueOf(long.MaxValue),            valueOf(long.MaxValue), 0),
            (valueOf(long.MaxValue).negate(),   valueOf(long.MaxValue), -1),
            (valueOf(long.MaxValue-1),          valueOf(long.MaxValue), -1),
            (valueOf(long.MaxValue-1).negate(), valueOf(long.MaxValue), -1),
            (valueOf(long.MinValue),            valueOf(long.MaxValue), -1),
            (valueOf(long.MinValue).negate(),   valueOf(long.MaxValue), 1),
            (valueOf(long.MinValue+1),          valueOf(long.MaxValue), -1),
            (valueOf(long.MinValue+1).negate(), valueOf(long.MaxValue), 0),
            (valueOf(long.MaxValue),            valueOf(long.MinValue), 1),
            (valueOf(long.MaxValue).negate(),   valueOf(long.MinValue), 1),
            (valueOf(long.MaxValue-1),          valueOf(long.MinValue), 1),
            (valueOf(long.MaxValue-1).negate(), valueOf(long.MinValue), 1),
            (valueOf(long.MinValue),            valueOf(long.MinValue), 0),
            (valueOf(long.MinValue).negate(),   valueOf(long.MinValue), 1),
            (valueOf(long.MinValue+1),          valueOf(long.MinValue), 1),
            (valueOf(long.MinValue+1).negate(), valueOf(long.MinValue), 1),
        };

        foreach ((BigDecimal a, BigDecimal b, int expected) in testCases)
        {
            BigDecimal a_negate = -a;
            BigDecimal b_negate = -b;

            Assert.That(a.compareTo(b), Is.EqualTo(expected));
            Assert.That(a_negate.compareTo(b_negate), Is.EqualTo(-expected));
        }

    }
}