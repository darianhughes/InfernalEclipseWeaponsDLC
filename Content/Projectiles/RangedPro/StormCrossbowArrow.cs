using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RangedPro
{
    public class StormCrossbowArrow : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[1])
            {
                return false;
            }

            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            if (Projectile.timeLeft == 360)
            {
                Projectile.velocity *= 0.65f;
                SoundEngine.PlaySound(SoundID.Item94.WithPitchOffset(Main.rand.NextFloat(0.5f, 1f)).WithVolumeScale(0.5f), Projectile.Center);
                Projectile.penetrate += Main.rand.Next(3);
                Projectile.ai[1] = -1;
                Projectile.position += Projectile.velocity * 5f;
            }

            if (Projectile.timeLeft % 2 == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2, 2), 4, 4, DustID.Electric);
                dust.velocity *= 0.25f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.velocity.Y -= 1f;
                dust.scale *= Main.rand.NextFloat(0.5f, 0.75f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = target.whoAmI;
            List<NPC> validNPCs = new List<NPC>();
            SoundEngine.PlaySound(SoundID.Item93.WithPitchOffset(Main.rand.NextFloat(0.5f, 1f)).WithVolumeScale(0.25f), Projectile.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2, 2), 4, 4, DustID.Electric);
                dust.velocity.Y -= 1f;
                dust.scale *= Main.rand.NextFloat(0.5f, 0.75f);
            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.CountsAsACritter && !npc.dontTakeDamage && npc.Distance(Projectile.Center) < 160f && Collision.CanHitLine(Projectile.Center, 16, 16, npc.Center, 16, 16) && npc.whoAmI != (int)Projectile.ai[1])
                {
                    validNPCs.Add(npc);
                }
            }

            if (validNPCs.Count > 0)
            {
                NPC newTarget = validNPCs[Main.rand.Next(validNPCs.Count)];
                Projectile.velocity = Vector2.Normalize(newTarget.Center - Projectile.Center) * Projectile.velocity.Length();
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item93.WithPitchOffset(Main.rand.NextFloat(0.5f, 1f)).WithVolumeScale(0.25f), Projectile.Center);

            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2, 2), 4, 4, DustID.Electric);
                dust.velocity.Y -= 1f;
                dust.velocity *= 1.25f;
                dust.scale *= Main.rand.NextFloat(0.5f, 0.75f);
            }
            return true;
        }
    }
}
