using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class MagicPurpleBouncyBalls : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.crit = 23;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 27;
            Item.width = 34;
            Item.height = 34;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0.27f;
            Item.rare = ItemRarityID.Green;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item56;
            Item.shoot = ModContent.ProjectileType<MagicPurpleBouncyBall>();
            Item.shootSpeed = 2f;
        }

        /*
        public override Vector2? HoldoutOffset()
        {
            return new Vector2?(Vector2.Zero);
        }
        */

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int projectileCount = 27; // number of projectiles

            for (int i = 0; i < projectileCount; i++)
            {
                float speedMult = Main.rand.NextFloat(0.5f, 1.5f);

                // Randomize slight spread in direction, e.g., ±5 degrees
                float spread = MathHelper.ToRadians(25f);
                float angle = Main.rand.NextFloat(-spread, spread);

                Vector2 newVelocity = velocity.RotatedBy(angle) * speedMult;

                Projectile.NewProjectile(
                    source,
                    position,
                    newVelocity,
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            return false; // Return false because we manually spawned projectiles
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Book, 1);
            recipe.AddIngredient(ItemID.PinkGel, 27);
            recipe.AddIngredient(ItemID.PurpleMucos, 1);
            recipe.AddTile(TileID.Bookcases);
            recipe.Register();
        }
    }
}
