using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Tiles.Abyss;
using InfernalEclipseWeaponsDLC.Content.Items.Materials;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer.Melee;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Tiles;

namespace InfernalEclipseWeaponsDLC.Core.ChestLoot
{
    public class WeaponDLCWorldGen : ModSystem
    {
        public override void PostWorldGen()
        {
            int drawlType = ModContent.ItemType<DeepSeaDrawl>();
            int tridentType = ModContent.ItemType<DeepseaTrident>();

            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);
                if (!tile.HasTile || tile.TileType != ModContent.TileType<AbyssTreasureChest>())
                    continue;

                // === Add DeepSeaDrawl ===
                bool hasDrawl = chest.item.Any(item => item.type == drawlType);
                if (!hasDrawl)
                {
                    int slot = Array.FindIndex(chest.item, i => i.IsAir);
                    if (slot != -1)
                    {
                        chest.item[slot].SetDefaults(drawlType);
                        chest.item[slot].stack = 1;
                    }
                }

                // === Add DeepseaTrident ===
                bool hasTrident = chest.item.Any(item => item.type == tridentType);
                if (!hasTrident)
                {
                    int slot = Array.FindIndex(chest.item, i => i.IsAir);
                    if (slot != -1)
                    {
                        chest.item[slot].SetDefaults(tridentType);
                        chest.item[slot].stack = 1;
                    }
                }
            }

            // === Aquatic Depths Biome Chest handling ===
            for (int i = 0; i < Main.chest.Length; i++)
            {
                var chest = Main.chest[i];
                if (chest == null) continue;

                var tile = Main.tile[chest.x, chest.y];
                if (tile.TileType == ModContent.TileType<AquaticDepthsBiomeChest>())
                {
                    var newItem = new Item(ModContent.ItemType<DeepSeaDrawlShard1>());
                    int slot = Array.FindIndex(chest.item, x => x.IsAir);
                    if (slot != -1)
                        chest.item[slot] = newItem;
                }
            }
        }
    }
}
