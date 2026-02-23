using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.RestoredDeepSeaDrawl
{
    public class SharknadoBolt : BardProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;

        public override void SetBardDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Typhoon);
            AIType = ProjectileID.Typhoon;

            // Enable local i-frames
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            base.AI();

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        // Typhoon’s tile collision isn’t inherited; emulate its bounce here.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Dust + impact sound like vanilla bounces
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // Reflect on the axes that actually collided, with light damping.
            const float damp = 0.95f; // tweak if you want “springier” (1.0f) or softer (~0.8f) bounces
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * damp;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * damp;

            // Do NOT kill the projectile on collision (Typhoon bounces)
            return false;
        }

        // Custom draw to fix rotation center
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);

            Vector2 origin = new Vector2(sourceRect.Width / 2f, frameHeight / 2f); // true center of the frame

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0
            );

            return false; // we handled the drawing
        }
    }
}
