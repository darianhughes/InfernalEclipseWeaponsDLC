using System;
using System.IO;
using InfernalEclipseWeaponsDLC.Content.Dusts;
using InfernalEclipseWeaponsDLC.Content.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Tiles;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes.GammaKnife
{
    public class GammaKnifeThrownProjectile : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Healer/GammaKnife";

        /// <summary>
        ///     The lifespan of the projectile, in ticks.
        /// </summary>
        public const int LIFESPAN = 500;

        /// <summary>
        ///     The maximum number of projectiles that can be stuck to an <see cref="NPC" /> instance at once.
        /// </summary>
        public const int MAX_STICK_COUNT = 1;

        /// <summary>
        ///     The state of the projectile when it is idling.
        /// </summary>
        public const float STATE_IDLE = 0f;

        /// <summary>
        ///     The state of the projectile when it is stuck to an <see cref="NPC" /> instance.
        /// </summary>
        public const float STATE_STICK = 1f;

        public const float STATE_RETURN = 2f;

        private int airTime;
        public const int RETURN_AFTER_TICKS = 120;

        /// <summary>
        ///     The sound played when the projectile explodes.
        /// </summary>
        public static readonly SoundStyle Sound = new($"{nameof(InfernalEclipseWeaponsDLC)}/Assets/Effects/Sounds/Explosion")
        {
            PitchVariance = 0.25f
        };

        /// <summary>
        ///     Gets the asset for the glowmask texture of the projectile.
        /// </summary>
        public static Asset<Texture2D> Glowmask { get; private set; }

        /// <summary>
        ///     Gets the swing projectile type.
        /// </summary>
        public static int SwingType { get; private set; }

        /// <summary>
        ///     The buffer that stores the positions of the projectiles that are stuck to an <see cref="NPC" />
        ///     instance.
        /// </summary>
        public readonly Point[] Buffer = new Point[MAX_STICK_COUNT];

        /// <summary>
        ///     Gets the position offset of the projectile when it is stuck to an <see cref="NPC" /> instance.
        /// </summary>
        public Vector2 Offset { get; private set; }

        /// <summary>
        ///     Gets or sets the state of the projectile. Shorthand for <c>Projectile.ai[0]</c>.
        /// </summary>
        public ref float State => ref Projectile.ai[0];

        /// <summary>
        ///     Gets or sets the index of the <see cref="NPC" /> instance that this projectile is currently stuck to.
        /// </summary>
        public ref float Index => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            SwingType = ModContent.ProjectileType<GammaKnifeProjectile>();

            if (Main.dedServ)
            {
                return;
            }

            Glowmask = Mod.Assets.Request<Texture2D>("Content/Projectiles/HealerPro/Scythes/GammaKnife/GammaKnife_Glow");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.friendly = true;
            Projectile.tileCollide = true;

            Projectile.width = 32;
            Projectile.height = 32;

            Projectile.penetrate = -1;

            Projectile.timeLeft = LIFESPAN;

            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Projectile.ArmorPenetration = 15;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            SoundEngine.PlaySound(in Sound, Projectile.Center);

            for (var i = 0; i < 5; i++)
            {
                var offset = Main.rand.NextVector2Circular(4f, 4f) * 8f;
                var position = Projectile.Center + offset;

                Projectile.NewProjectile
                (
                    Projectile.GetSource_Death(),
                    position,
                    Vector2.Zero,
                    ModContent.ProjectileType<GammaExplosionProjectile>(),
                    (Projectile.damage / 4),
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);

            // Trigger return-to-player
            State = STATE_RETURN;
            Projectile.netUpdate = true;

            if (!WeaponConfig.Instance.EnableScreenEffects)
                return;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            SoundEngine.PlaySound(in Sound, Projectile.Center);

            var position = Projectile.Center;
            var velocity = Main.rand.NextVector2Circular(2f, 2f);

            for (var i = 0; i < 5; i++)
            {
                var offset = Main.rand.NextVector2Circular(4f, 4f) * 8f;
                position = Projectile.Center + offset;

                Projectile.NewProjectile
                (
                    Projectile.GetSource_Death(),
                    position,
                    Vector2.Zero,
                    ModContent.ProjectileType<GammaExplosionProjectile>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            //if (!WeaponConfig.Instance.EnableScreenEffects)
            //{
            //    
            //}

            // Trigger return like boomerang
            State = STATE_RETURN;
            Projectile.netUpdate = true;

            return false; // do not kill on tile collision
        }


        private void SpawnDustEffects(in Vector2 position, in Vector2 velocity)
        {
            for (var i = 0; i < 10; i++)
            {
                var offset = Main.rand.NextVector2Circular(8f, 8f) * 2f;
                var dust = Dust.NewDustDirect(position + offset, 0, 0, ModContent.DustType<GammaDust>(), velocity.X, velocity.Y);

                dust.noGravity = true;

                dust.scale = Main.rand.NextFloat(1f, 3f);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);

            writer.WriteVector2(Offset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);

            Offset = reader.ReadVector2();
        }

        public override void AI()
        {
            base.AI();

            switch (State)
            {
                case STATE_IDLE:
                    UpdateEffects();
                    UpdateIdle();

                    airTime++;
                    if (airTime >= RETURN_AFTER_TICKS)
                    {
                        State = STATE_RETURN;
                        Projectile.netUpdate = true;
                    }
                    break;

                case STATE_STICK:
                    UpdateStick();
                    UpdateImpact();
                    break;

                case STATE_RETURN:
                    UpdateReturn();
                    break;

                default:
                    State = STATE_IDLE;
                    Projectile.netUpdate = true;
                    break;
            }

            UpdateOpacity();
        }

        private void UpdateEffects()
        {
            var rotation = Projectile.rotation;

            if (Projectile.direction == -1)
                rotation += MathHelper.Pi;

            var position = Projectile.Center - new Vector2(0f, 16f).RotatedBy(Projectile.rotation);
            var velocity = rotation.ToRotationVector2() * 16f;

            if (airTime < 1)
                return;
            SpawnDustEffects(in position, in velocity);
        }

        private void UpdateIdle()
        {
            var direction = Math.Sign(Projectile.velocity.X);

            Projectile.direction = direction;
            Projectile.spriteDirection = direction;

            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.99f;

            Projectile.rotation += Projectile.velocity.X * 0.025f;
        }

        private void UpdateStick()
        {
            var npc = Main.npc[(int)Index];

            if (!npc.active || npc.dontTakeDamage)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.Center = npc.Center - Offset * 2f;
                Projectile.velocity = Vector2.Zero;

                Projectile.gfxOffY = npc.gfxOffY;
            }
        }

        private void UpdateReturn()
        {
            Player player = Main.player[Projectile.owner];

            // Disable collisions while returning
            Projectile.tileCollide = false;

            // How strongly the knife homes back
            const float returnAcceleration = 0.8f;
            const float maxSpeed = 22f;

            Vector2 toPlayer = player.Center - Projectile.Center;
            float distance = toPlayer.Length();

            // Normalize direction
            if (distance > 6f)
                toPlayer.Normalize();

            // Accelerate toward the player
            Projectile.velocity = Vector2.Lerp(
                Projectile.velocity,
                toPlayer * maxSpeed,
                returnAcceleration
            );

            Projectile.rotation += 0.4f * Projectile.direction;

            // If close to the player, kill the projectile
            if (distance < 30f)
            {
                Projectile.Kill();
            }
        }


        private void UpdateOpacity()
        {
            const int max = 255;
            const int step = 5;

            Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha, 0, max);

            if (Projectile.timeLeft > max / step)
            {
                return;
            }

            Projectile.alpha += step;
        }

        private void UpdateImpact()
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.type != SwingType)
                {
                    continue;
                }

                var hitbox = Projectile.Hitbox;

                hitbox.Inflate(32, 32);

                var intersects = projectile.Hitbox.Intersects(hitbox);

                if (!intersects)
                {
                    continue;
                }

                for (var i = 0; i < 20; i++)
                {
                    var position = Projectile.Center;
                    var velocity = Main.rand.NextVector2Circular(4f, 4f) * i / 2f;

                    SpawnDustEffects(in position, in velocity);
                }

                Projectile.Kill();

                SoundEngine.PlaySound(in Sound, Projectile.Center);

                for (var i = 0; i < 5; i++)
                {
                    var offset = Main.rand.NextVector2Circular(4f, 4f) * 8f;
                    var position = Projectile.Center + offset;

                    Projectile.NewProjectile
                    (
                        Projectile.GetSource_Death(),
                        position,
                        Vector2.Zero,
                        ModContent.ProjectileType<GammaExplosionProjectile>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }

                if (!WeaponConfig.Instance.EnableScreenEffects)
                {
                    break;
                }

                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 10f, 60f, 10));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawProjectile(in lightColor);

            return false;
        }

        private void DrawProjectile(in Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;

            var color = Projectile.GetAlpha(lightColor);
            var origin = texture.Size() / 2f;

            var position = Projectile.Center - Main.screenPosition + new Vector2(DrawOffsetX, Projectile.gfxOffY);

            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, position, null, color, Projectile.rotation, origin, Projectile.scale, effects);

            var glowmask = Glowmask.Value;

            color = Projectile.GetAlpha(Color.White);

            Main.EntitySpriteDraw(glowmask, position, null, color, Projectile.rotation, origin, Projectile.scale, effects);
        }
    }
}