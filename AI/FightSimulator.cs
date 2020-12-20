using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{
    /// <summary>
    /// Should I iterate over all possibilities as deep as possible?
    /// How much will things actually blow up?  With fully unique deck (no repeat cards)
    /// 
    /// How about just normal play and do lots of sims vs a dumb simple enemy without.
    /// 
    /// TODO: it'd be beter
    /// </summary>
    public class FightSimulator
    {
        private string _Output = "C:/dl/output.txt";
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private IList<CardInstance> _CIs { get; set; }
        private bool _DoOutput { get; set; }
        private bool _OneStartingHandOnly { get; set; }

        private int _Depth { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public FightSimulator(IList<CardInstance> cis, Enemy enemy, Player player, bool doOutput = false, bool oneStartingHandOnly = false, int depth = 3)
        {
            _CIs = cis;
            _Enemy = enemy;
            _Player = player;
            _DoOutput = doOutput;
            _OneStartingHandOnly = oneStartingHandOnly;
            _Depth = depth;
        }

        /// <summary>
        /// Returns a list of fightnode roots based on initial draws.
        /// TODO add "randomchoices" and create a test for pommel strike.
        /// SS PS DD is your draw
        /// </summary>
        public FightNode Sim()
        {
            var fight = new Fight(_CIs, _Player, _Enemy);
            List<Tuple<List<CardInstance>, int>> handsAndWeights;
            FightNode rootNode;

            if (_OneStartingHandOnly) //megahack to just take the last N cards of the given CIs, for testing
            {
                var skipAmt = _CIs.Count - _Player.GetDrawAmount();
                var ll = _CIs.Skip(skipAmt).Take(_Player.GetDrawAmount()).ToList();
                handsAndWeights = new List<Tuple<List<CardInstance>, int>>() { new Tuple<List<CardInstance>, int>(ll, 1) };
            }
            else
            {
                var startingHands = GenSubsets(_CIs, Math.Min(_CIs.Count, _Player.GetDrawAmount()));
                handsAndWeights = GenHandWeights(startingHands);
            }

            rootNode = new FightNode(fight);
            foreach (var item in handsAndWeights)
            {
                var sh = item.Item1;
                // TODO: future - weigh by frequency
                //var count = item.Item2;

                var oneDraw = new FightNode(rootNode, randomChoice: true);
                var child = oneDraw.StartFight(initialHand: sh);
                Iter(child);

                if (_OneStartingHandOnly)
                {
                    break;
                }
            }

            return rootNode;
        }

        private void Iter(FightNode fn)
        {
            var actions = fn.Fight.GetAllActions();
            var turns = fn.Fight.TurnNumber;
            if (turns > _Depth)
            {
                fn.Fight.AssignLastAction(new FightAction(FightActionEnum.TooLong));
                return;
            }
            foreach (var action in actions)
            {
                var childNode = ApplyAction(fn, action);

                switch (childNode.Fight.Status)
                {
                    case FightStatus.Ongoing:
                        Iter(childNode);
                        break;
                    case FightStatus.Won:
                        //childNode.Display(_Output, leaf: true);
                        break;
                    case FightStatus.Lost:
                        break;
                    default:
                        throw new Exception("Other status");
                }
            }
        }

        /// <summary>
        /// actually an application of an action can result in multiple child nodes.
        /// For example: if you play battle trance, the child nodes are all the possible ways to draw.
        /// </summary>
        private FightNode ApplyAction(FightNode fn, FightAction action)
        {
            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var child = fn.PlayCard(action.Card);
                    return child;
                case FightActionEnum.Potion:
                    var child2 = fn.DrinkPotion(action.Potion, _Enemy);
                    return child2;
                case FightActionEnum.EndTurn:
                    var child3 = fn.EndTurn();
                    return child3;
                case FightActionEnum.EnemyMove:
                    var child4 = fn.EnemyMove();
                    return child4;
                default:
                    throw new Exception("Invalid action");
            }
        }

        /// <summary>
        /// Say you have a given situation S.
        /// Say there are 2 playthroughs from here, (only in this branch, no other mapping to similar branches).
        /// If one of them is a win and another a loss, state S is actually considered good. Basically we say the value of S is max(2 states).
        /// </summary>
        public void SaveResults(string path, FightNode rootNode)
        {
            if (!_DoOutput) return;
            var res = new List<string>();

            var fnodeactions = string.Join(',', rootNode.FightHistory);
            var fdesc = $"===Fight Situation: {rootNode} {rootNode.GetValue()} {fnodeactions}";
            res.Add(fdesc);

            //TODO node is the root node of fight
            foreach (var r in rootNode.Randoms)
            {
                SaveResults(path, r);
            }
            foreach (var cn in rootNode.Choices)
            {
                var actionCount = 0;
                var oneDrawNode = cn;
                while (true)
                {
                    var extra = "";
                    if (actionCount == 0)
                    {
                        extra = $"  ==Choice:{oneDrawNode.GetValue()}\n    ";
                    }
                    else
                    {
                        extra = "    ";
                    }
                    actionCount++;
                    var nodeactions = string.Join(',', oneDrawNode.FightHistory);
                    var desc = $"{extra}{oneDrawNode} {nodeactions}";
                    res.Add(desc);


                    if (RoundEndConditions.Contains(oneDrawNode.FightHistory.FightActionType))
                    {
                        break;
                    }

                    //follow the path down the (possibly multiple) best choices for the current turn.
                    oneDrawNode = oneDrawNode.Choices.FirstOrDefault(el => el.GetValue() == cn.GetValue());
                    if (oneDrawNode == null) //should never happen
                    {
                        throw new Exception("Somehow wasn't able to find a continuation with the expected value.");
                    }
                }
            }


            System.IO.File.AppendAllText(path, "First Action summaries\n");
            System.IO.File.AppendAllLines(path, res);
        }


        public IEnumerable<string> Summarize(List<Fight> fights)
        {
            var counts = new Dictionary<Tuple<FightStatus, int, int>, int>();
            var results = new Dictionary<FightStatus, int>();
            results[FightStatus.Won] = 0;
            results[FightStatus.Lost] = 0;
            results[FightStatus.Ongoing] = 0;

            foreach (var f in fights)
            {
                var key = new Tuple<FightStatus, int, int>(f.Status, Math.Max(f._Player.HP, 0), Math.Max(f._Enemies[0].HP, 0));
                if (!counts.ContainsKey(key))
                {
                    counts[key] = 0;
                }
                counts[key]++;

                results[f.Status]++;
            }

            foreach (var k in results.Keys)
            {
                yield return $"{k} {results[k]}";
            }

            foreach (var k in counts.Keys.OrderByDescending(el => el))
            {
                yield return $"{k.Item1} HP={k.Item2} en={k.Item3} Count={counts[k]}";
            }


        }
    }
}