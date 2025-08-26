using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro
{
    public class ListoftheDamnedSoul_Orange : ModProjectile
    {
        public override string Texture => "ThoriumMod/Projectiles/Boss/LichFlare";

        private bool accelerated = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.usesLocalNPCImmunity = true;

            // Number of ticks before this projectile can hit the same NPC again
            Projectile.localNPCHitCooldown = 30; // 10 ticks = 1/6 second
        }

        public override void AI()
        {
            // Animate frames
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }

            // Orange-ish light glow
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.4f, 0f) * 0.6f);

            // Slow out, then dash forward
            if (!accelerated)
            {
                Projectile.velocity *= 0.98f;
                if (Projectile.velocity.Length() < 0.5f)
                {
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 80f;

                    Projectile.damage = Math.Max(1, Projectile.damage * 2);

                    accelerated = true;
                }
            }

            // Rotation matches velocity
            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            // Base dust (always)
            int baseDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 150, default(Color), 1.2f);
            Main.dust[baseDust].noGravity = true;
            Main.dust[baseDust].velocity *= 0.2f;

            // Extra dust trail AFTER acceleration
            if (accelerated)
            {
                for (int i = 0; i < 3; i++) // spawn 3 extra dust particles per tick
                {
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-Projectile.width / 2f, Projectile.width / 2f),
                                                 Main.rand.NextFloat(-Projectile.height / 2f, Projectile.height / 2f));
                    int dust = Dust.NewDust(Projectile.position + offset, 1, 1, DustID.Torch, 0f, 0f, 150, default(Color), 1.2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);

            Projectile.damage = Math.Max(1, Projectile.damage / 2);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Keep original sprite colors, but make trail/glow slightly transparent
            return new Color(255, 255, 255, 150) * 0.8f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float scale = 0.5f;

            // Draw trail (faded old positions)
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                // Center the old position
                Vector2 oldDrawPos = Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length) * 0.5f;
                Main.EntitySpriteDraw(texture, oldDrawPos, sourceRectangle, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
            }

            // Draw main projectile
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }

    }
}
