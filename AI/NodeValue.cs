namespace StS
{



    public partial class FightNode
    {
        public class NodeValue
        {
            public NodeValue(double value, int cards)
            {
                Value = value;

                //How many cards played this round.
                Cards = cards;
            }

            public double Value { get; set; }
            public int Cards { get; set; }

            public static bool operator <(NodeValue a, NodeValue b)
            {
                return !a.Compare(b);
            }
            public static bool operator >(NodeValue a, NodeValue b)
            {
                return a.Compare(b);
            }
            public bool Compare(NodeValue other)
            {
                if (other == null)
                {
                    return true;
                }
                if (Value == other.Value)
                {
                    return Cards < other.Cards;
                }
                return Value > other.Value;
            }

            public override string ToString()
            {
                return $"NV {Value} C:{Cards}";
            }
        }
    }
}