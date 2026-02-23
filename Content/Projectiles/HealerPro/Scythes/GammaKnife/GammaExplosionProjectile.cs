using InfernalEclipseWeaponsDLC.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Tiles;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes.GammaKnife
{
    public class GammaExplosionProjectile : ModProjectile
    {
        /// <summary>
        ///     The lifespan of the projectile, in ticks.
        /// </summary>
        public const int LIFESPAN = 10;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.width = 64;
            Projectile.height = 64;

            Projectile.penetrate = -1;

            Projectile.timeLeft = LIFESPAN;

            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.ArmorPenetration = 15;
        }

        public override void AI()
        {
            base.AI();

            var position = Projectile.position;
            var velocity = Main.rand.NextVector2Circular(2f, 2f) * 4f;

            SpawnDustEffects(in position, Projectile.width, Projectile.height, in velocity);
        }

        private void SpawnDustEffects(in Vector2 position, int width, int height, in Vector2 velocity)
        {
            for (var i = 0; i < 5; i++)
            {
                var offset = Main.rand.NextVector2Circular(8f, 8f) * 2f;
                var dust = Dust.NewDustDirect(position + offset, width, height, ModContent.DustType<GammaDust>(), velocity.X, velocity.Y);

                dust.noGravity = true;

                dust.scale = Main.rand.NextFloat(1f, 3f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}