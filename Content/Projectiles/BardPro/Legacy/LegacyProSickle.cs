using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Buffs;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Projectiles.Scythe;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.Legacy
{
    public class LegacyProSickle : BardProjectile
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DemonSickle;

        public override void SetBardDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DemonSickle);
            AIType = ProjectileID.DemonSickle;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 2;
            Projectile.tileCollide = false;
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            base.BardOnHitNPC(target, hit, damageDone);
        }
    }
}
