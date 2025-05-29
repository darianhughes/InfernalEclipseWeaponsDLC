using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using CalamityMod.Items;
using Terraria.ModLoader;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;
using ThoriumMod.Items.BardItems;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class BrimstoneHarp : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(3);
            Empowerments.AddInfo<CriticalStrikeChance>(3);
            Empowerments.AddInfo<Defense>(3);
        }

        public override void SetBardDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<BrimstoneHarpPro>();
            Item.UseSound = SoundID.Item26;

            // TBD
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.knockBack = 1.5f;
            Item.damage = 80;
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;

            InspirationCost = 3;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
            player.itemLocation.Y += 10;
        }

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
        }
    }
}
