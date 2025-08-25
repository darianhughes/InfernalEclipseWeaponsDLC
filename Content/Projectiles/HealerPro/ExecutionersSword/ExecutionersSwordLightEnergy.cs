using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class ExecutionersSwordLightEnergy : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty"; // invisible

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => true;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Default; // not a damaging projectile
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Player target = null;
            float closestDist = 600f; // heal range

            // Find closest teammate (excluding self)
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team) // only teammates
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

            // Homing onto player
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                float speed = 10f;
                Projectile.velocity = (Projectile.velocity * 20f + direction * speed) / 21f;
            }
            else
            {
                Projectile.velocity *= 0.96f; // slow down if no target
            }

            // White dust trail
            for (int i = 0; i < 2; ++i)
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Enchanted_Gold,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    150,
                    Color.White,
                    1.2f
                );
                Main.dust[dustIndex].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player healer = Main.player[Projectile.owner];

            // Heal teammates only
            if (healer.team != 0 && healer.team == target.team && target.whoAmI != healer.whoAmI)
            {
                HealTeammateThorium(healer, target, baseHeal: 0);

                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(target.width / 2f, target.height / 2f);
                    Vector2 spawnPos = target.Center + offset;
                    Vector2 vel = offset.SafeNormalize(Vector2.UnitY) * 2f;

                    Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Enchanted_Gold, vel, 150, Color.White, 1.5f);
                    dust.noGravity = true;
                }

                Projectile.Kill();
            }
        }

        private void HealTeammateThorium(Player healer, Player target, int baseHeal)
        {
            if (healer.whoAmI != Main.myPlayer) return; // only run on owner client

            ThoriumPlayer thoriumHealer = healer.GetModPlayer<ThoriumPlayer>();
            ThoriumPlayer thoriumTarget = target.GetModPlayer<ThoriumPlayer>();

            int totalHeal = baseHeal + thoriumHealer.healBonus;

            target.statLife += totalHeal;
            if (target.statLife > target.statLifeMax2)
                target.statLife = target.statLifeMax2;

            target.HealEffect(totalHeal);

            thoriumTarget.mostRecentHeal = totalHeal;
            thoriumTarget.mostRecentHealer = healer.whoAmI;
            thoriumHealer.healedTarget = target.whoAmI;

            for (int i = 0; i < 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(16, 16),
                    DustID.Enchanted_Gold, Vector2.Zero, 150, Color.White, 1.3f);
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false; // invisible
    }
}
