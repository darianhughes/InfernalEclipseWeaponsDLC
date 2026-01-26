using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Utilities;
using CalamityMod.Items;
using CalamityMod.Items.Potions;
using CalamityMod.CalPlayer;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Core;
using System;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.SuperCell
{
    [AutoloadEquip(EquipType.Head)]
    public class SuperCellCirclet : ModItem
    {
        public static int wingsSlot = -1;
        public static Lazy<Asset<Texture2D>> wingAsset;

        public override void Load()
        {
            wingsSlot = EquipLoader.AddEquipTexture(Mod, Texture + "_WingsFake", EquipType.Wings, null, Name + "_Wings", new SuperCellWings());
            if (Main.dedServ)
                return;
            wingAsset = new Lazy<Asset<Texture2D>>(() => ModContent.Request<Texture2D>(Texture + "_Wings", AssetRequestMode.AsyncLoad));
        }

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

            const int supercellWingTime = 170;

            if (player.wings <= 0 || player.wingTimeMax < supercellWingTime)
            {
                player.wings = wingsSlot;
                player.wingsLogic = ArmorIDs.Wing.BeetleWings;
                player.wingTimeMax = supercellWingTime;
                player.noFallDmg = true;
            }
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

    public class SuperCellWings : EquipTexture
    {
        public override bool WingUpdate(Player player, bool inUse)
        {
            const int frames = 4;

            // GLIDE STATE: hold last frame (frame 3)
            if (player.wingsLogic > 0 && player.velocity.Y > 0f && !player.controlJump)
            {
                player.wingFrame = 2;           // hold frame 3
                player.wingFrameCounter = 0;    // stop animation
                return true;
            }

            // Normal animation logic
            int frameTime;

            if (player.velocity.Y < 0f || player.jump > 0) // flapping upwards
                frameTime = 4;
            else if (player.velocity.Y != 0f) // falling normally
                frameTime = 6;
            else // standing
            {
                frameTime = 0;
                player.wingFrame = 0;
                player.wingFrameCounter = 0;
                return true;
            }

            player.wingFrameCounter++;
            if (player.wingFrameCounter >= frameTime)
            {
                player.wingFrameCounter = 0;
                player.wingFrame++;

                if (player.wingFrame >= frames)
                    player.wingFrame = 0;
            }

            return true;
        }
    }
}
