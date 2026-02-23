using System.Collections.Generic;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.ShadowflameAxePro
{
    public class ShadowflameAxeProjectile : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Melee/ShadowflameAxe";

        public List<Vector2> OldPosition;
        public List<float> OldRotation;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();
            Projectile.netImportant = true;

            Projectile.scale = 1.25f;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            float rotation = 0f;

            if (owner.HeldItem.ModItem is not ShadowflameAxe)
            { // this is because the projectile lingers longer than its usetime
                Projectile.Kill();
            }

            switch (Projectile.ai[0])
            {
                case 0:  // down swing
                    rotation = Projectile.ai[2] - MathHelper.Pi * owner.direction - (Projectile.ai[1] + 50f) * 0.045f * owner.direction;
                    break;
                case 1:  // up swing
                    rotation = Projectile.ai[2] + MathHelper.Pi * owner.direction + (Projectile.ai[1] + 50f) * 0.045f * owner.direction;
                    break;
                case 2:  // circle + spawn projectiles
                    rotation = Projectile.ai[2] - MathHelper.PiOver2 * owner.direction - (Projectile.ai[1] + 60f) * 0.066f * owner.direction;
                    if (Projectile.ai[1] < 50 && Projectile.localAI[0] == 1)
                    {
                        Projectile.ResetLocalNPCHitImmunity();
                        Projectile.localAI[0] = 0;

                        SoundEngine.PlaySound(SoundID.Item103, owner.Center);
                        int projectileType = ModContent.ProjectileType<ShadowflameAxeBolt>();
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 velocity = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i).RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(7f, 13f);
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), owner.Center, velocity, projectileType, (int)(Projectile.damage * 0.5f), Projectile.knockBack * 0.5f, Projectile.owner, 90, 0f, Main.rand.Next(-15, 15));
                        }
                    }
                    break;
                default: // should never happen
                    Projectile.Kill();
                    break;
            }

            Vector2 shoulderOffset = new Vector2(-6 * owner.direction, 0); // Aligns the weapon with the player shoulder center
            Projectile.Center = owner.Center + Vector2.UnitY.RotatedBy(rotation) * 50f + shoulderOffset;
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            Projectile.rotation = (Projectile.Center - (owner.Center + shoulderOffset)).ToRotation() + MathHelper.PiOver4;
            Projectile.velocity = Vector2.UnitX * owner.direction * 0.0001f; // for knockback direction

            if (Projectile.ai[1] > 1)
            { // Keeps the projectile alive while being 'used'
                Projectile.timeLeft = 10;
            }

            if (Projectile.ai[1] > 5)
            {
                Projectile.localAI[1] = owner.direction; // used to prevent the player from flipping at the end of the swing animation
                Projectile.friendly = true;
            }
            else
            {
                Projectile.friendly = false; // So the projectile only deals damage while swinging;
            }

            Projectile.ai[1] *= 0.9f;

            // Trail & Dust

            OldPosition.Add(Projectile.Center);
            OldRotation.Add(Projectile.rotation);

            if (OldPosition.Count > 15)
            {
                OldPosition.RemoveAt(0);
                OldRotation.RemoveAt(0);
            }

            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            float rotationoffset = 0f;

            if (player.direction == -1)
            {
                if (Projectile.ai[0] != 1)
                { // upwards swing
                    spriteEffects = SpriteEffects.FlipVertically;
                    rotationoffset -= MathHelper.PiOver2;
                }
            }
            else if (Projectile.ai[0] == 1)
            { // upwards swing
                spriteEffects = SpriteEffects.FlipVertically;
                rotationoffset -= MathHelper.PiOver2;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Color color = new Color(182, 27, 248);
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                drawPosition2.Y += player.gfxOffY;
                spriteBatch.Draw(texture, drawPosition2, null, color * 0.05f * i, OldRotation[i] + rotationoffset, texture.Size() * 0.5f, Projectile.scale * i * 0.08f, spriteEffects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            drawPosition.Y += player.gfxOffY;
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation + rotationoffset, texture.Size() * 0.5f, Projectile.scale, spriteEffects, 0f);

            return false;
        }
    }
}
