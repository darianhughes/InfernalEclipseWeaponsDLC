using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.DukeSynth
{
    public class SulfurExplosionPro : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/SandPoisonCloudOldDuke";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 45; 
            Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 62;
            Projectile.alpha = 50;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.7f, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }

            // When all frames played, kill projectile
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.Kill();

            Projectile.velocity *= 0.995f;

            if (Math.Abs(Projectile.velocity.X) > 0f)
                Projectile.spriteDirection = -Projectile.direction;

            // Explosion visual burst/sound: simply play each time it's created
            if (Projectile.frame == 0 && Projectile.frameCounter == 1)
            {
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.Ghost,
                        Main.rand.NextVector2Circular(6f, 6f),
                        150
                    );
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Circular explosion hitbox, 28px radius (adjust as desired)
            float explosionRadius = 28f;
            return Vector2.Distance(Projectile.Center, targetHitbox.Center.ToVector2()) <= explosionRadius;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If you want to add afterimages or custom draw code, do it here.
            return true;
        }
    }
}
