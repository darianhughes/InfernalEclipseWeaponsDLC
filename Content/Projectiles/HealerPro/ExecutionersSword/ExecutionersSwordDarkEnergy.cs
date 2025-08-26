using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using InfernalEclipseWeaponsDLC.Content.Buffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class ExecutionersSwordDarkEnergy : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";
        // Use invisible projectile texture

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;

            Projectile.usesLocalNPCImmunity = true;

            // Number of ticks before this projectile can hit the same NPC again
            Projectile.localNPCHitCooldown = 20; // 10 ticks = 1/6 second
        }

        public override void AI()
        {
            NPC target = null;
            float closestDist = 700f;

            // Find nearest valid NPC
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(null, false) && !npc.friendly)
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }
            }

            // Homing logic
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                float speed = 12f;
                Projectile.velocity = (Projectile.velocity * 20f + direction * speed) / 21f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }

            // Black dust trail
            for (int i = 0; i < 3; ++i)
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Smoke, // smoke dust works good for black effects
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    0,
                    default,
                    Main.rand.NextFloat(0.8f, 1.2f)
                );

                Dust dust = Main.dust[dustIndex];
                dust.color = Color.Black;
                dust.noGravity = true;
                dust.fadeIn = 1.1f;
                dust.velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(target.width / 2f, target.height / 2f);
                Vector2 spawnPos = target.Center + offset;
                Vector2 vel = offset.SafeNormalize(Vector2.UnitY) * 2f;

                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Smoke, vel, 150, Color.Black, 1.5f);
                dust.noGravity = true;
                dust.alpha = 0; // fully opaque
            }
        }


        // Prevent default sprite draw
        public override bool PreDraw(ref Color lightColor) => false;
    }
}
