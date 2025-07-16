using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.DeusFlute
{
    public class DeusFluteStar : BardProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Starfury}";

        public int ColorType
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public int TargetNPC
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }



        public override void SetStaticDefaults()
        {
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.scale = 0.8f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.tileCollide = Projectile.Bottom.Y >= Main.npc[TargetNPC].Bottom.Y;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
            }

            Projectile.alpha -= 15;
            int targetAlpha = 150;
            if (Projectile.Center.Y >= Main.npc[TargetNPC].Bottom.Y)
                targetAlpha = 0;

            if (Projectile.alpha < targetAlpha)
                Projectile.alpha = targetAlpha;

            Projectile.localAI[0] += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * Projectile.direction;

            if (Main.rand.NextBool(4))
            {
                int dustType = ColorType == 1 ? ModContent.DustType<CalamityMod.Dusts.AstralBlue>() : ModContent.DustType<CalamityMod.Dusts.AstralOrange>();
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 127);
                dust.velocity *= 0.7f;
                dust.noGravity = true;
                dust.velocity += Projectile.velocity * 0.3f;
                if (Main.rand.NextBool(2))
                    dust.position -= Projectile.velocity * 4f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            int dustType = ColorType == 1 ? ModContent.DustType<CalamityMod.Dusts.AstralBlue>() : ModContent.DustType<CalamityMod.Dusts.AstralOrange>();
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default, 1.2f);
            }
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D aura = TextureAssets.Extra[ExtrasID.FallingStar].Value;
            Rectangle hitbox = new(0, 0, texture.Width, texture.Height);

            Vector2 origin = hitbox.Size() / 2f;
            Rectangle auraFrame = aura.Frame();
            Vector2 auraOrigin = new(aura.Frame().Width / 2f, 10f);

            float rotation = Projectile.rotation * 0.5f % ((float)Math.PI * 2f);
            if (rotation < 0f)
                rotation += (float)Math.PI * 2f;

            rotation /= (float)Math.PI * 2f;
            rotation = 1f - Utils.Remap(rotation, 0.15f, 0.5f, 0f, 1f) * Utils.Remap(rotation, 0.5f, 0.85f, 1f, 0f);
            float scale = Projectile.scale + 0.1f + rotation * 0.2f;

            Color starColor = new(229, 106, 89, 127);
            Color auraColor = Color.Lerp(Color.Orange, new Color(229, 106, 89, 64), rotation);
            Color trailColor = Color.Lerp(new Color(229, 106, 89, 127), new Color(255, 255, 255, 0) * 0.5f * 0.3f, rotation);

            if(ColorType > 0)
            {
                starColor = new(106, 231, 187, 127);
                auraColor = Color.Lerp(Color.Orange, new Color(106, 231, 187, 64), rotation);
                trailColor = Color.Lerp(new Color(106, 231, 187, 64), new Color(255, 255, 255, 0) * 0.5f * 0.3f, rotation);
            }

            Vector2 offsetY = new Vector2(0f, Projectile.gfxOffY) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;

            Main.EntitySpriteDraw(aura, Projectile.Center - Main.screenPosition + offsetY, auraFrame, auraColor, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, auraOrigin, Projectile.scale * 1.4f, SpriteEffects.None);
            Vector2 auraPos = Projectile.Center - Projectile.velocity * 0.5f;
            for (float f = 0f; f < 1f; f += 0.5f)
            {
                float sec = (float)Main.timeForVisualEffects / 60f;
                float timer = sec % 0.5f / 0.5f;
                timer = (timer + f) % 1f;
                float colorMult = timer * 2f;
                if (colorMult > 1f)
                    colorMult = 2f - colorMult;

                Main.EntitySpriteDraw(aura, auraPos - Main.screenPosition + offsetY, auraFrame, trailColor * colorMult, Projectile.velocity.ToRotation() + (float)Math.PI / 2f, auraOrigin, Projectile.scale * (0.5f + timer * 0.5f) * 0.75f, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), hitbox, starColor, Projectile.rotation, origin, scale, SpriteEffects.None);
            return false;
        }
    }
}