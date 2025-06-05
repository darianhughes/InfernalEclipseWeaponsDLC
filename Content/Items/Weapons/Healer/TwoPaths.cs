using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using ThoriumMod.Items.HealerItems;
using Terraria.DataStructures;
using Terraria.ID;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using rail;
using CalamityMod.Projectiles.Magic;
using ThoriumMod;
using InfernalEclipseWeaponsDLC.Content.Projectiles;
using ThoriumMod.Items.BossThePrimordials.Dream;
using CalamityMod;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class TwoPaths : ScytheItem
    {
        public override void SetStaticDefaults()
        {
            SetStaticDefaultsToScythe();
        }

        public override void SetDefaults()
        {
            SetDefaultsToScythe();
            Item.damage = 475;
            scytheSoulCharge = 4;
            Item.width = 208;
            Item.height = 190;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<TwoPathsPro2>();
            Item.autoReuse = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            Item.mana = 20;
            Item.Calamity().devItem = true;
        }

        public override void AddRecipes()
        {
            InfernalEclipseWeaponsDLC IEWeaponDLC = InfernalEclipseWeaponsDLC.Instance;

            Recipe recipe = Recipe.Create(Item.type, 1);

            if (IEWeaponDLC.ragnarok != null && IEWeaponDLC.calamitybardhealer != null)
            {
                recipe.AddIngredient(IEWeaponDLC.ragnarok.Find<ModItem>("ExecutionerMark05").Type, 1);
                recipe.AddIngredient(IEWeaponDLC.calamitybardhealer.Find<ModItem>("Disaster").Type, 1);
            }
            else if (IEWeaponDLC.ragnarok != null)
            {
                recipe.AddIngredient(IEWeaponDLC.ragnarok.Find<ModItem>("ExecutionerMark05").Type, 1);
                recipe.AddIngredient(ModContent.ItemType<AshesofAnnihilation>(), 10);
            }
            else if (IEWeaponDLC.calamitybardhealer != null)
            {
                recipe.AddIngredient(ModContent.ItemType<RealitySlasher>(), 1);
                recipe.AddIngredient(IEWeaponDLC.calamitybardhealer.Find<ModItem>("Disaster").Type, 1);
            }
            else
            {
                recipe.AddIngredient(ModContent.ItemType<RealitySlasher>(), 1);
                recipe.AddIngredient(ModContent.ItemType<AshesofAnnihilation>(), 10);
            }
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Color lerpedColor = Color.Lerp(Color.White, new Color(255, 105, 180), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 4.0) * 0.5 + 0.5));

            TooltipLine line1 = new(Mod, "TwoSideLore1", "They overcame Ragnarok.They dismantled Eclipse.They survived the Slime Monsoon.");
            line1.OverrideColor = Color.MediumPurple;
            tooltips.Add(line1);

            TooltipLine line2 = new(Mod, "TwoSideLore2", "They stood at the brink of infinity, which only then Ren asked them \"which path will you choose?\"");
            line2.OverrideColor = Color.MediumPurple;
            tooltips.Add(line2);

            //TooltipLine DedicatedLine = new(Mod, "DedicatedItem", "- Dedicated Item -");
            //DedicatedLine.OverrideColor = lerpedColor;
            //tooltips.Add(DedicatedLine);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Right-click (alternate)
            {
                SetDefaultsToScythe();
                Item.damage = 475;
                scytheSoulCharge = 4;
                Item.mana = 20;
                Item.shoot = ModContent.ProjectileType<TwoPathsPro2>();
                Item.shootSpeed = 10f;
                Item.UseSound = SoundID.Item8;
                ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            }
            else // Left-click (default)
            {
                // Default scythe stats (TwoPathsPro logic)
                SetDefaultsToScythe();
                Item.damage = 1425;
                scytheSoulCharge = 4;
                Item.width = 86;
                Item.height = 90;
                Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
                Item.rare = ModContent.RarityType<HotPink>();
                Item.shoot = ModContent.ProjectileType<TwoPathsPro>();
                Item.mana = 0;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // Right-click (Recitation-style attack)
            {
                float spread = 60f * 0.0174f;
                double startAngle = Math.Atan2(velocity.X, velocity.Y) - spread / 2;
                double deltaAngle = spread / 6f;
                double offsetAngle;
                int i;
                for (i = 0; i < 3; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(source, player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<WhiteScythe>(), damage, knockback, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(source, player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<WhiteScythe>(), damage, knockback, Main.myPlayer, 0f, 0f);
                }
                return base.Shoot(player, source, position, velocity, type, 150, knockback);
            }
            // Default behavior (left click)
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
