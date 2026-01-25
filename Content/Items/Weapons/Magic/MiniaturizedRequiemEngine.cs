using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro.MiniaturizedRequiemEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod.PlayerLayers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class MiniaturizedRequiemEngine : ModItem
    {
        public enum RequiemFireMode
        {
            Gatling = 0,
            Laser = 1,
            TheBigOne = 2
        }

        public RequiemFireMode fireMode;

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 6;
            Item.width = 76;
            Item.height = 32;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item33;
            Item.shoot = ModContent.ProjectileType<MiniaturizedRequiemEngineGatlingPro>();
            Item.shootSpeed = 12f;

            Item.noUseGraphic = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override Vector2? HoldoutOffset() => new Vector2(-14f, -2f);

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // Right-click switches modes
            {
                fireMode = fireMode switch
                {
                    RequiemFireMode.Gatling => RequiemFireMode.Laser,
                    RequiemFireMode.Laser => RequiemFireMode.TheBigOne,
                    _ => RequiemFireMode.Gatling
                };
                SoundEngine.PlaySound(SoundID.MenuTick, player.Center);
                ShowModeChangeText(player);

                return false; // Don't use while switching
            }

            // If in "The Big One" mode, check if projectile already exists
            if (fireMode == RequiemFireMode.TheBigOne)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.owner == player.whoAmI &&
                        (p.type == ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro>() ||
                         p.type == ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro2>()))
                    {
                        // Already exists, block using the item entirely
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage *= fireMode switch
            {
                RequiemFireMode.Gatling => 1f,
                RequiemFireMode.Laser => 0.8f,
                RequiemFireMode.TheBigOne => 3f,
                _ => 1f
            };
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (fireMode == RequiemFireMode.Gatling)
                return player.GetModPlayer<RequiemEnginePlayer>().GatlingMultiplier;

            return fireMode switch
            {
                RequiemFireMode.Laser => 3f,
                RequiemFireMode.TheBigOne => 1f,
                _ => 1f
            };
        }

        public override float UseAnimationMultiplier(Player player) => UseTimeMultiplier(player);

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            mult *= fireMode switch
            {
                RequiemFireMode.Gatling => 1f,
                RequiemFireMode.Laser => 4f,
                RequiemFireMode.TheBigOne => 8f,
                _ => 1f
            };
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float shootSpeedMultiplier = fireMode switch
            {
                RequiemFireMode.Gatling => 0.2f,
                RequiemFireMode.Laser => 1f,
                RequiemFireMode.TheBigOne => 0.1f,
                _ => 1f
            };

            velocity *= shootSpeedMultiplier;

            switch (fireMode)
            {
                case RequiemFireMode.Gatling:
                    type = ModContent.ProjectileType<MiniaturizedRequiemEngineGatlingPro>();
                    var mp = player.GetModPlayer<RequiemEnginePlayer>();
                    mp.GatlingCharge = Math.Min(mp.GatlingCharge + 1, RequiemEnginePlayer.MaxCharge);
                    float ramp = mp.GatlingCharge / (float)RequiemEnginePlayer.MaxCharge;
                    float spreadDegrees = MathHelper.Lerp(25f, 5f, ramp);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(spreadDegrees));
                    break;

                case RequiemFireMode.Laser:
                    type = ModContent.ProjectileType<MiniaturizedRequiemEngineLaserPro>();
                    break;

                case RequiemFireMode.TheBigOne:
                    type = ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro>();

                    // Prevent firing if a Big One already exists
                    bool alreadyExists = false;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile p = Main.projectile[i];
                        if (p.active && p.owner == player.whoAmI &&
                            (p.type == ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro>() ||
                             p.type == ModContent.ProjectileType<MiniaturizedRequiemEngineTheBigOnePro2>()))
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (alreadyExists)
                        return false;

                    break;
            }

            float muzzleOffset = 36f;
            Vector2 muzzlePosition = position + Vector2.Normalize(velocity) * muzzleOffset;

            if (Collision.CanHit(position, 0, 0, muzzlePosition, 0, 0))
                position = muzzlePosition;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            DrawTexture(spriteBatch, position, drawColor, scale);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawTexture(spriteBatch, Item.position - Main.screenPosition + Item.Size * 0.5f, lightColor, scale);
            return false;
        }

        private void DrawTexture(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
        {
            Texture2D tex = fireMode switch
            {
                RequiemFireMode.Gatling =>
                    ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineGatling").Value,
                RequiemFireMode.Laser =>
                    ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineLaser").Value,
                RequiemFireMode.TheBigOne =>
                    ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineTheBigOne").Value,
                _ =>
                    ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineGatling").Value,
            };
            Vector2 origin = tex.Size() * 0.5f;
            spriteBatch.Draw(tex, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (fireMode == RequiemFireMode.Gatling)
            {
                tooltips.Add(new TooltipLine(Mod, "EngineGatling", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.EngineGatling")) { OverrideColor = Color.DeepPink });
            }
            else if (fireMode == RequiemFireMode.Laser)
            {
                tooltips.Add(new TooltipLine(Mod, "EngineLaser", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.EngineLaser")) { OverrideColor = Color.CornflowerBlue });
            }
            else if (fireMode == RequiemFireMode.TheBigOne)
            {
                tooltips.Add(new TooltipLine(Mod, "EngineTheBigOne", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.EngineTheBigOne")) { OverrideColor = Color.Yellow });
            }

            if (Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", $"{Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.DedTo", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.Dedicated.Goldsock"))}\n{Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.Donor")}");
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
            else
            {
                TooltipLine line5 = new(Mod, "DedicatedItem", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.Donor"));
                line5.OverrideColor = new Color(196, 35, 44);
                tooltips.Add(line5);
            }
        }

        private void ShowModeChangeText(Player player)
        {
            string text;
            Color color;

            switch (fireMode)
            {
                case RequiemFireMode.Gatling:
                    text = "GATLING";
                    color = Color.DeepPink;
                    break;

                case RequiemFireMode.Laser:
                    text = "LASER";
                    color = Color.CornflowerBlue;
                    break;

                case RequiemFireMode.TheBigOne:
                    text = "THE BIG ONE";
                    color = Color.Yellow;
                    break;

                default:
                    return;
            }

            CombatText.NewText(
                player.Hitbox,
                color,
                text,
                dramatic: true
            );
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CosmiliteBar>(10);
            recipe.AddIngredient<AscendantSpiritEssence>(2);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.Register();
        }
    }

    //PLAYER LAYER DRAW
    public class MiniaturizedRequiemEngineDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
            => drawInfo.drawPlayer.itemAnimation > 0 &&
               drawInfo.drawPlayer.HeldItem.ModItem is MiniaturizedRequiemEngine;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            var engine = (MiniaturizedRequiemEngine)player.HeldItem.ModItem;

            Texture2D tex = ModContent.Request<Texture2D>(
                engine.fireMode switch
                {
                    MiniaturizedRequiemEngine.RequiemFireMode.Gatling =>
                        "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineGatling",
                    MiniaturizedRequiemEngine.RequiemFireMode.Laser =>
                        "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineLaser",
                    _ =>
                        "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Magic/MiniaturizedRequiemEngineTheBigOne"
                }).Value;

            Vector2 holdout = engine.HoldoutOffset() ?? Vector2.Zero;

            // Mirror for left
            if (player.direction == -1)
            {
                holdout.X = (-holdout.X * -4.5f);
            }
            holdout = holdout.RotatedBy(player.itemRotation);

            Vector2 position = player.MountedCenter - Main.screenPosition + holdout;

            Vector2 origin = new Vector2(player.direction == 1 ? 0f : tex.Width, tex.Height / 2f);


            float rotation = player.itemRotation;
            if (player.direction == -1)
                rotation += MathHelper.Pi;

            SpriteEffects effects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            DrawData data = new DrawData(
                tex,
                position,
                null,
                Lighting.GetColor((int)player.Center.X / 16, (int)player.Center.Y / 16),
                rotation,
                origin,
                1f,
                effects,
                0f
            );

            drawInfo.DrawDataCache.Add(data);
        }
    }
}