using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard;

namespace InfernalEclipseWeaponsDLC.Core.GlobalNPCs
{
    public class NPCDropChanges : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // Try to get the Calamity Mod and Signus NPC type
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.Find<ModNPC>("Signus") is ModNPC signus)
                {
                    if (npc.type == signus.Type)
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheParallel>(), 3));
                    }
                }
            }
        }
    }
}
