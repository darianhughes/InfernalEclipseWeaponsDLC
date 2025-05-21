using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro
{
    public class BellBalladSunlightLaser : BardProjectile
    {
        public override void SetBardDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.GreenLaser;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1.2f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item12, Projectile.position);
            }

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 2f)
            {
                if (Projectile.alpha > 0)
                    Projectile.alpha -= 15;

                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 60 * 2);
        }

        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
