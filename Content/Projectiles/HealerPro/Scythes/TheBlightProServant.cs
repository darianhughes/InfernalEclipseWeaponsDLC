using InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Buffs;
using ThoriumMod.Projectiles.Bard;
using ThoriumMod.Projectiles.Scythe;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes
{
    public class TheBlightProServant : ModProjectile
    {
        public int servantIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public ref float AI_Timer => ref Projectile.ai[1];

        public bool Despawn
        {
            get => Projectile.ai[2] > 0f;
            set => Projectile.ai[2] = value ? 1 : -1;
        }

        public Player Player => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 61;
            Projectile.height = 64;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;

            Despawn = false;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void AI()
        {
            AI_Timer++;
            Projectile.rotation = 0f;

            float movementProgress;
            if (Despawn)
            {
                Projectile.Opacity -= 0.05f;
                movementProgress = MathHelper.Clamp(Projectile.timeLeft / 20f, 0, 1);
            }
            else
            {
                Projectile.timeLeft = int.MaxValue;
                movementProgress = MathHelper.Clamp(AI_Timer / 20f, 0, 1);

                if (Projectile.Opacity < 1f)
                    Projectile.Opacity += 0.1f;

                if (Projectile.owner == Main.myPlayer)
                {
                    bool theBlightEquipped = Player.HeldItem.type == ModContent.ItemType<TheBlight>() || Main.mouseItem.type == ModContent.ItemType<TheBlight>();
                    if (!theBlightEquipped)
                    {
                        Despawn = true;
                        Projectile.timeLeft = 20;
                        Projectile.netUpdate = true;
                    }
                }

                if (Player.dead || !Player.active)
                {
                    Despawn = true;
                    Projectile.timeLeft = 20;
                    Projectile.netUpdate = true;
                }
            }

            float radians = (AI_Timer * 0.025f) % MathHelper.TwoPi + (MathHelper.TwoPi / 2f * servantIndex);
            Projectile.Center = Player.Center + new Vector2(0, Player.gfxOffY) + new Vector2(75 * movementProgress, 0).RotatedBy(radians);
            Projectile.rotation = AI_Timer * 0.025f % MathHelper.TwoPi;

            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            }
        }

        public virtual void Shoot(int damage, float knockBack)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default) * 20f;

                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TheBlightProServantLaser>(), damage, knockBack);
            }
        }
    }
}
