using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Projectiles.Scythe;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ThoriumMod.Buffs;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TheBlightProScythe : ScythePro
    {
        public override void SafeSetDefaults()
        {
            // Shared values
            dustOffset = new Vector2(-35, 7f);
            dustCount = 4;
            dustType = 75;
            Projectile.Size = new Vector2(140f, 150f);
        }
        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            base.SafeOnHitNPC(target, hit, damageDone);
        }
    }
}
