using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Scythe;
using Terraria.DataStructures;
using ThoriumMod.Buffs;

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
            dustType = 320;
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
        }

        // Remove any resizing from PreAI/AI!
        public override bool PreAI()
        {
            // No resizing here!
            return base.PreAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/TwoPathsPro").Value;
            Vector2 baseSize = new Vector2(208f, 190f);
            Rectangle frame = new Rectangle(0, 0, (int)baseSize.X, (int)baseSize.Y);
            Vector2 drawOrigin = baseSize / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            SpriteEffects spriteEffects = ((Projectile.spriteDirection > 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            spriteEffects |= SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(
                texture,
                drawPos,
                frame,
                lightColor,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                spriteEffects,
                0
            );
            return false;
        }
    }
}
