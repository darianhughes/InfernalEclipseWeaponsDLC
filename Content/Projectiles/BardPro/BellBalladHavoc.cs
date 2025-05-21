using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro
{
    public class BellBalladHavoc : BellBalladEleum
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetBardDefaults()
        {
            base.SetBardDefaults();
            Projectile.width = 20;
            Projectile.height = 24;
        }

        public override void Shoot(int damage, float knockBack)
        {
            if(Projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default) * 10f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.DD2SquireSonicBoom, Projectile.damage, Projectile.knockBack);
                proj.tileCollide = false; // TODO: might require dedicated ModProjectile for MP compat
            }
        }
    }
}
