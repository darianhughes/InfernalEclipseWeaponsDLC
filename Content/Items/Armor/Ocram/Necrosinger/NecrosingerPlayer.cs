using InfernalEclipseWeaponsDLC.Content.Projectiles.ArmorPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Armor.Ocram.Necrosinger
{
    public class NecrosingerPlayer : ModPlayer
    {
        public bool NecrosingerSet;

        private int rechargeTimer;

        public const int RechargeDelay = 300;

        public override void ResetEffects()
        {
            NecrosingerSet = false;
        }

        public override void PostUpdate()
        {
            if (rechargeTimer > 0)
                rechargeTimer--;

            if (!NecrosingerSet)
                return;

            // Only the owning client should spawn the orbitals.
            if (Player.whoAmI != Main.myPlayer)
                return;

            // Count current orbitals.
            int noteType = ModContent.ProjectileType<NecrosingerNote>();
            int ownedNotes = Player.ownedProjectileCounts[noteType];

            // If we’re missing any, only refill when recharge allows.
            if (ownedNotes < 3 && rechargeTimer <= 0)
            {
                // Spawn enough to reach 3.
                for (int i = ownedNotes; i < 3; i++)
                {
                    int p = Projectile.NewProjectile(
                        Player.GetSource_FromThis(),
                        Player.Center,
                        Vector2.Zero,
                        noteType,
                        0,
                        0f,
                        Player.whoAmI,
                        ai0: i,   // slot/index (0..2)
                        ai1: 0f
                    );

                    if (p >= 0 && p < Main.maxProjectiles)
                        Main.projectile[p].netUpdate = true;
                }
            }
        }

        /// <summary>
        /// Called by a note projectile when it successfully blocks a hostile projectile.
        /// Starts the recharge delay for missing notes.
        /// </summary>
        public void StartRecharge()
        {
            if (rechargeTimer < RechargeDelay)
                rechargeTimer = RechargeDelay;
        }
    }
}
