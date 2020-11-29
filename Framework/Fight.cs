using System;
using System.Collections.Generic;
using System.Linq;

using static StS.Helpers;

namespace StS
{
    public class Fight
    {

        public Fight Copy()
        {
            var p = _Player.Copy();
            var e = new List<IEnemy>() { _Enemies[0].Copy() };
            var d = _Deck.Copy();


            var newFight = new Fight(d, p, e);
            var historyCopy = ActionHistory.Select(el => el.Copy());
            newFight.ActionHistory = historyCopy.ToList();
            newFight.FirstTurnCalled = FirstTurnCalled;
            return newFight;
        }

        private List<IEnemy> _Enemies { get; set; }
        private Player _Player { get; set; }
        private Deck _Deck { get; set; }

        public FightStatus Status { get; set; }

        private void Init(Deck d, Player player, List<IEnemy> enemies, bool preserveOrder = false)
        {
            _Deck = d;
            _Player = player;
            _Enemies = enemies;
            Status = FightStatus.Ongoing;
        }

        public Fight(Deck d, Player player, List<IEnemy> enemies, bool preserveOrder = false)
        {
            Init(d, player, enemies, preserveOrder);
        }

        public Fight(List<CardInstance> initialCis, Player player, IEnemy enemy, bool preserveOrder = false)
        {

            var deck = new Deck(initialCis, preserveOrder);
            Init(deck, player, new List<IEnemy>() { enemy }, preserveOrder);
        }

        /// <summary>
        /// for testing
        /// </summary>
        public List<CardInstance> GetDrawPile()
        {
            return _Deck.DrawPile;
        }

        /// <summary>
        /// Should really only return distinct actions.
        /// </summary>
        internal List<SimAction> GetAllActions()
        {
            var res = new List<SimAction>();
            var hand = _Deck.Hand;
            var consideredCis = new List<string>();
            var en = _Player.Energy;
            foreach (var ci in hand)
            {
                if (ci.EnergyCost() <= en && ci.Playable(hand) && !consideredCis.Contains(ci.ToString()))
                {
                    var sa = new SimAction(simActionType: SimActionEnum.Card, null, ci);
                    res.Add(sa);
                    consideredCis.Add(ci.ToString());
                }
            }
            foreach (var pot in _Player.Potions)
            {
                var sa = new SimAction(SimActionEnum.Potion, pot, null);
                res.Add(sa);
            }

            res.Add(new SimAction(SimActionEnum.EndTurn, null, null));

            return res;
        }

        internal CardInstance FindIdenticalCard(CardInstance card)
        {
            CardInstance res = null;
            var cs = card.ToString();
            foreach (var c in _Deck.Hand)
            {
                if (c.ToString() == cs)
                {
                    return c;
                }
            }
            if (res == null)
            {
                throw new Exception("No card.");
            }
            return res;
        }

        private bool FirstTurnCalled = false;

        public int RoundNumber { get; set; }
        public List<SimAction> ActionHistory { get; internal set; } = new List<SimAction>();

        public void EndTurn()
        {
            if (!FirstTurnCalled)
            {
                throw new Exception("Calling EndTurn without firstturn started.");
            }

            _Deck.TurnEnds();
            //TODO this is affected by calipers, barricade, etc.

            var endTurnPlayerEf = new EffectSet();
            var endTurnEnemyEf = new EffectSet();

            var relicEf = new EffectSet();

            foreach (var si in _Player.StatusInstances)
            {
                si.EndTurn(_Player, endTurnPlayerEf);
            }
            _Player.StatusInstances = _Player.StatusInstances.Where(el => el.Duration != 0).ToList();

            foreach (var si in ((Entity)_Enemies[0]).StatusInstances)
            {
                si.EndTurn((Entity)_Enemies[0], endTurnEnemyEf);
            }

            foreach (var relic in _Player.Relics)
            {
                relic.EndTurn(_Player, _Enemies[0], relicEf);
            }

            ApplyEffectSet(endTurnPlayerEf, _Player, _Enemies[0]);
            ApplyEffectSet(endTurnEnemyEf, _Enemies[0], _Player);
            ApplyEffectSet(relicEf, _Player, _Enemies[0]);

            _Enemies[0].StatusInstances = _Enemies[0].StatusInstances.Where(el => el.Duration != 0).ToList();
        }

