using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Core.Graphics
{
    public sealed class GammaRenderingSystem : ModSystem
    {
        // Scale for buffer drawing
        public static readonly Matrix Scale = Matrix.CreateScale(0.5f, 0.5f, 1f);

        // Outline color
        public static readonly Color Color = new(42, 208, 49);

        // Render target for gamma effects
        public static RenderTarget2D Target { get; private set; }

        // Actions queued for rendering
        private static List<Action> _actions;

        private static readonly object _actionLock = new();

        // Shader asset
        public static Asset<Effect> Effect { get; private set; }

        public override void PostSetupContent()
        {
            base.PostSetupContent();

            if (Main.dedServ) return;

            // Load shader immediately
            Effect = ModContent.Request<Effect>("InfernalEclipseWeaponsDLC/Assets/Effects/Outline", AssetRequestMode.ImmediateLoad);
        }

        public override void Load()
        {
            base.Load();

            if (Main.dedServ) return;

            _actions = new List<Action>();

            // Create render target on main thread
            Main.RunOnMainThread(() =>
            {
                Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2);
            });

            // Hook Terraria drawing
            On_Main.CheckMonoliths += Main_CheckMonoliths_FillBuffer;
            On_Main.DrawProjectiles += Main_DrawProjectiles_DrawBuffer;

            Main.OnResolutionChanged += Main_OnResolutionChanged_ResizeBuffer;
        }

        public override void Unload()
        {
            base.Unload();

            if (!Main.dedServ)
            {
                Main.OnResolutionChanged -= Main_OnResolutionChanged_ResizeBuffer;
                On_Main.CheckMonoliths -= Main_CheckMonoliths_FillBuffer;
                On_Main.DrawProjectiles -= Main_DrawProjectiles_DrawBuffer;

                Main.RunOnMainThread(() =>
                {
                    Target?.Dispose();
                    Target = null;
                });

                lock (_actionLock)
                {
                    _actions?.Clear();
                    _actions = null;
                }
            }

            Effect = null;
        }

        /// <summary>
        /// Queue an action to draw into the gamma render buffer.
        /// </summary>
        public static void Queue(Action action)
        {
            if (Main.dedServ || action == null) return;

            lock (_actionLock)
                _actions.Add(action);
        }

        private static void Main_CheckMonoliths_FillBuffer(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            FillBuffer();
        }

        private static void FillBuffer()
        {
            if (Main.dedServ || Target == null || Target.IsDisposed) return;

            var device = Main.graphics.GraphicsDevice;
            var bindings = device.GetRenderTargets();

            device.SetRenderTarget(Target);
            device.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate,
                                   BlendState.AlphaBlend,
                                   SamplerState.PointClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullNone,
                                   null,
                                   Scale);

            lock (_actionLock)
            {
                foreach (var action in _actions)
                    action?.Invoke();

                _actions.Clear();
            }

            Main.spriteBatch.End();
            device.SetRenderTargets(bindings);
        }

        private static void Main_DrawProjectiles_DrawBuffer(On_Main.orig_DrawProjectiles orig, Main self)
        {
            DrawBuffer();
            orig(self);
        }

        private static void DrawBuffer()
        {
            if (Main.dedServ || Target == null || Target.IsDisposed) return;

            var shader = Effect?.Value;
            if (shader == null) return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.PointClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullNone,
                                   shader,
                                   Main.GameViewMatrix.TransformationMatrix);

            shader.Parameters["uImageSize0"]?.SetValue(Target.Size());
            shader.Parameters["uColor"]?.SetValue(Color.ToVector3());

            Main.spriteBatch.Draw(Target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

            Main.spriteBatch.End();
        }

        private static void Main_OnResolutionChanged_ResizeBuffer(Vector2 newSize)
        {
            ResizeBuffer(newSize);
        }

        private static void ResizeBuffer(Vector2 newSize)
        {
            if (Main.dedServ) return;

            Main.RunOnMainThread(() =>
            {
                Target?.Dispose();
                Target = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)(newSize.X / 2f), (int)(newSize.Y / 2f));
            });
        }
    }
}
