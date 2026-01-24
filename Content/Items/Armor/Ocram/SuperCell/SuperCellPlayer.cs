using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.SuperCell
{
    public class SuperCellPlayer : ModPlayer
    {
        public bool hasSuperCellGuardEquipped;
        public int superCellExtraWingTime;

        public override void ResetEffects()
        {
            hasSuperCellGuardEquipped = false;
            superCellExtraWingTime = 0;
        }

        // Called after armor/accessories have been processed
        public override void UpdateEquips()
        {
            // Only apply if the player actually has some wing time to boost
            if (hasSuperCellGuardEquipped && Player.wingTimeMax > 0)
            {
                // Add 15% extra wing time (rounded)
                superCellExtraWingTime = (int)(Player.wingTimeMax * 0.15f);
                Player.wingTimeMax += superCellExtraWingTime;
            }
        }
    }

}
