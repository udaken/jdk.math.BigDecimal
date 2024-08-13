/*
 * Copyright (c) 2005, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 6325535
 * @summary Test for the rounding behavior of negate(MathContext)
 * @author Joseph D. Darcy
 */

using NUnit.Framework;

namespace jdk.math.test;

public class NegateTests
{

    static BigDecimal negateThenRound(BigDecimal bd, MathContext mc)
    {
        return bd.negate().plus(mc);
    }


    static BigDecimal absThenRound(BigDecimal bd, MathContext mc)
    {
        return bd.abs().plus(mc);
    }


    [TestCaseSource(nameof(negateTests))]
    public static void negateTest(BigDecimal bd, BigDecimal expected, MathContext mc)
    {
        BigDecimal neg1 = bd.negate(mc);
        BigDecimal neg2 = negateThenRound(bd, mc);

        Assert.That(neg1, Is.EqualTo(expected));
        Assert.That(neg1, Is.EqualTo(neg2));

        // Test abs consistency
        BigDecimal abs = bd.abs(mc);
        BigDecimal expectedAbs = absThenRound(bd, mc);
        Assert.That(abs, Is.EqualTo(expectedAbs));
    }

    static IEnumerable<object[]> negateTests()
    {
        yield return new object[] { new BigDecimal("1.3"), new BigDecimal("-1"), new MathContext(1, RoundingMode.CEILING) };
        yield return new object[] { new BigDecimal("-1.3"), new BigDecimal("2"), new MathContext(1, RoundingMode.CEILING) };
        yield return new object[] { new BigDecimal("1.3"), new BigDecimal("-2"), new MathContext(1, RoundingMode.FLOOR) };
        yield return new object[] { new BigDecimal("-1.3"), new BigDecimal("1"), new MathContext(1, RoundingMode.FLOOR) };
    }
}
