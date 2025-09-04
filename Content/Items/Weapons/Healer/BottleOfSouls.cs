using CalamityMod.Items;
using InfernalEclipseWeaponsDLC.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Projectiles.Healer;
using System;
using ThoriumMod.Buffs.Healer;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    [ExtendsFromMod("ThoriumMod")]
    public class BottleOfSouls : ThoriumItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(18, 36);
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.damage = 40;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Item.noMelee = true;
            Item.mana = 5;
            radiantLifeCost = 2;

            Item.useTime = 8;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.UseSound = SoundID.NPCDeath39;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0f;

            isHealer = true;
            healType = HealType.Ally;
            healAmount = 0;
            healDisplay = true;
            isHealer = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Main.rand.NextBool() ? ModContent.ProjectileType<DamagingSoul>() : ModContent.ProjectileType<HealingSoul>();
            velocity = new Vector2(0, -10).RotatedBy(Main.rand.NextFloat(-.3f, .3f));
            damage = Item.damage;
        }
    }

    [ExtendsFromMod("ThoriumMod")]
    class DamagingSoul : HomingPro
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/BottleOfSouls/DamagingSoul";

        /// <summary>
        /// The debuff duration in ticks
        /// </summary>
        public int ShadowflameDuration = 60;

        public override bool HomingOnPlayer { get; } = false;

        public override float Speed => 7.5f;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24, 30);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 150;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
        }

        public override void SafeAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - rotationOffset;

            // Fade out when lifetime is about to end
            if (Projectile.timeLeft < 60) // last 1 second (60 ticks)
            {
                Projectile.alpha += 5; // Increase transparency
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            // Dust effect (purple)
            if (Main.rand.NextBool(3))
            { // 1 in 3 chance each tick
                Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.PurpleTorch,   // Nice glowing purple dust
                    Projectile.velocity * -0.2f, // Little trailing motion
                    150,
                    Color.Purple,
                    1.2f
                ).noGravity = true;
            }

            // Light emission (purple-ish)
            Lighting.AddLight(Projectile.Center, 1.2f, 0.0f, 1.8f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, ShadowflameDuration);
        }
    }

    [ExtendsFromMod("ThoriumMod")]
    class HealingSoul : ModProjectile // inherit ModProjectile directly, not HomingPro
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/BottleOfSouls/HealingSoul";

        public float Speed => 7.5f;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24, 30);
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 150;

            Projectile.friendly = false; // won't damage NPCs
            Projectile.hostile = false;  // won't damage players
            Projectile.penetrate = -1;   // infinite until it heals
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            // Rotate to match movement
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Fade out
            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            // Dust + Light
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.PinkTorch,
                    Projectile.velocity * -0.2f,
                    150,
                    Color.Pink,
                    1.2f
                ).noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 1.2f, 0.4f, 0.8f);

            Player owner = Main.player[Projectile.owner];

            Player target = null;
            float closestDist = 600f; // heal range

            // Find closest teammate (excluding self)
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team) // only teammates
                    {
                        float dist = Vector2.Distance(Projectile.Center, p.Center);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            target = p;
                        }
                    }
                }
            }

            // Homing onto player
            if (target != null)
            {
                Vector2 direction = target.Center - Projectile.Center;
                direction.Normalize();
                float speed = 10f;
                Projectile.velocity = (Projectile.velocity * 20f + direction * speed) / 21f;
            }
            else
            {
                Projectile.velocity *= 0.96f; // slow down if no target
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team) // teammates only
                    {
                        if (Projectile.Hitbox.Intersects(p.Hitbox))
                        {
                            HealTeammateThorium(owner, p, baseHeal: 0); // give some healing
                            Projectile.Kill(); // consume projectile after heal
                            break;
                        }
                    }
                }
            }
        }

        private void HealTeammateThorium(Player healer, Player target, int baseHeal)
        {
            if (healer.whoAmI != Main.myPlayer) return;
            if (healer == target) return;
            if (healer.team == 0 || healer.team != target.team) return;

            if (baseHeal <= 0 && healer.GetModPlayer<ThoriumPlayer>().healBonus <= 0)
                return; // Nothing to heal

            HealerHelper.HealPlayer(
                healer,
                target,
                healAmount: baseHeal, // <-- don't add healBonus here
                recoveryTime: 60,
                healEffects: true,
                extraEffects: p => p.AddBuff(ModContent.BuffType<Cured>(), 30, true, false)
            );
        }

        public override bool? CanHitNPC(NPC target) => false;
    }
}



