using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Abyss;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class ArckaneStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 110;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.width = 76;
            Item.height = 78;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<ArckaneStaffPro>();
            Item.shootSpeed = 12f;
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
            recipe.AddIngredient(ModLoader.TryGetMod("ThoriumMod", out Mod thor) ? thor.Find<ModItem>("MagickStaff").Type : ItemID.DiamondStaff);
            recipe.AddIngredient<AstralBar>(8);
            recipe.AddIngredient<AshesofCalamity>(5);
            recipe.AddIngredient<DepthCells>(5);
            if (thor != null) recipe.AddIngredient(thor.Find<ModItem>("AbyssalChitin"), 3);
            recipe.AddIngredient<InfectedArmorPlating>(3);
            recipe.AddIngredient<Voidstone>(3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
