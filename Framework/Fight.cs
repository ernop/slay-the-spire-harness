using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{
    public class Fight
    {
        private GameContext _GameContext { get; set; }
        private List<Enemy> _Enemies { get; set; }
        private Player _Player { get; set; }
        private Deck _Deck { get; set; }

        public Fight(List<CardInstance> initialCis, GameContext gameContext, Player player, List<Enemy> enemies, bool preserveOrder = false)
        {
            _GameContext = gameContext;
            _Enemies = enemies;
            _Player = player;
            _Deck = new Deck(initialCis, preserveOrder);
        }
        public void NextTurn(int drawCount)
        {
            _Deck.NextTurn(drawCount);
        }

        public void Play(CardInstance ci)
        {
            PlayCard(ci, _Player, _Enemies[0]);
        }
        public List<CardInstance> GetHand()
        {
            return _Deck.Hand;
        }
        public void Died(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Enemy:
                    _Enemies.Remove((Enemy)entity);
                    break;
                case EntityType.Player:
                    _GameContext.GameOver();
                    break;
                default:
                    break;
            };
        }
        /// <summary>
        /// From monster POV, player is the enemy.
        /// </summary>
        public void PlayCard(CardInstance cardInstance, Player player, Enemy enemy)
        {
            _Deck.PlayingCard(cardInstance);
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
            cardInstance.Play(ef, player, target);

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

            //relic effects apply first.
            foreach (var f in ef.HandEffect)
            {
                f.Invoke(_Deck.Hand);
            }

            //TODO not clear if this order is the most sensible really or not.
            //complex effects like afterImage + playing defend with neg dex.
            if (player == target)
            {
                var combined = Combine(ef.SourceEffect, ef.TargetEffect);
                GainBlock(player, combined);
                ReceiveDamage(player, combined, out bool alive);
                if (!alive)
                {
                    Died(player);
                }
                ApplyStatus(player, combined);
            }
            else
            {
                GainBlock(player, ef.SourceEffect);
                ReceiveDamage(target, ef.TargetEffect, out bool alive);
                if (!alive)
                {
                    Died(target);
                }

                GainBlock(target, ef.TargetEffect);
                ReceiveDamage(player, ef.SourceEffect, out bool alive2);
                if (!alive2)
                {
                    Died(player);
                }

                ApplyStatus(player, ef.SourceEffect);
                ApplyStatus(target, ef.TargetEffect);
            }

            //We resolve damage after dealing with statuses the player may just have gained.
            //i.e. we don't apply pen nib to the player til after attack is resolved.
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
            cardInstance.Play(ef, player, enemy);

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
            ReceiveDamage(player, ef.TargetEffect, out bool alive);
            if (!alive) Died(player);
            ReceiveDamage(enemy, ef.SourceEffect, out bool alive2);
            if (!alive2) Died(enemy);
        }
        public void GainBlock(Entity entity, IndividualEffect ef)
        {
            var val = ef.InitialBlock;
            foreach (var prog in ef.BlockAdjustments)
            {
                val = prog.Fun(val, entity);
            }
            if (val > 0)
            {
                entity.Block += val;
            }
        }
        public void ReceiveDamage(Entity entity, IndividualEffect ef, out bool alive)
        {
            alive = true;
            //We don't actually want to 
            if (ef.InitialDamage != null)
            {
                var val = ef.InitialDamage.Select(el => (double)el);
                foreach (var prog in ef.DamageAdjustments)
                {
                    val = prog.Fun(val);
                }

                var usingVal = val.Select(el => (int)Math.Floor(el));
                foreach (var el in usingVal)
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
                            alive = entity.ApplyDamage(elCopy);
                        }
                    }
                }
            }
        }
    }
}
