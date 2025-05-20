using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Steamworks;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC
{
	public class InfernalEclipseWeaponsDLC : Mod
	{
		internal static InfernalEclipseWeaponsDLC Instance;

		internal Mod calamity = null;
		internal Mod calamitybardhealer = null;
		internal Mod ragnarok = null;
		internal Mod thorium = null;

        public override void Load()
        {
			Instance = this;

			calamity = null;
			ModLoader.TryGetMod("CalamityMod", out  calamity);
			calamitybardhealer = null;
			ModLoader.TryGetMod("CalamityBardHealer", out calamitybardhealer);
			ragnarok = null;
			ModLoader.TryGetMod("RagnarokMod", out ragnarok);
			thorium = null;
			ModLoader.TryGetMod("ThoriumMod", out thorium);
        }
    }
}
