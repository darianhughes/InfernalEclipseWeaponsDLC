using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.SpearTipPro
{
    public class CryonicSpearTip : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Accessories/Melee/SpearTips/ArcticSpearTip";

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 5; // Pierces 5 times
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.light = 0.2f;
        }

        public override void AI()
        {
            // No gravity
            Projectile.velocity.Y += 0f;

            // Rotate to match velocity direction (looks like a spear tip)
            if (Projectile.velocity != Vector2.Zero)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4;

            // Emit blue dust (Ice)
            Dust dust = Dust.NewDustDirect(
                Projectile.position,
                Projectile.width,
                Projectile.height,
                DustID.Ice, // could try DustID.BlueCrystalShard for a different look
                0f, 0f, // No velocity
                150, // Alpha (transparency)
                default, // Color (default/automatic)
                1.2f // Scale
            );
            dust.noGravity = true; // Floats with the projectile
            dust.velocity *= 0.3f; // Stays close
            Vector2 dustPos = Projectile.Center - Projectile.velocity * 0.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
