using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Signus;
using CalamityMod.Items.TreasureBags;
using Terraria.UI;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.OldDuke;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Rogue;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Summoner;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod;
using Terraria.ID;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SupremeCalamitas;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void;

namespace InfernalEclipseWeaponsDLC.Core.GlobalNPCs
{
    public class NPCDropChanges : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<ForeverHungry>(), 3));

                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BottleOfSouls>(), 3));
            }

            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                if (npc.type == ModContent.NPCType<DesertScourgeHead>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SandSlasher>(), 3));
                }

                if (npc.type == ModContent.NPCType<AquaticScourgeHead>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<SulphuricShanty>(), 7));
                }

                if (npc.type == ModContent.NPCType<BrimstoneElemental>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BrimstoneHarp>(), 3));
                }

                if (npc.type == ModContent.NPCType<AstrumDeusHead>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DeusFlute>(), 3));
                }

                if (npc.type == ModContent.NPCType<RavagerBody>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<NecroticChorus>(), 3));
                }

                if (npc.type == ModContent.NPCType<Signus>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<TheParallel>(), 3));
                }

                if (npc.type == ModContent.NPCType<Bumblefuck>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<BirbSaxophone>(), 3));
                }

                if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DeathsWhisper>(), 3));
                }

                if (npc.type == ModContent.NPCType<OldDuke>())
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<CorrodedCane>(), 4));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<DukeSynth>(), 4));
                }

                if (npc.type == ModContent.NPCType<SupremeCataclysm>())
                {
                    npcLoot.Add(ItemDropRule.Common(ModLoader.HasMod("SOTS") ? Mod.Find<ModItem>("CataclysmicGauntletVoid").Type : ModContent.ItemType<CataclysmicGauntlet>(), 10));
                }

                if (npc.type == ModContent.NPCType<SupremeCatastrophe>())
                {
                    npcLoot.Add(ItemDropRule.Common(ModLoader.HasMod("SOTS") ? Mod.Find<ModItem>("CatastrophicLongbladeVoid").Type : ModContent.ItemType<CatastrophicLongblade>(), 10));
                }
            }

            if (ModLoader.TryGetMod("Consolaria", out Mod console))
            {
                if (npc.type == console.Find<ModNPC>("Ocram").Type)
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<OcramKnife>(), 4));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<TheBlight>(), 4));
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<Legacy>(), 4));
                }
            }

            if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
            {
                if (npc.type == thorium.Find<ModNPC>("TheGrandThunderBird").Type)
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GrandThunderWhip>(), 5));
                }

                if (npc.type == thorium.Find<ModNPC>("Lich").Type)
                {
                    var dropRule = new CommonDropNotScalingWithLuck(ModContent.ItemType<ListoftheDamned>(), 5, 1, 1)
                    {
                        chanceNumerator = 2 // 2/5 = 40%
                    };

                    // Wrap the custom rule in a LeadingConditionRule so it only fires when NotExpert is true
                    var leadingRule = new LeadingConditionRule(new Conditions.NotExpert());
                    leadingRule.Add(dropRule);

                    npcLoot.Add(leadingRule);
                }
            }

            if (ModLoader.TryGetMod("FargowiltasSouls", out Mod souls) && !ModLoader.TryGetMod("YharimEX", out _))
            {
                if (npc.type == souls.Find<ModNPC>("MutantBoss").Type)
                {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<AuricBrimfireCrosier>(), 1));
                }
            }
        }
    }
}
