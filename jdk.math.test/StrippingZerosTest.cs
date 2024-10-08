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
 * @bug 4108852
 * @summary A few tests of stripTrailingZeros
 * @run main StrippingZerosTest
 * @run main/othervm -XX:+IgnoreUnrecognizedVMOptions -XX:+EliminateAutoBox -XX:AutoBoxCacheMax=20000 StrippingZerosTest
 * @author Joseph D. Darcy
 */

using NUnit.Framework;

namespace jdk.math.test;

[TestFixture]
public class StrippingZerosTest
{
    private static readonly (BigDecimal, BigDecimal)[] testCases = new (BigDecimal, BigDecimal)[]
    {
        (new BigDecimal("1.00000"), new BigDecimal("1")),
        (new BigDecimal("1.000"), new BigDecimal("1")),
        (new BigDecimal("1"), new BigDecimal("1")),
        (new BigDecimal("0.1234"), new BigDecimal("0.1234")),
        (new BigDecimal("0.12340"), new BigDecimal("0.1234")),
        (new BigDecimal("0.12340000000"), new BigDecimal("0.1234")),
        (new BigDecimal("1234.5678"), new BigDecimal("1234.5678")),
        (new BigDecimal("1234.56780"), new BigDecimal("1234.5678")),
        (new BigDecimal("1234.567800000"), new BigDecimal("1234.5678")),
        (new BigDecimal("0"), new BigDecimal("0")),
        (new BigDecimal("0e2"), BigDecimal.Zero),
        (new BigDecimal("0e-2"), BigDecimal.Zero),
        (new BigDecimal("0e42"), BigDecimal.Zero),
        (new BigDecimal("+0e42"), BigDecimal.Zero),
        (new BigDecimal("-0e42"), BigDecimal.Zero),
        (new BigDecimal("0e-42"), BigDecimal.Zero),
        (new BigDecimal("+0e-42"), BigDecimal.Zero),
        (new BigDecimal("-0e-42"), BigDecimal.Zero),
        (new BigDecimal("0e-2"), BigDecimal.Zero),
        (new BigDecimal("0e100"), BigDecimal.Zero),
        (new BigDecimal("0e-100"), BigDecimal.Zero),
        (new BigDecimal("10"), new BigDecimal("1e1")),
        (new BigDecimal("20"), new BigDecimal("2e1")),
        (new BigDecimal("100"), new BigDecimal("1e2")),
        (new BigDecimal("1000000000"), new BigDecimal("1e9")),
        (new BigDecimal("100000000e1"), new BigDecimal("1e9")),
        (new BigDecimal("10000000e2"), new BigDecimal("1e9")),
        (new BigDecimal("1000000e3"), new BigDecimal("1e9")),
        (new BigDecimal("100000e4"), new BigDecimal("1e9")),
        (new BigDecimal("1.0000000000000000000000000000"), new BigDecimal("1")),
        (new BigDecimal("-1.0000000000000000000000000000"), new BigDecimal("-1")),
        (new BigDecimal("1.00000000000000000000000000001"), new BigDecimal("1.00000000000000000000000000001")),
        (new BigDecimal("1000000000000000000000000000000e4"), new BigDecimal("1e34")),
    };

    [Test]
    public void TestStrippingZeros()
    {
        foreach (var (input, expected) in testCases)
        {
            Assert.That(expected, Is.EqualTo(input.stripTrailingZeros()), $"For input {input} did not receive expected result {expected}, got {input.stripTrailingZeros()}");

            var negInput = input.negate();
            var negExpected = expected.negate();

            Assert.That(negExpected, Is.EqualTo(negInput.stripTrailingZeros()), $"For input {negInput} did not receive expected result {negExpected}, got {negInput.stripTrailingZeros()}");
        }
    }
}
