using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.StarScepter
{
    public class StarScepterStar : ModProjectile
    {
        private static Texture2D TextureGlow;
        public int TimeSpent = 0;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 310;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            TextureGlow ??= ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Projectile.ai[2]++; // time spent alive
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.1f);
            TimeSpent++;

            if (Projectile.localAI[1] == 0f)
            { // random values on spawn
                Projectile.localAI[0] = Main.rand.NextFloat(MathHelper.TwoPi); // light sine()
                Projectile.localAI[1] = Main.rand.NextFloat(MathHelper.TwoPi); // rotation sine()
                SoundEngine.PlaySound(SoundID.Item105.WithPitchOffset(Main.rand.NextFloat(-0.5f, 0f)).WithVolumeScale(0.66f), Projectile.Center);
            }

            if (Projectile.ai[0] != 0f)
            { // timeleft reset
                Projectile.ai[0] = 0f;
                Projectile.timeLeft = 310;
                SoundEngine.PlaySound(SoundID.Item105.WithPitchOffset(Main.rand.NextFloat(0.5f, 1f)).WithVolumeScale(0.5f), Projectile.Center);

                foreach (Projectile projectile in Main.projectile)
                { // So existing projectiles don't get stuck in a loop of disappearing and reappearing if the player uses the item a bit late (hard to explain, but makes the use feel better)
                    if (projectile.active && projectile.type == Projectile.type && Projectile.owner == projectile.owner && projectile.timeLeft < 90)
                    {
                        projectile.timeLeft = 90;
                    }
                }

                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2, 2), 4, 4, DustID.YellowStarDust);
                    dust.velocity *= 1.25f;
                    dust.scale *= Main.rand.NextFloat(0.5f, 0.75f);
                }
            }

            Projectile.rotation = (float)Math.Sin(Projectile.localAI[0] + TimeSpent * 0.035f) * 0.5f;

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch);
                dust.velocity *= 0.25f;
                dust.scale *= Main.rand.NextFloat(0.5f, 0.75f);
                dust.noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust);
                dust.velocity *= 0.5f;
                dust.scale *= Main.rand.NextFloat(0.2f, 0.4f);
            }

            // Pew

            Player owner = Main.player[Projectile.owner];
            if (TimeSpent % 24 == 0)
            {
                int projectileType = ModContent.ProjectileType<StarScepterBolt>();

                float closestDistance = 480f; // 30 tiles range
                NPC closestTarget = null;

                foreach (NPC npc in Main.npc)
                {
                    bool closestBoss = false; // for boss prio targetting
                    if (closestTarget != null)
                    {
                        if (closestTarget.boss)
                        {
                            closestBoss = true;
                        }
                    }

                    if (npc.active && !npc.friendly && !npc.CountsAsACritter && !npc.dontTakeDamage && Collision.CanHitLine(owner.Center - new Vector2(2, 2), 4, 4, npc.position, npc.width, npc.height))
                    {
                        float distance = Projectile.Center.Distance(npc.Center);
                        Point point = new Point((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y);
                        if ((distance < closestDistance || closestTarget == null && npc.Hitbox.Contains(point) || distance < 80f && npc.boss) && (!closestBoss || npc.boss))
                        {
                            closestTarget = npc;
                            closestDistance = distance;
                        }
                    }
                }

                if (closestTarget != null)
                {
                    Vector2 velocity = Vector2.Normalize(Projectile.Center - closestTarget.Center).RotatedByRandom(1f) * 3.8f; // 3.8f is a magic number that makes the trail look good ...
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: closestTarget.whoAmI);
                }
            }

            // Movement relative to player

            int count = 0; // nb of more recent projectiles
            int countTotal = 0; // nb of other projectiles
            float highestTimespent = 0;
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.type == Type && Projectile.owner == projectile.owner)
                {
                    countTotal++;

                    if (projectile.ai[2] < Projectile.ai[2])
                    {
                        count++;
                    }

                    if (projectile.ai[2] > highestTimespent)
                    {
                        highestTimespent = projectile.ai[2];
                    }
                }
            }

            if (owner.active && !owner.dead)
            {
                Vector2 targetPosition = owner.Center - Vector2.UnitY.RotatedBy(highestTimespent * 0.02f + MathHelper.TwoPi / countTotal * count) * (16f + Math.Max(owner.width, owner.height));
                Projectile.velocity = (targetPosition - Projectile.Center) * 0.1f + owner.velocity;
            }
            else if (Projectile.timeLeft > 10)
            { // kil!
                Projectile.timeLeft = 10;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact.WithPitchOffset(Main.rand.NextFloat(0.5f)).WithVolumeScale(0.75f), Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(2, 2), 4, 4, DustID.YellowStarDust);
                dust.velocity *= 1.25f;
                dust.scale *= Main.rand.NextFloat(0.66f, 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            drawPosition.Y += player.gfxOffY;
            spriteBatch.Draw(texture, drawPosition, null, lightColor * 1.5f, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            float sine = 0.6f + (float)Math.Sin(Projectile.localAI[0] + TimeSpent * 0.05f) * 0.15f;
            spriteBatch.Draw(texture, drawPosition, null, Color.White * sine, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureGlow, drawPosition, null, Color.White * sine, Projectile.rotation, TextureGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
