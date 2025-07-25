using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes;
using CalamityMod.Items.Materials;
using ThoriumMod.Buffs;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Multi
{
    public class SuperShotgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8; // Base damage
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 34; // How fast the weapon is used.
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = false;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(silver: 40);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item36; // Shotgun sound
            Item.autoReuse = false;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.width = 64;
            Item.height = 20;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Right click - melee (shortsword style)
            {
                Item.damage = 22;
                Item.DamageType = DamageClass.Melee;
                Item.useTime = 12;
                Item.useAnimation = 12;
                Item.knockBack = 4f;
                Item.noMelee = false;
                Item.shoot = ModContent.ProjectileType<SuperShotgunStab>();
                Item.useAmmo = 0;
                Item.shootSpeed = 1f;
                Item.UseSound = SoundID.Item1;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.noUseGraphic = true;
            }

            else // Left click - shotgun
            {
                Item.damage = 9;
                Item.DamageType = DamageClass.Ranged;
                Item.useTime = 34;
                Item.useAnimation = 34;
                Item.knockBack = 6f;
                Item.noMelee = false;
                Item.shoot = ProjectileID.Bullet;
                Item.useTurn = false;
                Item.UseSound = SoundID.Item36;
                Item.shootSpeed = 6f;
                Item.useAmmo = AmmoID.Bullet;
                Item.useStyle = 5;
                Item.noUseGraphic = false;
            }
            return base.CanUseItem(player);
        }

        //public override void UseItemFrame(Player player)
        //{
        //    if (player.altFunctionUse == 2)
        //    {
        //        // Remove rotation
        //        player.itemRotation = 0f;

        //        player.itemLocation = player.Center + new Vector2(0f, -12f);
        //    }
        //}

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Melee stab (shortsword style) - spawn manually!
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SuperShotgunStab>(), damage, knockback, player.whoAmI);
                return false;
            }

            // Left click - shotgun
            int numberProjectiles = 4 + Main.rand.Next(2);
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                float scale = 1f - Main.rand.NextFloat() * 0.1f;
                perturbedSpeed *= scale;
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }

            return false; // prevent default
        }


        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
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
