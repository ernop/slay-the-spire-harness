using System;
using System.Collections.Generic;

namespace StS
{
    /// <summary>
    /// This is all predicated on identical enemy & draw behavior.  Neither of which is the case.
    /// So I actually need to weigh by draw plus enemy action.
    /// 
    /// What is this, really? in no-redraw, no enemy choice world (where all choice is from player),
    /// it should be a single node in a tree such that player can always choose between children.
    /// </summary>
    public class FightNode
    {
        /// <summary>
        /// Root.
        /// </summary>
        /// <param name="f"></param>
        public FightNode(Fight f)
        {
            Fight = f ?? throw new ArgumentNullException(nameof(f));
        }

        /// <summary>
        /// Create a child node.
        /// RND determines if this is a "choice" path or a "random" path.
        /// </summary>
        public FightNode(FightNode parent, bool randomChoice)
        {
            Parent = parent;

            Fight = parent.Fight.Copy();

            if (randomChoice)
            {
                if (parent.Randoms.Count > 0)
                {
                    var ae = 4;
                }
                Parent.Randoms.Add(this);
            }
            else
            {
                Parent.Choices.Add(this);
            }

            Parent.ClearValue();

            Fight.FightNode = this;
        }

        /// <summary>
        /// When a new random or choice node is added, clear the value of a node.
        /// </summary>
        private void ClearValue()
        {
            _Calculated = false;
            Parent?.ClearValue();
        }

        public List<FightNode> Choices { get; set; } = new List<FightNode>();

        public List<FightNode> Randoms { get; set; } = new List<FightNode>();
        public Fight Fight { get; set; }
        public FightNode Parent { get; set; }
        private bool _Calculated { get; set; } = false;
        private NodeValue _Value { get; set; }
        private int _Depth { get; set; } = int.MinValue;
        private FightNode _BestChild { get; set; }

        /// <summary>
        /// The history of the single action here - including both player actions, draws, monster actions.
        /// </summary>
        public FightAction FightAction { get; internal set; }

        public int Depth
        {
            get
            {
                if (_Depth == int.MinValue)
                {
                    var res = 1;
                    var target = this;
                    while (target.Parent != null)
                    {
                        res++;
                        target = target.Parent;
                    }
                    _Depth = res;
                }
                return _Depth;
            }
        }

        internal void Display(string path, bool leaf = false)
        {
            if (leaf)
            {
                //find parent.
                var target = this;
                while (target.Parent.Choices.Count != 0)
                {
                    target = target.Parent;
                }

                target.Display(path);
            }
            else
            {
                foreach (var r in Randoms)
                {
                    var lastRound = DisplayRound(r, path);
                    while (lastRound != null)
                    {
                        lastRound = DisplayRound(lastRound, path);
                    }
                }
            }
        }

