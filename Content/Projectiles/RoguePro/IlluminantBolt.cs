using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.RoguePro
{
    public class IlluminantBolt : ModProjectile
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        private int helixRot;
        private int activationTimer; // 🟢 Counts up to 60 ticks (1 second)
        private bool activated => activationTimer >= 30;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 20;
            Projectile.scale = 1.1f;
        }

        public override void AI()
        {
            helixRot++;
            activationTimer++; // count frames since spawn

            // Soft cyan-green light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.9f, 0.8f);

            // Gentle drift
            Projectile.velocity *= 0.99f;

            // Small helix dust trail
            Vector2 helixOffset = HelixOffset();
            int dustIndex = Dust.NewDust(
                Projectile.Center + helixOffset - new Vector2(5f),
                0, 0,
                DustID.BlueTorch,
                0f, 0f,
                100,
                Color.Lerp(Color.Cyan, Color.Green, Main.rand.NextFloat(0.3f, 0.7f)),
                1.2f
            );
            Dust dust = Main.dust[dustIndex];
            dust.noGravity = true;
            dust.velocity *= 0.1f;

            // 🔸 Disable damage & homing for the first second
            if (!activated)
            {
                Projectile.friendly = false; // can't damage yet
                return; // skip homing logic until active
            }

            // Once active:
            Projectile.friendly = true;
            float homingRange = 600f;
            float turnSpeed = 0.1f;

            NPC target = FindClosestNPC(homingRange);
            if (target != null)
            {
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(
                    Projectile.velocity.SafeNormalize(Vector2.Zero),
                    desired,
                    turnSpeed
                ) * Projectile.velocity.Length();

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 6f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private Vector2 HelixOffset()
        {
            float wave = (float)Math.Sin(MathHelper.ToRadians(helixRot * 6f)) * 8f;
            float dir = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            return new Vector2(wave, 0f).RotatedBy(dir);
        }

        private NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closest = null;
            float sqrMax = maxDetectDistance * maxDetectDistance;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this))
                {
                    float sqrDist = Vector2.DistanceSquared(npc.Center, Projectile.Center);
                    if (sqrDist < sqrMax)
                    {
                        sqrMax = sqrDist;
                        closest = npc;
                    }
                }
            }
            return closest;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.BlueTorch,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
                    100,
                    Color.Lerp(Color.Cyan, Color.Green, Main.rand.NextFloat(0.3f, 0.7f)),
                    1.3f
                );
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Color color = Color.Lerp(Color.Cyan, Color.LightGreen, 0.5f) * 0.8f;

            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = HelixOffset() * (0.2f * i);
                Main.EntitySpriteDraw(
                    texture,
                    drawPos + offset,
                    null,
                    color * (1f - i * 0.15f),
                    Projectile.rotation,
                    drawOrigin,
                    Projectile.scale * (1.1f - i * 0.05f),
                    SpriteEffects.None
                );
            }

            return false;
        }
    }
}
