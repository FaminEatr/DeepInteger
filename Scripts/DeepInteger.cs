using System;
using System.Text;

namespace BottomlessIntegerNSA
{
    // Class nests itself to make large numbers without using strings
    // First number is always 1's place, with each nextPlaceNum being the next highest number
    public class DeepInteger : IDisposable
    {
        public DeepInteger()
        {
            placeNum = 0;
            isNegative = false;
        }

        public DeepInteger(string val)
        {
            DeepInteger lastVal = new DeepInteger(-1);
            foreach (var _char in val)
            {
                DeepInteger cv = new DeepInteger((int)char.GetNumericValue(_char));

                if (lastVal > -1)
                {
                    cv.nextPlaceNum = lastVal;
                }

                lastVal = cv;
            }
            placeNum = lastVal.placeNum;
            nextPlaceNum = lastVal.nextPlaceNum;
        }

        public DeepInteger(DeepInteger val)
        {
            placeNum = val.placeNum;
            isNegative = val.isNegative;

            if (!(val.nextPlaceNum is null))
            {
                nextPlaceNum = new DeepInteger(val.nextPlaceNum);
            }
        }

        public DeepInteger(int placeNumVal)
        {
            int initial = Math.Abs(placeNumVal);
            int final;

            if (initial > 9)
            {
                final = initial % 10;

                initial -= final;
                initial /= 10;

                nextPlaceNum = new DeepInteger(initial);
            }
            else
            {
                final = initial;
            }

            placeNum = (ShallowInteger)final;

            if (placeNumVal < 0)
            {
                isNegative = true;
            }
        }

        public DeepInteger(uint placeNumVal)
        {
            uint initial = placeNumVal;
            uint final;

            if (initial > 9)
            {
                final = initial % 10;

                initial -= final;
                initial /= 10;

                nextPlaceNum = new DeepInteger(initial);
            }
            else
            {
                final = placeNumVal;
            }

            placeNum = (ShallowInteger)((int)final);
        }

        public DeepInteger(long placeNumVal)
        {
            long initial = Math.Abs(placeNumVal);
            long final;

            if (initial > 9)
            {
                final = initial % 10;

                initial -= final;
                initial /= 10;

                nextPlaceNum = new DeepInteger(initial);
            }
            else
            {
                final = placeNumVal;
            }

            placeNum = (ShallowInteger)((int)final);
        }

        public DeepInteger(ulong placeNumVal)
        {
            ulong initial = placeNumVal;
            ulong final;

            if (initial > 9)
            {
                final = initial % 10;

                initial -= final;
                initial /= 10;

                nextPlaceNum = new DeepInteger(initial);
            }
            else
            {
                final = placeNumVal;
            }

            placeNum = (ShallowInteger)((int)final);
        }

        private DeepInteger(ZeroesHolder zeroes, DeepInteger numbers)
        {
            bool addZero = !(zeroes is null) && zeroes.Zero > -1;

            if (addZero)
            {
                nextPlaceNum = new DeepInteger(zeroes.NextPlaceZero, numbers);
                placeNum = 0;
            }
            else
            {
                placeNum = numbers.placeNum;
                isNegative = numbers.isNegative;

                if (!(numbers.nextPlaceNum is null))
                {
                    nextPlaceNum = new DeepInteger(numbers.nextPlaceNum);
                }
            }
        }

        enum ShallowInteger
        {
            _0 = 0,
            _1 = 1,
            _2 = 2,
            _3 = 3,
            _4 = 4,
            _5 = 5,
            _6 = 6,
            _7 = 7,
            _8 = 8,
            _9 = 9
        }

        // The 0-9 value of this place
        ShallowInteger placeNum;
        public int PlaceNum
        {
            get
            {
                return (int)placeNum;
            }
        }

        // The next highest place (i.e. this is ones, nextPlaceNum is tens)
        DeepInteger nextPlaceNum = null;

        public bool isNegative;

        public static DeepInteger Zero
        {
            get
            {
                return new DeepInteger(0);
            }
        }

        private void RecursiveAddLength(ref DeepInteger l)
        {
            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.RecursiveAddLength(ref l);
            }
            l.Increment();
        }

        public DeepInteger Length()
        {
            DeepInteger l = 1;
            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.RecursiveAddLength(ref l);
            }