        public IEnumerable<string> AALeafHistory()
        {
            var res = new List<string>();
            var target = this;
            while (target != null)
            {
                var val = target.ToString();
                res.Add(val);
                target = target.Parent;
                if (target?.Choices.Count == 0)
                {
                    //we don't follow history back through random nodes.
                    //this will make this method useless for non-short fights
                    if (target.Randoms.Count ==1 && target.Depth!=1) //can't backtrack past first.
                    {
                        res.Add(target.ToString());
                        target = target.Parent;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            res.Reverse();
            return res;
        }

        private FightNode DisplayRound(FightNode node, string path)
        {
            var bestChildren = new List<FightNode>();
            var target = node;
            while (true)
            {
                bestChildren.Add(target);

                if (Helpers.RoundEndConditions.Contains(target.FightAction.FightActionType))
                {
                    break;
                }
                target = target._BestChild;

                if (target == null)
                {
                    break;
                }
            }
            var lastRound = bestChildren[bestChildren.Count - 1];
            if (lastRound.Fight.Status != FightStatus.Ongoing)
            {
                return null;
            }
            return lastRound.BestChild();
        }

        /// <summary>
        /// modify the current fight, don't create child.
        /// </summary>
        internal FightNode StartFight(IList<CardInstance> initialHand)
        {
            Fight.StartTurn(initialHand: initialHand);
            return this;
        }

        internal FightNode StartTurn(IList<CardInstance> initialHand = null)
        {
            var child = new FightNode(this, true);
            child.Fight.StartTurn(initialHand: initialHand);
            return child;
        }
        /// <summary>
        /// Call various high level actions on nodes and they'll eventually return ienumerable<FightNode>
        /// </summary>
        internal FightNode PlayCard(CardInstance card)
        {
            var child = new FightNode(this, false);
            child.Fight.PlayCard(card);
            return child;
        }

        internal FightNode DrinkPotion(Potion potion, Enemy enemy)
        {
            var child = new FightNode(this, false);
            child.Fight.DrinkPotion(potion, enemy);
            return child;
        }

        internal FightNode EndTurn()
        {
            var child = new FightNode(this, false);
            child.Fight.EndTurn();
            return child;
        }

        internal FightNode EnemyMove(FightAction action)
        {
            var child = new FightNode(this, true);
            child.Fight.EnemyMove(action);
            return child;
        }

        /// <summary>
        /// If lastRound is Defend+Strike, how about just Strike as the besT?
        /// </summary>
        public NodeValue GetValue(bool force = false)
        {
            if (!_Calculated || force)
            {
                _CalcValue();
            }

            return _Value;
        }

        public FightNode BestChild()
        {
            if (!_Calculated)
            {
                _CalcValue();
            }

            return _BestChild;
        }

        private NodeType GetChoiceType()
        {
            if (Choices.Count != 0 && Randoms.Count != 0)
            {
                throw new Exception("Should not happen");
            }
            if (Choices.Count > 0)
            {
                return NodeType.Choice;
            }
            if (Randoms.Count > 0)
            {
                return NodeType.Random;
            }

            //fight is over
            return NodeType.Leaf;
        }

        private void _CalcValue()
        {
            switch (GetChoiceType())
            {
                case NodeType.Choice:
                    if (FightAction == null)
                    {
                        throw new Exception("This should not happen");
                    }
                    FightNode bestChild = null;
                    var bestVal = new NodeValue(int.MinValue, int.MaxValue);
                    foreach (var c in Choices)
                    {
                        var cval = c.GetValue();
                        if (cval > bestVal)
                        {
                            bestVal = cval;
                            bestChild = c;
                        }
                    }

                    _BestChild = bestChild;
                    int cards;

                    if (FightAction.FightActionType == FightActionEnum.PlayCard)
                    {
                        cards = _BestChild.GetValue().Cards + 1;
                    }
                    else if (FightAction.FightActionType == FightActionEnum.Potion)
                    {
                        cards = _BestChild.GetValue().Cards;
                    }
                    else
                    {
                        cards = 0;
                    }

                    _Value = new NodeValue(bestVal.Value, cards);
                    break;
                case NodeType.Random:
                    var rsum = 0.0d;
                    var rc = 0;
                    foreach (var c in Randoms)
                    {
                        rsum += c.GetValue().Value;
                        rc++;
                    }
                    _Value = new NodeValue(rsum * 1.0 / rc, 0);
                    //TODO okay to not assign bestchild? it doesn't make sense over random.
                    break;
                case NodeType.Leaf:
                    var val = 0;
                    switch (Fight.Status)
                    {
                        case FightStatus.Ongoing:
                            //TODO this is obviously wrong.
                            val = Fight._Player.HP - Fight._Enemies[0].HP;
                            break;
                        case FightStatus.Won:
                            val = Fight._Player.HP;
                            break;
                        case FightStatus.Lost:
                            val = -1 * Fight._Enemies[0].HP;
                            break;
                    }
                    _Value = new NodeValue(val, 0);
                    _BestChild = null;
                    break;
            }
        }

        public override string ToString()
        {
            var newRound = false;
            var showTurn = "";
            var detailActionTypes = new List<FightActionEnum>() { FightActionEnum.EndTurn, FightActionEnum.EndTurn, FightActionEnum.StartFight, FightActionEnum.StartTurn };
            if (FightAction.FightActionType == FightActionEnum.StartTurn)
            {
                showTurn = $"T:{Fight.TurnNumber} NV:{GetValue()} {Fight._Player.Details()} {Fight._Enemies[0].Details()}\n";
            }
            if (detailActionTypes.Contains(FightAction.FightActionType))
            {
                newRound = true;
            }

            if (newRound)
            {
                return $"{showTurn}\t\t{FightAction}";
            }
            else
            {
                return $"\t\t{FightAction}";
            }
        }
    }
}