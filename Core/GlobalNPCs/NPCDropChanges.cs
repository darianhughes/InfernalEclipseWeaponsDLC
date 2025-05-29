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

namespace InfernalEclipseWeaponsDLC.Core.GlobalNPCs
{
    public class NPCDropChanges : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                if (npc.type == ModContent.NPCType<Signus>())
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheParallel>(), 3));
                }

                if (npc.type == ModContent.NPCType<Bumblefuck>())
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BirbSaxophone>(), 3));
                }

                if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeathsWhisper>(), 3));
                }
            }
        }
    }
}
