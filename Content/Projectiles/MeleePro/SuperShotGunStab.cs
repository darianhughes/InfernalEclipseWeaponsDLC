using System;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Multi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro
{
    public class SuperShotgunStab : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Multi/SuperShotgun";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            //Projectile.aiStyle = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30; // initial, we’ll manage it in AI
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Must be the alt stab we spawned, same item in hand, and animating
            bool valid =
                Projectile.ai[0] == 1f &&
                player.HeldItem?.type == ModContent.ItemType<SuperShotgun>() &&
                player.itemAnimation > 0;

            if (!valid)
            {
                Projectile.Kill();
                return;
            }

            // Keep alive only while the stab animation is playing
            Projectile.timeLeft = 2;

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            // Safe direction towards mouse
            Vector2 dir = Main.MouseWorld - playerCenter;
            if (dir.LengthSquared() < 1f)
                dir = new Vector2(player.direction, 0f);
            else
                dir.Normalize();

            // Compute progress safely
            float animMax = Math.Max(1f, player.itemAnimationMax); // clamp
            float half = animMax * 0.5f;
            float anim = MathHelper.Clamp(player.itemAnimation, 0f, animMax);

            float progress = anim < half ? (anim / half) : ((animMax - anim) / half);
            progress = MathHelper.Clamp(progress, 0f, 1f);

            float stabDistance = 50f;
            Projectile.Center = playerCenter + dir * (stabDistance * progress);

            Projectile.direction = dir.X >= 0f ? 1 : -1;
            Projectile.spriteDirection = Projectile.direction;
            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);

            Projectile.rotation = dir.ToRotation() + MathHelper.Pi;

            // Absolute safety valve: if anything went bad, kill it
            if (float.IsNaN(Projectile.position.X) || float.IsNaN(Projectile.position.Y) ||
                float.IsInfinity(Projectile.position.X) || float.IsInfinity(Projectile.position.Y))
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (Projectile.direction == 1) // aiming right
            {
                effects |= SpriteEffects.FlipVertically;
                effects |= SpriteEffects.FlipHorizontally;

            }
            else // aiming left
            {
                effects |= SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor,
                Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false; // Skip default draw
        }


        public override bool? CanDamage()
        {
            return true; // Enable damage
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return projHitbox.Intersects(targetHitbox);
        }
    }
}
