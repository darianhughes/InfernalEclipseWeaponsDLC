using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using rail;
using CalamityMod;
using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee.Void;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Terraria.DataStructures;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.Void
{
    public class SupremeCataclysmFist : ModProjectile, ILocalizedModType
    {
        public ref float Time => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public override string Texture => "CalamityMod/Projectiles/Boss/SupremeCataclysmFist";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 126;
            Projectile.height = 54;

            Projectile.friendly = true;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 20;
            Projectile.Opacity = 1f;

            Projectile.DamageType = ModLoader.TryGetMod("SOTS", out Mod sots) ? sots.Find<DamageClass>("VoidMelee") : ModContent.GetInstance<TrueMeleeDamageClass>();

            Projectile.scale = 0.5f;
        }

        public override void AI()
        {
            // Keep flying exactly as fired; no velocity changes on purpose.

            // Face travel direction.
            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(-Projectile.velocity.Y, -Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            }

            // Simple 4-frame animation (every 5 ticks).
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 5) % Main.projFrames[Projectile.type];

            // Subtle red light
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0f);

            Time++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Owner.GiveUniversalIFrames(ModLoader.HasMod("SOTS") ? CataclysmicGauntletVoid.OnHitIFrames : CataclysmicGauntlet.OnHitIFrames);

            if (hit.Damage <= 0 || Projectile.Opacity != 1f) return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 240, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            // Keep optional alt texture if you spawn with ai[1] == 1f.
            if (Projectile.ai[1] == 1f)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/SupremeCataclysmFistAlt").Value;

            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(
                texture,
                pos,
                frame,
                Projectile.GetAlpha(lightColor),
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0);

            return false; // handled drawing
        }

        internal static void GenerateDustOnOwnerHand(Player player)
        {
            if (Main.dedServ)
                return;

            Vector2 handOffset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
                handOffset.X = player.bodyFrame.Width - handOffset.X;
            if (player.gravDir != 1f)
                handOffset.Y = player.bodyFrame.Height - handOffset.Y;

            handOffset -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - player.height) / 2f;
            Vector2 rotatedHandPosition = player.RotatedRelativePoint(player.position + handOffset, true);
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 0, 0, (int)CalamityDusts.Brimstone, 0f, 0f, 150, default, 1.3f);
                dust.position = rotatedHandPosition;
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity += player.velocity;
                if (Main.rand.NextBool())
                {
                    dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
                    dust.scale += Main.rand.NextFloat();
                }
            }
        }
    }
}