        public void StartTurn(int? drawCount = null)
        {
            FirstTurnCalled = true;
            RoundNumber++;
            drawCount = drawCount ?? _Player.GetDrawAmount();
            _Deck.NextTurnStarts(drawCount.Value);
            _Player.Energy = _Player.MaxEnergy();
            _Player.Block = 0;
            var ef = new EffectSet();
            foreach (var status in _Player.StatusInstances)
            {
                status.StartTurn(_Player, ef);
            }
            foreach (var relic in _Player.Relics)
            {
                relic.StartTurn(_Player, _Enemies[0], ef);
            }
            ApplyEffectSet(ef, _Player, _Enemies[0]);
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
        public void Died(IEntity entity)
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
        public void PlayCard(CardInstance cardInstance, List<CardInstance> cardTargets = null, bool forceExhaust = false, bool newCard = false)
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
                if (cardInstance.EnergyCost() > _Player.Energy)
                {
                    throw new Exception("Trying to play too expensive card");
                }
                _Player.Energy -= cardInstance.EnergyCost();
                _Deck.BeforePlayingCard(cardInstance);
            }

            IEntity target;
            switch (cardInstance.Card.TargetType)
            {
                case TargetType.Player:
                    target = _Player;
                    break;
                case TargetType.Enemy:
                    target = _Enemies[0];
                    break;
                default:
                    throw new NotImplementedException();
            }

            //set the initial effect, or status.
            var ef = new EffectSet();
            cardInstance.Play(ef, _Player, target, cardTargets, _Deck);

            //generate an effect containing all the changes that will happen.
            foreach (var si in _Player.StatusInstances)
            {
                var statusIsTargeted = _Player == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, true);
            }
            foreach (var si in _Enemies[0].StatusInstances)
            {
                var statusIsTargeted = _Enemies[0] == target;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, true);
            }

            foreach (var relic in _Player.Relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: _Player, enemy: _Enemies[0]);
            }

            //relic effects apply first.

            ApplyEffectSet(ef, _Player, target);

            //make sure to apply card effects before putting the just played card into discard, so it can't be drawn again by its own action.
            _Deck.AfterPlayingCard(cardInstance);
        }

        public void ApplyEffectSet(EffectSet ef, IEntity source, IEntity target)
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
                f.Action.Invoke(this, _Deck);
            }

            //We resolve damage after dealing with statuses the player may just have gained.
            //i.e. we don't apply pen nib to the player til after attack is resolved.
        }
        public void ApplyStatus(IEntity entity, List<StatusInstance> sis)
        {
            foreach (var status in sis)
            {
                entity.ApplyStatus(status);
            }
        }
        public void GainBlock(IEntity entity, IndividualEffect ef)
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
        public void ReceiveDamage(IEntity entity, IndividualEffect ef, out bool alive)
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

        internal void EnemyMove(EnemyAction enemyAction = null)
        {
            if (enemyAction == null)
            {
                enemyAction = _Enemies[0].GetAction();
            }
            if (enemyAction == null)
            {
                throw new Exception("No enemy action?");
            }
            if (enemyAction.Buffs != null)
            {
                ApplyStatus(_Enemies[0], enemyAction.Buffs);
            }
            if (enemyAction.Attack != null)
            {
                EnemyPlayCard(enemyAction.Attack);
            }
            if (enemyAction.PlayerStatusAttack != null)
            {
                ApplyStatus(_Player, enemyAction.Buffs);
            }
        }

        public void EnemyPlayCard(EnemyAttack enemyAttack)
        {
            var source = _Enemies[0];
            var target = _Player;
            var ef = new EffectSet();

            var cardInstance = new CardInstance(enemyAttack, 0);
            cardInstance.Play(ef, _Player, _Enemies[0]);

            foreach (var si in source.StatusInstances)
            {
                var statusIsTargeted = false;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, false);
            }
            foreach (var si in target.StatusInstances)
            {
                var statusIsTargeted = true;
                si.Apply(cardInstance.Card, ef.SourceEffect, ef.TargetEffect, statusIsTargeted, false);
            }

            //this won't be right for Torii for example.
            foreach (var relic in _Player.Relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: _Player, enemy: _Enemies[0]);
            }

            GainBlock(_Player, ef.TargetEffect);
            GainBlock(_Enemies[0], ef.SourceEffect);
            ReceiveDamage(_Player, ef.TargetEffect, out bool alive);
            if (!alive) Died(_Player);
            ReceiveDamage(_Enemies[0], ef.SourceEffect, out bool alive2);
            if (!alive2) Died(_Enemies[0]);
        }

        public override string ToString()
        {
            return $"Fight: {_Player}({_Player.Energy}) vs {_Enemies[0]} History: {ActionHistory.Count}";
        }
    }
}
