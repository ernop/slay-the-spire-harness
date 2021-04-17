using System;
using System.Diagnostics.CodeAnalysis;

namespace StS
{
    /// <summary>
    /// Can I make a class wrapping energyCost which compares like an int, but displays like a string (displaying "-" for maxint value?)
    /// Yes, but it's pretty annoying.  In tests for example, comparing something to an EnergyCostInt will force it to be evaluated as a string.
    /// </summary>
    public class EnergyCostInt : IComparable<int>, IComparable<EnergyCostInt>
    {
        public int En { get; set; }
        public EnergyCostInt(int ec)
        {
            En = ec;
        }

        public override string ToString()
        {
            if (En == int.MaxValue)
            {
                return "-";
            }
            if (En == int.MinValue)
            {
                return "-";
            }
            return En.ToString();
        }

        public int CompareTo([AllowNull] EnergyCostInt other)
        {
            if (this.En > other.En) return 1;
            if (this.En == other.En) return 0;
            return -1;
        }

        public int CompareTo([AllowNull] int other)
        {
            if (this.En > other) return 1;
            if (this.En == other) return 0;
            return -1;
        }

        public static bool operator ==(EnergyCostInt left, EnergyCostInt right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }
            if (!ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return false;
            }
            return left.En == right.En;
        }
        public static bool operator !=(EnergyCostInt left, EnergyCostInt right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return false;
            }
            if (!ReferenceEquals(left, null) && ReferenceEquals(right, null))
            {
                return true;
            }
            return left.En != right.En;
        }
        public static bool operator <(EnergyCostInt left, EnergyCostInt right)
        {
            return left.En < right.En;
        }
        public static bool operator >(EnergyCostInt left, EnergyCostInt right)
        {
            return left.En > right.En;
        }

        public static bool operator <(EnergyCostInt left, int right)
        {
            return left.En < right;
        }
        public static bool operator <=(EnergyCostInt left, int right)
        {
            return left.En <= right;
        }
        public static bool operator >=(EnergyCostInt left, int right)
        {
            return left.En >= right;
        }
        public static bool operator !=(EnergyCostInt left, int right)
        {
            return left.En != right;
        }
        public static bool operator ==(EnergyCostInt left, int right)
        {
            return left.En == right;
        }

        public static bool operator >(EnergyCostInt left, int right)
        {
            return left.En > right;
        }
    }
}
