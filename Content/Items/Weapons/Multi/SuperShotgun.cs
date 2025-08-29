using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using ThoriumMod.Buffs;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro;
using CalamityMod;
using CalamityMod.Items;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Multi
{
    public class SuperShotgun : ModItem
    {
        public override void SetDefaults()
        {
            // Base stats = shotgun mode
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = false;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item36;
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;

            Item.width = 64;
            Item.height = 20;
            Item.Calamity().canFirePointBlankShots = true;

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // right click
            {
                // Just change visuals & shoot behavior
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.UseSound = SoundID.Item1;
                Item.shoot = ModContent.ProjectileType<SuperShotgunStab>();
                Item.useAmmo = 0;
            }
            else // left click
            {
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.UseSound = SoundID.Item36;
                Item.shoot = ProjectileID.Bullet;
                Item.useAmmo = AmmoID.Bullet;
            }

            return base.CanUseItem(player);
        }

        public override float UseTimeMultiplier(Player player)
        {
            // Stab is faster
            return player.altFunctionUse == 2 ? 0.35f : 1f;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            return player.altFunctionUse == 2 ? 0.35f : 1f;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
                           Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // right click = stab
            {
                // Correctly apply melee scaling
                int meleeDamage = (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(22);
                float meleeKB = player.GetTotalKnockback(DamageClass.Melee).ApplyTo(knockback);
                int meleeCrit = (int)player.GetTotalCritChance(DamageClass.Melee);

                int proj = Projectile.NewProjectile(source, position, velocity,
                    ModContent.ProjectileType<SuperShotgunStab>(), meleeDamage, meleeKB, player.whoAmI);

                Main.projectile[proj].DamageType = DamageClass.Melee;
                Main.projectile[proj].CritChance = meleeCrit;

                return false;
            }

            // Left click = shotgun spread (base 9, ranged scaling)
            int numberProjectiles = 4 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                float scale = 1f - Main.rand.NextFloat() * 0.1f;
                perturbedSpeed *= scale;

                Projectile.NewProjectile(source, position, perturbedSpeed,
                    type, damage, knockback, player.whoAmI);
            }

            return false;
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.altFunctionUse == 2) // only stab applies stun
                target.AddBuff(ModContent.BuffType<Stunned>(), 120);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Boomstick)
                .AddRecipeGroup(RecipeGroupID.IronBar, 6)
                .AddIngredient<PearlShard>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
