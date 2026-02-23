using System;
using InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Eclipse;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword;
using InfernalEclipseWeaponsDLC.Core.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.ArmorPro
{
    public class EclipseEclipse : ModProjectile
    {
        public float HoverOffsetY = 25f;
        public float FireRange = 520f;
        public int FireCooldown = 45;
        public int BoltDamage = 45;
        public float BoltSpeed = 10f;
        public int HostCheckInterval = 10;
        public ref float FireTimer => ref Projectile.ai[1];

        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 2;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            var mp = player.GetModPlayer<EclipsePlayer>();
            if (!mp.EclipseSet)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            float above = MathF.Max(20f, player.height * 0.80f); // physics-only; no gfxOffY here
            Projectile.Center = player.Top + new Vector2(0f, -above);
            Projectile.velocity = Vector2.Zero;

            FireTimer++;
            if (FireTimer >= FireCooldown && Main.myPlayer == Projectile.owner)
            {
                int best = -1;
                float bestDist = FireRange;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (!n.active || n.friendly || !n.CanBeChasedBy()) continue;

                    float d = Vector2.Distance(n.Center, player.Center);
                    if (d <= bestDist && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1))
                    {
                        best = i;
                        bestDist = d;
                    }
                }

                if (best != -1)
                {
                    Vector2 dir = (Main.npc[best].Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * BoltSpeed;
                    int dmg = 75;
                    int bolt = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        dir,
                        ModContent.ProjectileType<ExecutionersSwordDarkEnergy>(),
                        dmg,
                        2f,
                        Projectile.owner
                    );
                    int healbolt = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        new(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4)),
                        ModContent.ProjectileType<ExecutionersSwordLightEnergy>(),
                        0,
                        2f,
                        Projectile.owner
                    );
                    if (Main.projectile.IndexInRange(bolt))
                        Main.projectile[bolt].tileCollide = true;

                    SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    FireTimer = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = Textures.AnyTexture256.Value;

            var effect = Effects.Eclipse?.Value;

            Player player = Main.player[Projectile.owner];


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, Main.Rasterizer, effect);


            effect?.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(texture, player.Center - Main.screenPosition - new Vector2(0, 45 - player.gfxOffY), null, Color.White, 0, texture.Size() / 2, 0.13f, SpriteEffects.None, 0);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            return false;
        }
    }
}
