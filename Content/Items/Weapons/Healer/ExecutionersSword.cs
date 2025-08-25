using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod;
using CalamityMod.Items;
using ThoriumMod.Items.HealerItems;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword;
using CalamityMod.Buffs.DamageOverTime;


namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class ExecutionersSword : ThoriumItem
    {
        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            healType = HealType.Ally;
            healAmount = 0;
            healDisplay = true;
            isHealer = true;
            Item.width = 64;
            Item.height = 68;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = new SoundStyle?(SoundID.Item1);

            Item.shoot = ModContent.ProjectileType<ExecutionersSwordSlashPro>();
            Item.shootsEveryUse = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;

            Item.shootSpeed = 10f;

            //temp until finished
            Item.scale = 1f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Right click
            {
                // Don't allow another projectile if one already exists
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ExecutionersSwordPro>()] > 0)
                {
                   return false;
                }

                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.shoot = ModContent.ProjectileType<ExecutionersSwordPro>();
                Item.shootsEveryUse = true;
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.knockBack = 3f;
            }
            else // Left click
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.shoot = ModContent.ProjectileType<ExecutionersSwordSlashPro>();
                Item.shootsEveryUse = true;
                Item.useTime = 16;
                Item.useAnimation = 16;
                Item.knockBack = 4f;
            }

            return true;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Right-click
            {
                // Direction to cursor
                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 dir = (mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);

                // Set projectile speed for flying sword
                float projectileSpeed = 30f;
                Vector2 finalVelocity = dir * projectileSpeed;

                // Optional: alternate rotation param
                float swingDir = (player.itemAnimation % 2 == 0) ? -1f : 1f;

                // Spawn right-click projectile
                Projectile.NewProjectile(
                    source,
                    player.Center,
                    finalVelocity,
                    type,      // Right-click projectile type (e.g., ExecutionersSwordPro)
                    damage / 2,
                    knockback,
                    player.whoAmI,
                    swingDir,
                    20f
                );

                return false; // Skip default shoot
            }

            // Left-click: do nothing here, keep existing behavior
            return true; // Let vanilla / CanUseItem handle the slash projectile
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HereticBreaker>()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient<CelestialFragment>(15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
