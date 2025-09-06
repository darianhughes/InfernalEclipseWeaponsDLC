using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace InfernalEclipseWeaponsDLC
{
    internal static class NPCHelper
    {
        public static bool IsHostile(this NPC npc, object attacker = null, bool ignoreDontTakeDamage = false)
        {
            return !npc.friendly && npc.lifeMax > 5 && npc.chaseable && !npc.dontTakeDamage | ignoreDontTakeDamage && !npc.immortal;
        }
    }
}
