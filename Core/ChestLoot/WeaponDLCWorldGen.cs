using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Tiles.Abyss;
using InfernalEclipseWeaponsDLC.Content.Items.Materials;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Tiles;

namespace InfernalEclipseWeaponsDLC.Core.ChestLoot
{
    public class WeaponDLCWorldGen : ModSystem
    {
        public override void PostWorldGen()
        {
            // Your custom item
            int yourItemType = ModContent.ItemType<DeepSeaDrawl>();

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);
                if (!tile.HasTile || tile.TileType != ModContent.TileType<AbyssTreasureChest>())
                    continue;

                // Optional: only add if your item isn't already present
                bool hasItem = false;
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (chest.item[i].type == yourItemType)
                    {
                        hasItem = true;
                        break;
                    }
                }

                if (!hasItem)
                {
                    // Try to place in the first empty slot
                    for (int i = 0; i < Chest.maxItems; i++)
                    {
                        if (chest.item[i].IsAir)
                        {
                            chest.item[i].SetDefaults(yourItemType);
                            chest.item[i].stack = 1;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < Main.chest.Length; i++)
            {
                var chest = Main.chest[i];

                if (chest == null) continue;

                var tile = Main.tile[chest.x, chest.y];

                if (tile.TileType == ModContent.TileType<AquaticDepthsBiomeChest>())
                {
                    var newItem = new Item(ModContent.ItemType<DeepSeaDrawlShard1>());

                    var firstNoItem = Array.FindIndex(chest.item, x => x.IsAir);

                    // this does what .Append does. .NET 8 is WEIRD yo
                    chest.item[firstNoItem] = newItem;
                }
            }
        }
    }
}
