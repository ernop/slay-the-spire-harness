using System.Collections.Generic;

namespace StS
{
    public static class AllRelics
    {

        public static Dictionary<string, Relic> Relics = GetAllRelics();

        private static Dictionary<string, Relic> GetAllRelics()
        {
            var relics = new Dictionary<string, Relic>();
            relics["Pen Nib"] = new PenNib();
            relics["Vajra"] = new Vajra();
            relics["Torii"] = new Torii();
            relics["MonkeyPaw"] = new MonkeyPaw();
            relics["BustedCrown"] = new BustedCrown();
            relics["FusionHammer"] = new FusionHammer();
            relics["BagOfEyes"] = new BagOfEyes();
            relics["Anchor"] = new Anchor();
            relics["HornCleat"] = new HornCleat();
            relics["BurningBlood"] = new BurningBlood();
            relics["Ectoplasm"] = new Ectoplasm();
            relics["TheBoot"] = new TheBoot();
            relics["Sundial"] = new Sundial();
            relics["NeowsLament"] = new NeowsLament();
            relics["Turnip"] = new Turnip();
            relics["CaptainsWheel"] = new CaptainsWheel();
            return relics;
        }
    }
}


