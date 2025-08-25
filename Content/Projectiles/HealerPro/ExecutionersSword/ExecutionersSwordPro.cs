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
using CalamityMod.Buffs.DamageOverTime;
using Terraria.ID;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class ExecutionersSwordPro : ModProjectile
    {
        private bool stuck = false;
        private int stuckTarget = -1;
        private Vector2 offsetFromNPC;

        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Healer/ExecutionersSword";
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 68;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.aiStyle = 0;

            Projectile.usesLocalNPCImmunity = true;

            // Number of ticks before this projectile can hit the same NPC again
            Projectile.localNPCHitCooldown = 20; // 10 ticks = 1/6 second
        }


        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1f, 0.9f, 0.2f);

            if (!stuck)
            {
                if (Projectile.velocity != Vector2.Zero)
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }
            else
            {
                if (stuckTarget > -1 && Main.npc[stuckTarget].active)
                {
                    // Follow the NPC with a fixed offset
                    Projectile.Center = Main.npc[stuckTarget].Center + offsetFromNPC;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.Kill();
                }
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
                target.AddBuff(ModContent.BuffType<SwordStuckBuff>(), 300);
                Projectile.timeLeft = 300;
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);

                // Store the initial offset from the NPC's center
                offsetFromNPC = Projectile.Center - target.Center;
            }

            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(target.width / 2f, target.height / 2f);
                Dust dust = Dust.NewDustPerfect(target.Center + offset, DustID.Enchanted_Gold, Vector2.Zero, 150, Color.White, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
