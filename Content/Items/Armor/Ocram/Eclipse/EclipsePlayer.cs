using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Eclipse
{
    public class EclipsePlayer : ModPlayer
    {
        public bool EclipseSet;

        public override void ResetEffects() => EclipseSet = false;
    }
}
