using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.ShadowflameAxePro
{
    public class ShadowflameAxeBolt : ModProjectile
    {
        private static Texture2D TextureGlow;

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 40;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.7f;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            TextureGlow ??= ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        { // this is a somewhat classic homing projectile, I'm too lazy to comment what it does, sorry :<
            if (Projectile.ai[2] != 0)
            {
                Projectile.timeLeft += (int)Projectile.ai[2];
                Projectile.ai[2] = 0;
            }

            if (Projectile.timeLeft <= 10)
            {
                Projectile.ai[1] = 1f;
                Projectile.friendly = false;
            }
            else if (Projectile.timeLeft <= 120)
            {
                Projectile.friendly = true;
                Projectile.localAI[0]++;
            }

            if (Projectile.ai[1] == 1f)
            {
                if (Projectile.timeLeft > 10)
                {
                    Projectile.timeLeft = 10;
                }

                Projectile.velocity *= 0.5f;
            }
            else
            {
                NPC target = null;

                float distanceClosest = 240f;
                foreach (NPC npc in Main.npc)
                {
                    float distance = Projectile.Center.Distance(npc.Center);
                    if (npc.active && !npc.friendly && !npc.CountsAsACritter && distance < distanceClosest && !npc.dontTakeDamage)
                    {
                        target = npc;
                        distanceClosest = distance;
                    }
                }


                if (target != null && Projectile.timeLeft > 10)
                {
                    float speed = Projectile.velocity.Length();
                    Projectile.velocity += (target.Center - Projectile.Center) * (0.004f + Projectile.localAI[0] * 0.001f);
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * speed;

                    if (Projectile.ai[1] > 0)
                    {
                        Projectile.timeLeft++;
                        Projectile.ai[1]--;
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);
            if (OldPosition.Count > 25)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }

            if (Main.rand.NextBool(2))
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame).velocity *= 0.3f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[1] = 1f;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            float colorMult = 1f;
            if (Projectile.timeLeft < 10) colorMult *= Projectile.timeLeft / 10f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = OldPosition[i] - Main.screenPosition;
                spriteBatch.Draw(TextureGlow, drawPosition2, null, Color.White * 0.03f * (i + 1) * colorMult, OldRotation[i], TextureGlow.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPosition, null, Color.White * colorMult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
