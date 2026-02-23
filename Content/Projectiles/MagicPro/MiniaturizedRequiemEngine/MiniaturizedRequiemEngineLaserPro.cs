using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine
{
    public class MiniaturizedRequiemEngineLaserPro : ModProjectile
    {
        private const int MaxBounces = 4;

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

            Projectile.penetrate = -1; // needed for bouncing
            Projectile.timeLeft = 240;

            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.scale = 0.6f;
            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
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


            // Spawn stationary helper projectile every frame
            int damage = (int)(Projectile.damage / 3f);

            Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<MiniaturizedRequiemEngineLaserPro2>(),
                damage,
                0f,
                Projectile.owner
            );
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= MaxBounces)
            {
                Projectile.Kill();
                return false;
            }

            // Bounce logic
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            //SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            return false;
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
                Projectile.scale * 1.0f,
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
