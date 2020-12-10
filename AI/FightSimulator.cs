using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{

    /// <summary>
    /// just keep copying the fight playing out every choice.
    /// All this copying of the fight is expensive!
    /// </summary>


    /// <summary>
    /// Should I iterate over all possibilities as deep as possible?
    /// How much will things actually blow up?  With fully unique deck (no repeat cards)
    /// 
    /// How about just normal play and do lots of sims vs a dumb simple enemy without.
    /// </summary>
    public class FightSimulator
    {
        private string _Output = "C:/dl/output.txt";
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private List<CardInstance> _CIs { get; set; }
        private bool _DoOutput { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public FightSimulator(List<CardInstance> cis, Enemy enemy, Player player, bool doOutput = false)
        {
            _DoOutput = doOutput;
            _CIs = cis;
            _Enemy = enemy;
            _Player = player;
        }

        /// <summary>
        /// Returns a list of fightnode roots  based on initial draws.
        /// </summary>
        public List<FightNode> Sim()
        {
            if (_DoOutput) System.IO.File.WriteAllText(_Output, "");
            var fight = new Fight(_CIs, _Player, _Enemy);

            var res = new List<FightNode>();
            var startingHands = GenSubsets(_CIs, Math.Min(_CIs.Count, _Player.GetDrawAmount()));
            var handsAndWeights = GenHandWeights(startingHands);
            foreach (var item in handsAndWeights)
            {
                var sh = item.Item1;
                var count = item.Item2;
                var fc = fight.Copy();
                var rootNode = new FightNode(fight: fc, parent: null);
                //var objectMatchedSh = sh.Select(el => fc.FindIdenticalCardInSource(fc.GetDeck.GetDrawPile.ToList(), el)).ToList();
                var drawableHand = GetDrawableHand(fc.GetDeck, sh);
                fc.StartTurn(initialHand: drawableHand);
                Iter(rootNode);
                res.Add(rootNode);

                //TODO let's just do one starting hand for now.
                break;
            }

            SaveResults(_Output, res);

            return res;
        }

        /// <summary>
        /// In finding all drawable combos, we generate lists like SSSDD.
        /// We then need to find the actual objects for those cards in the drawpile.
        /// Drawpile may have multiple effectively identical cards; we should find unique ones so we don't draw try to draw D_1 twice.
        /// </summary>
        public List<CardInstance> GetDrawableHand(Deck d, List<CardInstance> target)
        {
            var drawPile = d.GetDrawPile;
            var res = new List<CardInstance>();
            foreach (var t in target)
            {
                var found = FindIdenticalCardInSource(drawPile, t, res);
                res.Add(found);
            }
            return res;
        }



        private void Iter(FightNode fn)
        {
            var actions = fn.Fight.GetAllActions();
            var turns = fn.Fight.TurnNumber;
            if (turns > 2)
            {
                fn.Fight.AddHistory(FightActionEnum.TooLong);
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
                        childNode.Fight.AddHistory(FightActionEnum.WonFight, desc: new List<string>() { $"Won with HP: {childNode.Fight.GetPlayerHP()}" });
                        break;
                    case FightStatus.Lost:
                        childNode.Fight.AddHistory(FightActionEnum.LostFight, desc: new List<string>() { $"Lost with enemy hp: {childNode.Fight.GetEnemyHP()}" });
                        break;
                    default:
                        throw new Exception("Other status");
                }
            }
        }

        private FightNode ApplyAction(FightNode fn, FightAction action)
        {
            var child = new FightNode(fn.Fight.Copy(), fn);
            var fight = child.Fight;
            if (fn.Fight._Player.StatusInstances.Count != child.Fight._Player.StatusInstances.Count)
            {
                var ae = 4;
            }
            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var copiedCard = FindIdenticalCardInSource(fight.GetDeck.GetHand, action.Card);
                    fight.PlayCard(copiedCard);
                    return child;
                case FightActionEnum.EndTurn:
                    fight.EndTurn();
                    //endturn on copy, then create a node for enemymove
                    child = new FightNode(child.Fight.Copy(), parent: child);
                    child.Fight.EnemyMove();
                    if (child.Fight.Status == FightStatus.Ongoing)
                    {
                        child = new FightNode(child.Fight.Copy(), parent: child);
                        child.Fight.StartTurn();
                    }
                    //or just dead
                    return child;
                case FightActionEnum.Potion:
                    fight._Player.DrinkPotion(fight, action.Potion, _Enemy);
                    var fakePotion = fight._Player.Potions.First(el => el.ToString() == action.Potion.ToString());
                    fight._Player.Potions.Remove(fakePotion);
                    return child;
                default:
                    throw new Exception("Invalid action");
            }
        }

        /// <summary>
        /// Say you have a given situation S.
        /// Say there are 2 playthroughs from here, (only in this branch, no other mapping to similar branches).
        /// If one of them is a win and another a loss, state S is actually considered good. Basically we say the value of S is max(2 states).
        /// </summary>
        public void SaveResults(string path, List<FightNode> nodes)
        {
            if (!_DoOutput) return;
            var res = new List<string>();
            var endingConditions = new List<FightActionEnum>() { FightActionEnum.EndTurn, FightActionEnum.WonFight, FightActionEnum.LostFight, FightActionEnum.TooLong };
            foreach (var node in nodes)
            {
                var fnodeactions = string.Join(',', node.Fight.FightHistory);
                var fdesc = $"===Fight Situation: {node} {node.GetValue()} {fnodeactions}";
                res.Add(fdesc);
                foreach (var cn in node.Children)
                {
                    var actionCount = 0;
                    var theNode = cn;
                    while (true)
                    {
                        var extra = "";
                        if (actionCount == 0)
                        {
                            extra = $"  ==Choice:{theNode.GetValue()}\n    ";
                        }
                        else
                        {
                            extra = "    ";
                        }
                        actionCount++;
                        var nodeactions = string.Join(',', theNode.Fight.FightHistory);
                        var desc = $"{extra}{theNode} {nodeactions}";
                        res.Add(desc);


                        if (theNode.Fight.FightHistory.Any(el => endingConditions.Contains(el.FightActionType)))
                        {
                            break;
                        }
                        //follow the path down the (possibly multiple) best choices for the current turn.
                        theNode = theNode.Children.FirstOrDefault(el => el.GetValue() == cn.GetValue());
                        if (theNode == null) //should never happen
                        {
                            throw new Exception("Somehow wasn't able to find a continuation with the expected value.");
                        }
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