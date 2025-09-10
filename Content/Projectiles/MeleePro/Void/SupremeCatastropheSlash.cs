using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.Void
{
    public class SupremeCatastropheSlash : ModProjectile, ILocalizedModType
    {
        public ref float Time => ref Projectile.ai[0];

        public override string Texture => "CalamityMod/Projectiles/Boss/SupremeCatastropheSlash";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 60;

            Projectile.friendly = true;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 1500; 
            Projectile.Opacity = 1f;

            Projectile.DamageType = ModLoader.TryGetMod("SOTS", out Mod sots) ? sots.Find<DamageClass>("VoidMelee") : ModContent.GetInstance<TrueMeleeDamageClass>();
        }

        public override void AI()
        {
            Time++;

            // Face travel direction, no behavior changes.
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }

            // Simple 4-frame animation.
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 7) % Main.projFrames[Projectile.type];

            // Optional light.
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            if (Projectile.ai[1] == 0f)
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SupremeCatastropheSlashAlt").Value;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frame = tex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            // Soft afterimages.
            for (int i = 0; i < 3; i++)
            {
                Color c = Projectile.GetAlpha(lightColor) * (1f - i / 3f) * 0.5f;
                Vector2 offset = Projectile.velocity * -i * 4f;
                Main.EntitySpriteDraw(tex, pos + offset, frame, c, Projectile.rotation, origin, Projectile.scale, fx, 0);
            }

            Main.EntitySpriteDraw(tex, pos, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, fx, 0);
            return false;
        }
    }
}
