using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox3
{
    /// <summary>High accuracy storage of fractional values</summary>
    public struct Fraction
    {
        /// <summary>Zero value</summary>
        public static readonly Fraction Zero = 0;

        /// <summary>Numerator of the fraction</summary>
        public float Numerator;
        /// <summary>Denominator of the fraction</summary>
        public float Denominator;

        /// <summary>Create a new fraction</summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        public Fraction(float numerator, float denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>Create a new fraction from a decimal value</summary>
        /// <param name="value"></param>
        public Fraction(float value)
        {
            Numerator = value;
            Denominator = 1F;
        }

        /// <summary>Implicit fraction float assignment</summary>
        /// <param name="value"></param>
        public static implicit operator Fraction(float value) => new Fraction(value);

        /// <summary>Implicit string literal fraction assignment</summary>
        /// <param name="value"></param>
        public static implicit operator Fraction(string value) => Parse(value);

        /// <summary>Get a Fraction value from a string</summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Fraction Parse(string input)
        {
            float wholePart = 0;
            float denominator = 1;
            string[] inputParts;

            input = input.Trim();
            if (input.Contains(' '))
            {
                inputParts = input.Split(' ');
                float.TryParse(inputParts[0], out wholePart);
                input = inputParts[1];
            }

            inputParts = input.Split('/');
            float.TryParse(inputParts[0], out float numerator);

            if (inputParts.Length > 1)
            {
                // have a fraction
                float.TryParse(inputParts[1], out denominator);
            }
            else
            {
                // not a fraction
                denominator = 1;
            }

            numerator += wholePart * denominator;

            return new Fraction(numerator, denominator);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (Numerator > Denominator)
            {
                int wholePart = (int)Math.Truncate(Numerator / Denominator);
                float numerator = Numerator - wholePart * Denominator;
                if (numerator != 0)
                    return $"{wholePart} {numerator}/{Denominator}";
                else return $"{wholePart}";
            }
            else return $"{Numerator}/{Denominator}";
        }

        /// <summary>Get the approximate value of this fraction (decimal representation)</summary>
        public float ApproximateValue => Numerator / Denominator;

        /// <summary>The integer part of the fraction</summary>
        public int IntegerPart => Convert.ToInt32(Numerator / Denominator);

        /// <summary>The non-integer part of the fraction</summary>
        public Fraction FractionPart => new Fraction(Numerator - IntegerPart * Denominator, Denominator);

        /// <summary>Multiply two fraction values</summary>
        /// <param name="multiplicand"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Fraction operator *(Fraction multiplicand, Fraction multiplier)
        {
            return new Fraction(
                multiplicand.Numerator * multiplier.Numerator,
                multiplicand.Denominator * multiplier.Denominator);
        }

        /// <summary>Multiply a fraction by a decimal value</summary>
        /// <param name="multiplicand"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Fraction operator *(Fraction multiplicand, float multiplier)
        {
            return new Fraction(
                multiplicand.Numerator * multiplier,
                multiplicand.Denominator);
        }

        /// <summary>Divide a fraction by another fraction</summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static Fraction operator /(Fraction dividend, Fraction divisor)
        {
            return new Fraction(
                dividend.Numerator * divisor.Denominator,
                dividend.Denominator * divisor.Numerator);
        }

        /// <summary>Divide a fraction by a decimal value</summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public static Fraction operator /(Fraction dividend, float divisor)
        {
            return new Fraction(
                dividend.Numerator,
                dividend.Denominator * divisor);
        }

        /// <summary>Add two fraction together</summary>
        /// <param name="termA"></param>
        /// <param name="termB"></param>
        /// <returns></returns>
        public static Fraction operator +(Fraction termA, Fraction termB)
        {
            if (termA.Denominator == termB.Denominator)
                return new Fraction(termA.Numerator + termB.Numerator, termA.Denominator);
            else
            {
                float newNumerator = (termA.Numerator * termB.Denominator) + (termA.Denominator * termB.Numerator);
                float newDenominator = termA.Denominator * termB.Denominator;
                return new Fraction(newNumerator, newDenominator);
            }
        }

        /// <summary>Add a decimal value to a fraction</summary>
        /// <param name="termA"></param>
        /// <param name="termB"></param>
        /// <returns></returns>
        public static Fraction operator +(Fraction termA, float termB)
        {
            return new Fraction(termA.Numerator + (termB * termA.Denominator), termA.Denominator);
        }

        /// <summary>Negate a fraction</summary>
        /// <param name="negatee"></param>
        /// <returns></returns>
        public static Fraction operator -(Fraction negatee)
        {
            return new Fraction(-negatee.Numerator, negatee.Denominator);
        }

        /// <summary>Subtract a fraction from another fraction</summary>
        /// <param name="minuend">to be subtracted from</param>
        /// <param name="subtrahend">to be subtracted</param>
        /// <returns></returns>
        public static Fraction operator -(Fraction minuend, Fraction subtrahend)
        {
            return minuend + -subtrahend;
        }

        /// <summary>Subtract a decimal value from a fraction</summary>
        /// <param name="minuend"></param>
        /// <param name="subtrahend"></param>
        /// <returns></returns>
        public static Fraction operator -(Fraction minuend, float subtrahend)
        {
            return minuend + -subtrahend;
        }

        /// <summary>Test whether two fractions' values are equal</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Fraction left, Fraction right)
        {
            left.Simplify();
            right.Simplify();

            return (left.Numerator == right.Numerator && left.Denominator == right.Denominator);
        }

        /// <summary>Test whether two fractions' values are not equal</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Fraction left, Fraction right)
        {
            return !(left == right);
        }

        /// <summary>Determine if a fraction is greater than another</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Fraction left, Fraction right)
        {
            float commonDenominator = FindGCD(left.Denominator, right.Denominator);
            float leftMultiplier = left.Denominator / commonDenominator;
            float rightMultiplier = right.Denominator / commonDenominator;
            return (left.Numerator * leftMultiplier) > (right.Numerator * rightMultiplier);
        }

        /// <summary>Test if a fraction is greater than a float value</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Fraction left, float right)
        {
            return left > new Fraction(right);
        }

        /// <summary>Test if a fraction is less than another</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Fraction left, Fraction right)
        {
            return right > left;
        }

        /// <summary>Test if a fraction is less than a float value</summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Fraction left, float right)
        {
            return new Fraction(right) > left;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Fraction other)
                return this == other;
            else return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>Get the irreducible form of a fraction</summary>
        public Fraction Simplify()
        {
            float divisor = FindGCD(Numerator, Denominator);
            Fraction simplified = new Fraction(Numerator /= divisor, Denominator /= divisor);

            if (simplified.ToString().Length < this.ToString().Length) return simplified;
            else return this;
        }

        /// <summary>Rounds a fraction to the specified precision</summary>
        /// <param name="precision"></param>
        /// <returns></returns>
        public Fraction Round(Fraction precision)
        {
            // FIXME
            return new Fraction((float)Math.Round(ApproximateValue / precision.ApproximateValue) / 8);
        }

        private static float FindGCD(float a, float b)
        {
            if (a == 0) return b;
            if (b == 0) return a;
            return FindGCD(b, a % b);
        }
    }
}
