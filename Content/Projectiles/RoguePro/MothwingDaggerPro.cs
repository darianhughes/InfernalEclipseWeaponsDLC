using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro
{
    public class MothwingDaggerPro : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Rogue/MothwingDagger";

        private static Texture2D TextureGlow;
        private static Texture2D TextureGlow2;
        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public Color DrawColor;
        private bool fadingOut => Projectile.ai[1] == 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = true;
            Projectile.timeLeft = 540;

            // Make the sprite 1.5x larger visually
            Projectile.scale = 1.5f;

            Projectile.extraUpdates = 3;
            TextureGlow ??= ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/StarScepter/StarScepterBolt_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureGlow2 ??= ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/MagicPro/StarScepter/StarScepterBolt_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            
            if (Main.rand.NextBool(40))
            {
                Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.GlowingMushroom,
                    Projectile.velocity.X * 0.25f,
                    Projectile.velocity.Y * 0.25f,
                    75,
                    Color.White,
                   0.8f
                );
            }

            if (Main.rand.NextBool(40))
            {
                Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f,
                    75,
                    Color.LightSeaGreen,
                    0.9f
                );
            }

            // 🔹 Gentle homing logic
            float homingRange = 800f;
            float turnSpeed = 0.01f;
            NPC target = null;

            // Find the closest valid target within range
            float closestDist = homingRange;
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < closestDist)
                    {
                        closestDist = distance;
                        target = npc;
                    }
                }
            }

            if (target != null)
            {
                // Calculate the desired direction toward the target
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Vector2 current = Projectile.velocity.SafeNormalize(Vector2.Zero);

                // Interpolate between current and desired direction to "curve" smoothly
                Vector2 newDirection = Vector2.Lerp(current, desired, turnSpeed).SafeNormalize(Vector2.Zero);

                // Maintain original speed
                Projectile.velocity = newDirection * Projectile.velocity.Length();
            }

            // Record old positions for trail
            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);

            if (OldPosition.Count > 30) // or fewer if you want a shorter trail
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }


            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.timeLeft % 45 == 3)
                {
                    // Fire 1 IlluminantBolt in a light random direction
                    Vector2 randomDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                    randomDir.Normalize();
                    randomDir *= Main.rand.NextFloat(0.77f, 1.33f); // random launch speed

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        randomDir,
                        ModContent.ProjectileType<IlluminantBolt>(),
                        Projectile.damage / 2,
                        Projectile.knockBack / 2,
                        Projectile.owner
                    );
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.Calamity().stealthStrike)
            {
                for (int w = 0; w < 3; w++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(30, 30, 30);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        ModContent.ProjectileType<IlluminantBolt>(),
                        Projectile.damage / 4,
                        Projectile.knockBack / 4,
                        Main.myPlayer,
                        1f
                    );
                }
            }

            // 🔹 Random number of dusts between 1–3 for each type
            int mushCount = Main.rand.Next(2, 4);
            int cloudCount = Main.rand.Next(2, 4);

            for (int i = 0; i < mushCount; i++)
            {
                float speedMult = Main.rand.NextFloat(0.8f, 2f);
                int dustIndex = Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.GlowingMushroom,
                    Projectile.velocity.X * 0.25f * speedMult,
                    Projectile.velocity.Y * 0.25f * speedMult,
                    75,
                    Color.White,
                    0.8f
                );
                Main.dust[dustIndex].noGravity = true;
            }

            for (int i = 0; i < cloudCount; i++)
            {
                float speedMult = Main.rand.NextFloat(0.8f, 2f);
                int dustIndex = Dust.NewDust(
                    Projectile.position + Projectile.velocity,
                    Projectile.width,
                    Projectile.height,
                    DustID.Cloud,
                    Projectile.velocity.X * 0.5f * speedMult,
                    Projectile.velocity.Y * 0.5f * speedMult,
                    75,
                    Color.LightSeaGreen,
                    0.9f
                );
                Main.dust[dustIndex].noGravity = true;
            }

            Projectile.ai[1] = 1f;
            Projectile.friendly = false;
            Projectile.tileCollide = false;

            // Optional: shorten remaining life so it fades quickly
            if (Projectile.timeLeft > 30)
                Projectile.timeLeft = 30;

            if (fadingOut)
            {
                Projectile.velocity *= 0.33f;
                return;
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            // --- Additive pass for glows ---
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState,
                DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            float colorMult = 1f;
            if (Projectile.timeLeft < 20)
                colorMult *= Projectile.timeLeft / 20f;

            // Gradient glow colors
            Color startColor = new Color(0, 180, 255); // blue
            Color endColor = new Color(0, 255, 120);   // green

            // Trail
            for (int i = 0; i < OldPosition.Count; i++)
            {
                float progress = (float)i / (OldPosition.Count - 1);
                Color trailColor = Color.Lerp(startColor, endColor, progress);
                trailColor *= 0.03f * (i + 1) * colorMult;

                Vector2 drawPosition2 = OldPosition[i] - Main.screenPosition;
                spriteBatch.Draw(TextureGlow, drawPosition2, null, trailColor, OldRotation[i],
                    TextureGlow.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
            }

            // Main glow
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color centerGlowColor = Color.Lerp(startColor, endColor, 0.5f) * (0.6f * colorMult);
            spriteBatch.Draw(TextureGlow2, drawPosition, null, centerGlowColor, Projectile.ai[1],
                TextureGlow2.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);

            // --- End additive pass ---
            spriteBatch.End();

            // --- Normal alpha pass for base sprite ---
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            // Draw the base projectile normally (non-glowing)
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation,
                texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
