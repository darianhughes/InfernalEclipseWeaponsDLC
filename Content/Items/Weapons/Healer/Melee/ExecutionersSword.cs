using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using ThoriumMod;
using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword;
using CalamityMod.Items;
using Terraria.DataStructures;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer.Melee
{
    public class ExecutionersSword : ThoriumItem
    {
        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            healType = HealType.Ally;
            healAmount = 0;
            healDisplay = true;
            isHealer = true;

            Item.width = 64;
            Item.height = 64;

            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ModContent.ProjectileType<ExecutionersSwordSlashPro>();
            Item.shootSpeed = 10f;

            Item.noMelee = false;
            Item.noUseGraphic = false;

            Item.scale = 1.25f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // right click
            {
                // Block use entirely if throw is already out
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ExecutionersSwordPro>()] > 0)
                    return false;

                // Suppress melee swing + graphic
                Item.noMelee = true;
                Item.noUseGraphic = true;
            }
            else
            {
                // Left click restores normal behavior
                Item.noMelee = true;
                Item.noUseGraphic = false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position,
                                   Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2) // right-click throw
            {
                // prevent multiple swords
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ExecutionersSwordPro>()] > 0)
                    return false;

                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 dir = (mouseWorld - player.Center).SafeNormalize(Vector2.UnitX);

                int proj = Projectile.NewProjectile(
                    source,
                    player.Center,
                    dir * 30f,
                    ModContent.ProjectileType<ExecutionersSwordPro>(),
                    damage / 2,        // half damage
                    knockback * 0.75f, // reduced knockback
                    player.whoAmI
                );

                NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                SoundEngine.PlaySound(SoundID.Item1, player.position);

                return false; // don’t use default shoot
            }
            else // left-click slash
            {
                int proj = Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<ExecutionersSwordSlashPro>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
                NetMessage.SendData(MessageID.SyncProjectile, number: proj);

                return false; // suppress extra default projectiles
            }
        }

        // Longer use time for right-click
        public override float UseTimeMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f; // 50% slower
            return 1f;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            if (player.altFunctionUse == 2)
                return 1.5f;
            return 1f;
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
