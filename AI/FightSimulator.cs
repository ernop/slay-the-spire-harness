using System;
using System.Collections.Generic;
using System.Linq;

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
        private string _Output = "C:/dl/fight.txt";
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private List<CardInstance> _CIs { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public FightSimulator(List<CardInstance> cis, Enemy enemy, Player player)
        {
            _CIs = cis;
            _Enemy = enemy;
            _Player = player;
        }

        public List<Fight> Sim()
        {

            System.IO.File.WriteAllText(_Output, "");
            var fight = new Fight(_CIs, _Player, _Enemy);
            fight.StartTurn();
            var res = new List<Fight>();
            Iter(fight, res);

            var summary = Summarize(res);
            System.IO.File.AppendAllLines(_Output, summary);
            foreach (var f in res)
            {
                SaveFight(f);
            }
            return res;
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

        private void SaveFight(Fight f)
        {
            System.IO.File.AppendAllLines(_Output, f.FightHistory.Select(el => el.ToString()));
        }

        private List<Fight> Iter(Fight fight, List<Fight> res)
        {
            var actions = fight.GetAllActions();
            var turns = fight.FightHistory.Where(el => el.FightActionType == FightActionEnum.EndTurn).Count();
            if (turns > 6)
            {
                fight.AddHistory(FightActionEnum.TooLong);
                res.Add(fight);
                return res;
            }
            foreach (var action in actions)
            {
                var fc = fight.Copy();
                ApplyAction(fc, action);
                switch (fc.Status)
                {
                    case FightStatus.Ongoing:
                        Iter(fc, res);
                        break;
                    case FightStatus.Won:
                        fc.AddHistory(FightActionEnum.WonFight, desc: new List<string>() { $"Won with HP: {fc.GetPlayerHP()}" });
                        res.Add(fc);

                        break;
                    case FightStatus.Lost:
                        fc.AddHistory(FightActionEnum.LostFight, desc: new List<string>() { $"Lost with enemy hp: {fc.GetEnemyHP()}" });

                        res.Add(fc);

                        break;
                    default:
                        throw new Exception("Other status");
                }
            }

            return res;
        }

        private void ApplyAction(Fight fight, FightAction action)
        {
            switch (action.FightActionType)
            {
                case FightActionEnum.PlayCard:
                    var copiedCard = fight.FindIdenticalCard(action.Card);
                    fight.PlayCard(copiedCard);
                    break;
                case FightActionEnum.EndTurn:
                    fight.EndTurn();
                    fight.EnemyMove();
                    fight.StartTurn();
                    break;
                case FightActionEnum.Potion:
                    _Player.DrinkPotion(fight, action.Potion, _Enemy);
                    break;
                default:
                    break;
            }
        }
    }
}