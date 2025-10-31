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
using ThoriumMod.Items.BardItems;
using ThoriumMod.Utilities;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.NukePros;
using InfernalEclipseWeaponsDLC.Core.NewFolder;
using ThoriumMod.Sounds;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class NecrosingerRibs : ModItem
    {
        public override void SetDefaults()
        {
            ((Entity)((ModItem)this).Item).width = 18;
            ((Entity)((ModItem)this).Item).height = 18;
            ((ModItem)this).Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            ((ModItem)this).Item.rare = 7;
            ((ModItem)this).Item.defense = 16;
        }

        public override void UpdateEquip(Player player)
        {
            ThoriumPlayer thoriumPlayer = player.GetThoriumPlayer();
            ref StatModifier damage = ref player.GetDamage((DamageClass)(object)ThoriumDamageBase<BardDamage>.Instance);
            damage += 0.14f;
            player.GetCritChance((DamageClass)(object)ThoriumDamageBase<BardDamage>.Instance) += 5f;
            thoriumPlayer.bardBuffDuration += 180;
            thoriumPlayer.bardResourceDropBoost += 0.1f;
        }

        public override void AddRecipes()
        {
            Mod consolaria = null;
            Mod thorium = ModLoader.GetMod("ThoriumMod");

            // Try to safely get Calamity and Ragnarok
            ModLoader.TryGetMod("Consolaria", out consolaria);

            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.HallowedPlateMail);
            recipe.AddIngredient(ItemID.AdamantiteBar, 12);
            recipe.AddIngredient(thorium.Find<ModItem>("SoulofPlight").Type, 15);

            if (ModLoader.TryGetMod("Consolaria", out Mod consolariaMod))
            {
                    recipe.AddIngredient(consolariaMod.Find<ModItem>("SoulofBlight").Type, 15);
            }
            else
            {
                recipe.AddIngredient<AureusCell>(10);
                recipe.AddIngredient(ItemID.SoulofSight, 5);
                recipe.AddIngredient(ItemID.SoulofMight, 5);
                recipe.AddIngredient(ItemID.SoulofFright, 5);
                    recipe.AddIngredient(ItemID.CursedFlame, 8);
            }

            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();

            recipe2.AddIngredient(ItemID.HallowedPlateMail);
            recipe2.AddIngredient(ItemID.TitaniumBar, 12);
            recipe2.AddIngredient(thorium.Find<ModItem>("SoulofPlight").Type, 15);

            if (ModLoader.TryGetMod("Consolaria", out Mod consolariaMod2))
            {
                recipe2.AddIngredient(consolariaMod2.Find<ModItem>("SoulofBlight").Type, 15);
            }
            else
            {
                recipe2.AddIngredient<AureusCell>(10);
                recipe2.AddIngredient(ItemID.SoulofSight, 5);
                recipe2.AddIngredient(ItemID.SoulofMight, 5);
                recipe2.AddIngredient(ItemID.SoulofFright, 5);
                recipe2.AddIngredient(ItemID.CursedFlame, 8);
            }

            recipe2.AddTile(TileID.MythrilAnvil);
            recipe2.Register();

            base.AddRecipes();
        }
    }
}