            return l;
        }

        public bool AbsoluteGreaterThan(DeepInteger b, out bool nextIsEqual)
        {
            bool greater = false;
            bool equalsCheck = false;

            nextIsEqual = false;

            if (!(b is null))
            {
                if (!(nextPlaceNum is null))
                {
                    if (!(b.nextPlaceNum is null))
                    {
                        greater = nextPlaceNum.AbsoluteGreaterThan(b.nextPlaceNum, out equalsCheck);
                    }
                    else
                    {
                        greater = true;
                    }
                }
                else
                {
                    if (b.nextPlaceNum is null)
                    {
                        if (placeNum > b.placeNum)
                        {
                            greater = true;
                        }
                        else if (placeNum == b.placeNum)
                        {
                            nextIsEqual = true;
                        }
                    }
                }
            }
            else
            {
                greater = true;
            }

            if (equalsCheck)
            {
                if (placeNum > b.placeNum)
                {
                    greater = true;
                }
                else if (placeNum == b.placeNum)
                {
                    nextIsEqual = true;
                }
            }

            return greater;
        }

        public DeepInteger Increment()
        {
            if (isNegative)
            {
                int l = ((int)placeNum) - 1;

                if (l < 0)
                {
                    if (!(nextPlaceNum is null))
                    {
                        l = 9;

                        nextPlaceNum.Decrement();
                    }
                    else
                    {
                        l = 1;
                        isNegative = false;
                    }
                }
                else if (l == 0 && nextPlaceNum is null)
                {
                    isNegative = false;
                }

                placeNum = (ShallowInteger)l;
            }
            else
            {
                int l = ((int)placeNum) + 1;

                if (l >= 10)
                {
                    l = l % 10;

                    if (!(nextPlaceNum is null))
                    {
                        nextPlaceNum.Increment();
                    }
                    else
                    {
                        nextPlaceNum = 1;
                    }
                }

                placeNum = (ShallowInteger)l;
            }

            return this;
        }

        public DeepInteger Decrement()
        {
            if (isNegative)
            {
                int l = ((int)placeNum) + 1;

                if (l >= 10)
                {
                    l = l % 10;

                    if (!(nextPlaceNum is null))
                    {
                        nextPlaceNum.Increment();
                    }
                    else
                    {
                        nextPlaceNum = 1;
                    }
                }

                placeNum = (ShallowInteger)l;
            }
            else
            {
                int l = ((int)placeNum) - 1;

                if (l < 0)
                {
                    if (!(nextPlaceNum is null))
                    {
                        l = 9;

                        nextPlaceNum.Decrement();
                    }
                    else
                    {
                        l = 1;
                        isNegative = true;
                    }
                }

                placeNum = (ShallowInteger)l;
            }

            return this;
        }

        // Changing this by b
        // This will always be the largest number, and b will always be the smaller one
        // This will only ever be used to add positive numbers (i.e. A + -B will be routed to A.Subtract(B))
        private DeepInteger _add(DeepInteger b, int remainder = 0)
        {
            DeepInteger result = new DeepInteger(this);

            int a = ((int)result.placeNum) + remainder;
            remainder = 0;

            if (!(b is null))
            {
                a += (int)b.placeNum;
            }

            if (a >= 10)
            {
                a = a % 10;
                remainder = 1;
            }

            if (!(result.nextPlaceNum is null))
            {
                if (!(b is null) &&
                    !(b.nextPlaceNum is null))
                {
                    result.nextPlaceNum = result.nextPlaceNum._add(b.nextPlaceNum, remainder);
                }
                else if (remainder > 0)
                {
                    result.nextPlaceNum = result.nextPlaceNum._add(null, remainder);
                }
            }
            else if (remainder > 0)
            {
                result.nextPlaceNum = remainder;
            }

            result.placeNum = (ShallowInteger)a;

            return result;
        }

        // Changing this by b
        // This will always be the largest number, b will always be the smallest
        // This will only ever be used to subtract positive numbers (i.e. A - -B will be routed to A.Add(B))
        private DeepInteger _subtract(DeepInteger b, int remainder = 0)
        {
            DeepInteger result = new DeepInteger(this);

            int s = (int)result.placeNum;

            // If remainder is a number (1) and placeNum is 0
            if (remainder > ((int)result.placeNum))
            {
                s += 10 - remainder;
                remainder = 1;
            }
            else
            {
                s -= remainder;
                remainder = 0;
            }

            if (!(b is null))
            {
                if (s < ((int)b.placeNum))
                {
                    s += 10;
                    remainder = 1;
                }

                s -= (int)b.placeNum;
            }

            if (!(result.nextPlaceNum is null))
            {
                if (!(b is null) &&
                    !(b.nextPlaceNum is null))
                {
                    result.nextPlaceNum = result.nextPlaceNum._subtract(b.nextPlaceNum, remainder);

                    if ((result.nextPlaceNum is null) && s.Equals(0))
                    {
                        return null;
                    }
                }
                else if (remainder > 0)
                {
                    result.nextPlaceNum = result.nextPlaceNum._subtract(null, remainder);
                }
            }
            else
            {
                if (nextPlaceNum is null && s.Equals(0))
                {
                    return null;
                }
            }

            result.placeNum = (ShallowInteger)s;

            return result;
        }

        private DeepInteger _multiply(DeepInteger b, int remainder = 0)
        {
            DeepInteger finalResult = new DeepInteger();

            if (!(b is null))
            {
                DeepInteger otherNext = b;
                bool advanceOther = true;
                ZeroesHolder holder = new ZeroesHolder();

                while (advanceOther)
                {
                    // First make the multiplication
                    DeepInteger result = new DeepInteger(this);
                    result.MultiplyDepthBy((int)otherNext.placeNum);

                    // Add any zeroes for place
                    result = new DeepInteger(holder, result);

                    // Add to the final result
                    finalResult = Add(result, finalResult);

                    // Step to next
                    if (!(otherNext.nextPlaceNum is null))
                    {
                        otherNext = otherNext.nextPlaceNum;
                        holder.AddZero();
                    }
                    else
                    {
                        advanceOther = false;
                    }
                }
            }

            return finalResult;
        }

        private void MultiplyDepthBy(int b, int remainder = 0)
        {
            int m = ((int)placeNum) * b;
            m += remainder;

            remainder = 0;
            placeNum = (ShallowInteger)(m % 10);

            if (m > 9)
            {
                remainder = m - ((int)placeNum);
                remainder /= 10;
            }

            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.MultiplyDepthBy(b, remainder);
            }
            else if (remainder > 0)
            {
                nextPlaceNum = remainder;
            }
        }

        private void AppendRight(DeepInteger place)
        {
            if (placeNum != 0 || !(nextPlaceNum is null))
            {
                nextPlaceNum = new DeepInteger(this);
            }

            placeNum = place.placeNum;
        }

        private void AppendLeft(DeepInteger place)
        {
            DeepInteger next = nextPlaceNum;

            while (!(next is null))
            {
                next = next.nextPlaceNum;
            }

            next.nextPlaceNum = new DeepInteger(place);
        }

        private DeepInteger GetFurthestLeft(DeepInteger depth)
        {
            int result = ((int)placeNum);
            DeepInteger next = nextPlaceNum;

            DeepInteger targetDepth = Length() - depth;

            DeepInteger currDepth = new DeepInteger(1);

            while (currDepth < targetDepth)
            {
                result = (int)next.placeNum;
                next = next.nextPlaceNum;
                currDepth++;
            }

            return new DeepInteger(result);
        }

        private DeepInteger _divide(DeepInteger b, out DeepInteger remainder)
        {
            DeepInteger result = new DeepInteger(0);

            DeepInteger iterNum = new DeepInteger(0);

            DeepInteger currPiece;

            DeepInteger current = GetFurthestLeft(iterNum);

            DeepInteger littleRem = null;

            if (!(current is null))
            {
                currPiece = new DeepInteger(current);
                iterNum.Increment();

                DeepInteger thisLength = this.Length();
                bool next = thisLength >= iterNum;

                while (next)
                {
                    while (b > currPiece &&
                           (thisLength > iterNum))
                    {
                        current = GetFurthestLeft(iterNum);
                        if (!(current is null))
                        {
                            currPiece.AppendRight(current);
                            iterNum.Increment();
                        }
                        else
                        {
                            break;
                        }

                        if (b > currPiece &&
                           (iterNum >= b.Length()))
                        {
                            result.AppendRight(new DeepInteger(0));
                        }
                    }

                    DeepInteger placeNum = currPiece.SuperSlowDivide(b, out littleRem);

                    if (!(placeNum is null) &&
                        !(placeNum.Equals(0)))
                    {
                        result.AppendRight(placeNum);
                        currPiece = new DeepInteger(littleRem);
                    }

                    next = thisLength > iterNum;
                }
            }

            if (!(littleRem is null))
            {
                remainder = new DeepInteger(littleRem);
            }
            else
            {
                remainder = new DeepInteger(0);
            }

            return result;
        }

        private DeepInteger SuperSlowDivide(DeepInteger b, out DeepInteger remainder)
        {
            remainder = new DeepInteger(0);

            DeepInteger result = new DeepInteger(this);
            DeepInteger quotient = new DeepInteger(0);

            bool next = result >= b;

            while (next)
            {
                result = result._subtract(b);
                quotient.Increment();

                next = !(result is null) && (result >= b);
            }

            if (result is null)
            {
                result = new DeepInteger(0);
            }

            remainder = result;

            return quotient;
        }

        private int _modulus(DeepInteger b)
        {
            DeepInteger i = new DeepInteger(this);

            bool next = i >= b;

            while (next)
            {
                i = i._subtract(b);

                if (i is null)
                {
                    next = false;
                }
                else
                {
                    next = i >= b;
                }
            }

            if (i is null)
            {
                return 0;
            }
            else
            {
                return i.PlaceNum;
            }
        }

        public static DeepInteger operator +(DeepInteger a, DeepInteger b) => Add(a, b);

        public static DeepInteger operator ++(DeepInteger a) => a.Increment();

        public static DeepInteger operator --(DeepInteger a) => a.Decrement();

        public static DeepInteger operator -(DeepInteger a, DeepInteger b) => Subtract(a, b);

        public static DeepInteger operator *(DeepInteger a, DeepInteger b) => Multiply(a, b);

        public static DeepInteger operator /(DeepInteger a, DeepInteger b) => Divide(a, b);

        public static DeepInteger operator %(DeepInteger a, DeepInteger b) => Modulus(a, b);


        public static bool operator >(DeepInteger a, DeepInteger b) => GreaterThan(a, b);

        public static bool operator <(DeepInteger a, DeepInteger b) => LessThan(a, b);

        public static bool operator >=(DeepInteger a, DeepInteger b) => GreaterThanEqualTo(a, b);

        public static bool operator <=(DeepInteger a, DeepInteger b) => LessThanEqualTo(a, b);

        public static bool operator ==(DeepInteger a, DeepInteger b) => a.Equals(b);

        public static bool operator !=(DeepInteger a, DeepInteger b) => !a.Equals(b);

        public static DeepInteger Add(DeepInteger a, DeepInteger b)
        {
            DeepInteger result;

            // a + b
            if (!a.isNegative && !b.isNegative)
            {
                // a + B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._add(a);
                }
                // A + b
                else
                {
                    result = a._add(b);
                }
            }
            // -a + -b
            else if (a.isNegative && b.isNegative)
            {
                // -a + -B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._add(a);
                    result.isNegative = true;
                }
                // -A + -b
                else
                {
                    result = a._add(b);
                }
            }
            // -a + b
            else if (a.isNegative)
            {
                // -a + B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._subtract(a);
                }
                // -A + b
                else
                {
                    result = a._subtract(b);
                }
            }
            // a + -b
            else/* if (b.isNegative)*/
            {
                // a + -B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._subtract(a);
                    result.isNegative = true;
                }
                // A + -b
                else
                {
                    result = a._subtract(b);
                }
            }

            if (result is null)
            {
                result = new DeepInteger(0);
            }

            return result;
        }

        public static DeepInteger Subtract(DeepInteger a, DeepInteger b)
        {
            DeepInteger result;

            // a - b
            if (!a.isNegative && !b.isNegative)
            {
                // a - B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._subtract(a);
                    result.isNegative = true;
                }
                // A - b
                else
                {
                    result = a._subtract(b);
                }
            }
            // -a - -b
            else if (a.isNegative && b.isNegative)
            {
                // -a - -B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._subtract(a);
                    b.isNegative = false;
                }
                // -A - -b
                else
                {
                    result = a._subtract(b);
                }
            }
            // -a - b
            else if (a.isNegative)
            {
                // -a - B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._add(a);
                }
                // -A - b
                else
                {
                    result = a._add(b);
                }
            }
            // a - -b
            else
            {
                // a - -B
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._add(a);
                    result.isNegative = false;
                }
                // A - -b
                else
                {
                    result = a._add(b);
                    result.isNegative = false;
                }
            }

            if (result is null)
            {
                result = new DeepInteger(0);
            }

            return result;
        }

        public static DeepInteger Multiply(DeepInteger a, DeepInteger b)
        {
            DeepInteger result;

            if (a.Equals(0) || b.Equals(0))
            {
                result = new DeepInteger(0);
            }
            else
            {
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = b._multiply(a);

                    if (a.isNegative && !b.isNegative)
                    {
                        result.isNegative = true;
                    }
                    else if (a.isNegative && b.isNegative)
                    {
                        result.isNegative = false;
                    }
                }
                else
                {
                    result = a._multiply(b);

                    if (!a.isNegative && b.isNegative)
                    {
                        result.isNegative = true;
                    }
                    else if (a.isNegative && b.isNegative)
                    {
                        result.isNegative = false;
                    }
                }
            }

            return result;
        }

        public static DeepInteger Divide(DeepInteger a, DeepInteger b)
        {
            if (b.Equals(new DeepInteger(0)))
            {
                throw new DivideByZeroException();
            }

            DeepInteger result;

            if (!a.isNegative && !b.isNegative)
            {
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = 0;
                }
                else
                {
                    result = a._divide(b, out DeepInteger remainder);
                }
            }
            else if (a.isNegative && b.isNegative)
            {
                result = a._multiply(b);
                result.isNegative = false;
            }
            else
            {
                if (a.AbsoluteGreaterThan(b, out bool nIE))
                {
                    result = a._multiply(b);
                    result.isNegative = true;
                }
                else
                {
                    result = b._multiply(a);
                    result.isNegative = true;
                }
            }

            return result;
        }


        public static int Modulus(DeepInteger a, DeepInteger b)
        {
            if (b.Equals(new DeepInteger(0)))
            {
                throw new DivideByZeroException();
            }

            int result;

            if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
            {
                result = 0;
            }
            else
            {
                result = a._modulus(b);
            }

            return result;
        }

        internal static bool GreaterThan(DeepInteger a, DeepInteger b)
        {
            if (a.isNegative && !b.isNegative)
            {
                return false;
            }

            if (!a.isNegative && b.isNegative)
            {
                return true;
            }

            if (a.Equals(b))
            {
                return false;
            }

            bool absoluteGreaterThan = a.AbsoluteGreaterThan(b, out bool nIE);

            if (a.isNegative && b.isNegative)
            {
                return !absoluteGreaterThan;
            }

            return absoluteGreaterThan;
        }

        internal static bool LessThan(DeepInteger a, DeepInteger b)
        {
            if (a.isNegative && !b.isNegative)
            {
                return true;
            }

            if (!a.isNegative && b.isNegative)
            {
                return false;
            }

            if (a.Equals(b))
            {
                return false;
            }

            bool absoluteGreaterThan = a.AbsoluteGreaterThan(b, out bool nIE);

            if (a.isNegative && b.isNegative)
            {
                return absoluteGreaterThan;
            }

            return !absoluteGreaterThan;
        }

        internal static bool GreaterThanEqualTo(DeepInteger a, DeepInteger b)
        {
            if (a.Equals(b))
            {
                return true;
            }

            if (GreaterThan(a, b))
            {
                return true;
            }

            return false;
        }

        internal static bool LessThanEqualTo(DeepInteger a, DeepInteger b)
        {
            if (a.Equals(b))
            {
                return true;
            }

            if (LessThan(a, b))
            {
                return true;
            }

            return false;
        }

        internal static DeepInteger DivideRem(DeepInteger a, DeepInteger b, out DeepInteger remainder)
        {
            if (b.Equals(new DeepInteger(0)))
            {
                throw new DivideByZeroException();
            }

            DeepInteger result;
            remainder = new DeepInteger(0);

            if (!a.isNegative && !b.isNegative)
            {
                if (b.AbsoluteGreaterThan(a, out bool nextIsEqual))
                {
                    result = 0;
                }
                else
                {
                    result = a._divide(b, out remainder);
                }
            }
            else if (a.isNegative && b.isNegative)
            {
                result = a._multiply(b);
                result.isNegative = false;
            }
            else
            {
                if (a.AbsoluteGreaterThan(b, out bool nIE))
                {
                    result = a._multiply(b);
                    result.isNegative = true;
                }
                else
                {
                    result = b._multiply(a);
                    result.isNegative = true;
                }
            }

            return result;
        }

        public void RecursiveToString(ref StringBuilder sb)
        {
            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.RecursiveToString(ref sb);
            }
            sb.Append(((int)placeNum).ToString());
        }

        // Compiles the whole number into a string, then returns it
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.RecursiveToString(ref sb);
            }
            sb.Append(((int)placeNum).ToString());

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            DeepInteger other = null;

            if (obj is null)
            {
                return false;
            }

            if (obj is DeepInteger)
            {
                other = obj as DeepInteger;
            }
            else if (obj is int || obj is double || obj is float)
            {
                other = new DeepInteger((int)obj);
            }
            else if (obj is uint)
            {
                other = new DeepInteger((uint)obj);
            }
            else if (obj is long)
            {
                other = new DeepInteger((long)obj);
            }
            else if (obj is ulong)
            {
                other = new DeepInteger((ulong)obj);
            }

            if ((other is null))
            {
                return false;
            }
            else
            {
                // If these numbers are equal
                if (placeNum.Equals(other.placeNum))
                {
                    // If neither number has a next place
                    if (nextPlaceNum is null &&
                        other.nextPlaceNum is null)
                    {
                        return true;
                    }
                    // If both numbers have a next place
                    else if (!(other.nextPlaceNum is null) &&
                             !(nextPlaceNum is null))
                    {
                        return nextPlaceNum.Equals(other.nextPlaceNum);
                    }
                    // If only one has a next place
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            int nextPlaceHash = 0;

            if (!(nextPlaceNum is null))
            {
                nextPlaceHash = nextPlaceNum.GetHashCode();
            }

            return HashCode.Combine(isNegative, placeNum, nextPlaceHash);
        }

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (!(nextPlaceNum is null))
                {
                    nextPlaceNum.Dispose();
                    nextPlaceNum = null;
                }

                placeNum = 0;
                isNegative = true;
            }

            _disposed = true;
        }

        ~DeepInteger()
        {
            if (!(nextPlaceNum is null))
            {
                nextPlaceNum.Dispose();
                nextPlaceNum = null;
            }

            placeNum = 0;
            isNegative = true;
        }

        public static implicit operator DeepInteger(int v)
        {
            return new DeepInteger(v);
        }

        public static implicit operator DeepInteger(uint v)
        {
            return new DeepInteger(v);
        }

        static DeepInteger maxIntVal = new DeepInteger(int.MaxValue);
        static DeepInteger minIntVal = new DeepInteger(int.MinValue + 1);

        public static explicit operator int(DeepInteger v)
        {
            if (v > maxIntVal)
            {
                return int.MaxValue;
            }
            else if (v < minIntVal)
            {
                return int.MinValue;
            }
            else
            {
                int val = v.PlaceNum;
                int multi = 10;
                int multiVal = 10;
                DeepInteger next = v.nextPlaceNum;
                try
                {
                    while (!(next is null))
                    {
                        val += (next.PlaceNum * multiVal);
                        multiVal *= multi;
                        next = next.nextPlaceNum;
                    }
                }
                catch
                {
                    try
                    {
                        string deepIntStr = v.ToString();

                        if (int.TryParse(deepIntStr, out int conversion))
                        {
                            return conversion;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    catch
                    {
                        return -1;
                    }
                }
                return val;
            }
        }

        public static explicit operator uint(DeepInteger v)
        {
            DeepInteger abV = v;
            v.isNegative = false;

            if (v > uint.MaxValue)
            {
                return uint.MaxValue;
            }
            else
            {
                string deepIntStr = v.ToString();

                if (uint.TryParse(deepIntStr, out uint conversion))
                {
                    return conversion;
                }
                else
                {
                    return 0;
                }
            }
        }

        internal class ZeroesHolder
        {
            public ZeroesHolder()
            {

            }

            public ZeroesHolder(int val)
            {
                zero = val;
            }

            int zero = -1;
            public int Zero
            {
                get
                {
                    return zero;
                }
            }

            ZeroesHolder nextPlaceZero;
            public ZeroesHolder NextPlaceZero
            {
                get
                {
                    return nextPlaceZero;
                }
            }

            public void AddZero()
            {
                if (zero > -1)
                {
                    if (nextPlaceZero is null)
                    {
                        nextPlaceZero = new ZeroesHolder(0);
                    }
                    else
                    {
                        nextPlaceZero.AddZero();
                    }
                }
                else
                {
                    zero = 0;
                }
            }
        }
    }
}