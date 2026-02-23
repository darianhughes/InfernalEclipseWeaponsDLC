using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.StarScepter
{
    public class StarScepterBolt : ModProjectile
    {
        private static Texture2D TextureGlow;
        private static Texture2D TextureGlow2;
        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        public Color DrawColor;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.6f;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 3;
            TextureGlow ??= ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureGlow2 ??= ModContent.Request<Texture2D>(Texture + "_Glow2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            DrawColor = new Color(255, 250, 127);
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Projectile.ai[2] || Projectile.ai[1] == 1f) return false;
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = 1f;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (Projectile.timeLeft < 260)
            {
                Projectile.localAI[0]++;
            }

            if (Projectile.localAI[1] == 0f)
            { // random texture
                Projectile.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item110.WithPitchOffset(Main.rand.NextFloat(-0.5f, 0.5f)).WithVolumeScale(0.5f), Projectile.Center);
                Projectile.ai[1] = Main.rand.NextFloat(MathHelper.TwoPi);

                byte colorDiff = (byte)(Main.rand.Next(3) * 50);
                DrawColor.G -= colorDiff;
                DrawColor.B -= (byte)(colorDiff * 2);
            }

            if (Projectile.timeLeft <= 30)
            {
                Projectile.ai[1] = 1f;
                Projectile.friendly = false;
            }
            else if (Projectile.timeLeft <= 290)
            {
                Projectile.friendly = true;
            }

            if (Projectile.ai[1] == 1f)
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }

                Projectile.velocity *= 0.5f;
            }
            else
            {
                NPC target = Main.npc[(int)Projectile.ai[2]];

                if (target.active && !target.friendly && !target.CountsAsACritter && !target.dontTakeDamage && Projectile.timeLeft > 30)
                {
                    Projectile.velocity += (target.Center - Projectile.Center) * (0.003f + Projectile.localAI[0] * 0.0001f);
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 3.8f;
                }
                else if (Projectile.timeLeft > 30)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);
            if (OldPosition.Count > 30)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            float colorMult = 1f;
            if (Projectile.timeLeft < 20) colorMult *= Projectile.timeLeft / 20f;

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = OldPosition[i] - Main.screenPosition;
                spriteBatch.Draw(TextureGlow, drawPosition2, null, DrawColor * 0.03f * (i + 1) * colorMult, OldRotation[i], TextureGlow.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(TextureGlow2, drawPosition, null, DrawColor * colorMult * 0.6f, Projectile.ai[1], TextureGlow2.Size() * 0.5f, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, drawPosition, null, Color.White * colorMult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
