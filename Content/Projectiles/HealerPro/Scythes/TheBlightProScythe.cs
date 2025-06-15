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
    public class TheBlightProScythe : ScythePro
    {
        public override void SafeSetDefaults()
        {
            Projectile.Size = new Vector2(140f, 150f);
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    Texture2D texture = ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/TheBlightProScythe").Value;

        //    Rectangle frame = new Rectangle(0, 0, 140, 150);

        //    Vector2 drawOrigin = new Vector2(Projectile.Size.X / 2f, Projectile.Size.Y / 2f);
        //    Vector2 drawPos = Projectile.Center - Main.screenPosition;

        //    Main.EntitySpriteDraw(
        //        texture,
        //        drawPos,
        //        frame,
        //        lightColor,
        //        Projectile.rotation,
        //        drawOrigin,
        //        Projectile.scale,
        //        SpriteEffects.None,
        //        0
        //    );

        //    return false;
        //}
    }
}
