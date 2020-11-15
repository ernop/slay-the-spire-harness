﻿using System.Collections.Generic;

namespace StS
{
    public static class AllRelics
    {
        public static Dictionary<string, Relic> GetAllRelics()
        {
            var relics = new Dictionary<string, Relic>();
            relics["Pen Nib"] = new PenNib();
            relics["Vajra"] = new Vajra();
            return relics;
        }
    }
}


