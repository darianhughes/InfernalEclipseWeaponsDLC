using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using ThoriumMod.Buffs;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RangerPro;
using InfernalEclipseWeaponsDLC.Content.Buffs;
using CalamityMod;
using Terraria.Audio;
using System;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Ranger
{
    public class ButchersBloodmaker : ModItem
    {
        public override void SetDefaults()
        {
            // Base stats = shotgun mode
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 44;
            Item.useAnimation = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = false;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;

            Item.scale = 1.5f; // 1.5x bigger than default

            Item.width = 64;
            Item.height = 20;

            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // right-click
            {
                if (player.statLife > 50 && !player.GetModPlayer<BloodRagePlayer>().BloodRageActive)
                {
                    SoundEngine.PlaySound(SoundID.Item113, player.position);

                    // Consume life
                    player.statLife -= 50;

                    // Combat text
                    Color darkRed = new Color(180, 20, 20);
                    CombatText.NewText(player.Hitbox, darkRed, -50);

                    // Activate Blood Rage for 12 seconds (720 ticks)
                    player.GetModPlayer<BloodRagePlayer>().ActivateBloodRage(720);

                    // Blood burst effect
                    int dustCount = 30;
                    for (int i = 0; i < dustCount; i++)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        Vector2 spawnPos = player.Center + direction * Main.rand.NextFloat(0f, 20f);
                        Vector2 velocity = direction * Main.rand.NextFloat(2f, 5f);

                        int dustIndex = Dust.NewDust(spawnPos, 0, 0, DustID.Blood, velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(1.5f, 2.5f));
                        Main.dust[dustIndex].noGravity = true;
                        Main.dust[dustIndex].fadeIn = 0f;
                    }

                    return false; // prevent shooting on right-click
                }
                return false;
            }
            else
            {
                // Left-click (normal shooting)
                return base.CanUseItem(player);
            }
        }


        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
                           Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Apply Blood Rage bonus only if this weapon's buff is active
            if (player.GetModPlayer<BloodRagePlayer>().BloodRageActive)
            {
                damage = (int)(damage * 1.25f); // +25% damage
            }

            // Define the muzzle offset (distance in front of the gun)
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 64f;

            // --- Bullet spread ---
            int numberProjectiles = 4 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                float scale = 1f - Main.rand.NextFloat() * 0.1f;
                perturbedSpeed *= scale;

                Vector2 spawnPos = position + muzzleOffset;
                Projectile.NewProjectile(source, spawnPos, perturbedSpeed,
                    type, damage, knockback, player.whoAmI);
            }

            // --- Spawn BloodShotFriendly aligned with barrel ---
            Vector2 bloodSpawnPos = position + muzzleOffset;
            Projectile.NewProjectile(source, bloodSpawnPos, velocity * 2,
                ModContent.ProjectileType<BloodShotFriendly>(),
                damage, knockback * 0.5f, player.whoAmI);

            return false; // prevents default spawn
        }




        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f); // adjust as needed
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 12)
                .AddIngredient(ItemID.TissueSample, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
