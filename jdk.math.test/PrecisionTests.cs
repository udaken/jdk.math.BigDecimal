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
 * @summary Test that precision() is computed properly.
 * @author Joseph D. Darcy
 */

using NUnit.Framework;

namespace jdk.math.test;

using static jdk.math.BigDecimal;

public class PrecisionTests
{
	private static BigDecimal NINE = valueOf(9);

	[Test]
	public static void Zero()
	{
		Assert.That(new BigDecimal(0).precision(), Is.EqualTo(1));
	}
	[Test]
	public static void main()
	{

		// Smallest and largest values of a given length
		BigDecimal[] testValues = {
			valueOf(1),
			valueOf(9),
		};


		for (int i = 1; i < 100; i++)
		{
			foreach (BigDecimal bd in testValues)
			{
				Assert.That(bd.precision(), Is.EqualTo(i));
				Assert.That(bd.negate().precision(), Is.EqualTo(i));
			}

			testValues[0] = testValues[0].multiply(TEN);
			testValues[1] = testValues[1].multiply(TEN).add(NINE);
		}
	}

	// The following test tries to cover testings for precision of long values
	[TestCase(2147483648L, 10)] // 2^31:       10 digits
	[TestCase(-2147483648L, 10)] // -2^31:      10 digits
	[TestCase(98893745455L, 11)] // random:     11 digits
	[TestCase(3455436789887L, 13)] // random:     13 digits
	[TestCase(140737488355328L, 15)] // 2^47:       15 digits
	[TestCase(-140737488355328L, 15)] // -2^47:      15 digits
	[TestCase(7564232235739573L, 16)] // random:     16 digits
	[TestCase(25335434990002322L, 17)] // random:     17 digits
	[TestCase(9223372036854775807L, 19)] // 2^63 - 1:   19 digits
	[TestCase(-9223372036854775807L, 19)] // -2^63 + 1:  19 digits

	public static void RandomValues(long randomTestValue, int expectedPrecision)
	{
		BigDecimal bd = new BigDecimal(randomTestValue);
		Assert.That(bd.precision(), Is.EqualTo(expectedPrecision));
	}
}
