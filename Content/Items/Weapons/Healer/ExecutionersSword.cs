using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod;
using CalamityMod.Items;
using ThoriumMod.Items.HealerItems;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword;
using CalamityMod.Buffs.DamageOverTime;


namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class ExecutionersSword : ThoriumItem
    {
        public override void SetDefaults()
        {
            Item.damage = 400;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            healType = HealType.Ally;
            healAmount = 0;
            healDisplay = true;
            isHealer = true;
            Item.width = 64;
            Item.height = 68;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = new SoundStyle?(SoundID.Item1);

            //Item.shoot = ModContent.ProjectileType<LightSlashPro>();
            //Item.shootsEveryUse = true;

            //temp until finished
            Item.scale = 1.25f;
            Item.healLife = 5;
        }

        public override bool AltFunctionUse(Player player) => false;

        public override bool CanUseItem(Player player)
        {
            return true;

            if (player.altFunctionUse == 2) // Right click
            {
                Item.noMelee = true;
                Item.noUseGraphic = true;
                // Only throw if not already thrown (only one at a time)
                return !(player.ownedProjectileCounts[ModContent.ProjectileType<ExecutionersSwordPro>()] > 0);
            }
            Item.noMelee = false;
            Item.noUseGraphic = false;
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false; //WIP

            if (player.altFunctionUse == 2) // Right click: throw sword
            {
                type = ModContent.ProjectileType<ExecutionersSwordPro>();
                velocity = velocity.SafeNormalize(Vector2.UnitX) * 16f;
                return false;
            }
            else // Left click: fire slash
            {
                type = ModContent.ProjectileType<LightSlashPro>();

                // Direction to cursor
                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 dir = (mouseWorld - position).SafeNormalize(Vector2.UnitX);

                velocity = dir * 16f; // Or your desired speed
                return true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HereticBreaker>()
                .AddIngredient(ItemID.LunarBar, 10)
                .AddIngredient<CelestialFragment>(15)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
