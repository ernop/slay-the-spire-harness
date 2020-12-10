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

            //we no longer copy history; each fight just stores the action at that step.
            //var historyCopy = FightHistory.Select(el => el.Copy());
            //newFight.FightHistory = historyCopy.ToList();

            newFight.FirstTurnCalled = FirstTurnCalled;
            newFight.TurnNumber = TurnNumber;
            return newFight;
        }

        public List<IEnemy> _Enemies { get; set; }
        public Player _Player { get; set; }

        internal void SetEnemyHp(int v)
        {
            foreach (var enemy in _Enemies)
            {
                enemy.HP = 1;
                enemy.HPMax = 1;
            }
        }

        private Deck _Deck { get; set; }

        /// <summary>
        /// For tests of hooking to events.?
        /// </summary>
        public Deck GetDeck => _Deck;

        public FightStatus Status { get; set; }

        private void Init(Deck d, Player player, List<IEnemy> enemies, bool preserveOrder = false)
        {
            _Deck = d;
            _Player = player;

            _Enemies = enemies;
            Status = FightStatus.Ongoing;
        }

        private Fight(Deck d, Player player, List<IEnemy> enemies, bool preserveOrder = false)
        {
            Init(d, player, enemies, preserveOrder);
        }

        public Fight(List<CardInstance> initialCis, Player player, IEnemy enemy, bool preserveOrder = false)
        {

            var deck = new Deck(initialCis, preserveOrder);
            Init(deck, player, new List<IEnemy>() { enemy }, preserveOrder);
        }

        internal string GetPlayerHP()
        {
            return _Player.HP.ToString();
        }

        public string GetEnemyHP()
        {
            return string.Join(',', _Enemies.Select(el => el.HP.ToString()));
        }

        /// <summary>
        /// for testing
        /// </summary>
        public IList<CardInstance> GetDrawPile()
        {
            return _Deck.GetDrawPile;
        }

        /// <summary>
        /// Should really only return distinct actions.
        /// </summary>
        internal List<FightAction> GetAllActions()
        {
            var res = new List<FightAction>();
            var hand = _Deck.GetHand;
            var consideredCis = new List<string>();
            var en = _Player.Energy;
            foreach (var ci in hand)
            {
                if (ci.EnergyCost() <= en && ci.Playable(hand) && !consideredCis.Contains(ci.ToString()))
                {
                    var sa = new FightAction(fightActionType: FightActionEnum.PlayCard, card: ci);
                    res.Add(sa);
                    consideredCis.Add(ci.ToString());
                }
            }
            foreach (var pot in _Player.Potions)
            {
                var sa = new FightAction(FightActionEnum.Potion, potion: pot);
                res.Add(sa);
            }

            res.Add(new FightAction(FightActionEnum.EndTurn));

            return res;
        }



        private bool FirstTurnCalled = false;

        public int TurnNumber { get; set; }
        public List<FightAction> FightHistory { get; internal set; } = new List<FightAction>();

        public void StartTurn(int? drawCount = null, List<CardInstance> initialHand = null)
        {
            if (TurnNumber == 0)
            {
                //clear this.

                var startEf = new EffectSet();
                foreach (var relic in _Player.Relics)
                {
                    relic.StartFight(_Deck, startEf);
                }

                ApplyEffectSet(FightActionEnum.StartFight, startEf, _Player, _Enemies[0]);
            }
            FirstTurnCalled = true;
            TurnNumber++;
            var ef = new EffectSet();

            if (initialHand == null)
            {
                drawCount = drawCount ?? _Player.GetDrawAmount();

                _Deck.DrawCards(drawCount.Value, ef);
            }
            else
            {
                _Deck.ForceDrawCards(initialHand, ef);
            }

            AddHistory(FightActionEnum.StartTurn, desc: new List<string>() { TurnNumber.ToString(), "Drew:" + string.Join(',', _Deck.GetHand), _Player.Details(), _Enemies[0].Details() });
            if (_Deck.GetHand.Count > 2)
            {
                var ae = 4;
            }
            _Player.Energy = _Player.MaxEnergy();
            _Player.Block = 0;
            foreach (var status in _Player.StatusInstances)
            {
                status.StartTurn(_Player, ef);
            }
            foreach (var relic in _Player.Relics)
            {
                relic.StartTurn(_Player, _Enemies[0], ef);
            }

            ApplyEffectSet(FightActionEnum.StartTurnEffect, ef, _Player, _Enemies[0]);
        }


        public void AddHistory(FightActionEnum type, Potion potion = null, CardInstance card = null, IEntity enemy = null, List<string> desc = null)
        {
            var fh = new FightAction(type, potion, card, enemy, desc);
            var fhstr = fh.ToString();
            if (string.IsNullOrEmpty(fhstr))
            {
                return;
            }

            FightHistory.Add(fh);
        }

        public void EndTurn()
        {
            if (!FirstTurnCalled)
            {
                throw new Exception("Calling EndTurn without firstturn started.");
            }

            AddHistory(FightActionEnum.EndTurn, desc: new List<string>() { $"{TurnNumber}" });

            var endTurnEf = new EffectSet();
            _Deck.TurnEnds(endTurnEf);
            ApplyEffectSet(FightActionEnum.EndTurnDeckEffect, endTurnEf, _Player, _Enemies[0]);
            //TODO this is affected by calipers, barricade, etc.

            var endTurnPlayerEf = new EffectSet();
            var endTurnEnemyEf = new EffectSet();

            var relicEf = new EffectSet();

            foreach (var si in _Player.StatusInstances)
            {
                si.EndTurn(_Player, endTurnPlayerEf);
            }
            _Player.StatusInstances = _Player.StatusInstances.Where(el => el.Duration != 0 && el.Intensity != 0).ToList();

            foreach (var si in ((Entity)_Enemies[0]).StatusInstances)
            {
                si.EndTurn((Entity)_Enemies[0], endTurnEnemyEf);
            }

            foreach (var relic in _Player.Relics)
            {
                relic.EndTurn(_Player, _Enemies[0], relicEf);
            }

            ApplyEffectSet(FightActionEnum.EndTurnOtherEffect, endTurnPlayerEf, _Player, _Enemies[0]);

            ApplyEffectSet(FightActionEnum.EndTurnOtherEffect, endTurnEnemyEf, _Enemies[0], _Player);

            ApplyEffectSet(FightActionEnum.EndTurnOtherEffect, relicEf, _Player, _Enemies[0]);

            _Enemies[0].StatusInstances = _Enemies[0].StatusInstances.Where(el => el.Duration != 0 && el.Intensity != 0).ToList();
        }

        public IList<CardInstance> GetExhaustPile => _Deck.GetExhaustPile;
        public IList<CardInstance> GetDiscardPile => _Deck.GetDiscardPile;

        public IList<CardInstance> GetHand => _Deck.GetHand;

        public void Died(IEntity entity, bool player)
        {
            if (player)
            {
                AddHistory(FightActionEnum.LostFight);
            }
            else
            {
                AddHistory(FightActionEnum.EnemyDied, enemy: entity);
            }
            if (entity.Dead)
            {
                throw new Exception("Can't die twice");
            }
            entity.Dead = true;
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
                relic.EndFight(_Deck, ef);
                ApplyEffectSet(FightActionEnum.EndFightEffect, ef, _Player, _Player);
            }
            _Player.StatusInstances = new List<StatusInstance>();
        }

        /// <summary>
        /// From monster POV, player is the enemy.
        /// </summary>
        public EffectSet PlayCard(CardInstance cardInstance, List<CardInstance> cardTargets = null, bool forceExhaust = false, bool newCard = false)
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
                if (!cardInstance.Playable(_Deck.GetHand))
                {
                    throw new Exception("Trying to play unplayable card.");
                }
                if (cardInstance.EnergyCost() > _Player.Energy)
                {
                    throw new Exception("Trying to play too expensive card");
                }

                var ec = cardInstance.EnergyCost();
                _Player.Energy -= ec;
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

            //make sure to apply card effects before putting the just played card into discard, so it can't be drawn again by its own action.
            _Deck.AfterPlayingCard(cardInstance, ef);

            ApplyEffectSet(FightActionEnum.PlayCard, ef, _Player, target, card: cardInstance);

            _Deck.FinishCardPlay();
            return ef;
        }

        public void ApplyEffectSet(FightActionEnum action, EffectSet ef, IEntity source, IEntity target, Potion potion = null, CardInstance card = null)
        {
            //TODO not clear if this order is the most sensible really or not.
            //complex effects like afterImage + playing defend with neg dex.

            //What happens if a deckeffect has further effects like exhausting a card, and the player has a status that triggers on this?
            var history = new List<string>();

            foreach (var f in ef.DeckEffect)
            {
                history.Add(f.Invoke(_Deck));
            }

            if (source == target)
            {
                var combined = Combine(ef.SourceEffect, ef.TargetEffect);
                GainBlock(source, combined, history);
                ReceiveDamage(source, combined, history);
                ApplyStatus(source, combined.Status, history);
            }
            else
            {
                GainBlock(source, ef.SourceEffect, history);
                ReceiveDamage(target, ef.TargetEffect, history);

                GainBlock(target, ef.TargetEffect, history);
                ReceiveDamage(source, ef.SourceEffect, history);

                ApplyStatus(source, ef.SourceEffect.Status, history);
                ApplyStatus(target, ef.TargetEffect.Status, history);
            }

            if (ef.PlayerEnergy != 0)
            {
                _Player.Energy += ef.PlayerEnergy;
                history.Add($"Gained ${ef.PlayerEnergy} to {_Player.Energy}");
            }

            foreach (var f in ef.PlayerEffect)
            {
                history.Add(f.Invoke(_Player));
            }

            ef.FightEffect.ForEach(fe => history.Add(fe.Action.Invoke(this, _Deck)));

            AddHistory(action, potion: potion, card: card, desc: history);

            foreach (var en in _Enemies)
            {
                if (!en.Dead)
                {
                    if (en.HP <= 0)
                    {
                        Died(en, false);
                    }
                }
            }
            if (_Player.HP <= 0)
            {
                if (!_Player.Dead)
                {
                    Died(_Player, true);
                }
            }
        }


        /// <summary>
        /// For use during a fight; add a status to an entity
        /// </summary>
        public void ApplyStatus(IEntity entity, List<StatusInstance> sis, List<string> history)
        {
            foreach (var status in sis)
            {
                entity.ApplyStatus(_Deck, status);
                history.Add($"{entity} gained {status}");
            }
        }

        public void ApplyStatus(IEntity entity, StatusInstance si)
        {
            entity.ApplyStatus(_Deck, si);
        }


        public void GainBlock(IEntity entity, IndividualEffect ef, List<string> history)
        {
            //unlike attacks, initialBlock defaults to zero so that you can have adjustments on zero
            // (for example, exhausting a card with Feel No Pain on.)
            var val = ef.InitialBlock;
            foreach (var prog in ef.BlockAdjustments.OrderBy(el => el.Order))
            {
                val = prog.Fun(val, entity);
            }
            if (val > 0)
            {
                var gain = (int)val;
                entity.Block += gain;
                history.Add($"{entity} gained {gain}B");
            }
        }

        public void ReceiveDamage(IEntity entity, IndividualEffect ef, List<string> history)
        {
            //We don't actually want to 
            if (ef.InitialDamage != null)
            {
                var val = ef.InitialDamage.Select(el => (double)el);
                foreach (var prog in ef.DamageAdjustments.OrderBy(el => el.Order))
                {
                    val = prog.Fun(val);
                }

                var usingVal = val.Select(el => (int)Math.Floor(el));
                history.Add($"Attacked {entity} {entity.HP}/{entity.HPMax} {entity.Block}B for {string.Join(',', usingVal)}");
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
                            entity.ApplyDamage(elCopy);
                        }
                    }
                }
                history.Add($"{entity.Details()}");
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

        internal void EnemyMove()
        {
            var enemyAction = _Enemies[0].GetAction();
            var history = new List<string>();
            if (enemyAction == null)
            {
                throw new Exception("No enemy action?");
            }
            if (enemyAction.Buffs != null)
            {
                ApplyStatus(_Enemies[0], enemyAction.Buffs, history);
                AddHistory(FightActionEnum.EnemyBuff, desc: history);
            }
            if (enemyAction.Attack != null)
            {
                EnemyPlayCard(enemyAction.Attack);
            }
            if (enemyAction.PlayerStatusAttack != null)
            {
                ApplyStatus(_Player, enemyAction.Buffs, history);
                AddHistory(FightActionEnum.EnemyStatusAttack, desc: history);
            }
        }

        public void EnemyPlayCard(EnemyCard enemyCard)
        {
            var history = new List<string>();
            var source = _Enemies[0];
            var target = _Player;
            var ef = new EffectSet();

            var cardInstance = new CardInstance(enemyCard, 0);
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

            GainBlock(_Player, ef.TargetEffect, history);
            GainBlock(_Enemies[0], ef.SourceEffect, history);
            ReceiveDamage(_Player, ef.TargetEffect, history);
            ReceiveDamage(_Enemies[0], ef.SourceEffect, history);
            ApplyStatus(_Player, ef.TargetEffect.Status, history);
            ApplyStatus(_Enemies[0], ef.SourceEffect.Status, history);
            AddHistory(FightActionEnum.EnemyAttack, card: cardInstance, desc: history);
            if (_Enemies[0].HP <= 0)
            {
                Died(_Enemies[0], false);
            }
            if (_Player.HP <= 0)
            {
                Died(_Player, true);
            }
        }

        public override string ToString()
        {
            return $"Fight: {_Player}({_Player.Energy}) vs {_Enemies[0]} History: {FightHistory.Count}";
        }

        internal void DrinkPotion(Player player, Potion potion, Enemy enemy)
        {
            player.DrinkPotion(this, potion, enemy);
        }
    }
}
