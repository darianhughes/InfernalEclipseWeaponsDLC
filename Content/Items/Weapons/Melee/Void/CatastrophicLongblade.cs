using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.Void;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void
{
    public class CatastrophicLongblade : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return !ModLoader.HasMod("SOTS");
        }
        public override void SetDefaults()
        {
            Item.width = 85;
            Item.height = 86;
            Item.damage = 9250;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 49;
            Item.useAnimation = 49;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.UseSound = new Terraria.Audio.SoundStyle("CalamityMod/Sounds/Item/ExobladeBeamSlash");
            Item.shoot = ModContent.ProjectileType<SupremeCatastropheSlash>();

            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, Main.rand.Next(0, 2));
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SupremeCatastropheTrophy>()
                .AddTile<SCalAltar>()
                .Register();
        }
    }
}
