using System;
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
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.Void;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Core.NewFolder
{
    public class InfernalWeaponsPlayer : ModPlayer
    {
        public bool spearSearing;
        public bool spearArctic;
        public bool minionCrits;

        const int shard2chance = 20;

        public int missileIndex = 10;
        public int CataclysmFistShotCount = 0;

        public override void ResetEffects()
        {
            spearSearing = false;
            spearArctic = false;
            minionCrits = false;
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

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.hostile)
                return;

            if (minionCrits && IsSummonDamage(proj))
            {
                if (Main.rand.Next(100) < ActualClassCrit(Player, DamageClass.Summon))
                    modifiers.SetCrit();
            }
        }

        public override void PostUpdateMiscEffects()
        {
            MiscEffects();
        }

        private void MiscEffects()
        {
            if (ModLoader.HasMod("SOTS"))
            {
                
                if (Player.HeldItem.type == Mod.Find<ModItem>("CataclysmicGauntletVoid").Type) //we have to do it this way since the item doesn't load without SOTS.
                {
                    SupremeCataclysmFist.GenerateDustOnOwnerHand(Player);
                }
            }
            else
            {
                if (Player.HeldItem.type == ModContent.ItemType<CataclysmicGauntlet>())
                {
                    SupremeCataclysmFist.GenerateDustOnOwnerHand(Player);
                }
            }
        }

        // thank you fargos
        public static bool IsSummonDamage(Projectile projectile, bool includeMinionShot = true, bool includeWhips = true)
        {
            if (!includeWhips && ProjectileID.Sets.IsAWhip[projectile.type])
                return false;

            if (!includeMinionShot && (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                return false;

            return projectile.CountsAsClass(DamageClass.Summon) || projectile.minion || projectile.sentry || projectile.minionSlots > 0 || ProjectileID.Sets.MinionSacrificable[projectile.type]
                || (includeMinionShot && (ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                || (includeWhips && ProjectileID.Sets.IsAWhip[projectile.type]);
        }

        public float ActualClassCrit(Player player, DamageClass damageClass)
            => (damageClass == DamageClass.Summon || damageClass == DamageClass.SummonMeleeSpeed) && !(minionCrits)
            ? 0
            : player.GetTotalCritChance(damageClass);
    }
}
