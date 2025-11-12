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
    public class GlowstringBiwaPro : ModProjectile
    {
        public override string Texture => "CalamityMod/Particles/Light";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1; // Smooth movement and line
        }

        public int TileBounces = 2;

        public override void AI()
        {
            // Light
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * 0.6f);

            // Rotation
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            /*
            // Dust
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 150, default, 1.2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.3f;
            }
            */
        }

        private Projectile FindPartner()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active &&
                    proj.owner == Projectile.owner &&
                    proj.type == Projectile.type &&
                    proj.ai[1] == Projectile.ai[1] && // same pair
                    proj.whoAmI != Projectile.whoAmI)
                {
                    return proj;
                }
            }
            return null;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TileBounces--;

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                Projectile.velocity.X = -oldVelocity.X;
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                Projectile.velocity.Y = -oldVelocity.Y;

            return TileBounces < 0;
        }
    }
}
