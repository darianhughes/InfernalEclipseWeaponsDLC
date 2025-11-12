using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items;
using CalamityMod.Items.Potions;
using InfernalEclipseWeaponsDLC.Core;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class SuperCellSabatons : ModItem
    {
        public override void SetDefaults()
        {
            ((Entity)((ModItem)this).Item).width = 18;
            ((Entity)((ModItem)this).Item).height = 18;
            ((ModItem)this).Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            ((ModItem)this).Item.rare = 7;
            ((ModItem)this).Item.defense = 12;
        }

        public override void UpdateEquip(Player player)
        {
            ref StatModifier damage = ref player.GetDamage(DamageClass.Throwing);
            damage += 0.05f;
            player.GetCritChance(DamageClass.Throwing) += 10f;
            player.moveSpeed += 0.3f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.HallowedGreaves);
            recipe.AddRecipeGroup(RecipeGroups.Titanium, 12);
            recipe.AddIngredient(ItemID.SoulofFlight, 10);

            if (ModLoader.TryGetMod("Consolaria", out Mod consolariaMod))
            {
                recipe.AddIngredient(consolariaMod.Find<ModItem>("SoulofBlight").Type, 10);
            }
            else
            {
                recipe.AddIngredient<AureusCell>(10);
                recipe.AddIngredient(ItemID.SoulofSight, 5);
                recipe.AddIngredient(ItemID.SoulofMight, 5);
                recipe.AddIngredient(ItemID.SoulofFright, 5);
                recipe.AddIngredient(ItemID.CursedFlame, 8);
            }

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
