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
                var hh = n.AALeafHistory();
                n.CalcValue();
            }

            return n;
        }

        private void CalcValue()
        {
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

        //internal void Display(string path, bool leaf = false)
        //{
        //    if (leaf)
        //    {
        //        //find parent.
        //        var target = this;
        //        while (target.Parent.Choices.Count != 0)
        //        {
        //            target = target.Parent;
        //        }

        //        target.Display(path);
        //    }
        //    else
        //    {
        //        foreach (var r in Randoms)
        //        {
        //            var lastRound = DisplayRound(r, path);
        //            while (lastRound != null)
        //            {
        //                lastRound = DisplayRound(lastRound, path);
        //            }
        //        }
        //    }
        //}

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

        internal void SetAction(FightAction fightAction)
        {
            if (FightAction != null)
            {
                if (FightAction.FightActionType != FightActionEnum.NotInitialized)
                {
                    throw new Exception("protection");
                }
            }

            FightAction = fightAction;
        }

        //    var target = this;
        //    while (target != null)
        //    {
        //        var val = target.ToString();
        //        res.Add(val);
        //        target = target.Parent;
        //        if (target?.Choices.Count == 0)
        //        {
        //            //we don't follow history back through random nodes.
        //            //this will make this method useless for non-short fights
        //            if (target.Randoms.Count == 1 && target.Depth != 1) //can't backtrack past first.
        //            {
        //                res.Add(target.ToString());
        //                target = target.Parent;
        //            }
        //            else
        //            {
        //                break;
        //            }

        //        }
        //    }
        //    res.Reverse();
        //    return res;
        //}

        //private FightNode DisplayRound(FightNode node, string path)
        //{
        //    var bestChildren = new List<FightNode>();
        //    var target = node;
        //    while (true)
        //    {
        //        bestChildren.Add(target);

        //        if (Helpers.RoundEndConditions.Contains(target.FightAction.FightActionType))
        //        {
        //            break;
        //        }
        //        target = target._BestChild;

        //        if (target == null)
        //        {
        //            break;
        //        }
        //    }
        //    var lastRound = bestChildren[bestChildren.Count - 1];
        //    if (lastRound.Fight.Status != FightStatus.Ongoing)
        //    {
        //        return null;
        //    }
        //    return lastRound.BestChild();
        //}

        public FightNode GetNode()
        {
            var child = new FightNode(Fight.Copy(), Depth + 1);
            return child;
        }

        /// <summary>
        /// Also start the first turn.
        /// </summary>
        internal FightNode StartFight(IList<CardInstance> initialHand)
        {
            var c = GetNode();
            c.Fight.StartTurn(initialHand: initialHand);
            FightAction = new FightAction(FightActionEnum.StartFight);
            AddChild(c, true);
            return c;
        }

        internal FightNode StartTurn(IList<CardInstance> initialHand = null)
        {
            var c = GetNode();
            var rnd = c.Fight.StartTurn(initialHand: initialHand);
            AddChild(c, rnd);
            return c;
        }

        /// <summary>
        /// Call various high level actions on nodes and they'll eventually return ienumerable<FightNode>
        /// </summary>
        internal FightNode PlayCard(CardInstance card)
        {
            var c = GetNode();

            var rnd = c.Fight.PlayCard(card);
            AddChild(c, rnd);
            return c;
        }

        internal FightNode DrinkPotion(Potion potion, Enemy enemy)
        {
            var c = GetNode();
            var rnd = c.Fight.DrinkPotion(potion, enemy);
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
            try
            {
                var val = Value == null ? "" : Value.ToString();
                
                var newRound = false;
                var showTurn = "";
                var status = Fight.Status;
                var detailActionTypes = new List<FightActionEnum>() { FightActionEnum.EndTurn, FightActionEnum.EndTurn, FightActionEnum.StartFight, FightActionEnum.StartTurn };
                if (FightAction.FightActionType == FightActionEnum.StartTurn)
                {
                    showTurn = $"T:{Fight.TurnNumber} {Fight._Player.Details()} {Fight._Enemies[0].Details()}\n";
                }
                if (detailActionTypes.Contains(FightAction.FightActionType))
                {
                    newRound = true;
                }

                if (newRound)
                {
                    return $"{showTurn}    {FightAction} {status}";
                }
                else
                {
                    return $"{FightAction} {status}";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}