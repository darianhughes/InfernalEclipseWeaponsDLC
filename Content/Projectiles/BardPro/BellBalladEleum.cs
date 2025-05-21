using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Projectiles.Bard;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro
{
    public class BellBalladEleum : BardProjectile
    {
        public int BellIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public ref float AI_Timer => ref Projectile.ai[1];

        public Player Player => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
        }

        public override void SetBardDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 26;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void AI()
        {
            Projectile.timeLeft = 2;
            AI_Timer++;
            float radians = (AI_Timer * 0.01f) % MathHelper.TwoPi + (MathHelper.TwoPi / 3f * BellIndex);
            Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center + new Vector2(0, Player.gfxOffY) + new Vector2(75, 0).RotatedBy(radians), AI_Timer < 20 ? 0.1f : 0.4f);
            Projectile.rotation = 0f;
        }

        public virtual void Shoot(int damage, float knockBack)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default) * 10f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.FrostBoltStaff, damage, knockBack);
                proj.tileCollide = false; // TODO: might require dedicated ModProjectile for MP compat
            }
        }
    }
}
