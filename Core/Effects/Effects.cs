using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.Effects
{
    //idfk what to do man
    public static class Effects
    {
        public static Asset<Effect> EclipseShader { get; private set; }

        internal static void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            EclipseShader = ModContent.Request<Effect>("InfernalEclipseWeaponsDLC/Assets/Effects/Eclipse", AssetRequestMode.ImmediateLoad);
        }

        internal static void Unload()
        {
            EclipseShader = null;
        }
    }

    public class EclipseAssetsSystem : ModSystem
    {
        public override void Load() => Effects.Load(Mod);
        public override void Unload() => Effects.Unload();
    }
}
