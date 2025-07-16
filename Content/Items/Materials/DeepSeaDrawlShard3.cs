using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Materials
{
    public class DeepSeaDrawlShard3 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.buyPrice(0, 22, 80, 0);
            Item.value = CalamityGlobalItem.RarityLightPurpleBuyPrice;
            Item.rare = ItemRarityID.Lime;
        }
    }
}
