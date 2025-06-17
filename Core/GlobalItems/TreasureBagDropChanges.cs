using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using CalamityMod.Items.TreasureBags;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Rogue;

namespace InfernalEclipseWeaponsDLC.Core.GlobalItems
{
    public class TreasureBagDropChanges : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                if (item.type == ModContent.ItemType<BrimstoneWaifuBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrimstoneHarp>(), 3));
                }

                if (item.type == ModContent.ItemType<AstrumDeusBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeusFlute>(), 3));
                }

                if (item.type == ModContent.ItemType<RavagerBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<NecroticChorus>(), 3));
                }

                if (item.type == ModContent.ItemType<SignusBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheParallel>(), 3));
                }

                if (item.type == ModContent.ItemType<DragonfollyBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BirbSaxophone>(), 3));
                }

                if (item.type == ModContent.ItemType<DevourerofGodsBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeathsWhisper>(), 3));
                }

                if (item.type == ModContent.ItemType<OldDukeBag>())
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CorrodedCane>(), 3));
                }
            }

            if (ModLoader.TryGetMod("Consolaria", out Mod console))
            {
                if (item.type == console.Find<ModItem>("OcramBag").Type)
                {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OcramKnife>(), 4));
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheBlight>(), 4));
                }
            }
        }
    }
}
