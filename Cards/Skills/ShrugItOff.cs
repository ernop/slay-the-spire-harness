using System;
using System.Collections.Generic;

namespace StS
{
    public class ShrugItOff : IroncladSkillCard
    {
        public override string Name => nameof(ShrugItOff);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        public override List<CardInstance> GetTargetCards(Deck deck)
        {
            if (deck.DrawPile.Count == 0)
            {
                //do not actually reshuffle because the AI will hit this.
                return deck.DiscardPile;
            }
            return deck.DrawPile;
        }

        /// <summary>
        /// But the sim will ignore this and equally evaluate all the options
        /// </summary>
        public override bool RandomCardTarget => true;

        /// <summary>
        /// by default Play picks one randomly.  But how should I differentiate the case where the AI wants to force a choice for weighting?
        /// Normal play will consult
        /// </summary>
        internal override void Play(EffectSet ef, Entity source, Entity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.InitialBlock = upgradeCount == 0 ? 8 : 11;
            ef.DeckEffect.Add((Deck d) =>
            {
                if (deck.DrawPile.Count == 0)
                {
                    deck.Reshuffle();
                }

                //we will allow the AI to control the randomness here.
                CardInstance theCard;
                if (targets == null) //true random
                {
                    var generatedTargets = GetTargetCards(d);
                    if (generatedTargets.Count == 0)
                    {
                        return;
                    }
                    var rnd = new Random();
                    theCard = generatedTargets[rnd.Next(generatedTargets.Count)];

                }
                else //ai played it.
                {
                    if (targets.Count == 0)
                    {
                        return;
                    }
                    theCard = targets[0];
                }
                d.DrawPile.Remove(theCard);
                d.Hand.Add(theCard);
            });
        }
    }
}
