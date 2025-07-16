using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class LightEnergyProj : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }
        public override bool PreDraw(ref Color lightColor) => false; // don’t draw sprite
        public override void AI()
        {
            // Find the closest player (not just owner)
            Player closest = null;
            float closestDist = float.MaxValue;

            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    float dist = Vector2.Distance(Projectile.Center, player.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = player;
                    }
                }
            }

            if (closest != null)
            {
                Vector2 dir = closest.Center - Projectile.Center;
                float speed = 7f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, dir.SafeNormalize(Vector2.Zero) * speed, 0.08f);
            }

            // Spawn gold flame dust (for light energy)
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame,
                    Projectile.velocity.X * 0.3f, Projectile.velocity.Y * 0.3f, 150, default, 1.2f);
            }
        }

        public override bool? CanHitNPC(NPC target) => false; // ignore NPCs

        public override bool CanHitPlayer(Player target)
        {
            return target.active && !target.dead;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.myPlayer == target.whoAmI)
            {
                target.statLife += 8;
                target.HealEffect(8);
            }
            Projectile.Kill();
        }
    }
}
