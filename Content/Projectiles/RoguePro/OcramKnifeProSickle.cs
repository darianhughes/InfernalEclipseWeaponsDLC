using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro;
using Microsoft.Xna.Framework;
using Steamworks;
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
    public class OcramKnifeProSickle : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DemonSickle;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.DemonSickle);
            AIType = ProjectileID.DemonSickle;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = 1;

            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[ProjectileID.DemonSickle];
            }

            if (Projectile.ai[2] < 100f)
            {
                Projectile.ai[2] *= (Projectile.ai[0] == 1) ? 1.08f : 1.04f;
            }
            else
            {
                Projectile.ai[2] = 100f;
            }

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 15f * (Projectile.ai[2] / 100), 10f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
