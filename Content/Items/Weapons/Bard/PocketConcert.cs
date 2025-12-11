using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Empowerments;
using ThoriumMod;
using ThoriumMod.Items;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.PocketConcert;
using InfernalEclipseWeaponsDLC.Core.NewFolder;
using ThoriumMod.Sounds;
using CalamityMod.Items;
using CalamityMod.Rarities;
using static System.Net.Mime.MediaTypeNames;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class PocketConcert : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<EmpowermentProlongation>(3);
        }

        public override void SetBardDefaults()
        {
            Item.damage = 60;

            Item.autoReuse = true;
            Item.noMelee = true;

            Item.width = 32;
            Item.height = 18;

            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MusicalNoteProjectile>();

            ((ModItem)this).Item.GetGlobalItem<CalamityGlobalItem>().UsesCharge = true;
            ((ModItem)this).Item.GetGlobalItem<CalamityGlobalItem>().MaxCharge = 190f;
            ((ModItem)this).Item.GetGlobalItem<CalamityGlobalItem>().ChargePerUse = 0.1f;

            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;

            InspirationCost = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MysteriousCircuitry>(12)
                .AddIngredient<DubiousPlating>(18)
                .AddIngredient<UelibloomBar>(8)
                .AddIngredient(ItemID.LunarBar, 4)
                .AddTile(TileID.LunarCraftingStation)
                .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Func<bool> condition), condition)
                .Register();

            if (ModLoader.TryGetMod("CatalystMod", out Mod catalyst))
            {
                if (catalyst.TryFind("MetanovaBar", out ModItem metaNovaBar))
                {
                    // Catalyst-specific recipe using MetanovaBar instead of Uelibloom
                    CreateRecipe()
                        .AddIngredient<MysteriousCircuitry>(12)
                        .AddIngredient<DubiousPlating>(18)
                        .AddIngredient(metaNovaBar.Type, 4)
                        .AddIngredient(ItemID.LunarBar, 4)
                        .AddTile(TileID.LunarCraftingStation)
                        .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(4, out Func<bool> condition2), condition2)
                        .Register();
                }
            }
        }
    }
}