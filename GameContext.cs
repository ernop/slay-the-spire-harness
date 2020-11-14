﻿using System;
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
                si.Apply(ef);
            }
            foreach (var si in enemy.StatusInstances)
            {
                si.Apply(ef);
            }
            if (enemyList != null)
            {
                foreach (var oneEnemy in enemyList)
                {
                    foreach (var si in oneEnemy.StatusInstances)
                    {
                        si.Apply(ef);
                    }
                }
            }

            //apply all the changes.

            if (ef.PlayerStatus != null)
            {
                foreach (var status in ef.PlayerStatus)
                {
                    player.ApplyStatus(status);
                }
            }
            Console.WriteLine($"Player now: {player}");
            
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

            if (ef.EnemyBlock != null)
            {
                enemy.Block += ef.EnemyBlock(0);
            }

            if (ef.EnemyReceivesDamage != null && ef.EnemyReceivesDamage.Any())
            {
                var val = 0;
                foreach (var fcn in ef.EnemyReceivesDamage)
                {
                    val = fcn(val);
                }
                if (val > 0)
                {
                    enemy.ApplyDamage(val);
                }
            }

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