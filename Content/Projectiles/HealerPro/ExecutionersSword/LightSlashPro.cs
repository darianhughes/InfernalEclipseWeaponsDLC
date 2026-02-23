using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Buffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.ExecutionersSword
{
    public class LightSlashPro : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.TerraBeam;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            // Face the direction of movement
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Optional: trail dust (white)
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin,
                    Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 150, Color.White, 1.4f);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[ProjectileID.TerraBeam].Value;
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                texture.Size() / 2,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Only if sword is stuck in enemy
            if (target.HasBuff(ModContent.BuffType<SwordStuckBuff>()))
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    // Light energy (heals)
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        target.Center,
                        Main.rand.NextVector2Circular(4f, 4f),
                        ModContent.ProjectileType<LightEnergyProj>(),
                        0, 0, Projectile.owner
                    );
                    // Dark energy (damages, homes)
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        target.Center,
                        Main.rand.NextVector2Circular(4f, 4f),
                        ModContent.ProjectileType<DarkEnergyProj>(),
                        damageDone / 2, 0, Projectile.owner, target.whoAmI
                    );
                }
            }
        }
    }
}
