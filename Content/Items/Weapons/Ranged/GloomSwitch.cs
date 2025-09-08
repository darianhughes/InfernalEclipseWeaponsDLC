using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using ThoriumMod.Buffs;
using InfernalEclipseWeaponsDLC.Content.Buffs;
using CalamityMod;
using Terraria.Audio;
using System;
using CalamityMod.Items;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RangedPro;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Ranged
{
    public class GloomSwitch : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6; // Fast SMG feel
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item11; // SMG-like sound
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;

            Item.width = 15;
            Item.height = 11;

            Item.scale = 0.66f; // permanent small sprite
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
                           Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer = player.GetModPlayer<GloomSwitchPlayer>();
            var rushPlayer = player.GetModPlayer<DarkRushPlayer>();

            modPlayer.shotCounter++;

            // --- Shot spread ---
            // Base spread 35°, reduced to 5° while Overclock/Dark Rush is active
            float maxSpread = rushPlayer.OverclockActive ? 5f : 25f;
            Vector2 perturbedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(maxSpread));


            // Always start from the center of the player
            Vector2 gunOrigin = player.MountedCenter;

            // Normalize velocity to get aim direction
            Vector2 aimDir = Vector2.Normalize(velocity);

            // Forward muzzle offset (always along aim)
            Vector2 muzzleOffset = aimDir * 30f;

            // Consistent vertical offset (account for gravity flip)
            Vector2 verticalOffset = new Vector2(0f, -4f * player.gravDir);

            // Final spawn position
            Vector2 spawnPos = gunOrigin + muzzleOffset + verticalOffset;



            if (modPlayer.shotCounter >= 15)
            {
                int projIndex = Projectile.NewProjectile(
                    source,
                    spawnPos,
                    perturbedVelocity,
                    ProjectileID.CursedFlameFriendly,
                    (int)(damage * 1.25f),
                    knockback,
                    player.whoAmI
                );

                if (projIndex >= 0 && Main.projectile[projIndex].active)
                {
                    Projectile proj = Main.projectile[projIndex];
                    proj.DamageType = DamageClass.Ranged; // <-- force ranged scaling
                    proj.penetrate = 2; // <-- set custom pierce
                }

                // Cursed flame effect
                int dustCount = 15;
                for (int i = 0; i < dustCount; i++)
                {
                    // Pick a random angle inside a cone (match bullet spread range)
                    float angle = Main.rand.NextFloat(-MathHelper.ToRadians(maxSpread), MathHelper.ToRadians(maxSpread));
                    Vector2 dustVelocity = velocity.RotatedBy(angle).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1f, 4f);

                    // Randomize spawn position slightly around muzzle
                    Vector2 dustPos = spawnPos + Main.rand.NextVector2Circular(8f, 8f);

                    int dustIndex = Dust.NewDust(spawnPos, 0, 0, 75, (dustVelocity.X * 2.5f), (dustVelocity.Y * 2.5f), 0, default, Main.rand.NextFloat(2.0f, 3.0f));
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 0f;
                }

                modPlayer.shotCounter = 0;
            }

            else
            {
                // Normal bullet
                Projectile.NewProjectile(source, spawnPos, perturbedVelocity,
                    type, damage, knockback, player.whoAmI);

                int dustCount = 5;
                for (int i = 0; i < dustCount; i++)
                {
                    // Pick a random angle inside a cone (match bullet spread range)
                    float angle = Main.rand.NextFloat(-MathHelper.ToRadians(maxSpread), MathHelper.ToRadians(maxSpread));
                    Vector2 dustVelocity = velocity.RotatedBy(angle).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(1f, 4f);

                    // Randomize spawn position slightly around muzzle
                    Vector2 dustPos = spawnPos + Main.rand.NextVector2Circular(8f, 8f);

                    int dustIndex = Dust.NewDust(spawnPos, 0, 0, 18, dustVelocity.X, dustVelocity.Y, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].fadeIn = 0f;
                }
            }

            return false; // prevent default
        }

        public override bool AltFunctionUse(Player player)
        {
            return true; // enables right-click functionality
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // right-click
            {
                var mp = player.GetModPlayer<DarkRushPlayer>();

                if (player.statMana >= 200 && !mp.OverclockActive)
                {
                    // Consume 200 mana
                    player.statMana -= 200;
                    player.ManaEffect(200);

                    // Play sound
                    SoundEngine.PlaySound(SoundID.Item113, player.position);

                    // Activate for 12s (720 ticks)
                    mp.ActivateRush(720);

                    // Visual dust burst
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Vector2 dir = angle.ToRotationVector2();
                        Vector2 spawnPos = player.Center + dir * Main.rand.NextFloat(0f, 20f);
                        Vector2 velocity = dir * Main.rand.NextFloat(2f, 5f);

                        int dust = Dust.NewDust(spawnPos, 0, 0, DustID.CorruptTorch, velocity.X, velocity.Y, 0, default, 1.8f);
                        Main.dust[dust].noGravity = true;
                    }

                    return false; // stop firing on right-click
                }

                return false; // can't right-click if not enough mana
            }

            return base.CanUseItem(player); // left-click normal shooting
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 2f); // adjust as needed
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class GloomSwitchPlayer : ModPlayer
    {
        public int shotCounter;

        public override void ResetEffects()
        {
            // Nothing persistent, so we don’t reset here
        }
    }

}
