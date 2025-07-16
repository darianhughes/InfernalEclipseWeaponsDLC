using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ThoriumMod;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class DarkEnergyProj : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
        }
        public override bool PreDraw(ref Color lightColor) => false; // don’t draw sprite
        public override void AI()
        {
            if (Projectile.ai[0] > -1)
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (target.active)
                {
                    Vector2 dir = target.Center - Projectile.Center;
                    float speed = 6f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, dir.SafeNormalize(Vector2.Zero) * speed, 0.07f);
                }
            }

            // Spawn shadowflame dust (for dark energy)
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame,
                    Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 150, default, 1.2f);
            }
        }
    }
}
