using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro
{
    public class LightChainProjectile : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/LightChain";
        private int Timespent = 0;
        private static Texture2D TextureChainBack;
        private static Texture2D TextureChainFront;

        public const float MaxRange = 480f; // 320 = 20 tiles 

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            TextureChainBack ??= ModContent.Request<Texture2D>(Texture + "Back", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TextureChainFront ??= ModContent.Request<Texture2D>(Texture + "Front", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Projectile.netImportant = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Projectile.ai[1] && Timespent % 60 == 0 && Timespent > 0)
            { // hits 1 per second
                return base.CanHitNPC(target);
            }

            return false;
        }

        public override void AI()
        {
            Timespent++;
            Player owner = Main.player[Projectile.owner];

            if (!owner.active || owner.dead)
            {
                SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(0.5f), Projectile.Center);
                Projectile.Kill();
            }

            if (Projectile.timeLeft > 5 && Projectile.ai[2] == 1f)
            {
                Projectile.timeLeft = 5;
            }

            NPC target = Main.npc[(int)Projectile.ai[1]];
            if (target != null && target.active)
            {
                Projectile.Center = target.Center;

                if (owner.Distance(target.Center) > MaxRange + 80f && Projectile.ai[2] != 1f)
                { // Max Range + 5 tiles (break the chain if player too far)
                    Projectile.ai[2] = 1f;
                    Projectile.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(0.5f), Projectile.Center);
                }
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(0.5f), Projectile.Center);
                Projectile.Kill();
            }

            if (Timespent < 60)
            { // spawn animation
                Rectangle drawRectangle = TextureChainBack.Bounds;
                drawRectangle.Height /= 6; // there are 6 segment textures for the chain

                Vector2 segment = Projectile.Center - owner.Center;
                segment = Vector2.Normalize(segment) * (drawRectangle.Height - 6);

                int amountSegments = 0;
                while ((Projectile.Center - segment * amountSegments).Distance(owner.Center) > drawRectangle.Height && amountSegments < 100)
                { // counts the number of total segments for the stem, used for the spawn animation
                    amountSegments++;
                }

                if (Timespent % 3 == 0 && Timespent <= amountSegments)
                {
                    SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(Main.rand.NextFloat(-0.2f, 0.2f)).WithVolumeScale(0.35f));
                }
            }

            Projectile.velocity = Vector2.Normalize(owner.Center - target.Center) * 0.0001f; // for knockback direction
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;

            float colorMult = 1f;
            if (Projectile.timeLeft < 5) colorMult *= Projectile.timeLeft / 5f;

            Rectangle drawRectangleBack = TextureChainBack.Bounds;
            drawRectangleBack.Height /= 6; // there are 6 textures for the chain

            Rectangle drawRectangleFront = TextureChainFront.Bounds;
            drawRectangleFront.Height /= 6; // there are 6 textures for the chain

            Vector2 segment = Projectile.Center - owner.Center;
            segment = Vector2.Normalize(segment) * (drawRectangleBack.Height - 6);
            float rotation = segment.ToRotation() - MathHelper.PiOver2;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float distToMaxRange = MaxRange - Projectile.Center.Distance(owner.Center);

            int amountSegments = 0;
            while ((Projectile.Center - segment * amountSegments).Distance(owner.Center) > drawRectangleFront.Height && amountSegments < 100)
            { // counts the number of total segments for the chain
                amountSegments++;
            }

            Random random = new Random((int)Projectile.ai[0]); // generates the random from the "seed" contained in ai[0], in order to pcik the random stem textures

            float sineMult = 5f * ((distToMaxRange > 0 ? distToMaxRange : 0) / MaxRange); // less wavy stem if the player is further away

            Vector2 scaleSquish = new Vector2(Projectile.scale, Projectile.scale);
            if (distToMaxRange < 16f)
            {
                scaleSquish.X += 0.015f * (distToMaxRange - 16f);
                if (scaleSquish.X < 0.5f)
                {
                    scaleSquish.X = 0.5f;
                }
            }

            int count = 0;
            while (count < amountSegments)
            {
                count += 2;
                if (Timespent >= (amountSegments - count))
                {
                    drawRectangleBack.Y = random.Next(5) * drawRectangleBack.Height;

                    drawPosition = Projectile.Center - segment * count;
                    Color color = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f)) * colorMult;
                    drawPosition -= Main.screenPosition;
                    if (count < 4) sineMult *= count / 3f; // less wavy towards the base
                    drawPosition += Vector2.UnitY.RotatedBy(rotation - MathHelper.PiOver2) * (float)Math.Sin((Timespent + count * 5) * 0.1f) * sineMult; // wave offset
                    spriteBatch.Draw(TextureChainBack, drawPosition, drawRectangleBack, color, rotation, drawRectangleBack.Size() * 0.5f, scaleSquish, SpriteEffects.None, 0f);
                }
            }

            random = new Random((int)Projectile.ai[0] + 1);
            count = 0;
            while (count < amountSegments - 1)
            {
                count += 2;
                if (Timespent >= (amountSegments - count))
                {
                    drawRectangleFront.Y = random.Next(5) * drawRectangleFront.Height;

                    drawPosition = Projectile.Center - segment * (count + 1);
                    Color color = Lighting.GetColor((int)(drawPosition.X / 16f), (int)(drawPosition.Y / 16f)) * colorMult;
                    drawPosition -= Main.screenPosition;
                    if (count < 4) sineMult *= count / 3f; // less wavy towards the base
                    drawPosition += Vector2.UnitY.RotatedBy(rotation - MathHelper.PiOver2) * (float)Math.Sin((Timespent + count * 5) * 0.1f) * sineMult; // wave offset
                    spriteBatch.Draw(TextureChainFront, drawPosition, drawRectangleFront, color, rotation, drawRectangleFront.Size() * 0.5f, scaleSquish, SpriteEffects.None, 0f);
                }
            }


            return false;
        }
    }
}
