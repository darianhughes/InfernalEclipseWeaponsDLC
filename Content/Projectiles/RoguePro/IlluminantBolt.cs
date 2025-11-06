using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro
{
    public class IlluminantBolt : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        private int helixRot;
        private int activationTimer; // 🟢 Counts up to 60 ticks (1 second)
        private bool activated => activationTimer >= 90;

        private static Texture2D TextureGlow;
        private static Texture2D TextureGlow2;
        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public Color DrawColor;
        private bool fadingOut => Projectile.ai[1] == 1f;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 20;
            Projectile.scale = 1.1f;
            Projectile.extraUpdates = 3;
            TextureGlow ??= ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/StarScepter/StarScepterBolt_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureGlow2 ??= ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/StarScepter/StarScepterBolt_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            DrawColor = new Color(255, 250, 127);
        }

        public override void AI()
        {
            helixRot++;
            activationTimer++; // count frames since spawn

            // Soft cyan-green light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.9f, 0.8f);

            // Gentle drift
            Projectile.velocity *= 0.99f;

            /*
            // Small helix dust trail
            Vector2 helixOffset = HelixOffset();
            int dustIndex = Dust.NewDust(
                Projectile.Center + helixOffset - new Vector2(5f),
                0, 0,
                DustID.BlueTorch,
                0f, 0f,
                100,
                Color.Lerp(Color.Cyan, Color.Green, Main.rand.NextFloat(0.3f, 0.7f)),
                1.2f
            );
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0.1f;
            */

            // 🔸 Disable damage & homing for the first second
            if (!activated)
            {
                Projectile.friendly = false; // can't damage yet
                return; // skip homing logic until active
            }

            // Once active:
            Projectile.friendly = true;
            float homingRange = 600f;
            float turnSpeed = 0.033f;

            NPC target = FindClosestNPC(homingRange);
            if (target != null)
            {
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity.SafeNormalize(Vector2.Zero),
                    desired,
                    turnSpeed
                ) * Projectile.velocity.Length();

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f;
            }

            // Record old positions for trail
            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);

            if (OldPosition.Count > 30) // or fewer if you want a shorter trail
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }


            Projectile.rotation = Projectile.velocity.ToRotation();

            if (fadingOut)
            {
                Projectile.velocity *= 0.5f; // slow it down
                return; // skip normal logic while fading
            }
        }

        private Vector2 HelixOffset()
        {
            float wave = (float)Math.Sin(MathHelper.ToRadians(helixRot * 6f)) * 8f;
            float dir = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            return new Vector2(wave, 0f).RotatedBy(dir);
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closest = null;
            float sqrMax = maxDetectDistance * maxDetectDistance;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDist < sqrMax)
                    {
                        sqrMax = sqrDist;
                        closest = npc;
                    }
                }
            }
            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // trigger fade-out
            Projectile.ai[1] = 1f;
            Projectile.friendly = false;
            Projectile.tileCollide = false;

            // Optional: shorten remaining life so it fades quickly
            if (Projectile.timeLeft > 30)
                Projectile.timeLeft = 30;

            /*
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueTorch,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    100,
                    Color.Lerp(Color.Cyan, Color.Green, Main.rand.NextFloat(0.3f, 0.7f)),
                    1.3f
                );
                d.noGravity = true;
            }
            */

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            // Start additive blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            float colorMult = 1f;
            if (Projectile.timeLeft < 20)
                colorMult *= Projectile.timeLeft / 20f;

            // Define your gradient start and end colors
            Color startColor = new Color(0, 180, 255); // light blue
            Color endColor = new Color(0, 255, 120);   // bright green

            // Draw trail
            for (int i = 0; i < OldPosition.Count; i++)
            {
                float progress = (float)i / (OldPosition.Count - 1);
                Color trailColor = Color.Lerp(startColor, endColor, progress); // green → blue
                trailColor *= 0.03f * (i + 1) * colorMult; // fade with trail length

                Vector2 drawPosition2 = OldPosition[i] - Main.screenPosition;
                spriteBatch.Draw(
                    TextureGlow,
                    drawPosition2,
                    null,
                    trailColor,
                    OldRotation[i],
                    TextureGlow.Size() * 0.5f,
                    Projectile.scale * 0.8f,
                    SpriteEffects.None,
                    0f
                );
            }

            // Main glow burst and base texture
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color centerGlowColor = Color.Lerp(startColor, endColor, 0.5f) * (0.6f * colorMult); // mid-tone blend

            spriteBatch.Draw(TextureGlow2, drawPosition, null, centerGlowColor, Projectile.ai[1], TextureGlow2.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, Color.White * colorMult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            // End additive pass and resume normal alpha blending
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
