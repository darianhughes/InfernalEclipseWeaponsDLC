using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro
{
    public class OcramKnifePro : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Rogue/OcramKnife";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 75, new Color(100, 200, 0));
            }

            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Cloud, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 75, new Color(100, 200, 0));
            }

            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.rotation = (0.5f * Projectile.ai[0]) % MathHelper.TwoPi;

                if (Projectile.timeLeft % 5 == 3)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OcramKnifeProSickle>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, 0f, 1f, 1f);
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

                if (Projectile.timeLeft % 20 == 10)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<OcramKnifeProSickle>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, 0f, 0f, 1f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            if (Projectile.owner == Main.myPlayer)
            {
                for (int w = 0; w < 3; w++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<OcramKnifeProBolt>(), Projectile.damage / 6, Projectile.knockBack / 6, Main.myPlayer, 1f);
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
