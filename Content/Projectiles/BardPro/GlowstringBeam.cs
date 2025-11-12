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
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/UltimaRayMid"; // thin horizontal beam texture

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Get partner projectiles
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active)
            {
                Projectile.Kill();
                return;
            }

            Vector2 start = p1.Center;
            Vector2 end = p2.Center;
            Vector2 diff = end - start;

            Projectile.Center = start + diff / 2f;
            Projectile.width = (int)diff.Length();
            Projectile.height = 2; // beam thickness
            Projectile.rotation = diff.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active) return false;

            Vector2 start = p1.Center;
            Vector2 end = p2.Center;

            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height, ref collisionPoint);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile p1 = Main.projectile[(int)Projectile.ai[0]];
            Projectile p2 = Main.projectile[(int)Projectile.ai[1]];

            if (!p1.active || !p2.active)
                return false;

            Vector2 start = p1.Center;
            Vector2 end = p2.Center;
            Vector2 diff = end - start;

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            // The beam should be centered between the two projectiles
            Vector2 midPoint = (start + end) / 2f;

            // Origin = texture center so it stretches equally both ways
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);

            // Scale: X = beam thickness, Y = length relative to texture height
            Vector2 scale = new Vector2(2.5f, diff.Length() / tex.Height);

            // Rotation is along the direction of the line
            float rotation = diff.ToRotation() - MathHelper.PiOver2;

            // Draw the beam with 50% opacity
            Main.EntitySpriteDraw(
                tex,
                midPoint - Main.screenPosition,
                null,
                Color.White * 0.5f,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );

            return false; // Prevent default drawing
        }
    }
}
