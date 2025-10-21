using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.BarrenGarden
{
    public class BarrenGardenProHoming : ModProjectile
    {
        private int timer;
        private int dustTimer;

        public override void SetDefaults()
        {
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.width = 18;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150) * Projectile.Opacity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void AI()
        {
            timer++;
            dustTimer++;

            // Create less dust — once every 6 frames instead of 3
            if (dustTimer >= 6)
            {
                dustTimer = 0;

                // Use Frostburn dust
                int dust = Dust.NewDust(
                    Projectile.Center,
                    0,
                    0,
                    DustID.Frost,
                    Projectile.velocity.X * 0.1f,
                    Projectile.velocity.Y * 0.1f,
                    100,
                    default,
                    1f
                );
                Main.dust[dust].noGravity = true;
            }

            // Manual NPC targeting
            NPC target = null;
            float maxDetectRadius = 500f;
            float sqrMaxDetectRadius = maxDetectRadius * maxDetectRadius;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDistanceToNPC = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDistanceToNPC < sqrMaxDetectRadius)
                    {
                        sqrMaxDetectRadius = sqrDistanceToNPC;
                        target = npc;
                    }
                }
            }

            // Homing logic
            if (target != null)
            {
                timer = 0;
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 15f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.15f);
            }

            // Kill if no target for too long
            if (timer >= 100 && target == null)
            {
                Projectile.Kill();
            }

            // Match rotation to velocity
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override void OnKill(int timeLeft)
        {
            // Smaller burst of Frostburn dust on death
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Frost,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    0,
                    default,
                    1f
                );
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
