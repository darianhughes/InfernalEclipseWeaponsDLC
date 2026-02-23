using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine
{
    public class RequiemEnginePlayer : ModPlayer
    {
        public int GatlingCharge;

        public const int MaxCharge = 30;

        public float GatlingMultiplier =>
            MathHelper.Lerp(1f, 0.35f, GatlingCharge / (float)MaxCharge);

        public override void ResetEffects()
        {
            // If player isn't actively firing, decay the charge
            if (!Player.ItemAnimationActive)
                GatlingCharge = Math.Max(0, GatlingCharge - 2);
        }
    }
}
