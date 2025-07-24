using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Projectiles.Bard;
using Microsoft.Xna.Framework;

using ThoriumMod;
using CalamityMod.Buffs.DamageOverTime;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.RestoredDeepSeaDrawl
{
    public class OurSharknado : BardProjectile
    {
        List<Projectile> children = [];

        int numRecurs;
        const int RECURSION_DELAY = 5;
        const int RECURSION_AMT = 10;

        float spawnOffset;

        int[] projs;

        public bool shouldSpawnProjs = true;

        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public void SetSeaCreatureSpawning(bool enable)
        {
            shouldSpawnProjs = enable;

            // double damage so it's more effective when it doesn't fire off anything
            Projectile.damage = enable ? 60 : 120;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Sharknado}";
        public override void SetBardDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Sharknado);
            Projectile.damage = 60;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;

            spawnOffset = 30;

            Projectile.friendly = false;
            Projectile.hostile = false;

            projs = [
                ModContent.ProjectileType<DrawlEel>(),
                ModContent.ProjectileType<DrawlStarfish>(),
                ModContent.ProjectileType<DrawlIsopod>()
            ];

            //Fixes the projectile so other projectiles can hit the entity being hit by this proj.
            Projectile.usesIDStaticNPCImmunity = true;
            this.Projectile.idStaticNPCHitCooldown = Projectile.localNPCHitCooldown;
            Projectile.usesLocalNPCImmunity = false;
        }
        public override void OnKill(int timeLeft)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void PostAI()
        {

            if (shouldSpawnProjs)
            {
                foreach (var child in children)
                {
                    if (Main.rand.NextBool(180))
                    {
                        // spawn random projectile
                        int dir = Main.rand.NextBool(2) ? -1 : 1;

                        Projectile.NewProjectile(Entity.GetSource_FromThis(),
                            child.position + new Vector2(Main.rand.Next(child.width), Main.rand.Next(child.height)),
                            new Vector2(Main.rand.NextFloat(2f, 4f) * dir, Main.rand.NextFloat(-1f, -3f)),
                            projs[Main.rand.Next(projs.Length)], Projectile.damage, Projectile.knockBack, ai0: 20f * child.position.Y);
                    }
                }
            }

            if (numRecurs >= RECURSION_AMT) return;
            if (Projectile.timeLeft % RECURSION_DELAY != 0) return;

            var nado = Projectile.NewProjectileDirect(Entity.GetSource_FromThis(),
                Projectile.position - new Vector2(0, spawnOffset * numRecurs), Vector2.Zero, ProjectileID.Sharknado, Projectile.damage, Projectile.knockBack, Projectile.owner);

            nado.friendly = true;
            nado.hostile = false;

            numRecurs++;

            children.Add(nado);
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 180);
        }
    }
}
