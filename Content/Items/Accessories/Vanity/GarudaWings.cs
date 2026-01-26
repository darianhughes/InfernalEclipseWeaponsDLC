using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items.Materials;
using CalamityMod.Items;
using InfernalEclipseWeaponsDLC.Core.NewFolder;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.BasicAccessories;
using ThoriumMod.Items;
using InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.SuperCell;

namespace InfernalEclipseWeaponsDLC.Content.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Wings)]
    public class GarudaWings : ModItem
    {
        private int supercellWingTime = 170;

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 28;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void UpdateEquip(Player player)
        {
            const int supercellWingTime = 170;
            if (player.wings <= 0 || player.wingTimeMax < supercellWingTime)
            {
                player.wings = SuperCellCirclet.wingsSlot;
                player.wingsLogic = ArmorIDs.Wing.BeetleWings;
                player.wingTimeMax = supercellWingTime;
                player.noFallDmg = true;
            }
        }
    }

    public class GarudaWingsShimmer : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<GarudaWings>() ||
                   entity.type == ModContent.ItemType<SuperCellGuard>();
        }

        public override void SetStaticDefaults()
        {
            // Garuda Wings -> SuperCell Guard
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<GarudaWings>()] =
                ModContent.ItemType<SuperCellGuard>();

            // SuperCell Guard -> Garuda Wings
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<SuperCellGuard>()] =
                ModContent.ItemType<GarudaWings>();
        }
    }
}
