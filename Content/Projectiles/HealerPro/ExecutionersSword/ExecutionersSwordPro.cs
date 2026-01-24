using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using InfernalEclipseWeaponsDLC.Content.Buffs;

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
            Projectile.width = 32;
            Projectile.height = 32;
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
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(46.5f);
            }
            else
            {
                if (stuckTarget > -1 && Main.npc[stuckTarget].active)
                {
                    // Follow the NPC with a fixed offset
                    Projectile.Center = Main.npc[stuckTarget].Center + offsetFromNPC;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;

                    // --- Alternate projectile spawn logic ---
                    Projectile.ai[0]++; // timer
                    if (Projectile.ai[0] >= 10) // half second
                    {
                        Projectile.ai[0] = 0;

                        if (Projectile.owner == Main.myPlayer)
                        {
                            int projType;
                            Vector2 randDir;

                            // alternate between dark/light using ai[1]
                            if (Projectile.ai[1] == 0)
                            {
                                projType = ModContent.ProjectileType<ExecutionersSwordDarkEnergy>();
                                Projectile.ai[1] = 1;

                                // random direction + random speed 10–18
                                randDir = Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f, 18f);

                                SoundEngine.PlaySound(SoundID.Item103, Projectile.position);
                            }
                            else
                            {
                                projType = ModContent.ProjectileType<ExecutionersSwordLightEnergy>();
                                Projectile.ai[1] = 0;

                                // random direction + random speed 6–10
                                randDir = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 10f);

                                SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                            }

                            Projectile.NewProjectile(
                                Projectile.GetSource_FromAI(),
                                Projectile.Center,
                                randDir,
                                projType,
                                Projectile.damage,
                                Projectile.knockBack,
                                Projectile.owner
                            );
                        }
                    }

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
                target.AddBuff(ModContent.BuffType<SwordStuckBuff>(), 360);
                Projectile.timeLeft = 360;
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 360);

                SoundEngine.PlaySound(SoundID.Item71, Projectile.position);

                // Store the initial offset from the NPC's center
                offsetFromNPC = Projectile.Center - target.Center;

                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(target.width / 2f, target.height / 2f);
                    Vector2 spawnPos = target.Center + offset;
                    Vector2 vel = offset.SafeNormalize(Vector2.UnitY) * 6f;

                    Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Enchanted_Gold, vel, 150, Color.White, 1.5f);
                    dust.noGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(target.width / 2f, target.height / 2f);
                    Vector2 spawnPos = target.Center + offset;
                    Vector2 vel = offset.SafeNormalize(Vector2.UnitY) * 2f;

                    Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Enchanted_Gold, vel, 150, Color.White, 1.5f);
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition - new Vector2(0, 4f); // shift 12 pixels down
            Main.spriteBatch.Draw(texture, drawPos, null, lightColor, Projectile.rotation,
                texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false; // prevent default draw
        }
    }
}
