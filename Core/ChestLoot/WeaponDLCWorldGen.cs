using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Tiles.Abyss;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using Terraria;
using Terraria.ModLoader;

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
        }
    }
}
