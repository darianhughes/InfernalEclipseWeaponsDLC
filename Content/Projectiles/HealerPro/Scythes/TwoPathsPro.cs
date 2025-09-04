using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Scythe;
using Terraria.DataStructures;
using ThoriumMod.Buffs;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TwoPathsPro : ScythePro
    {
        public override void SafeSetStaticDefaults()
        {
        }

        public override void SafeSetDefaults()
        {
            // Shared values
            dustOffset = new Vector2(-35, 7f);
            dustCount = 4;
            dustType = 279;
            rotationSpeed = 0.25f;
            Projectile.light = 1f;
            fadeOutSpeed = 30;
            Projectile.scale = 1f; // Normal visual
        }


        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Only large (left-click) version applies effects
            if (Projectile.ai[0] == 0 && Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<BlackExplosion>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );

                target.AddBuff(ModContent.BuffType<Wither>(), 300);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            float scale = Projectile.ai[0] == 0 ? 2f : 1f;
            Projectile.scale = scale;

            Vector2 oldCenter = Projectile.Center;
            Projectile.Size = new Vector2(208f, 190f) * scale;
            Projectile.Center = oldCenter;

            // Decide dust settings based on click type
            if (Projectile.ai[0] == 1) // Right-click version
            {
                dustOffset = new Vector2(-35, 7f);
                dustType = 175;
            }
            else // Left-click version
            {
                dustOffset = new Vector2(-70, 14f);
                dustType = 109;
            }
        }



        // Remove any resizing from PreAI/AI!
        public override bool PreAI()
        {
            // No resizing here!
            return base.PreAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Fade with projectile alpha
            lightColor *= MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f);

            // --- Draw the scythe itself ---
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            float baseVisualRot = Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                baseVisualRot,
                texture.Size() / 2f,
                Projectile.scale,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                0f
            );

            // --- Custom Afterimages ---
            Texture2D slashTexture = (Texture2D)ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Slash_3");

            // Fade like before
            float alphaFade = MathHelper.Lerp(0.35f, 0f, Projectile.alpha / 255f);

            // Define colors
            Color darkRed = new Color(120, 0, 0) * alphaFade;
            darkRed.A = 0;
            Color whiteGold = new Color(255, 240, 180) * alphaFade;
            whiteGold.A = 0;

            // Decide which color goes on which side
            Color leftColor, rightColor;
            if (Projectile.spriteDirection == 1) // facing right
            {
                leftColor = darkRed;
                rightColor = whiteGold;
            }
            else // facing left
            {
                leftColor = whiteGold;
                rightColor = darkRed;
            }

            // --- Left side ---
            Main.EntitySpriteDraw(
                slashTexture,
                Projectile.Center - Main.screenPosition,
                null,
                leftColor,
                Projectile.rotation + MathHelper.Pi - MathHelper.ToRadians(85f),
                slashTexture.Size() / 2f,
                Projectile.scale * 2.7f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                0f
            );

            // --- Right side ---
            Main.EntitySpriteDraw(
                slashTexture,
                Projectile.Center - Main.screenPosition,
                null,
                rightColor,
                Projectile.rotation - MathHelper.ToRadians(85f),
                slashTexture.Size() / 2f,
                Projectile.scale * 2.7f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                0f
            );

            return false; // Suppress default drawing
        }
    }
}
