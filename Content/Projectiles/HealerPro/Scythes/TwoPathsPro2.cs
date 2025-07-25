using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Projectiles.Scythe;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TwoPathsPro2 : ScythePro
    {
        public override void SafeSetStaticDefaults()
        {
        }

        public override void SafeSetDefaults()
        {
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    // Get your texture
        //    Texture2D texture = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/TwoPathsPro").Value;

        //    // If your projectile has only one frame, use this:
        //    Rectangle frame = new Rectangle(0, 0, 208, 190);

        //    // Adjust draw position for new size and scale
        //    Vector2 drawOrigin = new Vector2(208 / 1.5f, 190 / 1.5f);
        //    Vector2 drawPos = Projectile.Center - Main.screenPosition;

        //    Main.EntitySpriteDraw(
        //        texture,
        //        drawPos,
        //        frame,
        //        lightColor,
        //        Projectile.rotation,
        //        drawOrigin,
        //        Projectile.scale, // <--- draws at double size
        //        SpriteEffects.None,
        //        0
        //    );
        //    return false;
        //}
    }
}
