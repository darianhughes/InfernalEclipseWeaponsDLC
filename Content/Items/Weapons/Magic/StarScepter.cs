using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Common;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.StarScepter;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class StarScepter : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 25;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.knockBack = 0.1f;
            Item.mana = 40;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.shoot = ModContent.ProjectileType<StarScepterStar>();
            Item.shootSpeed = 1f; // so the staff "held item" aims in the right direction when shooting lol
            Item.autoReuse = false;

            Item.GetGlobalItem<WeaponsGlobalItem>().verveineItem = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = 0;
            Projectile oldestProjectile = null;
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.type == Item.shoot && player.whoAmI == projectile.owner)
                {
                    count++;

                    if (oldestProjectile == null || projectile.timeLeft < oldestProjectile.timeLeft)
                    {
                        oldestProjectile = projectile;
                    }
                }
            }

            if (count < 5)
            {
                Projectile newProjectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, damage, 0f, player.whoAmI);
            }
            else
            {
                oldestProjectile.ai[0] = 1; // used to reset timeleft
                oldestProjectile.netUpdate = true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddTile(ModLoader.HasMod("CalamityMod") ? TileID.Anvils : TileID.MythrilAnvil);
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                recipe.AddIngredient(thorium.Find<ModItem>("LodeStoneIngot").Type, 5);
            }
            recipe.AddIngredient(ItemID.MeteoriteBar, thorium != null ? 10 : 15);
            recipe.AddIngredient(ItemID.CrystalShard, 15);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.Register();
        }
    }
}
