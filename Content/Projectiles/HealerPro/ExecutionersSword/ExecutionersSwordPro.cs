using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Buffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class ExecutionersSwordPro : ModProjectile
    {
        private bool stuck = false;
        private int stuckTarget = -1;
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Healer/ExecutionersSword";
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
            if (stuck && stuckTarget > -1 && Main.npc[stuckTarget].active)
            {
                Projectile.Center = Main.npc[stuckTarget].Center;
                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 60;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!stuck)
            {
                stuck = true;
                stuckTarget = target.whoAmI;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                target.AddBuff(ModContent.BuffType<SwordStuckBuff>(), Projectile.timeLeft);
            }
        }
    }
}
