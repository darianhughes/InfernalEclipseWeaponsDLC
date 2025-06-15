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

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TheBlightProServantLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.MiniRetinaLaser;
        
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.MiniRetinaLaser);
            AIType = ProjectileID.MiniRetinaLaser;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }
    }
}
