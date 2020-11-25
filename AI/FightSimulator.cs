using System;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    /// <summary>
    /// Should I iterate over all possibilities as deep as possible?
    /// How much will things actually blow up?  With fully unique deck (no repeat cards)
    /// 
    /// How about just normal play and do lots of sims vs a dumb simple enemy without.
    /// </summary>
    public class FightSimulator
    {
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private Deck _Deck { get; set; }
        private List<CardInstance> _CIs { get; set; }

        /// <summary>
        /// Records the actual state of the fight and runs sims to make a good decision.
        /// </summary>
        public FightSimulator(List<CardInstance> cis, Enemy enemy, Player player)
        {
            _CIs = cis;
            _Enemy = enemy;
            _Player = player;
            Sim();
        }

        ///would really be nice to iterate over all possible shufflings but i'll just do random shuffling.
        private void Sim()
        {
            var gc = new GameContext();
            var fight = new Fight(_CIs, gc, _Player, new List<Enemy>() { _Enemy });

            while (true)
            {

                while (ActionsPossible())
                {
                    var action = GetNextAction(fight);
                    if (action == null)
                    {
                        Console.WriteLine("No action.");
                        break;
                    }
                    fight.PlayCard(action.CardToPlay, _Player, _Enemy);
                }

                Console.WriteLine("Done with turn.");

                var enemyCi = _Enemy.GetAction();
                fight.EnemyPlayCard(enemyCi.Attack, _Enemy, _Player, _Player, _Enemy);
            }
        }

        private bool ActionsPossible()
        {
            return _Player.Energy > 0;
        }

        private CardChoice GetNextAction(Fight fight)
        {
            var scores = new Dictionary<CardChoice, double>();
            var hand = fight.GetHand();
            foreach (var ci in hand)
            {
                var dec = new CardChoice(ci);
                var score = SimFromHere(fight, ci);
                scores[dec] = score;
            }

            var best = scores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            Console.WriteLine($"Chose: {best}");
            return best;
        }

        public double SimFromHere(Fight fight, CardInstance ci)
        {
            return 4.0f;
        }
    }



    public class SimResult
    {
        public bool Dead { get; set; }
        public int HP { get; set; }
    }

    public class CardChoice
    {
        public CardInstance CardToPlay { get; set; }

        public CardChoice(CardInstance cardToPlay)
        {
            CardToPlay = cardToPlay;
        }

        public override string ToString()
        {
            return $"{CardToPlay}";
        }
    }
}
