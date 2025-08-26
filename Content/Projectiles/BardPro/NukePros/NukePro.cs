using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.NukePros
{
    public class NukePro : BardProjectile
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override string Texture => "CalamityMod/Projectiles/Boss/AresGaussNukeProjectile";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public sealed override void SetBardDefaults()
        {
            Projectile.width = 102;
            Projectile.height = 106;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999999;
            Projectile.tileCollide = false;
        }


        public int target = -1;
        public int random;
        public bool hit;
        public float groundY = 0;
        public bool firstFrame = true;
        public float mouseY = 0;
        public bool ignoreMousePos = false;
        public override void AI()
        {
            if (firstFrame)
            {
                firstFrame = false;
                mouseY = Main.MouseWorld.Y;
                Tile tile = Main.tile[new Point((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16)];
                ignoreMousePos = tile.HasTile && Main.tileSolid[tile.TileType];
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Projectile.frameCounter > 500)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter % 2 == 0)
            {
                Projectile.frame = Projectile.frame++ % 12;
            }


            if (Projectile.scale > 0 && hit)
            {
                Projectile.scale -= 0.01f;
            }


            if (hit && Projectile.scale < 0.01f)
            {
                Projectile.Kill();
            }

            if (hit)
            {
                Projectile.velocity.X = 0;
                Projectile.velocity.Y = 0;
                Projectile.Center += new Vector2(0, ((1 - Projectile.scale) * Projectile.height) / 100);
            }
            else
            {
                Lighting.AddLight(Projectile.Center, Color.Lime.ToVector3() * 0.8f);
                if (Projectile.Center.Y > mouseY || ignoreMousePos)
                {
                    Tile tile = Main.tile[new Point((int)(Projectile.Center.X + Projectile.rotation.ToRotationVector2().X * Projectile.height / 2) / 16, (int)(Projectile.Center.Y + Projectile.rotation.ToRotationVector2().Y * Projectile.height / 2) / 16)];
                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        groundY = Projectile.Center.Y;
                        // Projectile.Center -= Projectile.rotation.ToRotationVector2() * Projectile.height * 0.8f;
                        hit = true;
                        if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                        {
                            if (calamity.TryFind("WavePounderBoom", out ModProjectile boom))
                            {
                                Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                                    boom.Type, 20, 1f);
                                float j = 10;
                                projectile.ai[1] = 320f + j * 45f;
                                projectile.localAI[1] = Main.rand.NextFloat(0.08f, 0.25f);
                                projectile.Opacity = MathHelper.Lerp(0.18f, 0.6f, j / 7f) + Main.rand.NextFloat(-0.08f, 0.08f);
                                projectile.netUpdate = true;
                            }

                        }

                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
                    }
                }
                Projectile.rotation += 0.2f;

                Projectile.velocity.Y += 0.275f;
            }
        }
    }
}
