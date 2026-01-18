using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine
{
    public class MiniaturizedRequiemEngineGatlingPro : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        private static Texture2D GlowTex;

        private int hitCounter;
        private bool FadingOut => Projectile.ai[1] == 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;

            Projectile.scale = 1.2f;
            Projectile.extraUpdates = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            GlowTex ??= ModContent.Request<Texture2D>(
                "InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/StarScepter/StarScepterBolt_Glow",
                ReLogic.Content.AssetRequestMode.ImmediateLoad
            ).Value;
        }

        public override void AI()
        {
            if (FadingOut)
            {
                Projectile.velocity *= 0.5f;
                Projectile.alpha += 15;
                return;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.alpha > 0)
                Projectile.alpha -= 125;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            float lightStrength = (255f - Projectile.alpha) / 255f;
            Lighting.AddLight(
                Projectile.Center,
                1.0f * lightStrength,
                0.25f * lightStrength,
                0.7f * lightStrength
            );

            if (Projectile.velocity.Length() < 12f)
                Projectile.velocity *= 1.0025f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitCounter++;

            if (hitCounter >= 4 && !FadingOut)
            {
                Projectile.ai[1] = 1f;
                Projectile.friendly = false;
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Zero;

                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;

                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha < 200)
            {
                int c = 255 - Projectile.alpha;
                return new Color(c, c, c, 0);
            }
            return Color.Transparent;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            float forwardOffset = -24f;

            sb.End();
            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.Additive,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.Transform
            );

            float fadeMult = 1f;
            if (Projectile.timeLeft < 20)
                fadeMult = Projectile.timeLeft / 20f;

            Color trailColor = new Color(255, 90, 200); // pink

            // SMOOTH TRAIL USING oldPos
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float progress = 1f - i / (float)Projectile.oldPos.Length;
                Color c = trailColor * progress * 0.95f * fadeMult;

                Vector2 drawPos =
                    Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;

                sb.Draw(
                    GlowTex,
                    drawPos,
                    null,
                    c,
                    Projectile.rotation,
                    GlowTex.Size() * 0.5f,
                    Projectile.scale * progress,
                    SpriteEffects.None,
                    0f
                );
            }

            // MAIN LASER HEAD
            sb.Draw(
                tex,
                Projectile.Center - Main.screenPosition + forward * forwardOffset,
                null,
                Projectile.GetAlpha(lightColor),
                Projectile.rotation,
                tex.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            sb.End();
            sb.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.Transform
            );

            return false;
        }
    }
}
