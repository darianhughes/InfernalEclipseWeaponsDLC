using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;

public class MiniaturizedRequiemEngineTheBigOnePro2 : ModProjectile
{
    public override string Texture => "CalamityMod/Projectiles/Summon/RustyBeaconPulse";

    public float LifetimeCompletion => 1f - Projectile.timeLeft / 30f;

    private const int SelfSpawnDelay = 10;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.DamageType = DamageClass.Magic;
        Projectile.width = 96;
        Projectile.height = 96;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.timeLeft = 30;
        Projectile.scale = 0.001f;
    }

    public override void AI()
{
    Projectile.ai[0]++;

    // Initial random rotation setup
    if (Projectile.localAI[0] == 0f)
    {
        Projectile.rotation = Utils.NextFloat(Main.rand, MathHelper.TwoPi);
        Projectile.localAI[0] = Utils.ToDirectionInt(Utils.NextBool(Main.rand));
        Projectile.netUpdate = true;
    }

    // Spawn another Pro2 after 20 ticks (once, owner-only)
    if (Projectile.ai[0] == SelfSpawnDelay && Projectile.owner == Main.myPlayer)
    {
            Projectile.NewProjectile(
                    Projectile.GetSource_Death(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro3>(),
                    (Projectile.damage * 1),
                    0f,
                    Projectile.owner
                );
        }

    Projectile.Opacity = (1f - (float)Math.Pow(LifetimeCompletion, 1.56)) * 0.4f;
    Projectile.scale = MathHelper.Lerp(0.3f, 25f, LifetimeCompletion);
    Projectile.rotation += Projectile.localAI[0] * 0.012f;
}


    public override Color? GetAlpha(Color lightColor)
    {
        Color start = new Color(255, 160, 60, 0);   // orange
        Color end = new Color(255, 255, 255, 32);   // white glow
        Color blended = Color.Lerp(start, end, LifetimeCompletion);

        return blended * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Color drawColor = Projectile.GetAlpha(lightColor) * 0.33f;

        for (int i = 0; i < 8; i++)
        {
            float rotation = Projectile.rotation;
            Vector2 drawOffset = Utils.ToRotationVector2((float)Math.PI * 2f * i / 8f) * Projectile.scale;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + drawOffset;

            if (i % 2 == 1) rotation *= -1f;

            Main.EntitySpriteDraw(
                texture, drawPosition, null, drawColor,
                rotation, Utils.Size(texture) * 0.5f,
                Projectile.scale, SpriteEffects.None, 0f
            );
        }
        return false;
    }

    public override bool? CanHitNPC(NPC target) => !target.CountsAsACritter && !target.friendly && target.chaseable;

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        // Scale damage based on remaining lifetime
        // LifetimeCompletion goes 0 - 1, we want damage high - low, so invert it
        float damageScale = 1f - LifetimeCompletion; // linear fade
        modifiers.SourceDamage *= damageScale;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.scale * 48f, targetHitbox);
    }

    public override bool OnTileCollide(Vector2 oldVelocity) => false;
}
