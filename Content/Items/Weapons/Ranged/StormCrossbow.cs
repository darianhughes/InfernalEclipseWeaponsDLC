using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Common;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RangedPro;
using CalamityMod;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Ranged
{
    public class StormCrossbow : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 59;
            Item.width = 50;
            Item.height = 22;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 0.1f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.shoot = ModContent.ProjectileType<LightChainProjectile>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;

            Item.GetGlobalItem<WeaponsGlobalItem>().verveineItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type != 1) return;
            type = ModContent.ProjectileType<StormCrossbowArrow>();
            damage = (int)(damage * 0.5f);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, 0f);
        }

        public override void AddRecipes()
        {
            if (!ModLoader.HasMod("CalamityMod") && !ModLoader.HasMod("Consolaria"))
            {
                CreateRecipe()
                    .AddTile(TileID.MythrilAnvil)
                    .AddIngredient(ItemID.HallowedBar, 12)
                    .AddIngredient(ItemID.SoulofLight, 10)
                    .AddIngredient(ItemID.CrystalShard, 8)
                    .Register();
            }
        }
    }
}
