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
using SOTS.Void;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void
{
    [ExtendsFromMod("SOTS")]
    public class CatastrophicLongbladeVoid : VoidItem
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Melee/Void/CatastrophicLongblade";
        public override void SafeSetDefaults()
        {
            Item.width = 85;
            Item.height = 86;
            Item.damage = 10250;
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

        public override int GetVoid(Player player)
        {
            return 15;
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
