using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Magic;
using ThoriumMod;
using Terraria.GameContent.UI.Elements;
using Terraria.DataStructures;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Buffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles
{
    public class WhiteScythe : BeastScythe
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 54;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<LightCurse>(), 300, false);
            int index1 = -1;
            for (int index2 = 0; index2 < Main.maxNPCs; ++index2)
            {
                NPC npc = Main.npc[index2];
                float num = Projectile.DistanceSQ((npc).Center);
                if (npc.CanBeChasedBy(null, false) && (double)num < 490000.0 && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1))
                    index1 = index2;
            }
            if (index1 == -1)
                return;
            NPC npc1 = Main.npc[index1];
            IEntitySource sourceFromThis = Projectile.GetSource_FromThis(null);
            int num1 = ModContent.ProjectileType<ZRealitySlasherSlash>();
            int num2 = (int)(Projectile.damage * 0.25);
            int num3 = Main.rand.Next(4);
            if (num3 == 0)
                Projectile.NewProjectile(sourceFromThis, npc1.Center.X, npc1.Center.Y + 40f, 0.0f, -15f, num1, num2, 0.0f, Projectile.owner, 0.0f, 0.0f, 0.0f);
            if (num3 == 1)
                Projectile.NewProjectile(sourceFromThis, npc1.Center.X, npc1.Center.Y - 40f, 0.0f, 15f, num1, num2, 0.0f, Projectile.owner, 0.0f, 0.0f, 0.0f);
            if (num3 == 2)
                Projectile.NewProjectile(sourceFromThis, npc1.Center.X + 40f, npc1.Center.Y, -15f, 0.0f, num1, num2, 0.0f, Projectile.owner, 0.0f, 0.0f, 0.0f);
            if (num3 != 3)
                return;
            Projectile.NewProjectile(sourceFromThis, npc1.Center.X - 40f, npc1.Center.Y, 15f, 0.0f, num1, num2, 0.0f, Projectile.owner, 0.0f, 0.0f, 0.0f);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return base.PreDraw(ref lightColor);
        }
    }
}
