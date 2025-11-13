using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Utilities;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro
{
    public class GlowstringBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/UltimaRayMid";

        // --- Easy-to-edit offset from projectile centers ---
        private const float BeamOffset = 12f; // pixels from projectile center

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
        }

        public override void AI()
        {
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active)
            {
                Projectile.Kill();
                return;
            }

            // --- Calculate beam with offset ---
            Vector2 direction = (p2.Center - p1.Center).SafeNormalize(Vector2.Zero);
            Vector2 start = p1.Center + direction * BeamOffset;
            Vector2 end = p2.Center - direction * BeamOffset;
            Vector2 diff = end - start;

            Projectile.Center = start + diff / 2f;
            Projectile.width = (int)diff.Length();
            Projectile.height = 1;
            Projectile.rotation = diff.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active) return false;

            Vector2 direction = (p2.Center - p1.Center).SafeNormalize(Vector2.Zero);
            Vector2 start = p1.Center + direction * BeamOffset;
            Vector2 end = p2.Center - direction * BeamOffset;

            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active)
                return false;

            Vector2 direction = (p2.Center - p1.Center).SafeNormalize(Vector2.Zero);
            Vector2 start = p1.Center + direction * BeamOffset;
            Vector2 end = p2.Center - direction * BeamOffset;
            Vector2 diff = end - start;
            float length = diff.Length();

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            float rotation = direction.ToRotation() - MathHelper.PiOver2;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);

            // --- PULSE ---
            // Brightness and width pulse together
            float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.5f + 0.5f;
            Color pulseColor = Color.White * MathHelper.Lerp(0.35f, 0.9f, pulse);

            // Slight width growth with pulse (1.0 → 1.3)
            float widthPulse = 1.0f + MathHelper.Lerp(0f, 0.3f, pulse);

            int segments = 3;
            for (int i = 0; i < segments; i++)
            {
                float t;
                float thickness;
                float alpha;

                if (i == 0) // left segment
                {
                    t = 0.05f;
                    thickness = 0.5f;
                    alpha = 1f;
                }
                else if (i == 2) // right segment
                {
                    t = 0.95f;
                    thickness = 0.5f;
                    alpha = 1f;
                }
                else // center
                {
                    t = 0.5f;
                    thickness = 1f;
                    alpha = 1f;
                }

                Vector2 segmentCenter = Vector2.Lerp(start, end, t);
                float segLength = (i == 1) ? length * 0.8f : length * 0.1f;

                // Apply pulsing width
                Vector2 scale = new Vector2(thickness * widthPulse, segLength / tex.Height);
                Color segmentColor = pulseColor * alpha;

                Main.spriteBatch.Draw(
                    tex,
                    segmentCenter - Main.screenPosition,
                    null,
                    segmentColor,
                    rotation,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f
                );

                // White glow overlay (also pulsing in width)
                Vector2 glowScale = new Vector2(thickness * 1.1f * widthPulse, segLength / tex.Height);
                Color glowColor = Color.White * (0.1f * alpha);

                Main.spriteBatch.Draw(
                    tex,
                    segmentCenter - Main.screenPosition,
                    null,
                    glowColor,
                    rotation,
                    origin,
                    glowScale,
                    SpriteEffects.None,
                    0f
                );
            }

            return false;
        }
    }
}
