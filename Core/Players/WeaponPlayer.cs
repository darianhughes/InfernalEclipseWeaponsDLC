using InfernalEclipseWeaponsDLC.Content.Items.Weapons;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.Players
{
    public class WeaponPlayer : ModPlayer
    {
        public BellBalladEleum BellBalladEleum { get; set; }
        public BellBalladHavoc BellBalladHavoc { get; set; }
        public BellBalladSunlight BellBalladSunlight { get; set; }

        public override void ResetEffects()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                // Kill and unbind BellBallad projectiles
                bool bellBalladEquipped = Player.HeldItem.type == ModContent.ItemType<BellBallad>() || Main.mouseItem.type == ModContent.ItemType<BellBallad>();
                if (!bellBalladEquipped) 
                {
                    BellBalladEleum?.Projectile.Kill();
                    BellBalladEleum = null;
                    BellBalladHavoc?.Projectile.Kill();
                    BellBalladHavoc = null;
                    BellBalladSunlight?.Projectile.Kill();
                    BellBalladSunlight = null;  
                }
            }
        }
    }
}
