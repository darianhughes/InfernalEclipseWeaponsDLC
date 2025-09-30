﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CalamityMod.BiomeManagers;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Items.Materials;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Security.Policy;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.Void;

namespace InfernalEclipseWeaponsDLC.Core.NewFolder
{
    public class InfernalWeaponsPlayer : ModPlayer
    {
        public bool spearSearing;
        public bool spearArctic;

        const int shard2chance = 20;

        public int missileIndex = 10;
        public int CataclysmFistShotCount = 0;

        public override void ResetEffects()
        {
            spearSearing = false;
            spearArctic = false;
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool isSulfurCatch = Player.InModBiome<SulphurousSeaBiome>();
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (!isSulfurCatch || !inWater) return;

            bool goodEnoughLevel = attempt.fishingLevel >= 45;
            bool randomChanceSuccess = Main.rand.NextBool(shard2chance);

            if (!randomChanceSuccess || !goodEnoughLevel) return;

            itemDrop = ModContent.ItemType<DeepSeaDrawlShard2>();
        }

        public override void PostUpdateMiscEffects()
        {
            MiscEffects();
        }

        private void MiscEffects()
        {
            if (ModLoader.HasMod("SOTS"))
            {
                
                if (Player.ActiveItem().type == Mod.Find<ModItem>("CataclysmicGauntletVoid").Type) //we have to do it this way since the item doesn't load without SOTS.
                {
                    SupremeCataclysmFist.GenerateDustOnOwnerHand(Player);
                }
            }
            else
            {
                if (Player.ActiveItem().type == ModContent.ItemType<CataclysmicGauntlet>())
                {
                    SupremeCataclysmFist.GenerateDustOnOwnerHand(Player);
                }
            }
        }
    }
}
