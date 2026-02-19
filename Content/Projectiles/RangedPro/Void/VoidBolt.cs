using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RangedPro.Void
{
    public class VoidBolt : ModProjectile
    {
        // Balance needed
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 360;
            Projectile.velocity *= 2f;
            Projectile.extraUpdates = 25;
            Projectile.DamageType = ModLoader.TryGetMod("SOTS", out Mod sots) ? sots.Find<DamageClass>("VoidRanged") : DamageClass.Ranged;
        }
        bool rising = true;
        public override void AI()
        {
            /*
            if (rising == true)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 10)
                {
                    rising = false;
                }
            }
            else
            {
                Projectile.ai[0]--;
                if (Projectile.ai[0] <= -10)
                {
                    rising = true;
                }
            }
            */
           
            if (rising && Projectile.ai[0]++ >= 10)
            {
                rising = false;
            }
            else if (!rising && Projectile.ai[0]-- <= -10)
            {
                rising = true;
            }

            for (int i = -1; i < 2; i++)
            {
            //    int dustNumber = Dust.NewDust(Projectile.position, 6, 6, DustID.Vortex, Projectile.velocity.X, Projectile.velocity.Y, 100);
            //    Dust dust = Main.dust[dustNumber];
                Dust dust = Main.dust[Dust.NewDust(Projectile.position, 6, 6, DustID.Vortex, Projectile.velocity.X, Projectile.velocity.Y, 100)];
                dust.velocity = Vector2.Zero;
                dust.position -= Projectile.velocity / 5f + new Vector2(0, 1 * Projectile.ai[0]);
                dust.noGravity = true;
                dust.scale = 1.3f;
                dust.noLight = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }
    }
}