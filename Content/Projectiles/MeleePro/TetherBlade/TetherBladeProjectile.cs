using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Terraria.Player;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.TetherBlade
{
    public class TetherBladeProjectile : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Melee/TetherBlade";

        public List<Vector2> OldPosition;
        public List<float> OldRotation;
        Color color = Color.White;

        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 20;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            OldPosition = new List<Vector2>();
            OldRotation = new List<float>();

            color = Main.rand.Next(3) switch
            {
                0 => new Color(119, 179, 247),
                1 => new Color(188, 119, 247),
                _ => new Color(247, 119, 224)
            };

            Projectile.localAI[0] = Main.rand.NextFloat(0.2f, 0.5f); // transparency
            for (int i = 0; i < 8; i++) OldPosition.Add(Projectile.Center); // initialize the trail

        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => overPlayers.Add(index);

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (Projectile.timeLeft == 20)
            {
                SoundEngine.PlaySound(SoundID.Item39.WithPitchOffset(Main.rand.NextFloat(0.4f)), Projectile.Center);
            }

            if (Projectile.ai[2] == -2f)
            { // blink thrust
                Projectile.Center = owner.Center + Vector2.UnitY.RotatedBy(Projectile.ai[0]) * (80f - Projectile.ai[1] * 2f);
            }
            else
            { // normal thrust
                Projectile.Center = owner.Center + Vector2.UnitY.RotatedBy(Projectile.ai[0]) * (40f - Projectile.ai[1]);
            }

            Vector2 toProjectile = Projectile.Center - owner.Center;
            Projectile.rotation = toProjectile.ToRotation() + MathHelper.PiOver4;
            Projectile.velocity = Vector2.UnitX * owner.direction * 0.0001f; // for knockback direction

            Projectile.ai[1] *= 0.825f;

            if (Projectile.ai[2] >= 0)
            { // only called on the last spawned projectile
                CompositeArmStretchAmount stretch = Projectile.ai[2] switch
                {
                    0 => CompositeArmStretchAmount.Full,
                    1 => CompositeArmStretchAmount.Quarter,
                    2 => CompositeArmStretchAmount.ThreeQuarters,
                    _ => CompositeArmStretchAmount.None
                };
                owner.SetCompositeArmFront(true, stretch, Projectile.ai[0]);
            }

            // Trail & Dust

            if (Projectile.timeLeft > 8)
            {
                OldPosition.Add(Projectile.Center);

                if (OldPosition.Count > 15)
                {
                    OldPosition.RemoveAt(0);
                }
            }
            else if (OldPosition.Count > 1)
            {
                OldPosition.RemoveAt(0);
                OldPosition.RemoveAt(0);
                Projectile.friendly = false;
            }

            if (Main.rand.NextBool(2))
            {
                int dustType = Main.rand.Next(2) switch
                {
                    0 => DustID.BlueCrystalShard,
                    _ => DustID.PurpleCrystalShard
                };

                Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(12, 12), Projectile.width - 24, Projectile.height - 24, dustType);
                dust.noGravity = true;
                dust.alpha = Main.rand.Next(80, 120);
                dust.scale = Main.rand.NextFloat(0.6f, 1f);
                dust.velocity *= 0.5f;
                dust.velocity += toProjectile * 0.1f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            float colorMult = 1f;
            if (Projectile.timeLeft < 5) colorMult *= Projectile.timeLeft / 5f;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            for (int i = 0; i < OldPosition.Count; i++)
            {
                Vector2 drawPosition2 = Vector2.Transform(OldPosition[i] - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
                drawPosition2.Y += player.gfxOffY;
                spriteBatch.Draw(texture, drawPosition2, null, color * Projectile.localAI[0] * 0.09f * i, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * i * 0.07f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Vector2 drawPosition = Vector2.Transform(Projectile.Center - Main.screenPosition, Main.GameViewMatrix.EffectMatrix);
            drawPosition.Y += player.gfxOffY;
            spriteBatch.Draw(texture, drawPosition, null, lightColor * colorMult * Projectile.localAI[0], Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
