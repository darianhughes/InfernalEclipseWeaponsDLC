using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Buffs
{
    public class DefaultSummonTag : ModBuff
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        public static readonly int TagDamage = 5;

        public override void Update(Player player, ref int buffIndex)
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class TagedNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<DefaultSummonTag>() && projectile.IsMinionOrSentryRelated)
            {
                modifiers.FlatBonusDamage += DefaultSummonTag.TagDamage * ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            }
        }
    }
}
