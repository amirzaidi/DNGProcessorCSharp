using System;

namespace DNGProcessor
{
    class Fraction
    {
        private long mNumerator, mDenominator;

        public Fraction(long num, long den)
        {
            long gcd = GCD(Math.Abs(num), Math.Abs(den));
            mNumerator = num / gcd;
            mDenominator = den / gcd;
        }

        public override string ToString()
        {
            return mNumerator.ToString() + '/' + mDenominator.ToString();
        }

        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(a.mNumerator * b.mDenominator + b.mNumerator * a.mDenominator, 
                a.mDenominator * b.mDenominator);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            return new Fraction(a.mNumerator * b.mDenominator - b.mNumerator * a.mDenominator,
                a.mDenominator * b.mDenominator);
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.mNumerator * b.mNumerator, a.mDenominator * b.mDenominator);
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new Fraction(a.mNumerator * b.mDenominator, a.mDenominator * b.mNumerator);
        }

        public static implicit operator double(Fraction f)
        {
            return (double)f.mNumerator / f.mDenominator;
        }

        private static long GCD(long a, long b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
