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
        public FightNode(Fight fight, FightNode parent)
        {
            Fight = fight;
            Parent = parent;
            Children = new List<FightNode>();
            Parent?.Children.Add(this);
        }

        internal void Display(string path)
        {
            if (Children == null || Children.Count == 0)
            {
                DisplayFullHistory(path);
            }
            else
            {
                foreach (var c in Children)
                {
                    c.Display(path);
                }
            }
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
            foreach (var h in Fight.FightHistory)
            {
                System.IO.File.AppendAllText(path, tabs);
                var s = h.ToString();
                var value = $" {GetValue()} ";
                System.IO.File.AppendAllText(path, s + value + "\n");
            }

        }

        /// <summary>
        /// TODO: this should actually be a weighted map of draw/monster action => list<FightNode> so that we can weigh things later.
        /// </summary>
        public List<FightNode> Children { get; set; }

        /// <summary>
        /// The state here.
        /// </summary>
        public Fight Fight { get; set; }
        public FightNode Parent { get; set; }
        private bool _Calculated { get; set; } = false;
        private int _Value { get; set; } = 0;
        private FightNode _BestChild { get; set; }
        public int GetValue()
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


        private void _CalcValue()
        {
            if (Children.Count == 0)
            {
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

                //leaf nodes have no bestchild.
                _BestChild = null;
            }
            else
            {
                FightNode bestChild = null;
                int bestVal = int.MinValue;
                foreach (var c in Children)
                {
                    var val = c.GetValue();
                    if (val > bestVal)
                    {
                        bestVal = val;
                        bestChild = c;
                    }
                }

                _BestChild = bestChild;
                _Value = bestVal;
            }
        }

        public override string ToString()
        {
            return $"{Fight._Player.Details()} - {Fight._Enemies[0].Details()}";
        }
    }
}