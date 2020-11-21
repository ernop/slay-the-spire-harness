using System;
using System.Collections.Generic;

using static StS.Helpers;

namespace StS
{
    public class FightTestCase
    {
        public string TestName { get; set; }
        public string EnemyName { get; set; } = "Enemy";
        public List<CardInstance> CardsToPlay = new List<CardInstance>();
        public int PlayerBlock { get; set; }
        public int EnemyBlock { get; set; }
        public int EnemyHp { get; set; }
        public int PlayerHp { get; set; }
        public int? PlayerEnergy { get; set; }
        public int FinalEnemyHp { get; set; }
        public int FinalEnemyBlock { get; set; }
        public int FinalPlayerHp { get; set; }
        public int FinalPlayerBlock { get; set; }
        public int? FinalEnergy { get; set; }
        public List<StatusInstance> PlayerStatuses { get; set; }
        public List<StatusInstance> EnemyStatuses { get; set; }
        public List<StatusInstance> PlayerFinalStatuses { get; set; }
        public List<StatusInstance> EnemyFinalStatuses { get; set; }
        public List<CardInstance> EnemyCards { get; set; } = new List<CardInstance>();
        public List<Relic> Relics { get; set; }


        public Player SetupPlayer()
        {
            var player = new Player(hpMax: PlayerHp, hp: PlayerHp);
            if (Relics != null)
            {
                player.Relics = Relics;
                foreach (var relic in player.Relics)
                {
                    relic.Player = player;
                }
            }

            player.StatusInstances = PlayerStatuses;
            player.Block = PlayerBlock;
            return player;
        }

        public List<Enemy> SetupEnemies()
        {
            var enemy = new Enemy(EnemyName, EnemyHp, EnemyHp);
            enemy.Block = EnemyBlock;

            if (EnemyStatuses != null)
            {
                enemy.StatusInstances = EnemyStatuses;
            }

            var enemies = new List<Enemy>() { enemy };
            return enemies;
        }

        public void Run()
        {
            Console.WriteLine($"====Testcase {TestName}");

            var gc = new GameContext();
            var player = SetupPlayer();
            gc.Player = player;


            var enemies = SetupEnemies();
            var enemy = enemies[0];

            var fight = new Fight(CardsToPlay, gc, player, enemies);

            //todo player.GetDrawAmount()
            fight.NextTurn(5);

            if (PlayerEnergy.HasValue)
            {
                player.Energy = PlayerEnergy.Value;
            }

            Console.WriteLine($"Enemy: {enemy}");
            Console.WriteLine($"Player: {player}");

            foreach (var ci in CardsToPlay)
            {
                fight.PlayCard(ci, player, enemy);

                Console.WriteLine($"Player:{player}");
                Console.WriteLine($"Enemy:{enemy}");
            }

            foreach (var ci in EnemyCards)
            {
                //For now no targeting for enemy cards.
                fight.EnemyPlayCard(ci, enemy, player, player, enemy);

                Console.WriteLine($"Player:{player}");
                Console.WriteLine($"Enemy:{enemy}");
            }

            if (Helpers.PrintDetails)
            {
                Console.WriteLine("\tFinal State:");
                Console.WriteLine("\t" + player);
                Console.WriteLine("\t" + enemy);
            }
            if (player.HP != FinalPlayerHp)
            {
                throw new Exception($"{TestName} Player hp={player.HP} expected to be={FinalPlayerHp}");
            }
            if (enemy.HP != FinalEnemyHp)
            {
                throw new Exception($"{TestName} Enemy hp={enemy.HP} expected to be={FinalEnemyHp}");
            }
            if (player.Block != FinalPlayerBlock)
            {
                throw new Exception($"{TestName} PlayerBlock expected:{FinalPlayerBlock} actual:{player.Block}");
            }
            if (PlayerFinalStatuses.Count > 0)
            {
                if (!CompareStatuses(PlayerFinalStatuses, player.StatusInstances, out var error))
                {
                    throw new Exception($"bad statuses. {error}");
                }
            }
            if (EnemyFinalStatuses.Count > 0)
            {
                if (!CompareStatuses(EnemyFinalStatuses, enemy.StatusInstances, out var error))
                {
                    throw new Exception($"bad statuses. {error}");
                }
            }

            if (FinalEnergy.HasValue)
            {
                if (player.Energy != FinalEnergy.Value)
                {
                    throw new Exception($"Expected energy: {FinalEnergy.Value} actual: {player.Energy}");
                }
            }

            Console.WriteLine($"====Testcase {TestName} is valid\n");
        }

        public override string ToString()
        {
            return $"TC:{TestName}";
        }
    }
}