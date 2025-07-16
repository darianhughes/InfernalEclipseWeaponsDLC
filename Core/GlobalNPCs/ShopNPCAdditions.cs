using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.TownNPCs;
using InfernalEclipseWeaponsDLC.Content.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.GlobalNPCs
{
    public class ShopNPCAdditions : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            // istg, calamity is WHAT the hell is DILF that is not PERMAFROST
            if (shop.NpcType == ModContent.NPCType<DILF>())
            {
                shop.Add(
                    new NPCShop.Entry(
                        ModContent.ItemType<DeepSeaDrawlShard3>(),
                        Condition.DownedGolem
                        )
                    );
            }
        }
    }
}
