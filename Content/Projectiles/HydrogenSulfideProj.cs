using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles
{
    public class HydrogenSulfideProj : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 36;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255; // fully invisible
        }

        public override void AI()
        {
            // Make some wispy, drifting dust
            int dust = Dust.NewDust(Projectile.Center, 0, 0, DustID.OrangeTorch,
                Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 140, default, Main.rand.NextFloat(0.7f, 1.1f));
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.2f;
            Main.dust[dust].fadeIn = 1.2f;

            // Homing
            float homingRange = 320f;
            float lerpAmount = 0.13f;
            NPC closest = null;
            float dist = homingRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this) && !npc.friendly && npc.active)
                {
                    float currDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (currDist < dist)
                    {
                        dist = currDist;
                        closest = npc;
                    }
                }
            }

            if (closest != null)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(closest.Center) * Projectile.velocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, lerpAmount);
            }

            // Faintly shrink over time for wispiness
            Projectile.scale *= 0.98f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 90); // 1.5 seconds poison
        }

        public override bool PreDraw(ref Color lightColor) => false; // completely invisible
    }
}
