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
        private const int TinyHoldTime = 45;

        private int SoundRepeatDelay = 30;

        private const int TotalLifetime = GrowTime + HoldTime + ShrinkTime + TinyHoldTime;

        private const float ColorPulseSpeed = 0.04f;

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

            // Fake looping vanilla sound
            Projectile.localAI[1]++;

            if (Projectile.localAI[1] >= SoundRepeatDelay)
            {
                Projectile.localAI[1] = 0f;

                SoundEngine.PlaySound(
                    SoundID.Item15 with
                    {
                        Volume = MathHelper.Clamp(Projectile.scale / 6f, 0.25f, 0.7f),
                        PitchVariance = 0.05f
                    },
                    Projectile.Center
                );
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

            // Phase boundaries
            int growEnd = GrowTime;
            int holdEnd = growEnd + HoldTime;
            int shrinkEnd = holdEnd + ShrinkTime;
            int tinyEnd = shrinkEnd + TinyHoldTime;

            if (Projectile.ai[0] <= growEnd)
            {
                // Grow
                float t = Projectile.ai[0] / (float)GrowTime;
                float eased = MathHelper.SmoothStep(0f, 1f, t);
                newScale = BaseScale * MaxScaleMultiplier * eased;
            }
            else if (Projectile.ai[0] <= holdEnd)
            {
                // Full-size hold
                newScale = BaseScale * MaxScaleMultiplier;
            }
            else if (Projectile.ai[0] <= shrinkEnd)
            {
                // Shrink
                float t = (Projectile.ai[0] - holdEnd) / (float)ShrinkTime;
                float eased = t * t;
                newScale = BaseScale * MaxScaleMultiplier * (1f - eased);
            }
            else
            {
                // Tiny hold before death
                newScale = BaseScale * 0.05f; // tiny size (adjust if needed)
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
