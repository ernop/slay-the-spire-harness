using System;
using System.Collections.Generic;
using System.Linq;

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

            newFight.TurnNumber = TurnNumber;
            newFight.Status = Status;
            newFight.PlayerTurn = PlayerTurn;

            return newFight;
        }

        public FightAction FightAction { get; set; }
        public FightNode FightNode { get; set; }
        public List<IEnemy> _Enemies { get; set; }
        public Player _Player { get; set; }
        private Deck _Deck { get; set; }
        public FightStatus Status { get; set; }
        public int TurnNumber { get; set; }

        /// <summary>
        /// Designating that you have already called "StartTurn";
        /// </summary>
        public bool PlayerTurn { get; set; } = false;
        public bool EnemyDone { get; set; } = true;
        public IList<CardInstance> GetExhaustPile => _Deck.GetExhaustPile;
        public IList<CardInstance> GetDiscardPile => _Deck.GetDiscardPile;
        public IList<CardInstance> GetHand => _Deck.GetHand;

        /// <summary>
        /// accumulate lastActions here for collection by fightSim and ignoring by others.
        /// </summary>
        internal string GetPlayerHP()
        {
            return _Player.HP.ToString();
        }

        public int GetEnemyHP()
        {
            return _Enemies[0].HP;
        }

        private void Init(Deck d, Player player, List<IEnemy> enemies)
        {
            _Deck = d;
            _Player = player;

            _Enemies = enemies;
            Status = FightStatus.Ongoing;
        }

        private Fight(Deck d, Player player, List<IEnemy> enemies)
        {
            Init(d, player, enemies);
        }

        public Fight(IList<CardInstance> initialCis, Player player, IEnemy enemy, bool preserveOrder = true)
        {
            var d = new Deck(initialCis, preserveOrder);
            Init(d, player, new List<IEnemy>() { enemy });
        }

        public Fight(Deck deck, Player player, IEnemy enemy, bool preserveOrder = true)
        {
            Init(deck, player, new List<IEnemy>() { enemy });
        }

        /// <summary>
        /// for testing
        /// </summary>
        public IList<CardInstance> GetDrawPile()
        {
            return _Deck.GetDrawPile;
        }

        /// <summary>
        /// Three choices:
        /// * StartTurn
        /// * NormalActions
        /// * EnemyMove
        /// </summary>
        internal IList<FightAction> GetAllActions()
        {
            if (!PlayerTurn)
            {
                if (EnemyDone)
                {
                    var cards = _Deck.WouldDraw(_Player.GetDrawAmount());
                    return new List<FightAction>() { new FightAction(FightActionEnum.StartTurn, cardsDrawn:cards) };
                }
                else
                {
                    return new List<FightAction>() { _Enemies[0].GetAction(TurnNumber) };
                }
            }
            return GetNormalActions();
        }

        /// <summary>
        /// All playable cards, potions, or endturn.
        /// </summary>
        internal IList<FightAction> GetNormalActions() { 

            var res = new List<FightAction>();
            var hand = _Deck.GetHand;
            var consideredCis = new List<string>();
            var en = _Player.Energy;
            foreach (var ci in hand)
            {
                if (!consideredCis.Contains(ci.ToString()) && ci.EnergyCost() <= en && ci.Playable(hand))
                {
                    var sa = new FightAction(fightActionType: FightActionEnum.PlayCard, card: ci);
                    res.Add(sa);
                    consideredCis.Add(ci.ToString());
                }
            }

            var seenPots = new HashSet<string>();
            foreach (var pot in _Player.Potions)
            {
                var key = pot.ToString();
                if (seenPots.Contains(key))
                {
                    continue;
                }
                seenPots.Add(key);
                var sa = new FightAction(FightActionEnum.Potion, potion: pot);
                res.Add(sa);
            }

            res.Add(new FightAction(FightActionEnum.EndTurn));

            return res;
        }

        private void StartFight(List<string> history)
        {
            var startEf = new EffectSet();
            foreach (var relic in _Player.Relics)
            {
                relic.Apply(this, _Deck, _Player);
                relic.StartFight(_Deck, startEf);
            }
            _Deck.StartFight();

            ApplyEffectSet(startEf, _Player, _Enemies[0], history);
        }

        public bool StartTurn(IList<CardInstance> initialHand = null)
        {
            if (PlayerTurn)
            {
                throw new Exception("Turn Already Started");
            }
            if (!EnemyDone)
            {
                throw new Exception("Enemy not done");
            }
            PlayerTurn = true;

            var history = new List<string>();

            if (TurnNumber == 0)
            {
                StartFight(history);
            }

            TurnNumber++;
            var ef = new EffectSet();

            if (initialHand == null)
            {
                var drawCount = _Player.GetDrawAmount();
                _Deck.DrawCards(drawCount, ef, history);
            }
            else
            {
                _Deck.ForceDrawCards(initialHand, ef, history);
            }

            history.Add($"Drew:{string.Join(',', _Deck.GetHand.OrderBy(el=>el.ToString()))}");

            _Player.Energy = _Player.MaxEnergy();
            _Player.Block = 0;

            foreach (var status in _Player.StatusInstances)
            {
                status.StartTurn(_Player, ef.PlayerEffect, ef.EnemyEffect);
            }

            foreach (var relic in _Player.Relics)
            {
                relic.StartTurn(_Player, _Enemies[0], ef);
            }

            ApplyEffectSet(ef, _Player, _Enemies[0], history);

            AssignLastAction(new FightAction(FightActionEnum.StartTurn, cardsDrawn: initialHand, desc: history));
            return true;
        }

        public void EndTurn()
        {
            if (!PlayerTurn)
            {
                throw new Exception("Not your turn");
            }
            var history = new List<string>();
            if (TurnNumber == 0)
            {
                throw new Exception("Calling EndTurn without firstturn started.");
            }

            history.Add($"{TurnNumber} (E{_Player.Energy})");

            var endTurnEf = new EffectSet();


            foreach (var ci in _Deck.GetHand)
            {
                ci.LeftInHandAtEndOfTurn(endTurnEf.PlayerEffect);
            }

            _Deck.TurnEnds(endTurnEf, history);
            ApplyEffectSet(endTurnEf, _Player, _Enemies[0], history);

            //todo why do I have two EFs here?
            var relicEf = new EffectSet();
            var endTurnPlayerEf = new EffectSet();
            foreach (var si in _Player.StatusInstances)
            {
                si.EndTurn(_Player, endTurnPlayerEf.PlayerEffect, endTurnPlayerEf.EnemyEffect);
            }

            _Player.StatusInstances = _Player.StatusInstances.Where(el => el.Duration != 0 && el.Intensity != 0).ToList();

            foreach (var relic in _Player.Relics)
            {
                relic.EndTurn(_Player, _Enemies[0], relicEf);
            }

            ApplyEffectSet(endTurnPlayerEf, _Player, _Enemies[0], history);

            ApplyEffectSet(relicEf, _Player, _Enemies[0], history);

            AssignLastAction(new FightAction(FightActionEnum.EndTurn, desc: history));
            PlayerTurn = false;
            EnemyDone = false;
        }

        public void AssignLastAction(FightAction a)
        {
            //unless a fight is part of a fightnode, don't assign history.  e.g. tests.
            if (FightAction != null)
            {
                throw new Exception("protection");
            }
            FightAction = a;
        }

        /// <summary>
        /// Enemy statuses actually apply at the start of their turn.
        /// </summary>
        private void StartEnemyTurn(List<string> history)
        {
            if (PlayerTurn)
            {
                throw new Exception("Out of order.");
            }
            var endTurnEnemyEf = new EffectSet();
            foreach (var si in ((Entity)_Enemies[0]).StatusInstances)
            {
                si.EndTurn((Entity)_Enemies[0], endTurnEnemyEf.EnemyEffect, endTurnEnemyEf.PlayerEffect);
            }

            ApplyEffectSet(endTurnEnemyEf, _Player, _Enemies[0], history);
            _Enemies[0].StatusInstances = _Enemies[0].StatusInstances.Where(el => el.Duration != 0 && el.Intensity != 0).ToList();
        }

        private void Died(IEntity entity, List<string> history)
        {
            if (entity.Dead)
            {
                throw new Exception("Can't die twice");
            }
            entity.Dead = true;
            switch (entity.EntityType)
            {
                case EntityType.Enemy:
                    Status = FightStatus.Won;
                    WinFight(history);
                    break;
                case EntityType.Player:
                    Status = FightStatus.Lost;
                    break;
                default:
                    break;
            };
        }
        private void WinFight(List<string> history)
        {
            foreach (var relic in _Player.Relics)
            {
                var ef = new EffectSet();
                relic.EndFight(_Deck, ef);
                ApplyEffectSet(ef, _Player, _Enemies[0], history);
            }
            _Player.StatusInstances = new List<StatusInstance>();
        }
        /// <summary>
        /// From monster POV, player is the enemy.
        /// 
        /// TODO: why not send a cardDescriptor, and have this method just find a matching card? would make external combinatorics easier.
        /// </summary>
        public bool PlayCard(CardInstance cardInstance, List<CardInstance> cardTargets = null, 
            bool forceExhaust = false, bool newCard = false, IList<CardInstance> source = null)
        {
            if (!PlayerTurn) throw new Exception("Not your turn");
            //get a copy since action was generated from an earlier version.
            if (source == null)
            {
                source = _Deck.GetHand;
            }
            cardInstance = Helpers.FindIdenticalCardInSource(source, cardInstance);
            var history = new List<string>();
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
            cardInstance.Play(ef, _Player, _Enemies[0], cardTargets, _Deck);

            //generate an effect containing all the changes that will happen.
            foreach (var si in _Player.StatusInstances)
            {
                var statusIsTargeted = _Player == target;
                si.Apply(cardInstance.Card, ef.PlayerEffect, ef.EnemyEffect, statusIsTargeted, true);
            }
            foreach (var si in _Enemies[0].StatusInstances)
            {
                var statusIsTargeted = _Enemies[0] == target;
                si.Apply(cardInstance.Card, ef.PlayerEffect, ef.EnemyEffect, statusIsTargeted, true);
            }

            foreach (var relic in _Player.Relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: _Player, enemy: _Enemies[0]);
            }

            //relic effects apply first.

            //make sure to apply card effects before putting the just played card into discard, so it can't be drawn again by its own action.
            _Deck.AfterPlayingCard(cardInstance, ef, history);

            ApplyEffectSet(ef, _Player, _Enemies[0], history: history, ci: cardInstance);

            AssignLastAction(new FightAction(FightActionEnum.PlayCard, card: cardInstance, desc: history));
            _Deck.CardPlayCleanup();
            return false;
        }

        /// <summary>
        /// Returns whether there was randomness involved in drinking the potion. 
        /// </summary>
        public bool DrinkPotion(Potion p, Enemy e)
        {
            if (!PlayerTurn) throw new Exception("Not your turn");
            var fakePotion = _Player.Potions.First(el => el.ToString() == p.ToString());
            _Player.Potions.Remove(fakePotion);
            var history = new List<string>();
            var ef = new EffectSet();
            p.Apply(this, _Player, e, ef);
            Entity target;
            if (p.SelfTarget())
            {
                target = _Player;
            }
            else
            {
                target = e;
            }

            ApplyEffectSet(ef, _Player, e, history);

            AssignLastAction(new FightAction(FightActionEnum.Potion, potion: p, target: target, desc: history));
            return false;
        }

        private void ApplyEffectSet(EffectSet ef, Player player, IEnemy enemy, List<string> history, Potion potion = null, CardInstance ci = null, bool subEffectSet = false)
        {
            //TODO not clear if this order is the most sensible really or not.
            //complex effects like afterImage + playing defend with neg dex.

            //What happens if a deckeffect has further effects like exhausting a card, and the player has a status that triggers on this?

            //TODO this is complicated.  Evolve actually adds new deckeffects in the deckeffect evaluation.
            foreach (var f in ef.DeckEffect)
            {
                history.Add(f.Invoke(_Deck, history));
            }

            GainBlock(player, ef.PlayerEffect, history);
            ReceiveDamage(enemy, ef.EnemyEffect, ef, history, ci);

            GainBlock(enemy, ef.EnemyEffect, history);
            ReceiveDamage(player, ef.PlayerEffect, ef, history, ci);
            ApplyStatus(player, ef.PlayerEffect.Status, history);
            ApplyStatus(enemy, ef.EnemyEffect.Status, history);

            if (ef.PlayerEnergy != 0)
            {
                _Player.Energy += ef.PlayerEnergy;
                history.Add($"Gained ${ef.PlayerEnergy} to {_Player.Energy}");
            }


            ef.FightEffect.ForEach(fe => history.Add(fe.Action.Invoke(this, _Deck, history)));

            foreach (var en in _Enemies)
            {
                if (!en.Dead)
                {
                    if (en.HP <= 0)
                    {
                        history.Add("Enemy Died");
                        Died(en, history);
                    }
                }
            }
            if (_Player.HP <= 0)
            {
                if (!_Player.Dead)
                {
                    history.Add("Player Died");
                    Died(_Player, history);
                }
            }

            if (!subEffectSet)
            {
                while (ef.NextEffectSet.Count > 0)
                {
                    var next = ef.NextEffectSet.First();
                    ef.NextEffectSet.RemoveAt(0);
                    ApplyEffectSet(next, player, enemy, history, potion, ci);
                }
            }
        }

        /// <summary>
        /// For use during a fight; add a status to an entity
        /// </summary>
        private void ApplyStatus(IEntity entity, List<StatusInstance> sis, List<string> history)
        {
            foreach (var status in sis)
            {
                entity.ApplyStatus(this, _Deck, status);
                history.Add($"{entity} gained {status}");
            }
        }

        private void GainBlock(IEntity entity, IndividualEffect ef, List<string> history)
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

        private void ReceiveDamage(IEntity entity, IndividualEffect ie, EffectSet ef, List<string> history, CardInstance ci)
        {
            if (ie.GetInitialDamage() == null && ie.DamageAdjustments?.Count > 0)
            {
                throw new Exception("should not happen");
                //Vuln will only add a progression if initialdamage != null
            }

            //We don't actually want to 
            if (ie.GetInitialDamage() != null)
            {
                var val = ie.GetInitialDamage().Select(el => (double)el);

                foreach (var prog in ie.DamageAdjustments.OrderBy(el => el.Order))
                {
                    val = prog.Fun(val.ToList());
                }

                var usingVal = val.Select(el => (int)Math.Floor(el));
                history.Add($"attack for {string.Join(',', usingVal)}");
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
                            entity.ApplyDamage(elCopy, ef, ci);
                        }
                    }
                }
                history.Add($"{entity.Details()}");
            }
        }

        public void EnemyMove(int amount, int count)
        {
            if (PlayerTurn) throw new Exception("Not your turn");
            var ea = new FightAction(FightActionEnum.EnemyMove, card: new CardInstance(new EnemyCard(amount, count), 0));
            EnemyMove(ea);
        }

        public void EnemyMove(FightAction enemyAction = null)
        {
            if (enemyAction == null)
            {
                enemyAction = _Enemies[0].GetAction(TurnNumber);
            }
            var history = new List<string>();
            StartEnemyTurn(history);

            if (enemyAction == null)
            {
                throw new Exception("No enemy action?");
            }
            if (enemyAction.Card != null)
            {
                _Attack(enemyAction.Card, history);
                AssignLastAction(enemyAction);
            }
            else
            {
                throw new Exception("Noop");
            }
            EnemyDone = true;
        }

        public void _Attack(CardInstance cardInstance, List<string> history)
        {
            var enemy = _Enemies[0];
            var player = _Player;
            var ef = new EffectSet();

            cardInstance.Play(ef, _Player, _Enemies[0]);
            _Player.NotifyAttacked(ef);

            foreach (var si in enemy.StatusInstances)
            {
                var statusIsTargeted = false;
                si.Apply(cardInstance.Card, ef.EnemyEffect, ef.PlayerEffect, statusIsTargeted, false);
            }
            foreach (var si in player.StatusInstances)
            {
                var statusIsTargeted = true;
                si.Apply(cardInstance.Card, ef.EnemyEffect, ef.PlayerEffect, statusIsTargeted, false);
            }

            //this won't be right for Torii for example.
            foreach (var relic in _Player.Relics)
            {
                relic.CardPlayed(cardInstance.Card, ef, player: _Player, enemy: _Enemies[0]);
            }

            GainBlock(_Player, ef.PlayerEffect, history);
            GainBlock(_Enemies[0], ef.EnemyEffect, history);
            ReceiveDamage(_Enemies[0], ef.EnemyEffect, ef, history, cardInstance);
            ReceiveDamage(_Player, ef.PlayerEffect, ef, history, cardInstance);

            var aaa = _Enemies[0].HP;

            ApplyStatus(_Player, ef.PlayerEffect.Status, history);
            ApplyStatus(_Enemies[0], ef.EnemyEffect.Status, history);

            if (_Enemies[0].HP <= 0)
            {
                history.Add("Enemy Died on his turn");
                Died(_Enemies[0], history);
            }
            if (_Player.HP <= 0)
            {
                history.Add("Player Died from enemy attack");
                Died(_Player, history);
            }
        }

        public override string ToString()
        {
            return $"Fight: {_Player}({_Player.Energy}) vs {_Enemies[0]}";
        }
        
        internal void SetEnemyHp(int v)
        {
            foreach (var enemy in _Enemies)
            {
                enemy.HP = 1;
                enemy.HPMax = 1;
            }
        }
    }
}
