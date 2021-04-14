using System;
using System.Collections.Generic;

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
            SetRandom(13);
            var relics = GetRandomRelics(3);
            var potions = GetRandomPotions(2);
            var enemyHp = 100;
            _Player = new Player(hp: 80, relics: relics, potions: potions);
            _Enemy = new Cultist(enemyHp);
            //var hand = gsl("Strike", "Strike", "Strike", "Strike", "Strike", "Defend", "Defend", "Defend", "Defend", "Bash","WildStrike","PommelStrike+");
            var cis = GetRandomCards(20);
            Console.WriteLine("Deck: " + SJ(cis));
            var deck = new Deck(cis);
            _Fight = new Fight(deck, _Player, _Enemy);
            _Root = new FightNode(_Fight);
            _Current = _Root;
            _Fight.FightNode = _Root;
            _Fight.StartFight();
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine(_Current);
                Console.WriteLine(_Current.Fight._Player.Details());
                Console.WriteLine(_Current.Fight._Enemies[0].Details());
                Console.WriteLine($"What should I do? Energy{_Current.Fight._Player.Energy}");
                Console.WriteLine($"Hand: {SJ(_Current.Fight.GetHand)}");
                var ii = 0;
                var actionMap = new Dictionary<int, FightAction>() { };

                //problem: this hides duplicates, etc.
                //should create a method to display all cards.
                var actions = _Current.Fight.GetAllActions();
                foreach (var a in actions)
                {
                    var cost = "";
                    if (a.FightActionType == FightActionEnum.PlayCard)
                    {
                        cost = $" E: {a.Card.EnergyCost()}";
                    }
                    Console.WriteLine($"{ii}: {a}{cost}");
                    actionMap[ii] = a;
                    ii++;
                }

                var parsed = Int32.TryParse(Console.ReadLine(), out int num);
                if (!parsed)
                {
                    continue;
                }
                if (!actionMap.ContainsKey(num))
                {
                    continue;
                }
                var action = actionMap[num];
                _Current = _Current.ApplyAction(action);
                if (_Current.Fight.Status != FightStatus.Ongoing)
                {
                    Console.WriteLine(_Current.FightAction);
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
