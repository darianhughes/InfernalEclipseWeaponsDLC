﻿using System.Collections.Generic;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using CalamityMod.Items;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Items.Placeables;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class DeepSeaDrawl : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(2);
            Empowerments.AddInfo<MovementSpeed>(2);
            Empowerments.AddInfo<CriticalStrikeChance>(2);
        }

        public override void SetBardDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<DeepSeaDrawlPro>();
            Item.UseSound = ThoriumSounds.Bard_Horn;

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.knockBack = 1.5f;
            Item.damage = 72;
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;

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

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for(int i = 0; i < 2; i++)
                Projectile.NewProjectileDirect(source, position, velocity.RotatedBy(MathHelper.Pi / 16 * (i == 0 ? -1f : 1f)) * 0.75f, ModContent.ProjectileType<HomingPlankton>(), damage / 2, knockback, Main.myPlayer);

            return true;
        }

        public override void AddRecipes()
        {
            //Adding a recipe for Infernum Mode only due chests not spawning in the Abyss in Infernum.
            if (ModLoader.TryGetMod("InfernumMode", out Mod _))
            {
                CreateRecipe()
                    .AddIngredient<AbyssGravel>(10)
                    .AddIngredient<PlantyMush>(5)
                    .AddIngredient(ItemID.Bone, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }
}
