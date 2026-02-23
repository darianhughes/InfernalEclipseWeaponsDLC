using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.TetherBlade
{
    public class TetherBladeProjectileBlink : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Melee/TetherBlade";

        private static Texture2D TextureLink;
        public int TimeSpent;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        Color color = Color.White;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            TextureLink ??= ModContent.Request<Texture2D>(Texture + "Link", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.alpha = 255;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.netImportant = true;

            Projectile.localAI[1] = Main.rand.NextBool() ? 1f : -1f; // random rotation direction
            color = Main.rand.Next(3) switch
            { // random trail color
                0 => new Color(119, 179, 247),
                1 => new Color(188, 119, 247),
                _ => new Color(247, 119, 224)
            };
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.Length() > 1f)
            {
                if (Projectile.localAI[0] == 0)
                { // sound only plays once
                    Projectile.localAI[0] = 1f;
                    SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, Projectile.Center);
                }

                Projectile.velocity = oldVelocity * -0.5f;
            }
            return false;
        }

        public override void AI()
        {
            TimeSpent++;

            if (TimeSpent == 1)
            {
                SoundEngine.PlaySound(SoundID.Item46.WithPitchOffset(Main.rand.NextFloat(0.5f)).WithVolumeScale(0.5f), Projectile.Center);
            }

            if (TimeSpent % 61 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse.WithPitchOffset(Main.rand.NextFloat(0.5f)).WithVolumeScale(0.5f), Projectile.Center);
            }

            if (TimeSpent > 10)
            {
                Projectile.velocity *= 0.9f;
                Projectile.rotation += TimeSpent * 0.001f * Projectile.localAI[1];
            }

            if (TimeSpent > 15)
            {
                Projectile.rotation += 0.1f * Projectile.localAI[1];
            }
            else Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Projectile.timeLeft > 8)
            {
                OldPosition.Add(Projectile.Center);
                OldRotation.Add(Projectile.rotation);

                if (OldPosition.Count > 30)
                {
                    OldPosition.RemoveAt(0);
                    OldRotation.RemoveAt(0);
                }
            }
            else if (OldPosition.Count > 1)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
                Projectile.friendly = false;
            }

            if (Main.rand.NextBool(2))
            {
                int dustType = Main.rand.Next(2) switch
                {
                    0 => DustID.BlueCrystalShard,
                    _ => DustID.PurpleCrystalShard
                };

                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
                dust.noGravity = true;
                dust.alpha = Main.rand.Next(80, 120);
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
                dust.velocity *= 0.75f;
                dust.velocity += Projectile.velocity * 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            float colorMult = Projectile.timeLeft > 30 ? 1f : Projectile.timeLeft / 30f;

            // Draw code here

            Player player = Main.player[Projectile.owner];
            Vector2 dir = Projectile.Center - player.Center;
            float abs = -1f;
            while (dir.Length() > 50)
            {
                abs += 0.1f;
                float scalemult2 = 1.5f + Math.Abs(abs);

                if (scalemult2 > 2.5f)
                {
                    scalemult2 = 2.5f;
                }

                float colorMult2 = Math.Abs(1f - Math.Abs(abs));
                float newDist = player.Center.Distance(Projectile.Center);
                if (newDist > 700f)
                {
                    colorMult2 *= 1f - (newDist - 700f) * 0.003f;
                    if (colorMult2 < 0f) colorMult2 = 0f;
                }
                Vector2 drawPosition2 = Vector2.Transform(player.Center + dir - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(TextureLink, drawPosition2, null, color * colorMult * colorMult2 * 0.5f, dir.ToRotation(), TextureLink.Size() * 0.5f, Projectile.scale * scalemult2, SpriteEffects.None, 0f);
                dir -= Vector2.Normalize(dir) * TextureLink.Width * 0.8f;
            }

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(texture, drawPosition2, null, color * 0.0225f * i * colorMult, OldRotation[i], texture.Size() * 0.5f, Projectile.scale * i * 0.035f, SpriteEffects.None, 0f);
            }

            // Draw code ends here

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(texture, drawPosition, null, lightColor * colorMult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
