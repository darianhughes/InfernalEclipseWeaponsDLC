using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine
{
    public class MiniaturizedRequiemEngineTheBigOnePro : ModProjectile
    {
        public override string Texture =>
            "InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/MiniaturizedRequiemEngine/MiniaturizedRequiemEngineLaserPro";

        private const float BaseScale = 0.6f;
        private const float MaxScaleMultiplier = 10f;

        private const int GrowTime = 210;
        private const int HoldTime = 120;
        private const int ShrinkTime = 60;

        private const int TotalLifetime = GrowTime + HoldTime + ShrinkTime;
        private const float ColorPulseSpeed = 0.04f;

        private SlotId loopingSoundSlot;

        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 1;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = TotalLifetime;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1.2f;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            // Start looping sound once
            if (!loopingSoundSlot.IsValid)
            {
                loopingSoundSlot = SoundEngine.PlaySound(WhirrSound, Projectile.Center);
            }

            // Keep sound attached to projectile
            if (SoundEngine.TryGetActiveSound(loopingSoundSlot, out ActiveSound activeSound))
            {
                activeSound.Position = Projectile.Center;

                // Optional: scale volume with size
                activeSound.Volume = MathHelper.Clamp(Projectile.scale / 6f, 0.2f, 1f);
            }

            // Continuous rotation
            Projectile.rotation += 0.15f * Projectile.direction;

            // Color pulse
            float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * ColorPulseSpeed);
            pulse = (pulse + 1f) * 0.5f; // 0 → 1
            Color orange = new Color(255, 140, 40);
            Color yellow = new Color(255, 220, 80);
            Color pulseColor = Color.Lerp(orange, yellow, pulse);

            Lighting.AddLight(
                Projectile.Center,
                pulseColor.R / 255f * Projectile.scale,
                pulseColor.G / 255f * Projectile.scale,
                pulseColor.B / 255f * Projectile.scale
            );

            // Scale phases
            float newScale;
            if (Projectile.ai[0] <= GrowTime)
            {
                float t = Projectile.ai[0] / (float)GrowTime;
                float eased = MathHelper.SmoothStep(0f, 1f, t);
                newScale = BaseScale * MaxScaleMultiplier * eased;
            }
            else if (Projectile.ai[0] <= GrowTime + HoldTime)
            {
                newScale = BaseScale * MaxScaleMultiplier;
            }
            else
            {
                float t = (Projectile.ai[0] - GrowTime - HoldTime) / (float)ShrinkTime;
                float eased = t * t;
                newScale = BaseScale * MaxScaleMultiplier * (1f - eased);
            }

            Projectile.scale = newScale;
        }

        // Scale the hitbox for NPC collisions
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int scaledWidth = (int)(Projectile.width * Projectile.scale);
            int scaledHeight = (int)(Projectile.height * Projectile.scale);

            hitbox = new Rectangle(
                (int)(Projectile.Center.X - scaledWidth / 2),
                (int)(Projectile.Center.Y - scaledHeight / 2),
                scaledWidth,
                scaledHeight
            );
        }

        private static readonly SoundStyle WhirrSound = SoundID.Item15 with
        {
            IsLooped = true,
            Volume = 0.6f
        };

        // Optional: circular collision (like FroststeelPulse)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float radius = Projectile.width / 2f * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
        }

        public override void OnKill(int timeLeft)
        {

            float lifeProgress = MathHelper.Clamp( Projectile.ai[0] / TotalLifetime, 0f, 1f);

            float damageScale = lifeProgress;

            // Stop looping sound
            if (SoundEngine.TryGetActiveSound(loopingSoundSlot, out ActiveSound activeSound))
            {
                activeSound.Stop();
            }

            int scaledDamage = (int)(Projectile.damage * 2f * lifeProgress);
            scaledDamage = Math.Max(1, scaledDamage); // safety

            Projectile.NewProjectile(
                Projectile.GetSource_Death(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro2>(),
                scaledDamage,
                0f,
                Projectile.owner
            );

            // Play a Calamity-style sound on death
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/TeslaCannonFire")
            {
                Volume = 1f,
                PitchVariance = 0.2f
            }, Projectile.Center);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * ColorPulseSpeed);
            pulse = (pulse + 1f) * 0.5f;

            Color orange = new Color(255, 140, 40);
            Color yellow = new Color(255, 220, 80);

            return Color.Lerp(orange, yellow, pulse) * 0.9f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = tex.Size() / 2f;

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
                Projectile.GetAlpha(lightColor),
                Projectile.rotation,
                origin,
                Projectile.scale,
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
