using System;
using System.Collections.Generic;
using System.Text;

using static StS.Helpers;

namespace StS
{
    public class StsConsole
    {
        private Fight _Fight { get; set; }
        private Player _Player { get; set; }
        private Enemy _Enemy { get; set; }
        private FightNode _Root { get; set; }
        private FightNode _Current { get; set; }

        public StsConsole()
        {
            Setup();
            Start();
        }
        
        public void Setup()
        {
            _Player = new Player(hp: 80);
            _Enemy = new Cultist(47);
            var hand = Gsl("Strike", "Strike", "Strike", "Strike", "Strike", "Defend", "Defend", "Defend", "Defend", "Bash");
            var deck = new Deck(hand, Gsl(), Gsl(), Gsl());
            _Fight = new Fight(deck, _Player, _Enemy);
            _Root = new FightNode(_Fight);
            _Current = _Root;
            _Fight.FightNode = _Root;
        }

        public void Start()
        {
            while(true)
            {
                Console.WriteLine(_Current);
                Console.WriteLine($"What should I do? Energy{_Current.Fight._Player.Energy}");
                var ii = 0;
                var actionMap = new Dictionary<int, FightAction>() { };
                var actions = _Current.Fight.GetAllActions();
                foreach (var a in actions)
                {
                    Console.WriteLine($"{ii}: {a}");
                    actionMap[ii] = a;
                    ii++;
                }

                var parsed = Int32.TryParse(Console.ReadLine(), out int num);
                if (!parsed)
                {
                    continue;
                }
                var action = actionMap[num];
                _Current = _Current.ApplyAction(action);
                if (_Current.Fight.Status != FightStatus.Ongoing)
                {
                    Console.Write("End");
                    break;
                }
            }
        }

        public enum Command
        {
            //meta
            AddCard,
            RemoveCard,
            GainPotion,

            //normal
            PlayCard,
            DrinkPotion,
            EndTurn,
        }
    }
}
