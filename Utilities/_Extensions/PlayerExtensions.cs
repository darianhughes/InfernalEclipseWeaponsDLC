using Terraria;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Utilities
{
    public static class PlayerExtensions
    {
        public static bool HasAltFunctionUse(this Player player)
        {
            return player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed;
        }
    }
}

/// <summary>
///     Provides <see cref="Player"/> extension methods.
/// </summary>
