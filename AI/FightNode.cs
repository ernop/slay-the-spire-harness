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
        /// How many times it's been encountered. When calculating value, weight this.
        /// </summary>
        public int Weight { get; set; } = 1;
        public NodeValue Value { get; set; }
        public bool ValueCalculated { get; set; } = false;
        public FightNode(Fight f, int depth = 1)
        {
            Fight = f;
            Depth = depth;
        }

        public FightNode AddChild(FightNode n, bool r)
        {
            n.Parent = this;
            n.FightAction = n.Fight.FightAction;
            if (r)
            {
                Randoms.Add(n);
            }
            else
            {
                Choices.Add(n);
            }

            //Fight is over. Calc values.
            if (n.Fight.Status != FightStatus.Ongoing)
            {
                n.CalcValue();
            }

            return n;
        }

        private void CalcValue()
        {
            ValueCalculated = true;
            switch (GetChoiceType())
            {
                case NodeType.TooLong:
                    var tooLongVal = Fight._Player.HP - Fight._Enemies[0].HP;
                    SetValue(new NodeValue(tooLongVal, 0, null));
                    break;
                case NodeType.Leaf:
                    var val = 0;
                    var tt = this;
                    switch (Fight.Status)
                    {
                        case FightStatus.Ongoing:
                            var hh = AALeafHistory();
                            throw new Exception("Leaves can't have ongoing fights.");
                            //this happens when we get toolong
                            //A
                            //BC
                            // B is toolong, C ended.
                            // we backgrack to A but can't calc b.  B is a leaf.
                            //TODO this is obviously wrong.
                            //no point in calculating them as we go.
                            //val = Fight._Player.HP - Fight._Enemies[0].HP;
                            return;
                        case FightStatus.Won:
                            val = Fight._Player.HP;
                            break;
                        case FightStatus.Lost:
                            val = -1 * Fight._Enemies[0].HP;
                            break;
                    }
                    var cards = 0;
                    if (Fight.FightAction.FightActionType == FightActionEnum.PlayCard)
                    {
                        cards++;
                    }
                    SetValue(new NodeValue(val, cards, null));
                    break;
                case NodeType.Choice:
                    if (FightAction == null)
                    {
                        throw new Exception("This should not happen");
                    }
                    NodeValue value = null;
                    FightNode bc = null;
                    foreach (var c in Choices)
                    {
                        var cval = c.GetValue();
                        if (value == null || cval > value)
                        {
                            value = cval;
                            bc = c;
                        }
                    }
                    int cards2 = 0;
                    //Todo prefer playing cards to playing potions!
                    if (FightAction.FightActionType == FightActionEnum.PlayCard || FightAction.FightActionType == FightActionEnum.Potion)
                    {
                        cards2++;
                    }
                    var myVal = new NodeValue(value.Value, value.Cards+cards2, bc);
                    SetValue(myVal);
                    break;
                case NodeType.Random:
                    var rsum = 0.0d;
                    var rc = 0;
                    foreach (var r in Randoms)
                    {
                        rsum += r.GetValue().Value * r.Weight;
                        rc += r.Weight;
                    }
                    SetValue(new NodeValue(rsum * 1.0 / rc, 0, null));
                    //TODO okay to not assign bestchild? it doesn't make sense over random.
                    break;
            }
        }

        /// <summary>
        /// We fill in values bottom down.  So sometimes when you set a new value you should check parents too.
        /// Also if we are a choice and just added a worse value, don't recalculate parent.
        /// </summary>
        private void SetValue(NodeValue v)
        {
            var oldValue = Value;
            Value = v;
            switch (GetChoiceType())
            {
                case NodeType.Choice:
                    if (v > oldValue || object.ReferenceEquals(oldValue,null))
                    {
                        Parent?.CalcValue();
                    }
                    break;
                case NodeType.Random:
                    //TODO this is optimizable.
                    Parent?.CalcValue();
                    break;
                case NodeType.Leaf:
                    Parent.CalcValue();
                    break;
            }
            
        }

        public List<FightNode> Choices { get; set; } = new List<FightNode>();

        public List<FightNode> Randoms { get; set; } = new List<FightNode>();
        public Fight Fight { get; set; }
        public FightNode Parent { get; set; }
        public int Depth { get; private set; } = int.MinValue;
        /// <summary>
        /// The history of the single action here - including both player actions, draws, monster actions.
        /// </summary>
        public FightAction FightAction { get; internal set; } = new FightAction(FightActionEnum.NotInitialized);

        public IEnumerable<string> AALeafHistory()
        {
            
            var target = this;
            var res = new List<string>();

            while (!object.ReferenceEquals(target, null))
            {
                res.Add(target.ToString());
                target = target.Parent;
            }
            res.Reverse();
            return res;
        }

        public IEnumerable<string> GetTurnHistory()
        {
            var target = this;
            var res = new List<string>();

            while (!object.ReferenceEquals(target, null))
            {
                res.Add(target.ToString());
                target = target.Value?.BestChoice;
            }
            return res;
        }

        public FightNode GetNode()
        {
            var child = new FightNode(Fight.Copy(), Depth + 1);
            return child;
        }

        /// <summary>
        /// Also start the first turn.
        /// </summary>
        internal void StartFight()
        {
            FightAction = new FightAction(FightActionEnum.StartFight);
        }

        internal FightNode StartTurn(FightAction action)
        {
            var c = GetNode();
            var rnd = c.Fight.StartTurn(initialHand: action.CardTargets);
            AddChild(c, rnd);
            return c;
        }

        /// <summary>
        /// Call various high level actions on nodes and they'll eventually return ienumerable<FightNode>
        /// </summary>
        internal FightNode PlayCard(FightAction action)
        {
            var c = GetNode();

            var rnd = c.Fight.PlayCard(action.Card, action.CardTargets);
            AddChild(c, rnd);
            return c;
        }

        internal FightNode DrinkPotion(FightAction action)
        {
            var c = GetNode();
            var rnd = c.Fight.DrinkPotion(action.Potion, (Enemy)action.Target);
            AddChild(c, rnd);
            return c;
        }
        
        internal FightNode EndTurn()
        {
            var c = GetNode();
            c.Fight.EndTurn();
            AddChild(c, false);
            return c;
        }

        internal FightNode EnemyMove(FightAction action)
        {
            var c = GetNode();
            c.Fight.EnemyMove(action);
            AddChild(c, true);
            return c;
        }

        /// <summary>
        /// If lastRound is Defend+Strike, how about just Strike as the besT?
        /// </summary>
        public NodeValue GetValue()
        {
            if (Value == null)
            {
                CalcValue();
            }
            return Value;
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


        public override string ToString()
        {
            var val = object.ReferenceEquals(Value, null) ? "" : Value.ToString();
            var status = Fight.Status;
            var fa = FightAction.ToString();
            if (FightAction.FightActionType == FightActionEnum.StartTurn)
            {
                var showTurn = $"T:{Fight.TurnNumber,2} {Fight._Player.Details()} {Fight._Enemies[0].Details()}\n";
                var res = $"{showTurn}  {fa} {status} {val}";
                return res;
            }
            else
            {
                var res = $"  {fa}";
                return res;
            }
         }
    }
}