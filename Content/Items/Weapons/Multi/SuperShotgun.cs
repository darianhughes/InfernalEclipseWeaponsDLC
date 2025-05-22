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

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Multi
{
    public class SuperShotgun : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 15; // Base damage
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 34; // How fast the weapon is used.
            Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = false;
            Item.knockBack = 6f;
            Item.value = Item.sellPrice(silver: 50);
            Item.rare = ItemRarityID.Blue;
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
            if (player.altFunctionUse == 2) // Right click - melee
            {
                // Melee setup
                Item.damage = 25;
                Item.DamageType = DamageClass.Melee;
                Item.useTime = 12;
                Item.useAnimation = 12;
                Item.knockBack = 4f;
                Item.noMelee = false;
                Item.shoot = 0;
                Item.useTurn = true;
                Item.UseSound = SoundID.Item1;
                Item.shootSpeed = 1f;
                Item.useAmmo = 0;
                Item.useStyle = 5;
            }
            else // Left click - shotgun
            {
                Item.damage = 15;
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

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Only shoot on left click
            if (player.altFunctionUse == 2)
                return false;

            int numberProjectiles = 4 + Main.rand.Next(2); // 4 or 5 bullets
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12));
                float scale = 1f - Main.rand.NextFloat() * 0.1f;
                perturbedSpeed *= scale;
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false; // Prevent vanilla projectile
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
