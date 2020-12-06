using System.Collections.Generic;

namespace StS
{
    public class TrueGrit : IroncladSkillCard
    {
        public override string Name => nameof(TrueGrit);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => 1;

        /// <summary>
        /// Bit annoying that targets actually covers two cases:
        /// 1. AI forcing a certain random choice
        /// 2. Normal play of the upgraded card, specifying the target
        /// </summary>
        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            ef.TargetEffect.InitialBlock = upgradeCount == 0 ? 7 : 10;
            ef.DeckEffect.Add((Deck d) =>
            {
                if (targets == null)
                {
                    var target = d.GetRandomCardFromHand();
                    d.ExhaustFromHand(target, ef);
                }
                else
                {
                    if (targets.Count != 1)
                    {
                        throw new System.Exception();
                    }
                    var hand = d.GetHand;
                    var ci = targets[0];
                    if (!hand.Contains(ci))
                    {
                        throw new System.Exception();
                    }
                    d.ExhaustFromHand(ci, ef);
                }
            });
        }
    }
}
