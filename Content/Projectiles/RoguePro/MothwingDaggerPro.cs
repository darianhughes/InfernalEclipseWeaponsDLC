using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro
{
    public class MothwingDaggerPro : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Rogue/MothwingDagger";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;

            // Make the sprite 1.5x larger visually
            Projectile.scale = 1.5f;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            // 🔹 Reduced dust spawn rates (less prominent)
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.GlowingMushroom,
                    Projectile.velocity.X * 0.25f,
                    Projectile.velocity.Y * 0.25f,
                    75,
                    Color.White,
                    0.8f
                );
            }

            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f,
                    75,
                    Color.LightSeaGreen,
                    0.9f
                );
            }

            // 🔹 Gentle homing logic
            float homingRange = 800f;
            float turnSpeed = 0.025f;
            NPC target = null;

            // Find the closest valid target within range
            float closestDist = homingRange;
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        target = npc;
                    }
                }
            }

            if (target != null)
            {
                // Calculate the desired direction toward the target
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Vector2 current = Projectile.velocity.SafeNormalize(Vector2.Zero);

                // Interpolate between current and desired direction to "curve" smoothly
                Vector2 newDirection = Vector2.Lerp(current, desired, turnSpeed).SafeNormalize(Vector2.Zero);

                // Maintain original speed
                Projectile.velocity = newDirection * Projectile.velocity.Length();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 15 == 3)
                {
                    // Fire 1 IlluminantBolt in a light random direction
                    Vector2 randomDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    randomDir.Normalize();
                    randomDir *= Main.rand.NextFloat(2f, 4f); // random launch speed

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        randomDir,
                        ModContent.ProjectileType<IlluminantBolt>(),
                        Projectile.damage / 2,
                        Projectile.knockBack / 2,
                        Projectile.owner
                    );
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
            {
                for (int w = 0; w < 3; w++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(30, 30, 30);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        ModContent.ProjectileType<IlluminantBolt>(),
                        Projectile.damage / 4,
                        Projectile.knockBack / 4,
                        Main.myPlayer,
                        1f
                    );
                }
            }

            // 🔹 Random number of dusts between 1–3 for each type
            int mushCount = Main.rand.Next(2, 4);
            int cloudCount = Main.rand.Next(2, 4);

            for (int i = 0; i < mushCount; i++)
            {
                float speedMult = Main.rand.NextFloat(0.8f, 2f);
                int dustIndex = Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.GlowingMushroom,
                    Projectile.velocity.X * 0.25f * speedMult,
                    Projectile.velocity.Y * 0.25f * speedMult,
                    75,
                    Color.White,
                    0.8f
                );
                Main.dust[dustIndex].noGravity = true;
            }

            for (int i = 0; i < cloudCount; i++)
            {
                float speedMult = Main.rand.NextFloat(0.8f, 2f);
                int dustIndex = Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.5f * speedMult,
                    Projectile.velocity.Y * 0.5f * speedMult,
                    75,
                    Color.LightSeaGreen,
                    0.9f
                );
                Main.dust[dustIndex].noGravity = true;
            }

            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
