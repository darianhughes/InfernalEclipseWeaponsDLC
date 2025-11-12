using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core
{
    public class RecipeGroups : ModSystem
    {
        public static RecipeGroup Titanium;
        public override void Unload()
        {
            Titanium = null;
        }

        public override void AddRecipeGroups()
        {
            string modName = "InfernalEclipseWeaponsDLC";

            Titanium = new RecipeGroup(() => "Adamantite or Titanium Bar", new int[2]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            RecipeGroup.RegisterGroup($"{modName}:TitaniumRecipeGroup", Titanium);
        }
    }
}
