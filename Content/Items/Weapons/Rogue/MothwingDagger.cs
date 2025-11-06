using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Tiles;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Rogue
{
    public class MothwingDagger : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.damage = 27;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 16;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MothwingDaggerPro>();
            Item.shootSpeed = 4f;
            Item.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }

        public override float StealthDamageMultiplier => 1.2f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            if (!ModLoader.TryGetMod("SOTS", out Mod _))
            {
                CreateRecipe()
                    .AddIngredient(ItemID.Obsidian, 20)
                    .AddIngredient(ItemID.GlowingMushroom, 50)
                    .AddTile(ModContent.TileType<ArcaneArmorFabricator>())
                    .Register();
            }
        }

    }
}
