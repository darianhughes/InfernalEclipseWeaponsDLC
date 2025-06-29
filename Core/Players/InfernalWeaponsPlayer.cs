using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.NewFolder
{
    public class InfernalWeaponsPlayer : ModPlayer
    {
        public bool spearSearing;
        public bool spearArctic;

        public override void ResetEffects()
        {
            spearSearing = false;
            spearArctic = false;
        }
    }
}
