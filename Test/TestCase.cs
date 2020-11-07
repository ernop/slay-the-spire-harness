using System;
using System.Collections.Generic;

namespace StS
{
    public class TestCase
    {
        public List<CardInstance> CardsToPlay = new List<CardInstance>();
        public int EnemyHP { get; set; }
        public int PlayerHP { get; set; }
        public int PostCardsEnemyHp { get; set; }
        public int PostCardsPlayerHp { get; set; }
        public string TestName { get; set; }
        public string EnemyName { get; set; } = "Enemy";

        public void Run()
        {
            Console.WriteLine($"====Testcase {TestName}");
            var gc = new GameContext();
            var player = new Player(gc, PlayerHP, PlayerHP);
            var enemy = new Enemy(EnemyName, gc, EnemyHP, EnemyHP);
            foreach (var ci in CardsToPlay)
            {
                ci.Play(player, enemy, null);
            }
            if (Helpers.PrintDetails)
            {
                Console.WriteLine("\tFinal State:");
                Console.WriteLine("\t" + player);
                Console.WriteLine("\t" + enemy);
            }
            if (player.HP != PostCardsPlayerHp)
            {
                throw new Exception($"{TestName} Player hp={player.HP} expected to be={PostCardsPlayerHp}");
            }
            if (enemy.HP != PostCardsEnemyHp)
            {
                throw new Exception($"{TestName} Enemy hp={enemy.HP} expected to be={PostCardsEnemyHp}");
            }
            Console.WriteLine($"====Testcase {TestName} is valid\n");
        }
    }
}