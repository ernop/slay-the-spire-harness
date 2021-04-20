using System;
using System.Collections.Generic;
using System.Linq;

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
            SetRandom(164);
            var relics = GetRandomRelics(3);
            var potions = GetRandomPotions(2);
            var enemyHp = 100;
            _Player = new Player(hp: 80, relics: relics, potions: potions);
            _Enemy = new Cultist(enemyHp);
            //var hand = gsl("Strike", "Strike", "Strike", "Strike", "Strike", "Defend", "Defend", "Defend", "Defend", "Bash","WildStrike","PommelStrike+");
            var cis = GetRandomCards(10);
            cis.Add(Helpers.GetCi("Armaments"));
            cis.Add(Helpers.GetCi("Armaments"));
            cis.Add(Helpers.GetCi("Armaments+"));
            cis.Add(Helpers.GetCi("TrueGrit"));
            Console.WriteLine("Deck: " + SJ(separator: ' ', cis.OrderBy(el => el.Card.Name)));
            var deck = new Deck(cis);
            deck.InteractiveContext = true;
            _Fight = new Fight(deck, _Player, _Enemy);
            _Root = new FightNode(_Fight);
            _Current = _Root;
            _Fight.FightNode = _Root;
            _Fight.StartFight();
        }

        private void DisplayStatus(FightNode current)
        {
            var fa = _Current.Fight.FightAction?.GetList();

            if (fa != null)
            {
                Console.WriteLine("===== Last Action:");
                foreach (var part in fa)
                {
                    Console.WriteLine($"\t{part}");
                }
            }
            var status = _Current.Fight.Status;
            Console.WriteLine($"\nTurn:{ _Current.Fight.TurnNumber,2} - {status}");
            Console.WriteLine(_Current.Fight._Player.Details());
            Console.WriteLine(_Current.Fight._Enemies[0].Details());
            Console.WriteLine($"\tEnergy: {_Current.Fight._Player.Energy}/{_Current.Fight._Player.MaxEnergy()}");
        }

        public void Start()
        {
            while (true)
            {
                DisplayStatus(_Current);

                var ii = 0;
                var actionMap = new Dictionary<int, FightAction>() { };

                var actions = _Current.Fight.GetAllActions(includeUnplayable: true);

                FightAction action;
                if (actions.Count == 1)
                {
                    action = actions[0];
                }
                else
                {
                    var orderedActions = actions
                        .OrderBy(el => el.CardInstance == null)
                        .ThenByDescending(el => el.Playable)
                        .ThenBy(el => el.CardInstance?.Card?.Name)
                        .ThenBy(el => el.CardInstance?.EnergyCost());
                    foreach (var a in orderedActions)
                    {
                        var cost = "";
                        if (a.FightActionType == FightActionEnum.PlayCard)
                        {
                            ///no indexer but still show the energy cost.
                            if (a.Playable)
                            {
                                cost = $" E: {a.CardInstance.EnergyCost()}";
                            }
                            else
                            {
                                cost = $" E: -";
                            }
                        }
                        if (a.Playable)
                        {
                            Console.WriteLine($"\t{ii}: {a}{cost}");
                            actionMap[ii] = a;
                            ii++;
                        }
                        else
                        {
                            Console.WriteLine($"\t-: {a}{cost}");
                        }
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
                    action = actionMap[num];
                }
                _Current = _Current.ApplyAction(action);
                if (_Current.Fight.Status != FightStatus.Ongoing)
                {
                    DisplayStatus(_Current);
                    break;
                }

            }
        }
    }
}
