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


            // Forward offset (muzzle distance)
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 30f;

            // Consistent vertical offset (always upwards relative to player)
            Vector2 verticalOffset = new Vector2(0f, -2f * player.gravDir);

            // Final spawn position
            Vector2 spawnPos = position + muzzleOffset + verticalOffset;


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

                modPlayer.shotCounter = 0;
            }

            else
            {
                // Normal bullet
                Projectile.NewProjectile(source, spawnPos, perturbedVelocity,
                    type, damage, knockback, player.whoAmI);
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

        public override void HoldItem(Player player)
        {
            if (player.itemAnimation > 0) // while using
            {
                Item.scale = 0.66f; // one-third smaller
            }
            else
            {
                Item.scale = 1f; // normal size
            }
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
