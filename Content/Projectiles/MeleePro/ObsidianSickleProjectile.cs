using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro
{
    public class ObsidianSickleProjectile : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.GitGudWeapon;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DeathSickle);
            AIType = ProjectileID.DeathSickle;
            Projectile.width = 30;
            Projectile.height = 30;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 25) Projectile.alpha += 10;
        }
    }
}
