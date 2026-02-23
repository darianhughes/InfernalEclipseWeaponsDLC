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
        private bool pendingRechargeReset;
        private bool notesInitialized;

        int RechargeDelay = 300;

        public override void ResetEffects()
        {
            if (!NecrosingerSet && notesInitialized)
            {
                KillAllNotes();
                notesInitialized = false;
            }

            NecrosingerSet = false;
        }

        public override void PostUpdate()
        {
            // Tick cooldown regardless of set
            if (rechargeTimer > 0)
            {
                rechargeTimer--;

                // Cooldown JUST finished this tick
                if (rechargeTimer == 0 && pendingRechargeReset)
                {
                    if (NecrosingerSet && Player.whoAmI == Main.myPlayer)
                    {
                        ResetNotes();
                        notesInitialized = true;
                    }

                    pendingRechargeReset = false;
                }
            }

            if (!NecrosingerSet)
                return;

            // Initial spawn (only once)
            if (!notesInitialized &&
                rechargeTimer == 0 &&
                Player.whoAmI == Main.myPlayer)
            {
                ResetNotes();
                notesInitialized = true;
            }
        }

        private void ResetNotes()
        {
            int noteType = ModContent.ProjectileType<NecrosingerNote>();

            int ownedNotes = Player.ownedProjectileCounts[noteType];

            // Kill all existing notes
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active &&
                    proj.owner == Player.whoAmI &&
                    proj.type == noteType)
                {
                    proj.Kill();
                }
            }

            // Spawn all 3 fresh notes
            for (int i = 0; i < 3; i++)
            {
                int p = Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    Player.Center,
                    Vector2.Zero,
                    noteType,
                    0,
                    0f,
                    Player.whoAmI,
                    ai0: i,
                    ai1: 0f
                );

                if (p >= 0 && p < Main.maxProjectiles)
                    Main.projectile[p].netUpdate = true;
            }
        }

        private void KillAllNotes()
        {
            int noteType = ModContent.ProjectileType<NecrosingerNote>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];

                if (proj.active &&
                    proj.owner == Player.whoAmI &&
                    proj.type == noteType)
                {
                    proj.Kill();
                }
            }
        }

        /// <summary>
        /// Called by a note projectile when it successfully blocks a hostile projectile.
        /// Starts the recharge delay for missing notes.
        /// </summary>
        public void StartRecharge()
        {
            if (rechargeTimer <= 0)
            {
                rechargeTimer = RechargeDelay;
                pendingRechargeReset = true;
            }
        }
    }
}
