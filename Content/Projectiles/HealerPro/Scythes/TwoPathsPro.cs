using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Projectiles.Scythe;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
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
            if (Projectile.ai[0] == 1)
            {
                Projectile.scale = 2f;
                Projectile.Size = new Vector2(208f, 190f);
                dustOffset = new Vector2(-35, 7f);
                dustCount = 4;
                dustType = 320;
                rotationSpeed = 0.25f;
                Projectile.light = 1;
                this.fadeOutSpeed = 30;
            }
            else
            {
                Projectile.scale = 1f;
                Projectile.Size = new Vector2(208f, 190f);
                dustOffset = new Vector2(-35, 7f);
                dustCount = 4;
                dustType = 320;
                rotationSpeed = 0.25f;
                Projectile.light = 1;
                this.fadeOutSpeed = 30;
            }
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 1)
            {
                // Spawn black explosion at hit location
                if (Main.myPlayer == Projectile.owner)
                {
                    int proj = Projectile.NewProjectile(
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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Get your texture
            Texture2D texture = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/TwoPathsPro").Value;

            // If your projectile has only one frame, use this:
            Rectangle frame = new Rectangle(0, 0, 208, 190);

            // Adjust draw position for new size and scale
            Vector2 drawOrigin = new Vector2(208 / 2f, 190 / 2f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            // adjust direction based on Projectile.direction
            SpriteEffects spriteEffects = ((Projectile.spriteDirection > 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            spriteEffects |= SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(
                texture,
                drawPos,
                frame,
                lightColor,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale, // <--- draws at double size
                spriteEffects,
                0
            );
            return false;
        }
    }
}
