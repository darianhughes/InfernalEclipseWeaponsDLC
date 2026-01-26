using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.Effects
{
    public static class Effects
    {
        private const string prefix = "InfernalEclipseWeaponsDLC/Assets/Effects/";

        public static readonly Asset<Effect> Eclipse = LoadEffect("Eclipse");

        private static Asset<Effect> LoadEffect(string EffectPath)
        {
            if (Main.dedServ)
                return null;

            return ModContent.Request<Effect>(prefix + EffectPath);
        }

        private static Asset<Effect>[] LoadEffects(string EffectPath, int count)
        {
            if (Main.dedServ)
                return null;

            Asset<Effect>[] effects = new Asset<Effect>[count];

            for (int i = 0; i < count; i++)
                effects[i] = ModContent.Request<Effect>(prefix + EffectPath + i);

            return effects;
        }
    }

    public static class Textures
    {
        private const string prefix = "InfernalEclipseWeaponsDLC/Assets/Textures/";

        public static readonly Asset<Texture2D> AnyTexture256 = LoadTexture2D("AnyTexture256");

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
            if (Main.dedServ)
                return null;

            return ModContent.Request<Texture2D>(prefix + TexturePath);
        }

        private static Asset<Texture2D>[] LoadTexture2Ds(string TexturePath, int count)
        {
            if (Main.dedServ)
                return null;

            Asset<Texture2D>[] textures = new Asset<Texture2D>[count];

            for (int i = 0; i < count; i++)
                textures[i] = ModContent.Request<Texture2D>(prefix + TexturePath + i);

            return textures;
        }
    }
}
