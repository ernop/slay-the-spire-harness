namespace StS
{
    /// <summary>
    /// Node value is HP, or at least how much damage you did,
    /// *then* minimum number of cards played.
    /// </summary>
    public class NodeValue
    {
        public NodeValue(double value, int cards, FightNode bestChoice)
        {
            Value = value;

            //How many cards played this round. inherited from non-round ending child bestnode.
            Cards = cards;
            BestChoice = bestChoice;
        }

        public double Value { get; set; }
        public int Cards { get; set; }
        public FightNode BestChoice {get;set;}

        public static bool operator <(NodeValue a, NodeValue b)
        {
            return b > a;
        }

        public static bool operator >(NodeValue a, NodeValue b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }
            if (a.Value > b.Value)
            {
                return true;
            }
            else if (a.Value == b.Value)
            {
                //fewer cards played is better
                return a.Cards < b.Cards;
            }

            return false;

        }
        public static bool operator ==(NodeValue a, NodeValue b)
        {
            if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
            {
                return true;
            }
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }
            return a.Value == b.Value && a.Cards == b.Cards;
        }

        public static bool operator !=(NodeValue a, NodeValue b)
        {
            return !(a.Value == b.Value && a.Cards == b.Cards);
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}