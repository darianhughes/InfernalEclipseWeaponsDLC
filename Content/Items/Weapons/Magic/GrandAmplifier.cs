using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.GrandAmplifier;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class GrandAmplifier : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 44;
            Item.height = 58;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<GrandAmplifierPro>();
            Item.shootSpeed = 16f;

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // Always allow using right-click
            if (player.altFunctionUse == 2)
            {
                // Right-click: no shooting projectile
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shoot = ModContent.ProjectileType<GrandAmplifierPro2>();
            }
            else
            {
                // Left-click: normal shooting
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.shoot = ModContent.ProjectileType<GrandAmplifierPro>();
            }

            return true;
        }

        public override float UseTimeMultiplier(Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2) // right-click
            {
                // spawn invisible projectile to do the effect
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(
                        player.GetSource_ItemUse(Item),
                        player.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<GrandAmplifierPro2>(),
                        Item.damage,
                        0f,
                        player.whoAmI
                    );
                }

                return true; // counts as using the item
            }

            return base.UseItem(player); // left-click normal
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
            Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Prevent projectile on right-click
            return player.altFunctionUse != 2;
        }
    }
}
