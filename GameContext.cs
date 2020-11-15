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

        public void GainBlock(Entity entity, IndividualEffect ef)
        {
            var val = ef.InitialBlock;
            foreach (var prog in ef.BlockAdjustments)
            {
                val = prog.Fun(val,entity);
            }
            if (val > 0)
            {
                entity.Block += val;
            }
        }

        public void ReceiveDamage(Entity entity, IndividualEffect ef)
        {
            //We don't actually want to 
            if (ef.InitialDamage!= null)
            {
                var val = ef.InitialDamage;
                foreach (var prog in ef.DamageAdjustments)
                {
                    val = prog.Fun(val);
                }


                foreach (var el in val)
                {
                    var elCopy = el;
                    if (elCopy > 0)
                    {
                        //handle block here.
                        if (entity.Block > 0)
                        {
                            if (elCopy > entity.Block)
                            {
                                elCopy = elCopy - entity.Block;
                                entity.Block = 0;
                            }
                            else
                            {
                                entity.Block -= el;
                                elCopy = 0;
                            }
                        }
                        if (elCopy > 0)
                        {
                            entity.ApplyDamage(elCopy);
                        }
                    }
                }
            }
        }

        public IndividualEffect Combine(IndividualEffect ef1, IndividualEffect ef2)
        {
            var combined = new IndividualEffect();
            combined.InitialBlock = ef1.InitialBlock + ef2.InitialBlock;
            var res = new List<int>();
            if (ef1.InitialDamage != null || ef2.InitialDamage != null)
            {
                foreach (var ls in new List<IEnumerable<int>>() { ef1.InitialDamage, ef2.InitialDamage })
                {
                    foreach (var el in ls)
                    {
                        res.Add(el);
                    }
                }
                combined.InitialDamage = res;
            }

            combined.DamageAdjustments.AddRange(ef1.DamageAdjustments);
            combined.DamageAdjustments.AddRange(ef2.DamageAdjustments);
            combined.BlockAdjustments.AddRange(ef1.BlockAdjustments);
            combined.BlockAdjustments.AddRange(ef2.BlockAdjustments);
            combined.Status.AddRange(ef1.Status);
            combined.Status.AddRange(ef2.Status);
            return combined;
        }

        public void ApplyStatus(Entity entity, IndividualEffect ef)
        {
            foreach (var status in ef.Status)
            {
                entity.ApplyStatus(status);
            }
        }


        public void EnemyPlayCard(CardInstance cardInstance, Entity source, Entity target, Player player, Enemy enemy)
        {
            var ef = new EffectSet();
            cardInstance.Apply(ef, player, enemy, cardInstance.UpgradeCount);

            foreach (var si in source.StatusInstances)
            {
                var statusIsTargeted = enemy == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, false);
            }
            foreach (var si in target.StatusInstances)
            {
                var statusIsTargeted = player == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, false);
            }

            //this won't be right for Torii for example.
            foreach (var relic in player.relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: player, enemy: enemy);
            }

            GainBlock(player, ef.TargetEffect);
            GainBlock(enemy, ef.SourceEffect);
            ReceiveDamage(player, ef.TargetEffect);
            ReceiveDamage(enemy, ef.SourceEffect);
        }

        /// <summary>
        /// From monster POV, player is the enemy.
        /// </summary>
        public void PlayCard(CardInstance cardInstance, Player player, Enemy enemy)
        {
            Entity target;
            switch (cardInstance.Card.TargetType)
            {
                case TargetType.Player:
                    target = player;
                    break;
                case TargetType.Enemy:
                    target = enemy;
                    break;
                default:
                    throw new NotImplementedException();
            }

            //set the initial effect, or status.
            var ef = new EffectSet();
            cardInstance.Apply(ef, player, target, cardInstance.UpgradeCount);

            //generate an effect containing all the changes that will happen.
            foreach (var si in player.StatusInstances)
            {
                var statusIsTargeted = player == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, true);
            }
            foreach (var si in enemy.StatusInstances)
            {
                var statusIsTargeted = enemy == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, true);
            }

            foreach (var relic in player.relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: player, enemy: enemy);
            }

            //TODO not clear if this order is the most sensible really or not.
            //complex effects like afterImage + playing defend with neg dex.
            if (player == target)
            {
                var combined = Combine(ef.SourceEffect, ef.TargetEffect);
                GainBlock(player, combined);
                ReceiveDamage(player, combined);
                ApplyStatus(player, combined);
            }
            else
            {
                GainBlock(player, ef.SourceEffect);
                ReceiveDamage(target, ef.TargetEffect);

                GainBlock(target, ef.TargetEffect);
                ReceiveDamage(player, ef.SourceEffect);

                ApplyStatus(player, ef.SourceEffect);
                ApplyStatus(target, ef.TargetEffect);
            }

            //We resolve damage after dealing with statuses the player may just have gained.
            //i.e. we don't apply pen nib to the player til after attack is resolved.
        }
    }
}