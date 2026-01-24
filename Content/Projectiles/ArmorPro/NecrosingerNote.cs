using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Necrosinger;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.ArmorPro
{
    public class NecrosingerNote : ModProjectile
    {
        // Orbit tuning
        private const float OrbitRadius = 54f;
        private const float OrbitAngularSpeed = 0.045f; // radians per tick
        private const float FollowLerp = 0.25f;

        private const int HitboxExpand = 8;

        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/ArmorPro/EighthNote";

        public static readonly HashSet<int> ExceptionTypes = new()
        {
            //add vanilla exceptions here
        };

        public override void SetStaticDefaults()
        {
            // add moded exception types here
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

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

            var mp = player.GetModPlayer<NecrosingerPlayer>();
            if (!mp.NecrosingerSet)
            {
                return;
            }

            Projectile.timeLeft = 2;

            // Orbit by slot (ai[0] = 0..2)
            int slot = (int)Projectile.ai[0];
            float baseAngle = (Main.GameUpdateCount * OrbitAngularSpeed) + (MathHelper.TwoPi / 3f) * slot;
            Vector2 desiredCenter = player.Center + baseAngle.ToRotationVector2() * OrbitRadius;

            // Smooth follow
            Projectile.Center = Vector2.Lerp(Projectile.Center, desiredCenter, FollowLerp);

            // A little spin for visuals
            Projectile.rotation = baseAngle + MathHelper.PiOver2;

            // Purple “phantom” vibe (light + occasional dust)
            Lighting.AddLight(Projectile.Center, 0.55f, 0.10f, 0.75f);
            if (Main.rand.NextBool(6))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity *= 0.2f;
            }

            // Block hostile projectiles on contact.
            TryBlockHostileProjectile(player, mp);
        }



        private void TryBlockHostileProjectile(Player player, NecrosingerPlayer mp)
        {
            Rectangle myBox = Projectile.Hitbox;
            myBox.Inflate(HitboxExpand, HitboxExpand);

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (!other.active)
                    continue;

                // Only block true hostile projectiles that can actually hurt players.
                if (!other.hostile || other.friendly)
                    continue;

                // Don’t block “intangible” / non-colliding projectiles.
                if (other.damage <= 0)
                    continue;

                // Ignore exceptions
                if (ExceptionTypes.Contains(other.type)) continue;

                if (!myBox.Intersects(other.Hitbox))
                    continue;

                // Success: destroy incoming projectile and consume this note.
                other.Kill();

                // Start recharge delay for missing notes.
                mp.StartRecharge();

                // Small feedback
                SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

                Projectile.Kill();
                return;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            Color purpleTint = Color.Purple;

            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                frame,
                purpleTint,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false;
        }
    }
}
