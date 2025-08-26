using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using static System.Net.Mime.MediaTypeNames;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class ListoftheDamned : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            isHealer = true;

            Item.damage = 100;
            Item.width = 28;
            Item.height = 28;

            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 20f;

            Item.mana = 10;
            radiantLifeCost = 6;

            Item.value = CalamityGlobalItem.RarityLightPurpleBuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<ListoftheDamnedSoul_Cyan>();
            Item.shootSpeed = 3f;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Item.UseSound = SoundID.Item20;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
    Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = 6;
            float spread = MathHelper.ToRadians(45);
            float baseSpeed = 2f;

            // Offset from player so projectiles appear in front of the weapon
            Vector2 spawnOffset = velocity.SafeNormalize(Vector2.UnitY) * 46f;
            Vector2 spawnPosition = player.Center + spawnOffset;

            for (int i = 0; i < numberProjectiles; i++)
            {
                float offset = -spread / 2 + (spread / (numberProjectiles - 1)) * i;
                Vector2 perturbedSpeed = velocity.RotatedBy(offset).SafeNormalize(Vector2.UnitY) * baseSpeed;

                int projType = (i % 2 == 0)
                    ? ModContent.ProjectileType<ListoftheDamnedSoul_Cyan>()
                    : ModContent.ProjectileType<ListoftheDamnedSoul_Orange>();

                Projectile.NewProjectile(source, spawnPosition, perturbedSpeed, projType, damage, knockback, player.whoAmI);
            }

            return false;
        }

    }
}
