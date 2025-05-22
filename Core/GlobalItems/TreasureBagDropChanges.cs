using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;

namespace InfernalEclipseWeaponsDLC.Core.GlobalItems
{
    public class TreasureBagDropChanges : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.Find<ModItem>("SignusBag") is ModItem signusBag)
                {
                    if (item.type == signusBag.Type)
                    {
                        itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheParallel>(), 3));
                    }
                }
            }
        }
    }
}
