using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public partial class GameContext
    {

        List<Enemy> Enemies = new List<Enemy>();
        Player Player { get; }

        public GameContext()
        {
            Player = new Player(this, 100, 100);
        }

        
        public void Died(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Enemy:
                    Enemies.Remove((Enemy)entity);
                    break;
                case EntityType.Player:
                    GameOver();
                    break;
                default:
                    break;
            };
        }

        public void GameOver()
        {
            Console.WriteLine("Died");
        }

        public void PlayCard(CardInstance cardInstance, Player player, Enemy enemy, List<Enemy> enemyList)
        {
            var ef = cardInstance.Apply(player, enemy, enemyList, cardInstance.UpgradeCount);

            //generate an effect containing all the changes that will happen.
            foreach (var si in player.StatusInstances)
            {
                si.Apply(cardInstance.Card, ef);
            }
            foreach (var si in enemy.StatusInstances)
            {
                si.Apply(cardInstance.Card, ef);
            }
            if (enemyList != null)
            {
                foreach (var oneEnemy in enemyList)
                {
                    foreach (var si in oneEnemy.StatusInstances)
                    {
                        si.Apply(cardInstance.Card, ef);
                    }
                }
            }
            foreach (var relic in player.relics)
            {
                relic.CardPlayed(cardInstance.Card, ef);
            }
            
            if (ef.PlayerGainBlock != null && ef.PlayerGainBlock.Any())
            {
                var val = 0;
                foreach (var fcn in ef.PlayerGainBlock)
                {
                    val = fcn(val);
                }
                if (val > 0)
                {
                    player.Block += val;
                }
            }

            if (ef.PlayerReceivesDamage != null && ef.PlayerReceivesDamage.Any())
            {
                var val = 0;
                foreach (var fcn in ef.PlayerReceivesDamage)
                {
                    val = fcn(val);
                }
                if (val > 0)
                {
                    player.ApplyDamage(val);
                }
            }            


            if (ef.EnemyReceivesDamage != null && ef.EnemyReceivesDamage.Any())
            {
                var damageAmount = 0;
                foreach (var fcn in ef.EnemyReceivesDamage)
                {
                    damageAmount = fcn(damageAmount);
                }
                if (damageAmount > 0)
                {
                    //handle block here.
                    if (enemy.Block > 0)
                    {
                        if (damageAmount > enemy.Block)
                        {
                            damageAmount = damageAmount - enemy.Block;
                            enemy.Block = 0;
                        }
                        else
                        {
                            enemy.Block -= damageAmount;
                            damageAmount = 0;
                        }
                    }
                    if (damageAmount > 0)
                    {
                        enemy.ApplyDamage(damageAmount);
                    }
                }
            }

            //Aggressive triggers block based on new status.

            if (ef.EnemyGainsBlock != null && ef.EnemyGainsBlock.Any())
            {
                var val = 0;
                foreach (var fcn in ef.EnemyGainsBlock)
                {
                    val = fcn(val);
                }
                if (val > 0)
                {
                    enemy.ApplyBlock(val);
                }
            }

            //We resolve damage after dealing with statuses the player may just have gained.
            //i.e. we don't apply pen nib to the player til after attack is resolved.

            if (ef.PlayerStatus != null && ef.PlayerStatus.Any())
            {
                foreach (var status in ef.PlayerStatus)
                {
                    player.ApplyStatus(status);
                }
            }
            Console.WriteLine($"Player:{player}");
            Console.WriteLine($"Enemy:{enemy}");

            if (ef.EnemyStatus != null && ef.EnemyStatus.Any())
            {
                foreach (var status in ef.EnemyStatus)
                {
                    enemy.ApplyStatus(status);
                }
            }
        }
    }
}