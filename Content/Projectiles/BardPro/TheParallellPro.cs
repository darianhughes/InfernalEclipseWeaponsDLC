using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ThoriumMod;
using ReLogic.Content;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro
{
    public class TheParallellPro : BardProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CosmicKunai";
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 13;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 2;
            Projectile.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
        }

        public override void AI()
        {
            // Start slow, accelerate up to a maximum spin speed
            float lifeFraction = 1f - (Projectile.timeLeft / 300f); // 0 at spawn, 1 at death (if 300 timeLeft)
            float minSpin = 0.04f;  // starting speed
            float maxSpin = 0.4f;   // ending speed
            float rotationSpeed = MathHelper.Lerp(minSpin, maxSpin, lifeFraction);

            Projectile.rotation += rotationSpeed * Projectile.direction;

            //// Delay homing by 20 frames (1/3 second at 60 FPS)
            int homingDelay = 5;
            Projectile.ai[0]++;

            if (Projectile.ai[0] > homingDelay)
            {
                NPC target = FindNearestTarget(200f);
                if (target != null)
                {
                    Vector2 toTarget = target.Center - Projectile.Center;
                    float homingStrength = 1.00f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget.SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length(), homingStrength);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() / 2f;
            float fade = MathHelper.Min(1f, Projectile.timeLeft / 20f);

            // Signus color palette (customize these as desired)
            Color[] colorA = { new Color(84, 39, 89, 255), new Color(36, 16, 56, 160) };         // Main Signus body to tail
            Color[] colorB = { new Color(218, 164, 255, 180), new Color(84, 39, 89, 100) };       // Magical highlight to deep purple

            // Trail: negative wave
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                double wave = Math.Sin((Projectile.timeLeft + Projectile.whoAmI + i) * Math.PI / 5.0);
                if (wave < 0)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.oldRot[i]) * (float)(wave * 12.0) * i / Projectile.oldPos.Length;
                    Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition - offset;
                    Color color = Color.Lerp(colorA[0], colorA[1], i / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, pos, null, color, 0f, origin, Projectile.scale * fade * MathHelper.Lerp(0.4f, 0.1f, i / (float)Projectile.oldPos.Length), SpriteEffects.None, 0f);
                }
            }

            // Trail: positive wave
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                double wave = Math.Sin((Projectile.timeLeft + Projectile.whoAmI + i) * Math.PI / 5.0);
                if (wave >= 0)
                {
                    Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.oldRot[i]) * (float)(wave * 12.0) * i / Projectile.oldPos.Length;
                    Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + offset;
                    Color color = Color.Lerp(colorA[0], colorA[1], i / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, pos, null, color, 0f, origin, Projectile.scale * fade * MathHelper.Lerp(0.4f, 0.1f, i / (float)Projectile.oldPos.Length), SpriteEffects.None, 0f);
                }
            }

            // Outward highlight trail (magical purple to deep purple)
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.oldRot[i]) * (float)(Math.Sin((Projectile.timeLeft + Projectile.whoAmI + i) * Math.PI / 5.0) * 12.0) * i / Projectile.oldPos.Length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + offset;
                Color color = Color.Lerp(colorB[0], colorB[1], i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, pos, null, color, 0f, origin, Projectile.scale * fade * MathHelper.Lerp(0.4f, 0.1f, i / (float)Projectile.oldPos.Length), SpriteEffects.None, 0f);
            }

            // Draw the projectile itself
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        public override void BardOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];
            Vector2 toPlayer = owner.Center - Projectile.Center;
            // Point the projectile's 'down' (texture's positive Y) towards the player
            Projectile.rotation = toPlayer.ToRotation() + MathHelper.PiOver2;
            // If you want the point to face away from the player (as if thrown from the player), use:
            // Projectile.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
        }

        private NPC FindNearestTarget(float range)
        {
            NPC closest = null;
            float minDist = range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this))
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = npc;
                    }
                }
            }
            return closest;
        }
    }
}
