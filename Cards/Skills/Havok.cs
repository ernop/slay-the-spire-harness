using System.Collections.Generic;
using System.Linq;

namespace StS
{
    public class Havok : IroncladSkillCard
    {
        public override string Name => nameof(Havok);

        public override TargetType TargetType => TargetType.Player;

        public override int CiCanCallEnergyCost(int upgradeCount) => upgradeCount == 0 ? 1 : 0;

        internal override void Play(EffectSet ef, IEntity source, IEntity target, int upgradeCount, List<CardInstance> targets = null, Deck deck = null)
        {
            var x = new OneEffect
            {
                Action = (Fight f, Deck d) =>
                {
                    var theCard = d.Draw(this, null, 1, true).SingleOrDefault();
                    if (theCard == null)
                    {
                        return;
                    }
                    f.PlayCard(theCard, forceExhaust: true, newCard: true);
                }
            };
            ef.FightEffects.Add(x);
        }
    }
}
