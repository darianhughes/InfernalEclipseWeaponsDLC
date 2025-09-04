using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Projectiles.Scythe;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ThoriumMod.Buffs;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TheBlightProScythe : ScythePro
    {
        public override void SafeSetDefaults()
        {
            // Shared values
            dustOffset = new Vector2(-35, 7f);
            dustCount = 4;
            dustType = 75;
            Projectile.Size = new Vector2(140f, 150f);
        }
        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            base.SafeOnHitNPC(target, hit, damageDone);
        }

        public override void PostAI()
        {
            // Fade with projectile alpha
            float fade = 1f - (Projectile.alpha / 255f);

            // Add cursed flame green light (RGB in 0–1 range)
            Lighting.AddLight(
                Projectile.Center,
                new Vector3(0.05f, 1f, 0.05f) * fade
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Fade with projectile alpha
            lightColor *= MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f);

            // Main scythe texture
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                texture.Size() / 2f,
                Projectile.scale,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None),
                0f
            );

            // Cursed flame green afterimage
            Color cursedGreen = new Color(100, 255, 100) *
                                MathHelper.Lerp(0.15f, 0f, Projectile.alpha / 255f);
            cursedGreen.A = 0;

            // Use your mod’s Slash_3 texture
            Texture2D slashTexture = (Texture2D)ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Slash_3");

            // Two forward-facing afterimages
            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation, slashTexture.Size() / 2f, Projectile.scale * 1.85f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);

            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation, slashTexture.Size() / 2f, Projectile.scale * 1.85f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);

            // Two mirrored afterimages (rotated by 180°)
            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation + MathHelper.Pi, slashTexture.Size() / 2f, Projectile.scale * 1.85f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);

            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation + MathHelper.Pi, slashTexture.Size() / 2f, Projectile.scale * 1.85f,
                (Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0f);

            return false; // Suppress default drawing
        }
    }
}
