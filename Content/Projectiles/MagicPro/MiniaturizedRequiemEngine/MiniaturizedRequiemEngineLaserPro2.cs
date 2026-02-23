using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine
{
    public class MiniaturizedRequiemEngineLaserPro2 : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/MiniaturizedRequiemEngine/MiniaturizedRequiemEngineLaserPro";

        private const int MaxBounces = 3;
        private const int InitialLifetime = 60;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.scale = 0.6f;
            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            // Rotate to velocity
            if (Projectile.velocity.Length() > 0.1f)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Blue light glow
            Lighting.AddLight(
                Projectile.Center,
                0.1f,  // R
                0.4f,  // G
                0.9f   // B
            );

            /*
            // Blue dust trail
            int dust = Dust.NewDust(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.BlueTorch,
                0f,
                0f,
                150,
                default,
                1.1f
            );
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity *= 0.1f;
            */

            // Shrink during second half of lifetime
            float halfLife = InitialLifetime * 0.8f;

            if (Projectile.timeLeft < halfLife)
            {
                float progress = Projectile.timeLeft / halfLife; // 1 - 0
                Projectile.scale = 0.6f * MathHelper.Clamp(progress, 0f, 1f);
            }


        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Blue tint (white sprite → blue laser)
            return new Color(100, 160, 255, 200);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2f;

            // Additive blue glow
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Additive,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                Main.Rasterizer,
                null,
                Main.Transform
            );

            Main.spriteBatch.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                new Color(100, 160, 255, 180),
                Projectile.rotation,
                origin,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0f
            );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred,
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
