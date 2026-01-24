using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Utilities;
using CalamityMod.Items;
using CalamityMod.Items.Potions;
using CalamityMod.CalPlayer;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Core;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.SuperCell
{
    [AutoloadEquip(EquipType.Head)]
    public class SuperCellCirclet : ModItem
    {
        public override void SetStaticDefaults()
        {
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[equipSlot] = true;   // Draw all hair
            ArmorIDs.Head.Sets.DrawHatHair[equipSlot] = false;   // Don’t limit hair shape
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 10;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SuperCellGuard>() && legs.type == ModContent.ItemType<SuperCellSabatons>();
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer calamityPlayer = player.Calamity();

            calamityPlayer.rogueStealthMax += 1.1f;
            player.setBonus = this.GetLocalizedValue("SetBonus");
            player.GetThoriumPlayer().techPointsMax += 2;
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            ref StatModifier damage = ref player.GetDamage(DamageClass.Throwing);
            damage += 0.05f;
        }

        public override void AddRecipes()
        {
            Mod thorium = ModLoader.GetMod("ThoriumMod");

            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(thorium.Find<ModItem>("HallowedGuise").Type, 1);
            recipe.AddRecipeGroup(RecipeGroups.Titanium, 12);
            recipe.AddIngredient(ItemID.SoulofFlight, 10);

            if (ModLoader.TryGetMod("Consolaria", out Mod consolariaMod))
            {
                recipe.AddIngredient(consolariaMod.Find<ModItem>("SoulofBlight").Type, 10);
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
        }
    }
}
