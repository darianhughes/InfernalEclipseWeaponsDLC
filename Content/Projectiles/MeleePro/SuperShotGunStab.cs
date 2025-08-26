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
            Projectile.aiStyle = 19; // Shortsword behavior
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.scale = 1f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.direction);
            Projectile.direction = player.direction;
            Projectile.spriteDirection = Projectile.direction;

            // The direction of the stab
            Vector2 stabDirection = Vector2.Normalize(Main.MouseWorld - playerCenter);
            if (stabDirection.HasNaNs()) stabDirection = player.DirectionTo(Main.MouseWorld);

            // Simulate animation-based stab movement
            float halfDuration = player.itemAnimationMax / 2f;
            float progress;
            if (player.itemAnimation < halfDuration)
                progress = player.itemAnimation / halfDuration;
            else
                progress = (player.itemAnimationMax - player.itemAnimation) / halfDuration;

            float stabDistance = 50f; // Adjust reach
            Vector2 stabOffset = stabDirection * (stabDistance * progress);

            Projectile.Center = playerCenter + stabOffset;

            Projectile.rotation = stabDirection.ToRotation() + MathHelper.ToRadians(180f); // or (float)(Math.PI * 3 / 4)
            Projectile.timeLeft = 2; // Keeps it alive as long as player is stabbing

            // Optional: Visualize with dust for debugging
            // Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Fire);
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
