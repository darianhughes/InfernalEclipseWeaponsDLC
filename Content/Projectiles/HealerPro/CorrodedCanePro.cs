using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;


namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro
{
    public class CorrodedCanePro : ModProjectile
    {
        public static readonly SoundStyle SpawnSound = OldDukeVortex.SpawnSound;
        public override string Texture => ModContent.GetInstance<OldDukeVortex>().Texture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 408;
            Projectile.height = 408;
            Projectile.scale = 0.004f;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800;
            CooldownSlot = 1;
        }

        public override void AI()
        {
            if (Projectile.scale < 1f)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha--;
                }
                Projectile.scale += 0.004f;
                if (Projectile.scale > 1f)
                {
                    Projectile.scale = 1f;
                }
                Projectile.width = (Projectile.height = (int)(408f * Projectile.scale));
            }
            else if (Projectile.timeLeft <= 85)
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 3;
                }
                Projectile.scale += 0.012f;
                Projectile.width = (Projectile.height = (int)(408f * Projectile.scale));
            }
            else
            {
                Projectile.width = (Projectile.height = 408);
            }
            Projectile.velocity = Vector2.Normalize(new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center) * 1.5f;
            Projectile.rotation -= 0.1f * (float)(1.0 - (double)Projectile.alpha / 255.0);
            float lightAmt = 2f * Projectile.scale;
            Lighting.AddLight(Projectile.Center, lightAmt, lightAmt * 2f, lightAmt);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 174;
                SoundEngine.PlaySound(in SpawnSound, Projectile.Center);
            }
            if (Projectile.timeLeft <= 85)
            {
                return;
            }
            int numDust = (int)(512.70795f * Projectile.scale);
            float angleIncrement = (float)Math.PI * 2f / (float)numDust;
            Vector2 dustOffset = new Vector2(408f * Projectile.scale, 0f);
            dustOffset = dustOffset.RotatedByRandom(6.2831854820251465);
            int var = (int)(408f * Projectile.scale);
            for (int j = 0; j < numDust; j++)
            {
                if (Main.rand.NextBool(var))
                {
                    dustOffset = dustOffset.RotatedBy(angleIncrement);
                    int dust = Dust.NewDust(Projectile.Center, 1, 1, 75);
                    Main.dust[dust].position = Projectile.Center + dustOffset;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Normalize(Projectile.Center - Main.dust[dust].position) * 10f;
                    Main.dust[dust].scale = Projectile.scale * 3f;
                }
            }
            float distanceRequired = 800f * Projectile.scale;
            float succPower = 0.5f;
            for (int i = 0; i < 255; i++)
            {
                Player player = Main.player[i];
                float distance = Vector2.Distance(player.Center, Projectile.Center);
                if (distance < distanceRequired && player.grappling[0] == -1 && Collision.CanHit(Projectile.Center, 1, 1, player.Center, 1, 1))
                {
                    float distanceRatio = distance / distanceRequired;
                    float wingTimeSet = (float)Math.Ceiling((double)((float)player.wingTimeMax * 0.5f * distanceRatio));
                    if (player.wingTime > wingTimeSet)
                    {
                        player.wingTime = wingTimeSet;
                    }
                    float multiplier = 1f - distanceRatio;
                    if (player.Center.X < Projectile.Center.X)
                    {
                        player.velocity.X += succPower * multiplier;
                    }
                    else
                    {
                        player.velocity.X -= succPower * multiplier;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.timeLeft <= 1680)
            {
                return Projectile.timeLeft > 85;
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, 210f * Projectile.scale, targetHitbox);
        }
    }
}
