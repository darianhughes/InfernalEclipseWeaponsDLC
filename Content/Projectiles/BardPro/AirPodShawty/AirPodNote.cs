using CalamityMod.Buffs.Potions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.AirPodShawty
{
    public class AirPodNote : BardProjectile
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        // Store the chosen frame locally
        private int chosenFrame;
        private Color glowColor;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 120;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Pick a random frame (0,1,2)
            chosenFrame = Main.rand.Next(3);
            Projectile.frame = chosenFrame;

            // Assign glow color based on chosen frame
            switch (chosenFrame)
            {
                case 0: glowColor = Color.Yellow; break;
                case 1: glowColor = Color.CornflowerBlue; break;
                case 2: glowColor = Color.LimeGreen; break;
                default: glowColor = Color.White; break;
            }
        }

        public override void AI()
        {
            // Slowdown
            Projectile.velocity *= 0.98f;

            // Fade-out in last 30 ticks
            int fadeTime = 30;
            if (Projectile.timeLeft < fadeTime)
            {
                float fadeProgress = 1f - (Projectile.timeLeft / (float)fadeTime);
                Projectile.alpha = (int)MathHelper.Lerp(0, 255, fadeProgress);
            }

            // Optional rotation
            Projectile.rotation += Projectile.velocity.X * 0.05f;

            // Add light at projectile position
            // Multiply by 1.5 to make it clearly visible
            float intensity = 1.5f * (1f - Projectile.alpha / 255f);

            Lighting.AddLight(Projectile.Center,
                MathHelper.Clamp(glowColor.R / 255f * intensity, 0f, 1f),
                MathHelper.Clamp(glowColor.G / 255f * intensity, 0f, 1f),
                MathHelper.Clamp(glowColor.B / 255f * intensity, 0f, 1f)
            );
        }

        // Optional: draw glow overlay based on chosen color
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            var texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle sourceRect = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Vector2 origin = sourceRect.Size() / 2f;

            // Normal draw (with lighting)
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRect,
                Color.White * (1f - Projectile.alpha / 255f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            // Additive glow overlay
            Main.spriteBatch.End(); // End default SpriteBatch
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRect,
                glowColor * 0.75f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End(); // End additive
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false; // skip default draw
        }


        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.ElementalMix>(), 60 * 2);
        }
    }
}
