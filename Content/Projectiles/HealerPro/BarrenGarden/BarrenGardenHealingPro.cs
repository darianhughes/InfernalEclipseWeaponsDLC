using CalamityMod.Buffs.DamageOverTime;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using InfernalEclipseWeaponsDLC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Buffs;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Buffs.Healer;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.BarrenGarden
{
    public class BarrenGardenHealingPro : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.width = 18;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = false; // doesn’t damage enemies
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false; // allows smooth homing
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 150) * Projectile.Opacity;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Rotate to match velocity
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            // Find nearest teammate to home in on
            Player target = null;
            float closestDist = 600f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team)
                    {
                        float dist = Vector2.Distance(Projectile.Center, p.Center);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            target = p;
                        }
                    }
                }
            }

            // Homing behavior (sharper like HealingSoul)
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                float speed = 10f;
                Projectile.velocity = (Projectile.velocity * 20f + direction * speed) / 21f;
            }
            else
            {
                // Slow down if no target
                Projectile.velocity *= 0.96f;
            }

            // Heal on contact with a teammate
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team)
                    {
                        if (Projectile.Hitbox.Intersects(p.Hitbox))
                        {
                            HealTeammateThorium(owner, p, baseHeal: 1);
                            Projectile.Kill();
                            break;
                        }
                    }
                }
            }
        }

        private void HealTeammateThorium(Player healer, Player target, int baseHeal)
        {
            if (healer.whoAmI != Main.myPlayer) return;
            if (healer == target) return;
            if (healer.team == 0 || healer.team != target.team) return;

            if (baseHeal <= 0 && healer.GetModPlayer<ThoriumPlayer>().healBonus <= 0)
                return;

            HealerHelper.HealPlayer(
                healer,
                target,
                healAmount: baseHeal,
                recoveryTime: 60,
            healEffects: true,
                extraEffects: p => p.AddBuff(ModContent.BuffType<Cured>(), 30, true, false)
            );
        }

        public override void OnKill(int timeLeft)
        {
            // Smaller burst of Frostburn dust on death
            for (int i = 0; i < 3; i++)
            {
                int dust = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Frost,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    0,
                    default,
                    1f
                );
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool? CanHitNPC(NPC target) => false;
    }
}
