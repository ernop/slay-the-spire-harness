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

        public FightStatus Status { get; set; }

        public Fight(List<CardInstance> initialCis, GameContext gameContext, Player player, List<Enemy> enemies, bool preserveOrder = false)
        {
            _GameContext = gameContext;
            _Enemies = enemies;
            _Player = player;
            _Deck = new Deck(initialCis, preserveOrder);
            Status = FightStatus.Ongoing;
        }

        /// <summary>
        /// for testing
        /// </summary>
        public List<CardInstance> GetDrawPile()
        {
            return _Deck.DrawPile;
        }

        internal void FirstTurnStarts(int? drawCount = null)
        {
            FirstTurnCalled = true;
            drawCount = drawCount ?? _Player.GetDrawAmount();
            _Deck.TurnEnds();
            _Deck.NextTurnStarts(drawCount.Value);
            _Player.Energy = _Player.MaxEnergy();

            //var firstTurnPlayerEf = new EffectSet();
            //var firstTurnEnemyEf = new EffectSet();
            var relicEf = new EffectSet();

            //foreach (var si in _Player.StatusInstances)
            //{
            //    si.FirstTurn(_Player, firstTurnPlayerEf);
            //}
            //foreach (var si in _Enemies[0].StatusInstances)
            //{
            //    si.FirstTurn(_Enemies[0], firstTurnEnemyEf);
            //}
            foreach (var relic in _Player.Relics)
            {
                relic.FirstRoundStarts(_Player, _Enemies[0], relicEf);
            }

            //ApplyEffectSet(firstTurnPlayerEf, _Player, _Enemies[0]);
            //ApplyEffectSet(firstTurnEnemyEf, _Enemies[0], _Player);
            ApplyEffectSet(relicEf, _Player, _Enemies[0]);
        }

        private bool FirstTurnCalled = false;

        /// <summary>
        /// Is nextturn actually equivalent to first turn?  This is messing up some tests.
        /// </summary>
        public void NextTurn(int? drawCount = null)
        {
            if (!FirstTurnCalled)
            {
                throw new Exception("Calling nextTurn without firstturn started.");
            }
            drawCount = drawCount ?? _Player.GetDrawAmount();
            _Deck.TurnEnds();
            _Deck.NextTurnStarts(drawCount.Value);
            _Player.Energy = _Player.MaxEnergy();

            //TODO this is affected by calipers, barricade, etc.
            _Player.Block = 0;
            var endTurnPlayerEf = new EffectSet();
            var endTurnEnemyEf = new EffectSet();

            var relicEf = new EffectSet();

            foreach (var si in _Player.StatusInstances)
            {
                si.EndTurn(_Player, endTurnPlayerEf);
            }
            _Player.StatusInstances = _Player.StatusInstances.Where(el => el.Duration != 0).ToList();

            foreach (var si in _Enemies[0].StatusInstances)
            {
                si.EndTurn(_Enemies[0], endTurnEnemyEf);
            }
            _Enemies[0].StatusInstances = _Enemies[0].StatusInstances.Where(el => el.Duration != 0).ToList();

            foreach (var relic in _Player.Relics)
            {
                relic.EndTurn(_Player, _Enemies[0], relicEf);
            }

            ApplyEffectSet(endTurnPlayerEf, _Player, _Enemies[0]);
            ApplyEffectSet(endTurnEnemyEf, _Enemies[0], _Player);
            ApplyEffectSet(relicEf, _Player, _Enemies[0]);
        }

        public List<CardInstance> GetExhaustPile()
        {
            return _Deck.ExhaustPile;
        }

        public List<CardInstance> GetHand()
        {
            return _Deck.Hand;
        }

        /// <summary>
        /// Do nothing right now.
        /// </summary>
        public void Died(Entity entity)
        {
            switch (entity.EntityType)
            {
                case EntityType.Enemy:
                    Status = FightStatus.Won;
                    WinFight();
                    break;
                case EntityType.Player:
                    Status = FightStatus.Lost;
                    break;
                default:
                    break;
            };
        }

        private void WinFight()
        {
            foreach (var relic in _Player.Relics)
            {
                var ef = new EffectSet();
                relic.EndFight(ef);
                ApplyEffectSet(ef, _Player, _Player);
            }
        }

        /// <summary>
        /// From monster POV, player is the enemy.
        /// </summary>
        public void PlayCard(CardInstance cardInstance, Player player, Enemy enemy, List<CardInstance> cardTargets = null, bool forceExhaust = false, bool newCard = false)
        {
            if (forceExhaust)
            {
                cardInstance.OverrideExhaust = true;
            }
            if (newCard)
            {
                //don't do checks; definitely playable, etc.
            }
            else
            {
                if (!cardInstance.Playable(_Deck.Hand))
                {
                    throw new Exception("Trying to play unplayable card.");
                }
                if (cardInstance.EnergyCost() > player.Energy)
                {
                    throw new Exception("Trying to play too expensive card");
                }
                player.Energy -= cardInstance.EnergyCost();
                _Deck.BeforePlayingCard(cardInstance);
            }

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
            cardInstance.Play(ef, player, target, cardTargets, _Deck);

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

            foreach (var relic in player.Relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: player, enemy: enemy);
            }

            //relic effects apply first.

            ApplyEffectSet(ef, _Player, target);

            //make sure to apply card effects before putting the just played card into discard, so it can't be drawn again by its own action.
            _Deck.AfterPlayingCard(cardInstance);
        }

        public void ApplyEffectSet(EffectSet ef, Entity source, Entity target)
        {
            //TODO not clear if this order is the most sensible really or not.
            //complex effects like afterImage + playing defend with neg dex.
            if (source == target)
            {
                var combined = Combine(ef.SourceEffect, ef.TargetEffect);
                GainBlock(source, combined);
                ReceiveDamage(source, combined, out bool alive);
                if (!alive)
                {
                    Died(source);
                }
                ApplyStatus(source, combined.Status);
            }
            else
            {
                GainBlock(source, ef.SourceEffect);
                ReceiveDamage(target, ef.TargetEffect, out bool alive);
                if (!alive)
                {
                    Died(target);
                }

                GainBlock(target, ef.TargetEffect);
                ReceiveDamage(source, ef.SourceEffect, out bool alive2);
                if (!alive2)
                {
                    Died(source);
                }

                ApplyStatus(source, ef.SourceEffect.Status);
                ApplyStatus(target, ef.TargetEffect.Status);


            }

            foreach (var f in ef.DeckEffect)
            {
                f.Invoke(_Deck);
            }

            foreach (var f in ef.PlayerEffect)
            {
                f.Invoke(_Player);
            }

            foreach (var f in ef.FightEffects)
            {
                f.Action.Invoke(this, _Deck, _Player, _Enemies[0]);
            }

            //We resolve damage after dealing with statuses the player may just have gained.
            //i.e. we don't apply pen nib to the player til after attack is resolved.
        }
        public void ApplyStatus(Entity entity, List<StatusInstance> sis)
        {
            foreach (var status in sis)
            {
                entity.ApplyStatus(status);
            }
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

        internal bool EnemyDead()
        {
            return _Enemies[0].HP <= 0;
        }

        internal bool PlayerDead()
        {
            return _Player.HP <= 0;
        }

        internal void ApplyEnemyAction(EnemyAction enemyAction, Enemy enemy, Player player)
        {
            if (enemyAction == null)
            {
                return;
            }
            if (enemyAction.Buffs != null)
            {
                ApplyStatus(enemy, enemyAction.Buffs);
            }
            if (enemyAction.Attack != null)
            {
                EnemyPlayCard(enemyAction.Attack, _Enemies[0], _Player, _Player, _Enemies[0]);
            }
            if (enemyAction.PlayerStatusAttack != null)
            {
                ApplyStatus(player, enemyAction.Buffs);
            }
        }

        public void EnemyPlayCard(EnemyAttack enemyAttack, Entity source, Entity target, Player player, Enemy enemy)
        {
            var ef = new EffectSet();

            var cardInstance = new CardInstance(enemyAttack, 0);
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
            foreach (var relic in player.Relics)
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

    }
}
