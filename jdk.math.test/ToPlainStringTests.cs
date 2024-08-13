/*
 * Copyright (c) 2004, 2023, Oracle and/or its affiliates. All rights reserved.
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
 * @bug 4984872 8318915
 * @summary Basic tests of toPlainString method
 * @run main ToPlainStringTests
 * @run main/othervm -XX:+IgnoreUnrecognizedVMOptions -XX:+EliminateAutoBox -XX:AutoBoxCacheMax=20000 ToPlainStringTests
 * @author Joseph D. Darcy
 */
using System;
using NUnit.Framework;
using System.Numerics;

namespace jdk.math.test;

[TestFixture]
public class ToPlainStringTests
{
    [Test]
    public void TestToPlainString()
    {
        string[][] testCases = [
            ["0",                       "0"],
            ["1",                       "1"],
            ["10",                      "10"],
            ["2e1",                     "20"],
            ["3e2",                     "300"],
            ["4e3",                     "4000"],
            ["5e4",                     "50000"],
            ["6e5",                     "600000"],
            ["7e6",                     "7000000"],
            ["8e7",                     "80000000"],
            ["9e8",                     "900000000"],
            ["1e9",                     "1000000000"],
            
            [".0",                      "0.0"],
            [".1",                      "0.1"],
            [".10",                     "0.10"],
            ["1e-1",                    "0.1"],
            ["1e-1",                    "0.1"],
            ["2e-2",                    "0.02"],
            ["3e-3",                    "0.003"],
            ["4e-4",                    "0.0004"],
            ["5e-5",                    "0.00005"],
            ["6e-6",                    "0.000006"],
            ["7e-7",                    "0.0000007"],
            ["8e-8",                    "0.00000008"],
            ["9e-9",                    "0.000000009"],
            ["9000e-12",                "0.000000009000"],
            
            ["9000e-22",                 "0.0000000000000000009000"],
            ["12345678901234567890",     "12345678901234567890"],
            ["12345678901234567890e22",  "123456789012345678900000000000000000000000"],
            ["12345678901234567890e-22", "0.0012345678901234567890"],

            ["12345e-1",                 "1234.5"],
            ["12345e-2",                 "123.45"],
            ["12345e-3",                 "12.345"],
            ["12345e-4",                 "1.2345"],
        ];

        foreach (string[] testCase in testCases)
        {
            BigDecimal bd = new BigDecimal(testCase[0]);

            Assert.AreEqual(bd.toPlainString(), testCase[1]);

            bd = new BigDecimal("-" + testCase[0]);
            Assert.True(bd.signum() == 0 || bd.toPlainString().Equals("-" + testCase[1], StringComparison.Ordinal));
        }
    }

    [Test]
    public void TestFailingCases()
    {
        string[] failingCases = [
            "1E-" + (int.MaxValue - 0),            // MAX_VALUE + 2 chars
            "1E-" + (int.MaxValue - 1),            // MAX_VALUE + 1 chars

            "-1E-" + (int.MaxValue - 0),           // MAX_VALUE + 3 chars
            "-1E-" + (int.MaxValue - 1),           // MAX_VALUE + 2 chars
            "-1E-" + (int.MaxValue - 2),           // MAX_VALUE + 1 chars

            "123456789E-" + (int.MaxValue - 0),    // MAX_VALUE + 2 chars
            "123456789E-" + (int.MaxValue - 1),    // MAX_VALUE + 1 chars

            "-123456789E-" + (int.MaxValue - 0),   // MAX_VALUE + 3 chars
            "-123456789E-" + (int.MaxValue - 1),   // MAX_VALUE + 2 chars
            "-123456789E-" + (int.MaxValue - 2),   // MAX_VALUE + 1 chars

            "1E" + int.MaxValue,                   // MAX_VALUE + 1 chars
            "123456789E" + int.MaxValue,           // MAX_VALUE + 9 chars

            "-1E" + int.MaxValue,                  // MAX_VALUE + 2 chars
            "-123456789E" + int.MaxValue,          // MAX_VALUE + 10 chars
        ];

        foreach (string failingCase in failingCases)
        {
            Assert.Catch<OutOfMemoryException>(() => new BigDecimal(failingCase).toPlainString());
        }

    }
}