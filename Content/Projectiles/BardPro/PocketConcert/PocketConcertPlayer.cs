using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria.Audio;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.PocketConcert
{
    public class PocketConcertPlayer : ModPlayer
    {
        public static int Type { get; private set; }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Type = ModContent.ProjectileType<MusicalNoteProjectile>();
        }

        public override void PostUpdate()
        {
            base.PostUpdate();

            UpdateAudio();
        }

        private void UpdateAudio()
        {
            if (Player.ownedProjectileCounts[Type] == 0)
            {
                PocketConcertAudioSystem.Stop();
            }
            else
            {
                PocketConcertAudioSystem.Play(Player.Center);
            }
        }
    }
}