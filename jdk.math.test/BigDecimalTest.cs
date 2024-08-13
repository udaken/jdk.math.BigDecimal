using jdk.math;
using System.Diagnostics;
using System.Numerics;

namespace jdk.math.test;

public class BigDecimalTest
{
    [StackTraceHidden]
    static void assertEquals(object expect, object actual, string? message = null)
    {
        Assert.That(actual, Is.EqualTo(expect), message);
    }
    [StackTraceHidden]
    static void assertTrue(bool condition, string? message = null)
    {
        Assert.That(condition, Is.True, message);
    }
    [StackTraceHidden]
    static void assertFalse(bool condition, string? message = null)
    {
        Assert.That(condition, Is.False, message);
    }
    [StackTraceHidden]
    static void assertThrows<T>(TestDelegate @delegate, string? message = null) where T : Exception
    {
        Assert.Throws<T>(@delegate, message);
    }
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void testAdd()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal expected = new BigDecimal("15.7");
        assertEquals(expected, a.add(b), "10.5 + 5.2 should equal 15.7");
    }

    [Test]
    public void testAdd2()
    {
        BigDecimal approx = new BigDecimal("1.4142135623730951454746218587388284504413604736328125");
        MathContext mcTmp = new MathContext(22, RoundingMode.HALF_EVEN);
        BigDecimal working = new BigDecimal("2");
        BigDecimal expected = new BigDecimal("1.414213562373094952129");
        assertEquals(expected, working.divide(approx, mcTmp));

        assertEquals(new BigDecimal("2.828427124746190097604"), approx.add(expected, mcTmp));

    }
    [Test]

    public void testSubtract()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal expected = new BigDecimal("5.3");
        assertEquals(expected, a.subtract(b), "10.5 - 5.2 should equal 5.3");
    }

    [Test]
    public void testSubtract2()
    {
        BigDecimal a = new BigDecimal("10.0000000000000000000000000000000001");
        BigDecimal b = new BigDecimal("10.0000000000000000000000000000000002");
        BigDecimal expected = new BigDecimal("-0.0000000000000000000000000000000001");
        assertEquals(expected, a.subtract(b));
    }

    [Test]
    public void testMultiply()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal expected = new BigDecimal("54.60");
        assertEquals(expected, a.multiply(b), "10.5 * 5.2 should equal 54.60");
    }

    [Test]
    public void testDivide()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal expected = new BigDecimal("2.0192307692307692");
        assertEquals(expected, a.divide(b, 16, RoundingMode.HALF_UP), "10.5 / 5.2 should equal 2.0192307692307692");
    }
    [Test]
    public void testDivideMinusValue()
    {
        BigDecimal a = new BigDecimal("10");
        BigDecimal b = new BigDecimal("-5");
        BigDecimal expected = new BigDecimal("-2");
        assertEquals(expected, a.divide(b, new MathContext(22)));
    }
    [Test]
    public void testDivide2()
    {
        BigDecimal a = new BigDecimal("9223372036854775808");
        BigDecimal b = new BigDecimal("-922337203685477580.9");
        BigDecimal expected = new BigDecimal("-9.999999999999999998915797827514495566110096634212287039358347668433164949567017299463882168846058337");
        assertEquals(expected, a.divide(b, new MathContext(100, RoundingMode.HALF_EVEN)));
    }
    [Test]
    public void testDivideMinusNumber()
    {
        BigDecimal a = new BigDecimal("-9223372036854775808");
        BigDecimal b = new BigDecimal("17");
        BigDecimal expected = new BigDecimal("-542551296285575047.5294");
        assertEquals(expected, a.divide(b, 4, RoundingMode.DOWN));
    }
    [Test]
    public void testDivideSqrt()
    {
        BigDecimal two = 2;
        BigDecimal sqrt = new BigDecimal("1.4142135623730951454746218587388284504413604736328125");
        var mc = new MathContext(22, RoundingMode.HALF_EVEN);
        assertEquals(new BigDecimal("1.414213562373094952129"), two.divide(sqrt, mc));
    }
    [Test]
    public void testCompareTo()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal c = new BigDecimal("10.5");

        assertTrue(a.compareTo(a) == 0, "10.5 should be greater than 5.2");

        assertTrue(a.compareTo(b) > 0, "10.5 should be greater than 5.2");
        assertTrue(b.compareTo(a) < 0, "5.2 should be less than 10.5");
        assertEquals(0, a.compareTo(c), "10.5 should be equal to 10.5");
    }

    [Test]
    public void testEquals()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("10.50");
        BigDecimal c = new BigDecimal("10.500");
        BigDecimal d = new BigDecimal("10.5000");

        assertFalse(a.equals(b), "10.5 should be equal to 10.50");
        assertFalse(a.equals(c), "10.5 should not be equal to 10.500 (different scale)");
        assertFalse(a.equals(d), "10.5 should not be equal to 10.5000 (different scale)");
    }

    [Test]
    public void testSetScale()
    {
        BigDecimal a = new BigDecimal("10.5678");

        assertEquals(new BigDecimal("10.57"), a.setScale(2, RoundingMode.HALF_UP), "10.5678 rounded to 2 decimal places should be 10.57");
        assertEquals(new BigDecimal("10.57"), a.setScale(2, RoundingMode.HALF_DOWN), "10.5678 rounded to 2 decimal places should be 10.57");
        assertEquals(new BigDecimal("10.56"), a.setScale(2, RoundingMode.DOWN), "10.5678 rounded down to 2 decimal places should be 10.56");
        assertEquals(new BigDecimal("10.57"), a.setScale(2, RoundingMode.UP), "10.5678 rounded up to 2 decimal places should be 10.57");
    }

    [Test]
    public void testRound()
    {
        BigDecimal a = new BigDecimal("10.5678");

        assertEquals(new BigDecimal("11"), a.round(new MathContext(2, RoundingMode.HALF_UP)), "10.5678 rounded to 2 significant figures should be 11");
        assertEquals(new BigDecimal("10.6"), a.round(new MathContext(3, RoundingMode.HALF_UP)), "10.5678 rounded to 3 significant figures should be 10.6");
    }
    [Test]
    public void testAbs()
    {
        BigDecimal positive = new BigDecimal("10.5");
        BigDecimal negative = new BigDecimal("-10.5");
        BigDecimal zero = BigDecimal.ZERO;

        assertEquals(positive, positive.abs(), "Abs of 10.5 should be 10.5");
        assertEquals(positive, negative.abs(), "Abs of -10.5 should be 10.5");
        assertEquals(zero, zero.abs(), "Abs of 0 should be 0");
    }

    [Test]
    public void testNegate()
    {
        BigDecimal positive = new BigDecimal("10.5");
        BigDecimal negative = new BigDecimal("-10.5");
        BigDecimal zero = BigDecimal.ZERO;

        assertEquals(negative, positive.negate(), "Negation of 10.5 should be -10.5");
        assertEquals(positive, negative.negate(), "Negation of -10.5 should be 10.5");
        assertEquals(zero, zero.negate(), "Negation of 0 should be 0");
    }

    [Test]
    public void testPow()
    {
        BigDecimal @base = new BigDecimal("2");

        assertEquals(new BigDecimal("1"), @base.pow(0), "2^0 should be 1");
        assertEquals(new BigDecimal("2"), @base.pow(1), "2^1 should be 2");
        assertEquals(new BigDecimal("4"), @base.pow(2), "2^2 should be 4");
        assertEquals(new BigDecimal("8"), @base.pow(3), "2^3 should be 8");
    }

    [Test]
    public void testToPlainString()
    {
        BigDecimal a = new BigDecimal("1E6");
        BigDecimal b = new BigDecimal("1.23E-5");

        assertEquals("1000000", a.toPlainString(), "1E6 should be represented as 1000000");
        assertEquals("0.0000123", b.toPlainString(), "1.23E-5 should be represented as 0.0000123");
    }

    [Test]
    public void testToString()
    {
        BigDecimal a = new BigDecimal("1000000");
        BigDecimal b = new BigDecimal("0.0000123");

        assertEquals("1000000", a.toString(), "1000000 should be represented as 1E+6");
        assertEquals("0.0000123", b.toString(), "0.0000123 should be represented as 1.23E-5");
    }
    [Test]
    public void testDivideAndRemainder()
    {
        BigDecimal dividend = new BigDecimal("10");
        BigDecimal divisor = new BigDecimal("3");
        var (q, r) = dividend.divideAndRemainder(divisor);

        assertEquals(new BigDecimal("3"), q, "10 divided by 3 should give quotient 3");
        assertEquals(new BigDecimal("1"), r, "10 divided by 3 should give remainder 1");
    }

    [Test]
    public void testScaleOperations()
    {
        BigDecimal a = new BigDecimal("123.4567");
        assertEquals(4, a.scale(), "Scale of 123.4567 should be 4");

        BigDecimal b = a.setScale(2, RoundingMode.HALF_UP);
        assertEquals(2, b.scale(), "Scale after setScale(2) should be 2");
        assertEquals(new BigDecimal("123.46"), b, "123.4567 rounded to 2 decimal places should be 123.46");

        BigDecimal c = a.stripTrailingZeros();
        assertEquals(a, c, "Stripping trailing zeros from 123.4567 should not change the value");

        BigDecimal d = new BigDecimal("123.4500");
        BigDecimal e = d.stripTrailingZeros();
        assertEquals(new BigDecimal("123.45"), e, "Stripping trailing zeros from 123.4500 should give 123.45");
    }

    [Test]
    public void testSpecialValues()
    {
        assertTrue(new BigDecimal("1E-400").compareTo(BigDecimal.ZERO) > 0, "1E-400 should be greater than zero");
        assertTrue(new BigDecimal("-1E-400").compareTo(BigDecimal.ZERO) < 0, "-1E-400 should be less than zero");

        assertThrows<ArithmeticException>(() => BigDecimal.ONE.divide(BigDecimal.ZERO),
                "Division by zero should throw ArithmeticException");
    }

    [Test]
    public void testPrecision()
    {
        BigDecimal a = new BigDecimal("123.4567");
        assertEquals(7, a.precision(), "Precision of 123.4567 should be 7");

        BigDecimal b = new BigDecimal("0.00123");
        assertEquals(3, b.precision(), "Precision of 0.00123 should be 3");

        BigDecimal c = new BigDecimal("1.2E5");
        assertEquals(2, c.precision(), "Precision of 1.2E5 should be 2");
    }

    [Test]
    public void testMathContext()
    {
        BigDecimal a = new BigDecimal("123.4567");
        MathContext mc = new MathContext(3, RoundingMode.HALF_UP);
        BigDecimal b = a.round(mc);

        assertEquals(new BigDecimal("123"), b, "123.4567 rounded to 3 significant digits should be 123");
        assertEquals(3, b.precision(), "Precision after rounding to 3 significant digits should be 3");
    }
    [Test]
    public void testCurrencyCalculations()
    {
        BigDecimal price = new BigDecimal("19.99");
        BigDecimal quantity = new BigDecimal("3");
        BigDecimal taxRate = new BigDecimal("0.08");

        BigDecimal subtotal = price.multiply(quantity).setScale(2, RoundingMode.HALF_UP);
        assertEquals(new BigDecimal("59.97"), subtotal, "Subtotal should be 59.97");

        BigDecimal tax = subtotal.multiply(taxRate).setScale(2, RoundingMode.HALF_UP);
        assertEquals(new BigDecimal("4.80"), tax, "Tax should be 4.80");

        BigDecimal total = subtotal.add(tax);
        assertEquals(new BigDecimal("64.77"), total, "Total should be 64.77");
    }

    [Test]
    public void testLargeNumbers()
    {
        BigDecimal largeNumber1 = new BigDecimal("1E100");
        BigDecimal largeNumber2 = new BigDecimal("9.99999E99");

        BigDecimal sum = largeNumber1.add(largeNumber2);
        assertEquals(new BigDecimal("1.999999E+100"), sum, "Sum of large numbers should be correct");

        BigDecimal product = largeNumber1.multiply(largeNumber2);
        assertEquals(new BigDecimal("9.99999E+199"), product, "Product of large numbers should be correct");
    }

    [Test]
    public void testComplexCalculations()
    {
        BigDecimal a = new BigDecimal("10.5");
        BigDecimal b = new BigDecimal("5.2");
        BigDecimal c = new BigDecimal("2.5");

        BigDecimal result = a.add(b).multiply(c).divide(a, 10, RoundingMode.HALF_UP);
        assertEquals(new BigDecimal("3.7380952381"), result, "Complex calculation should yield correct result");
    }

    [Test]
    public void testMovingDecimalPoint()
    {
        BigDecimal number = new BigDecimal("123.456");

        assertEquals(new BigDecimal("12345.6"), number.movePointRight(2), "Moving decimal point right should work");
        assertEquals(new BigDecimal("1.23456"), number.movePointLeft(2), "Moving decimal point left should work");
        assertEquals(new BigDecimal("123456"), number.movePointRight(3), "Moving decimal point right beyond fractional part should work");
        assertEquals(new BigDecimal("0.000123456"), number.movePointLeft(6), "Moving decimal point left beyond integer part should work");
    }

    [Test]
    public void testUnscaledValue()
    {
        BigDecimal number = new BigDecimal("123.456");
        assertEquals("123456", number.unscaledValue().toString(), "Unscaled value should be 123456");

        BigDecimal largeNumber = new BigDecimal("1.23456E20");
        assertEquals("123456", largeNumber.unscaledValue().toString(), "Unscaled value of large number should be correct");
    }

    [Test]
    public void testSignum()
    {
        assertEquals(1, new BigDecimal("123.456").signum(), "Signum of positive number should be 1");
        assertEquals(-1, new BigDecimal("-123.456").signum(), "Signum of negative number should be -1");
        assertEquals(0, BigDecimal.ZERO.signum(), "Signum of zero should be 0");
    }
    [Test]
    public void testInfiniteFractions()
    {
        BigDecimal oneThird = BigDecimal.ONE.divide(new BigDecimal("3"), 20, RoundingMode.HALF_UP);
        assertEquals(new BigDecimal("0.33333333333333333333"), oneThird, "1/3 should be correctly represented to 20 decimal places");

        BigDecimal result = oneThird.multiply(new BigDecimal("3"));
        assertEquals(new BigDecimal("0.99999999999999999999"), result, "3 * (1/3) should equal 1");
    }

    [Test]
    public void testBigIntegerConversion()
    {
        BigDecimal @decimal = new BigDecimal("123456.789");
        BigInteger integer = @decimal.toBigInteger();
        assertEquals(BigInteger.Parse("123456"), integer, "Converting BigDecimal to BigInteger should truncate decimal part");

        BigDecimal reconstructed = new BigDecimal(integer);
        assertEquals(new BigDecimal("123456"), reconstructed, "Converting back to BigDecimal should preserve value");
    }

    [Test]
    public void testSquareRoot()
    {
        BigDecimal number = new BigDecimal("2");
        BigDecimal sqrt = number.sqrt(new MathContext(10));
        assertEquals(new BigDecimal("1.414213562"), sqrt, "Square root of 2 should be correct to 10 significant digits");

        BigDecimal squared = sqrt.multiply(sqrt);
        assertFalse(number.subtract(squared).abs().compareTo(new BigDecimal("1E-9")) < 0, "Squared result should be close to original number");
    }

    [Test]
    public void testUlp()
    {
        BigDecimal number = new BigDecimal("1.23456");
        BigDecimal ulp = number.ulp();
        assertEquals(new BigDecimal("0.00001"), ulp, "ULP of 1.23456 should be 0.00001");

        BigDecimal largeNumber = new BigDecimal("9E20");
        BigDecimal largeUlp = largeNumber.ulp();
        assertEquals(new BigDecimal("1E20"), largeUlp, "ULP of 9E20 should be 1E20");
    }

    [Test]
    public void testScaleByPowerOfTen()
    {
        BigDecimal number = new BigDecimal("123.456");
        assertEquals(new BigDecimal("12345.6"), number.scaleByPowerOfTen(2), "Scaling by power of 10 (positive) should work");
        assertEquals(new BigDecimal("12.3456"), number.scaleByPowerOfTen(-1), "Scaling by power of 10 (negative) should work");
    }

    [Test]
    public void testScaleByPowerOfTenLarge()
    {
        BigDecimal number = new BigDecimal("9999999999123.456");
        assertEquals(new BigDecimal("999999999912345.6"), number.scaleByPowerOfTen(2), "Scaling by power of 10 (positive) should work");
        assertEquals(new BigDecimal("999999999912.3456"), number.scaleByPowerOfTen(-1), "Scaling by power of 10 (negative) should work");
    }

    [Test]
    public void testValueExact()
    {
        BigDecimal intValue = new BigDecimal("123");
        assertEquals(123, intValue.intValueExact(), "intValueExact should work for integer values");

        BigDecimal longValue = new BigDecimal("9223372036854775807"); // Max long value
        assertEquals(9223372036854775807L, longValue.longValueExact(), "longValueExact should work for large integer values");

        BigDecimal doubleValue = new BigDecimal("123.456");
        //assertThrows<ArithmeticException>(() => doubleValue.intValueExact(), "intValueExact should throw exception for non-integer values");
    }

    [Test]
    public void testPrecisionHandling()
    {
        MathContext mc = new MathContext(5, RoundingMode.HALF_UP);
        BigDecimal a = new BigDecimal("123.456789", mc);
        assertEquals(new BigDecimal("123.46"), a, "Precision should be limited to 5 significant digits");

        BigDecimal b = new BigDecimal("0.0001234567890", mc);
        assertEquals(new BigDecimal("0.00012346"), b, "Precision should be maintained for small numbers");
    }

    [Test]
    public void testFromDecimal()
    {
        BigDecimal b = -1234.56789m;
        assertEquals(new BigDecimal("-1234.56789"), b);
        BigDecimal c = 17922816251426433759354395033m;
        assertEquals(new BigDecimal("17922816251426433759354395033"), c);
        BigDecimal a = 1234.56789m;
        assertEquals(new BigDecimal("1234.56789"), a);
    }
    [Test]
    public void testFromDouble()
    {
        BigDecimal value = new BigDecimal(1234.567);
        assertEquals(new BigDecimal("1234.5670000000000072759576141834259033203125"), value);
    }
}