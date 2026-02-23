using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace InfernalEclipseWeaponsDLC
{
    public class WeaponConfig : ModConfig
    {
        public static WeaponConfig Instance;

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("General")]

        [DefaultValue(false)]
        [ReloadRequired]
        public bool UnfinishedContent;

        [DefaultValue(false)]
        public bool EnableScreenEffects;

        [Header("VerveineWeapons")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AIGenedWeapons;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool GitGudWeapon;
    }
}
