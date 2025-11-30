using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro
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
            const float range = 800f;

            // Optional: Dust ring for visuals
            int points = 80;
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

                if (!npc.HasBuff(BuffID.Electrified))
                    continue; // skip NPCs without Electrified

                // Spawn Electrosphere projectile
                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    npc.Center,
                    Vector2.Zero,
                    ProjectileID.Electrosphere,
                    player.HeldItem.damage,
                    0f,
                    player.whoAmI
                );

                if (proj >= 0 && proj < Main.maxProjectiles)
                {
                    Projectile p = Main.projectile[proj];
                    p.timeLeft = 30;
                    p.DamageType = DamageClass.Magic;
                    spawnedAny = true;
                }

                npc.DelBuff(BuffID.Electrified); // remove Electrified
            }

            if (spawnedAny)
            {
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
            }

            Projectile.Kill(); // remove immediately after execution
        }
    }
}
