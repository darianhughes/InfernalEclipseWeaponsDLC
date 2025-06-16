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
    public class OcramKnifeProBolt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.None;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            for (int i = 0; i < 3; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 150);
                dust.velocity = dust.velocity * 0.5f + Projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
                dust.scale = (5 - i) * 0.4f;
                dust.noGravity = true;
                dust.position = Projectile.Center + Projectile.velocity * 2f * (i + 1);
            }

            if (Projectile.ai[0] <= 60f)
            {
                Projectile.velocity *= 0.9f;
            }
            else
            {
                Projectile.friendly = true;
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 10f, 5f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60);

            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
