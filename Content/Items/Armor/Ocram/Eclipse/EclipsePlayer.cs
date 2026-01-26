using InfernalEclipseWeaponsDLC.Content.Projectiles.ArmorPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Eclipse
{
    public class EclipsePlayer : ModPlayer
    {
        public bool EclipseSet;

        public override void ResetEffects() => EclipseSet = false;

        public override void PostUpdate()
        {
            if (EclipseSet)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<EclipseEclipse>()] < 1)
                {
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<EclipseEclipse>(), 0, 0, Main.myPlayer);
                }
            }
        }
    }
}
