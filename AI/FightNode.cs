using System;
using System.Collections.Generic;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

using static StS.Helpers;
using System.Linq;

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

        /// <summary>
        /// Add node to the proper place and return it for further MCing
        /// </summary>
        public FightNode AddChild(FightNode child)
        {
            //interesting, although enemymoves are also randoms, there is no point in having an intermediate copy node.
            //i.e. Cendturn => RenemyMove => [Rvarious enemy moves] is kind of of pointless
            //unlike CstartTurn => Rwildstrike => [Rvarious wildstrikes]
            //                  => Rother random card => [ROther random card random outputs]
            child.FightAction = child.Fight.FightAction;

            if (child.FightAction.FightActionType == FightActionEnum.EnemyMove)
            {
                child.Parent = this;
                child.FightAction = child.Fight.FightAction;
                Randoms.Add(child);
            }

            else if (child.FightAction.Random)
            {
                //two cases:
                //the intermediate node is already created:
                FightNode intermediateNode = null;
                foreach (var c in Choices)
                {
                    if (c.FightAction.IsEqual(child.FightAction))
                    {
                        intermediateNode = c;
                        break;
                    }
                }

                if (intermediateNode == null)
                {
                    //if we have a random, then consider it a c with this as a random child.
                    intermediateNode = new FightNode(child.Fight.Copy());
                    intermediateNode.Parent = this;
                    var src = child.Fight.FightAction;

                    //we convert the specced out action into a generic one again.
                    var nonspecifiecAction = new FightAction(fightActionType: src.FightActionType,
                        card: src.Card, cardTargets: src.CardTargets, potion: src.Potion, target: src.Target,
                        hadRandomEffects: src.Random);
                    intermediateNode.FightAction = nonspecifiecAction;

                    //zero these out since at this stage they represent the action pre-randomization.
                    if (intermediateNode.Fight.FightNode != null)
                        intermediateNode.Fight.FightNode.FightAction.Key = null;
                    intermediateNode.FightAction.Key = null;
                    Choices.Add(intermediateNode);
                    //there is not a random with this key already since this is the first time we played this card with random children.
                }

                //we also have to check for identity with the child nodes.
                foreach (var other in intermediateNode.Randoms)
                {
                    if (other.FightAction.IsEqual(child.Fight.FightAction) && other.FightAction.Key==child.Fight.FightAction.Key)
                    {
                        //I should just compare the order of the draw pile actually.
                        other.Weight++;
                        return other;
                    }
                    else
                    {
                        var ae = 4;
                    }
                }
                child.Parent = intermediateNode;
                child.FightAction = child.Fight.FightAction;
                //child.fightaction still has its key
                intermediateNode.Randoms.Add(child);
            }
            else
            {
                child.Parent = this;
                child.FightAction = child.Fight.FightAction;
                Choices.Add(child);
            }

            //Fight is over. Calc values.
            if (child.Fight.Status != FightStatus.Ongoing)
            {
                child.CalcValue();
            }

            return child;
        }

        /// <summary>
        /// Todo: this is actually a bit inaccurate because of innate random effects - monkey paw + power card selection, wildstrike
        /// </summary>
        public List<FightNode> Choices { get; set; } = new List<FightNode>();
        public List<FightNode> Randoms { get; set; } = new List<FightNode>();
        public Fight Fight { get; set; }
        public FightNode Parent { get; set; }
        public int Depth { get; private set; } = int.MinValue;
        /// <summary>
        /// The history of the single action here - including both player actions, draws, monster actions.
        /// </summary>
        public FightAction FightAction { get; internal set; } = new FightAction(FightActionEnum.NotInitialized);


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
                    var myVal = new NodeValue(value.Value, value.Cards + cards2, bc);
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
                    if (v > oldValue || object.ReferenceEquals(oldValue, null))
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
        /// when trying to add a child node, we check if there already is one for the same action.
        /// complexity: for random actions, we first add a choice then the child randoms.
        /// This searches for both the child + the child random and if not returns nothing.
        /// If the intermediate node *does* exist, but the appropriate random child isn't found,
        /// that will be detected in AddChild
        /// </summary>
        private FightNode FindDuplicate(FightAction action)
        {
            if (action.FightActionType == FightActionEnum.EnemyMove)
            {
                foreach (var r in Randoms)
                {
                    if (r.FightAction.IsEqual(action))
                    {
                        return r;
                    }
                }
            }
            else if (Randoms.Count > 0)
            {
                throw new Exception("Should not happen");
            }
            else
            {
                if (action.FightActionType == FightActionEnum.PlayCard)
                {
                    if (Choices.Count > 0)
                    {
                        if (action.Card.Card.Name == nameof(PommelStrike))
                        {
                            var ae = 4;
                        }
                    }
                }

                //there should only be one cardplay of this.
                var c = Choices.SingleOrDefault(el => el.FightAction.IsEqual(action));
                if (c == null) { return null; }
                if (c.FightAction.IsEqual(action))
                {
                    if (!action.Random) //the nonrandom case; you simply found a normal node that represents this state already.
                    {
                        return c;
                    }
                    //e.g. we have played wildstrike before so we found it in the choices.
                    else
                    {
                        foreach (var r in c.Randoms)
                        {
                            if (r.FightAction.Key == action.Key)
                            {
                                return r;
                            }
                            if (r.FightAction.Key == null) throw new Exception("Should not happen");
                        }
                        //there is a node for playing this card, but no appropriate rchild.
                        return null;
                    }
                }
            }

            return null;
        }

        private void GenerateKeyIfNecessary(FightAction action)
        {
            if (action.Keys != null)
            {
                action.Key = Rnd.Next(action.Keys.Count);
                //note down which randomness we chose for this so that we can identify duplicates later.
            }
        }

        public FightNode ApplyAction(FightAction action)
        {
            //check if a child already has this action; if so just return that one.
            //would be a lot better to just MC the child directly rather than MCing the parent and then redoing this traversal.

            GenerateKeyIfNecessary(action);
            if (action.Random && action.Key == null) throw new Exception("Guard");

            var dup = FindDuplicate(action);
            if (dup != null)
            {
                dup.Weight++;
                return dup;
            }

            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var child = PlayCard(action); //non-rnd
                    return child;
                case FightActionEnum.Potion:
                    var child2 = DrinkPotion(action); //non-rnd
                    return child2;
                case FightActionEnum.EndTurn:
                    var child3 = EndTurn(); //non-rnd
                    return child3;
                case FightActionEnum.EnemyMove:
                    var child4 = EnemyMove(action); //rnd
                    return child4;
                case FightActionEnum.StartTurn:
                    var child5 = StartTurn(action);
                    return child5;
                default:
                    throw new Exception("Invalid action");
            }
        }

        /// <summary>
        /// Also start the first turn.
        /// </summary>
        internal void StartFight()
        {
            FightAction = new FightAction(FightActionEnum.StartFight, hadRandomEffects: true);
        }

        internal FightNode StartTurn(FightAction action = null)
        {
            var c = GetNode();
            c.Fight.StartTurn(action);
            AddChild(c);
            return c;
        }

        /// <summary>
        /// Call various high level actions on nodes and they'll eventually return ienumerable<FightNode>
        /// </summary>
        internal FightNode PlayCard(FightAction action)
        {
            var c = GetNode();

            c.Fight.PlayCard(action);
            AddChild(c);
            return c;
        }

        internal FightNode DrinkPotion(FightAction action)
        {
            var c = GetNode();
            var rnd = c.Fight.DrinkPotion(action.Potion, (Enemy)action.Target);
            AddChild(c);
            return c;
        }

        internal FightNode EndTurn()
        {
            var c = GetNode();
            c.Fight.EndTurn();
            AddChild(c);
            return c;
        }

        internal FightNode EnemyMove(FightAction action)
        {
            var c = GetNode();
            c.Fight.EnemyMove(action);
            AddChild(c);
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
                var val = object.ReferenceEquals(Value, null) ? "" : Value.ToString();
                var status = Fight.Status;
                var fa = FightAction.ToString();
                if (FightAction.FightActionType == FightActionEnum.StartTurn)
                {
                    var showTurn = $"T:{Fight.TurnNumber,2} {Fight._Player.Details()} {Fight._Enemies[0].Details()}\n";
                    var res = $"{showTurn} {Weight} {val}  {fa} {status} ";
                    return res;
                }
                else
                {
                    var res = $"  {val}{Weight}  {fa} ";
                    return res;
                }
            }
            catch (Exception ex)
            {
                return $"Failure {ex}";
            }
        }
    }
}