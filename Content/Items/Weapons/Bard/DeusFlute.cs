using System;
using System.Collections.Generic;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using CalamityMod.Items;
using Terraria.ModLoader;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.DeusFlute;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class DeusFlute : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(2);
            Empowerments.AddInfo<Defense>(3);
            Empowerments.AddInfo<CriticalStrikeChance>(3);
        }

        public override void SetBardDefaults()
        {
            Item.width = 62;
            Item.height = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<DeusFlutePro>();
            Item.UseSound = ThoriumSounds.Flute_Sound;

            // TBD
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 1.5f;
            Item.damage = 200;
            Item.shootSpeed = 22f;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();

            InspirationCost = 2;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 6 * player.direction;
            player.itemLocation.Y -= 4;
        }

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                position += velocity * 1.25f;
                Projectile.NewProjectile(source, position.X, position.Y - 4, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockback, player.whoAmI, ai0: i % 2);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line1 = new(Mod, "DuesFluteLore", "They had found the source of the Astral Infection, and in a attempt to destroy it they used the Starcore on the infected altar.");
            line1.OverrideColor = Color.MediumPurple;
            tooltips.Add(line1);

            TooltipLine line2 = new(Mod, "DuesFluteLore2", "However, the altar had an unexpected reaction... two lights had started bouncing off each other heading the direction of the sky, opening a rift above it");
            line2.OverrideColor = Color.MediumPurple;
            tooltips.Add(line2);

            TooltipLine line3 = new(Mod, "DuesFluteLore3", "What came out of it was truly something out of this world, but they apreciation had to be cut short as the servant of the stars rushed towards them.");
            line3.OverrideColor = Color.MediumPurple;
            tooltips.Add(line3);
        }
    }
}
