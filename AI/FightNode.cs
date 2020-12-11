using System;
using System.Collections.Generic;
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
        /// Create a child node.
        /// RND determines if this is a "choice" path or a "random" path.
        /// </summary>
        public FightNode(FightNode parent, bool rnd, bool root = false, Fight fight = null)
        {
            if (root)
            {
                Fight = fight ?? throw new ArgumentNullException(nameof(fight));
            }
            else
            {
                Parent = parent;

                Fight = parent.Fight.Copy();

                if (rnd)
                {
                    Parent.Randoms.Add(this);
                }
                else
                {
                    Parent.Choices.Add(this);
                }
            }
        }

        /// <summary>
        /// TODO: this should actually be a weighted map of draw/monster action => list<FightNode> so that we can weigh things later.
        /// </summary>
        public List<FightNode> Choices { get; set; } = new List<FightNode>();

        /// <summary>
        /// fights either have randoms or choices.
        /// minimaxer choose either the best choice, or weighted by distribution of randoms
        /// </summary>
        public List<FightNode> Randoms { get; set; } = new List<FightNode>();

        /// <summary>
        /// The state here.
        /// </summary>
        public Fight Fight { get; set; }
        public FightNode Parent { get; set; }
        private bool _Calculated { get; set; } = false;
        private double _Value { get; set; } = 0;
        private FightNode _BestChild { get; set; }
        public List<string> LastHistory { get; set; }

        public List<FightAction> FightHistory { get; internal set; } = new List<FightAction>();

        /// <summary>
        /// example: Play TrueGrit.
        /// in this case, card would be TG
        /// desc could be 
        ///     "gained 7b"
        ///     "randomly exhausted X"
        ///     "did 3 damage to enemy due to charon's ashes"
        /// </summary>
        public void AddHistory()
        {
            if (Fight.LastAction == null)
            {
                throw new Exception("Should not be null.");
            }
            var fa = Fight.LastAction;
            var fhstr = fa.ToString();
            if (string.IsNullOrEmpty(fhstr))
            {
                return;
            }

            FightHistory.Add(fa);
        }

        private int Depth
        {
            get
            {
                var res = 0;
                var target = this;
                while (target.Parent != null)
                {
                    res++;
                    target = target.Parent;
                }
                return res;
            }
        }

        /// <summary>
        /// We are a choice.
        /// </summary>
        internal void DisplayFirstRound(string path)
        {
            var bestChildren = new List<FightNode>();

            var target = this;
            while (true)
            {
                bestChildren.Add(target);

                if (target.FightHistory.Any(el => Helpers.RoundEndConditions.Contains(el.FightActionType)))
                {
                    break;
                }
                target = target?._BestChild;

                if (target == null)
                {
                    break;
                }
            }

            foreach (var b in bestChildren)
            {
                Write(path, b.FightHistory);
            }
        }

        private static void Write(string path, List<FightAction> l)
        {
            var j = string.Join(',', l.Select(el => el.ToString()));
            System.IO.File.AppendAllText(path, j + "\n");
        }

        internal void StartTurn(List<CardInstance> initialHand)
        {
            Fight.StartTurn(initialHand: initialHand);
        }

        internal void DisplayFullHistory(string path)
        {
            var nodes = new List<FightNode>();
            var node = this;
            nodes.Add(this);
            while (node.Parent != null)
            {
                node = node.Parent;
                nodes.Add(node);
            }

            nodes.Reverse();
            foreach (var hnode in nodes)
            {
                hnode.DisplayCurrentAction(path);
            }
            System.IO.File.AppendAllText(path, $"==========Done\n");
        }

        private void DisplayCurrentAction(string path)
        {
            //if fight.history has startturn, then fighthistories before it should be considered to have one less turnnumber
            var tabs = new string(' ', Fight.TurnNumber * 2);
            foreach (var h in FightHistory)
            {
                System.IO.File.AppendAllText(path, tabs);
                var s = h.ToString();
                var value = $" {GetValue()} ";
                System.IO.File.AppendAllText(path, s + value + "\n");
            }

        }

        public double GetValue()
        {
            if (!_Calculated)
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

        private enum ChoiceType
        {
            Choice = 1,
            Random = 2,
            Leaf = 3,
        }

        private ChoiceType GetChoiceType()
        {
            if (Choices.Count != 0 && Randoms.Count != 0)
            {
                throw new Exception("Should not happen");
            }
            if (Choices.Count > 0)
            {
                return ChoiceType.Choice;
            }
            if (Randoms.Count > 0)
            {
                return ChoiceType.Random;
            }
            return ChoiceType.Leaf;
        }

        private void _CalcValue()
        {
            switch (GetChoiceType())
            {
                case ChoiceType.Choice:
                    FightNode bestChild = null;
                    var bestVal = double.MinValue;
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
                    _Value = bestVal;
                    break;
                case ChoiceType.Random:
                    var rsum = 0.0d;
                    var rc = 0;
                    foreach (var c in Randoms)
                    {
                        rsum += c.GetValue();
                        rc++;
                    }
                    _Value = rsum * 1.0 / rc;
                    //TODO okay to not assign bestchild? it doesn't make sense over random.
                    break;
                case ChoiceType.Leaf:
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
                    _Value = val;
                    _BestChild = null;
                    break;
            }
        }

        public override string ToString()
        {
            return $"{GetValue()} D{Depth} {Fight._Player.Details()} - {Fight._Enemies[0].Details()}";
        }
    }
}