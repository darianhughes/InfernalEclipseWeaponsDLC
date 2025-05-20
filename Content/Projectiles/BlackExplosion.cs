using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Terraria;
    using Terraria.ID;
    using Terraria.ModLoader;
    using ThoriumMod;
    using ThoriumMod.Projectiles.Healer;

    public class BlackExplosion : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.DaybreakExplosion];
        }

        public override void SetDefaults()
        {
            Projectile.scale *= 1.5f;
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite targets
            Projectile.timeLeft = 8;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                // Play explosion sound once at the start
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.localAI[0] = 1f;
            }

            // Animate like Daybreak
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[ProjectileID.DaybreakExplosion])
                {
                    Projectile.Kill();
                    return;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // Use Daybreak Explosion texture but draw it black
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.DaybreakExplosion].Value;
            int frameHeight = texture.Height / Main.projFrames[ProjectileID.DaybreakExplosion];
            Rectangle frame = new Rectangle(0, Projectile.frame * frameHeight, texture.Width, frameHeight);

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color color = Color.Black * 0.9f;
            Main.EntitySpriteDraw(texture, drawPos, frame, color, 0f,
                frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
