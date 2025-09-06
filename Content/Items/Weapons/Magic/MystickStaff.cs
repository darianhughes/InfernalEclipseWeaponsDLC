using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class MystickStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1300;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 35;
            Item.width = 125;
            Item.height = 118;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<MystickStaffPro>();
            Item.shootSpeed = 16f;
        }

        /*
        public override Vector2? HoldoutOffset()
        {
            return new Vector2?(Vector2.Zero);
        }
        */

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<ArckaneStaff>();
            recipe.AddIngredient<ShadowspecBar>(5);
            recipe.AddIngredient<CosmiliteBar>(3);
            recipe.AddIngredient<RuinousSoul>(5);
            recipe.AddIngredient<DivineGeode>(5);
            recipe.AddIngredient<MiracleMatter>(1);
            recipe.AddTile<DraedonsForge>();
            recipe.Register();
        }
    }
}
