using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.GrandAmplifier;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.GrandAmplifier
{
    public class GrandAmplifierPro2 : ModProjectile
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Assets/Textures/Empty";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 2; // lives only 2 ticks
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            const float range = 400f;

            // Optional: Dust ring for visuals
            int points = 40;
            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.ToRadians(360f * i / points);
                Vector2 offset = angle.ToRotationVector2() * range;
                Dust d = Dust.NewDustPerfect(player.Center + offset, DustID.Electric);
                d.noGravity = true;
                d.velocity = Vector2.Zero;
                d.scale = 1.2f;
                d.alpha = 150;
            }

            bool spawnedAny = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || !npc.CanBeChasedBy())
                    continue;

                Vector2 closestPoint = new Vector2(
                    MathHelper.Clamp(player.Center.X, npc.Hitbox.Left, npc.Hitbox.Right),
                    MathHelper.Clamp(player.Center.Y, npc.Hitbox.Top, npc.Hitbox.Bottom)
                );
                float distance = Vector2.Distance(player.Center, closestPoint);

                if (distance > range)
                    continue;

                var calNPC = npc.Calamity();

                if (calNPC.electrified <= 0)
                    continue; // skip NPCs without Electrified

                Vector2 spawnPos = npc.Center + new Vector2(0f, -150f);

                int proj2 = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    Vector2.Zero,
                    ModContent.ProjectileType<GrandAmplifierLightning>(),
                    player.HeldItem.damage,
                    0f,
                    player.whoAmI,
                    npc.whoAmI
                );

                // === REMOVE ELECTRIFIED ===
                if (npc.HasBuff(BuffID.Electrified))
                    npc.DelBuff(BuffID.Electrified);

                if (calNPC.electrified > 0)
                    calNPC.electrified = 0;
            }

            if (spawnedAny)
            {
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
            }

            Projectile.Kill(); // remove immediately after execution
        }
    }
}
