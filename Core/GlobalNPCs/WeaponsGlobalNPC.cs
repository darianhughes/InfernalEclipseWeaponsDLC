using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.GlobalNPCs
{
    public class WeaponsGlobalNPC : GlobalNPC
    {
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.type != ModContent.ItemType<Stick>())
                return;

            if (!npc.boss)
                return;

            if (BossHasNoContactDamage(npc))
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        private static bool BossHasNoContactDamage(NPC npc)
        {
            if (npc.damage <= 0)
                return true;

            return false;
        }
    }
}
