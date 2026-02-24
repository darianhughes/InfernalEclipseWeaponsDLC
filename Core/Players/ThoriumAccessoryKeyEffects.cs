using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Core.Players
{
    public class ThoriumAccessoryKeyEffects : ModPlayer
    {
        public bool canFreezeCamera = false;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            ModKeybind accessoryKey = ThoriumHotkeySystem.AccessoryKey;

            if (accessoryKey.JustPressed)
            {
                if (canFreezeCamera)
                {
                    Main.NewText("Camera Frozen");
                    CameraFreezeSystem.ToggleFreeze();
                }
            }
        }
    }

    public sealed class CameraFreezeSystem : ModSystem
    {
        public static bool Frozen { get; private set; }

        private static Vector2 _frozenScreenPos;

        private static bool _useFocusPoint;
        private static Vector2 _focusPointWorld;

        public static void FreezeToCurrentCamera()
        {
            if (Main.dedServ)
                return;

            Frozen = true;
            _useFocusPoint = false;
            _frozenScreenPos = Main.screenPosition;
        }

        public static void FreezeToWorldPoint(Vector2 worldPoint)
        {
            if (Main.dedServ)
                return;

            Frozen = true;
            _useFocusPoint = true;
            _focusPointWorld = worldPoint;
            RecomputeFrozenPosFromFocus();
        }

        public static void Unfreeze()
        {
            Frozen = false;
            _useFocusPoint = false;
        }

        public static void ToggleFreeze()
        {
            if (Frozen) Unfreeze();
            else FreezeToCurrentCamera();
        }

        private static void RecomputeFrozenPosFromFocus()
        {
            Vector2 halfScreen = new Vector2(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
            _frozenScreenPos = _focusPointWorld - halfScreen;
        }

        public override void PostUpdateEverything()
        {
            if (Main.dedServ || Main.gameMenu)
                return;

            if (!Frozen)
                return;

            if (Main.myPlayer < 0 || Main.myPlayer >= Main.maxPlayers)
                return;

            if (_useFocusPoint)
                RecomputeFrozenPosFromFocus();

            Main.screenPosition = _frozenScreenPos;
        }

        public override void OnWorldUnload()
        {
            Frozen = false;
            _useFocusPoint = false;
            _frozenScreenPos = Vector2.Zero;
            _focusPointWorld = Vector2.Zero;
        }
    }
}
