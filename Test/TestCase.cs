using System;
using System.Collections.Generic;

namespace StS
{
    public class TestCase
    {
        public string TestName { get; set; }
        public string EnemyName { get; set; } = "Enemy";
        public List<CardInstance> CardsToPlay = new List<CardInstance>();
        public int PlayerBlock { get; set; }
        public int EnemyBlock { get; set; }
        public int EnemyHp { get; set; }
        public int PlayerHp { get; set; }
        public List<StatusInstance> PlayerStatuses { get; set; }
        public List<StatusInstance> EnemyStatuses { get; set; }
        public int FinalEnemyHp { get; set; }
        public int FinalEnemyBlock { get; set; }
        public int FinalPlayerHp { get; set; }
        public int FinalPlayerBlock { get; set; }
        public List<CardInstance> EnemyCards { get; set; } = new List<CardInstance>();
        public List<Relic> Relics { get; set; }
        
        public void Run()
        {
            Console.WriteLine($"====Testcase {TestName}");
            var gc = new GameContext();
            var player = new Player(gc, PlayerHp, PlayerHp);
            
            if (Relics != null)
            {
                player.relics = Relics;
                foreach (var relic in player.relics)
                {
                    relic.Player = player;
                }
            }
            player.StatusInstances = PlayerStatuses;
            player.Block = PlayerBlock;

            var enemy = new Enemy(EnemyName, gc, EnemyHp, EnemyHp);
            enemy.Block = EnemyBlock;

            if (EnemyStatuses != null)
            {
                enemy.StatusInstances = EnemyStatuses;
            }

            Console.WriteLine($"Enemy: {enemy}");
            Console.WriteLine($"Player: {player}");

            foreach (var ci in CardsToPlay)
            {
                gc.PlayCard(ci, player, enemy);

                Console.WriteLine($"Player:{player}");
                Console.WriteLine($"Enemy:{enemy}");
            }

            foreach (var ci in EnemyCards)
            {
                //For now no targeting for enemy cards.
                gc.EnemyPlayCard(ci, enemy, player, player, enemy);

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
                throw new Exception($"PlayerBlock expected:{FinalPlayerBlock} actual:{player.Block}");
            }

            Console.WriteLine($"====Testcase {TestName} is valid\n");
        }

        public override string ToString()
        {
            return $"TC:{TestName}";
        }
    }
}