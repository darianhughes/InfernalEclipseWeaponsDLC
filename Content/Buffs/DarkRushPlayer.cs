using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Buffs
{
    public class DarkRushPlayer : ModPlayer
    {
        public bool OverclockActive;
        private int overclockTimer;

        public override void ResetEffects()
        {
            OverclockActive = false; // reset each tick

            if (overclockTimer > 0)
            {
                overclockTimer--;

                // Only active if GloomSwitch is being held
                if (Player.HeldItem.type == ModContent.ItemType<Items.Weapons.Ranged.GloomSwitch>())
                {
                    OverclockActive = true;
                    Player.moveSpeed += 0.25f; // +25% movement speed
                }
            }
        }

        public void ActivateRush(int duration)
        {
            overclockTimer = duration;
        }

        public override void PostUpdate()
        {
            if (!OverclockActive) return;

            // Particle effect while buff is active & weapon held
            if (Main.rand.NextBool(4))
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-15f, 15f), Main.rand.NextFloat(-20f, 20f));
                Vector2 pos = Player.Center + offset;

                int dustIndex = Dust.NewDust(pos, 0, 0, DustID.CorruptTorch, 0f, 0f, 0, default, Main.rand.NextFloat(1.5f, 2f));
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity = offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.2f, 0.5f);
                Main.dust[dustIndex].scale *= Main.rand.NextFloat(1.1f, 1.5f);
                Main.dust[dustIndex].alpha = 0;
            }
        }
    }
}
