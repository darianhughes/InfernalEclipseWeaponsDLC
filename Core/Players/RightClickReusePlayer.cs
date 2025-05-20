using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.Players
{
    public class RightClickReusePlayer : ModPlayer
    {
        private int altReuseTimer = 0;

        public override void PostUpdate()
        {
            // Only act if holding your custom item!
            if (Player.HeldItem.type == ModContent.ItemType<TwoPaths>())
            {
                // If right-click is held and alternate function is available
                if (Player.altFunctionUse == 2 && Main.mouseRight && Main.mouseRightRelease)
                {
                    // Start using item
                    if (Player.itemTime == 0 && Player.itemAnimation == 0)
                    {
                        Player.controlUseItem = true;
                        Main.mouseRightRelease = false; // Trick the game into thinking you just clicked
                    }
                }
            }
        }
    }
}
