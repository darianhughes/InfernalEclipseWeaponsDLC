using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod;
using ThoriumMod.Utilities;
using CalamityMod.Items;
using CalamityMod.Items.Potions;
using InfernalEclipseWeaponsDLC.Core;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Eclipse
{
    [AutoloadEquip(EquipType.Legs)]
    public class EclipseGreaves : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
            return WeaponConfig.Instance.UnfinishedContent;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 19;
        }

        public override void UpdateEquip(Player player)
        {
            ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
            ref StatModifier damage = ref player.GetDamage(DamageClass.Generic);
            damage -= 0.16f;
            ref StatModifier damage2 = ref player.GetDamage((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance);
            damage2 += 0.32f;
            player.manaCost -= 0.15f;
            player.moveSpeed += 0.25f;
            thoriumPlayer.healBonus += 3;
            player.GetCritChance((DamageClass)(object)ThoriumDamageBase<HealerDamage>.Instance) += 4f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.HallowedGreaves);
            recipe.AddRecipeGroup(RecipeGroups.Titanium, 12);
            recipe.AddIngredient(ItemID.SoulofLight, 10);

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
