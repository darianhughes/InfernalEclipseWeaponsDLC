using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
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
                    if(BellBalladEleum != null)
                    {
                        BellBalladEleum.Projectile.timeLeft = 20;
                        BellBalladEleum = null;
                    }

                    if (BellBalladHavoc != null)
                    {
                        BellBalladHavoc.Projectile.timeLeft = 20;
                        BellBalladHavoc = null;
                    }

                    if (BellBalladSunlight != null)
                    {
                        BellBalladSunlight.Projectile.timeLeft = 20;
                        BellBalladSunlight = null;
                    }
                }
            }
        }
    }
}
