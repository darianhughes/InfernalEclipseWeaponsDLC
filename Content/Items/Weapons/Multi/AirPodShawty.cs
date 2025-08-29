using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items.BardItems;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.AirPodShawty;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using Terraria.Audio;
using CalamityMod;
using CalamityMod.Items;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Multi
{
    public class AirPodShawty : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<EmpowermentProlongation>(3);
            Empowerments.AddInfo<LifeRegeneration>(1);
            Empowerments.AddInfo<ResourceGrabRange>(2);

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetBardDefaults()
        {
            Item.damage = 120;
            Item.DamageType = ModContent.GetInstance<BardDamage>();
            Item.width = 48;
            Item.height = 36;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<AirPod>();
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;

            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyInspirationCost(Player player, ref int cost)
        {
            cost = (player.altFunctionUse == 2) ? 0 : 5;
        }

        public override void ModifyEmpowermentPool(Player player, Player target, EmpowermentPool empPool)
        {
            // Only allow empowerments on left-click
            if (player.altFunctionUse == 2) // right-click
            {
                // Clear the pool to prevent any empowerments
                empPool.Clear();
                return;
            }

            // Otherwise, do normal behavior
            base.ModifyEmpowermentPool(player, target, empPool);
        }

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 64f;

            if (player.altFunctionUse == 2)
            {
                // Right-click = shotgun spread, scale with ranged instead of bard
                int numberProjectiles = 7 + Main.rand.Next(2);

                // Take the item’s base damage and apply player’s ranged bonuses
                int rangedDamage = (int)player.GetTotalDamage(DamageClass.Ranged).ApplyTo(Item.damage);
                float rangedKnockback = player.GetTotalKnockback(DamageClass.Ranged).ApplyTo(Item.knockBack);

                for (int i = 0; i < numberProjectiles; i++)
                {
                    Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12)) * 0.9f;

                    int proj = Projectile.NewProjectile(source, player.Center + muzzleOffset,
                        perturbedSpeed, type, rangedDamage, rangedKnockback, player.whoAmI);

                    Main.projectile[proj].DamageType = DamageClass.Ranged;
                }

                return false; // don't fire bard note
            }
            else
            {
                // Left-click = normal bard note
                Projectile.NewProjectile(source, player.Center + muzzleOffset, velocity * 2f,
                    ModContent.ProjectileType<AirPod>(), damage, knockback, player.whoAmI);

                return false;
            }
        }

        public override Vector2? HoldoutOffset() => new Vector2(-12.5f, 3f);

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient<MusicPlayerNotActivated>()
                .AddIngredient(ItemID.IllegalGunParts)
                .AddIngredient<ShootingStarFragment>(15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
