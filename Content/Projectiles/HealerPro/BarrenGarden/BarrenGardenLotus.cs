using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.BarrenGarden
{
    public class BarrenGardenLotus : ModProjectile
    {
        private int shootTimer;
        private bool alternate;

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 164;

            // Draw adjustments — anchor from bottom center properly
            DrawOriginOffsetY = 1; // Move origin up to match bottom alignment
            DrawOffsetX = 0; // Remove horizontal shift — fixes left offset

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            ((ModProjectile)this).Projectile.penetrate = -1;
        }


        public override void OnSpawn(IEntitySource source)
        {
            int tileX = (int)(Projectile.Center.X / 16f);
            int tileY = (int)(Projectile.Center.Y / 16f);

            SoundEngine.PlaySound(SoundID.Item46, Projectile.position);

            for (int y = tileY; y < Main.maxTilesY - 10; y++)
            {
                Tile tile = Main.tile[tileX, y];
                if (tile == null)
                    continue;

                bool solidBelow =
                    tile.HasTile &&
                    (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);

                if (solidBelow)
                {
                    // Directly set the projectile's bottom to match the tile
                    Projectile.position.Y = y * 16f - Projectile.height; // <-- subtract full height instead of using Bottom
                    break;
                }
            }

            // Center horizontally on the tile
            Projectile.position.X = tileX * 16f + 8f - Projectile.width / 2f;

            Projectile.velocity = Vector2.Zero;

            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, -2f, 150, default, 1.3f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            shootTimer++;

            // Gentle idle dust
            if (Main.rand.NextBool(20))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, -1f, 100, Color.White, 1.2f);
                Main.dust[dust].noGravity = true;
            }

            if (shootTimer >= 30) // every 1/2 second
            {
                shootTimer = 0;
                alternate = !alternate;

                if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
                {
                    string projName = alternate ? "BlackLotusPro2" : "LotusPro2";
                    int projType = thorium.Find<ModProjectile>(projName).Type;

                    Vector2 spawnPos = Projectile.Center - new Vector2(0f, Projectile.height / 2f - 12f);

                    // Base velocity (straight up)
                    Vector2 baseVel = new Vector2(0f, -30f);

                    // Fire 3 projectiles: center, slight left, slight right
                    for (int i = -1; i <= 1; i++)
                    {
                        // Rotate by small angle (5 degrees spread)
                        Vector2 shootVel = baseVel.RotatedBy(MathHelper.ToRadians(0 * i));

                        Projectile.NewProjectile(
                            Projectile.GetSource_FromAI(),
                            spawnPos,
                            shootVel,
                            projType,
                            Projectile.damage * 2,
                            Projectile.knockBack,
                            Projectile.owner
                        );
                    }

                    // Dust burst
                    for (int i = 0; i < 5; i++)
                    {
                        int dust = Dust.NewDust(spawnPos, 10, 10, DustID.Frost, 0f, -3f, 100, Color.White, 1.4f);
                        Main.dust[dust].noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Frost, 0f, -2f, 150, default, 1.3f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
