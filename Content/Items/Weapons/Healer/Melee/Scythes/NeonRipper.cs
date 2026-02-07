using CalamityMod.CustomRecipes;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Projectiles.Scythe;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer.Melee.Scythes
{
    [ExtendsFromMod("ThoriumMod")]
    public class NeonRipper : ScytheItem
    {
        public static readonly float ThrowDistance = 240f; // customise here

        public override void SetDefaults()
        {
            SetDefaultsToScythe();
            Item.shoot = ModContent.ProjectileType<NeonRipperPro>();
            scytheSoulCharge = 3;
            Item.damage = 35;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source,
    Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int projIndex = -1;

                if (player.altFunctionUse != 2) // Normal left click swing
                {
                    projIndex = Projectile.NewProjectile(
                        source,
                        position,
                        velocity,
                        type,
                        damage,
                        knockback,
                        player.whoAmI
                    );
                }
                else // Right click throw
                {
                    Vector2 throwVel = Vector2.Normalize(Main.MouseWorld - player.MountedCenter) * -ThrowDistance;

                    projIndex = Projectile.NewProjectile(
                        source,
                        position,
                        throwVel,
                        type,
                        (int)MathF.Round(damage * 1.3f),
                        knockback,
                        player.whoAmI,
                        (Main.rand.Next(2, 5) + 1) * 0.1f, // ai[0]
                        player.itemTime // ai[1]
                    );
                }

                if (projIndex >= 0)
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, projIndex);
            }
            return false;
        }

        public override float UseTimeMultiplier(Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            return player.altFunctionUse == 2 ? 2f : 1f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Lore lines
            tooltips.Add(new TooltipLine(Mod, "NeonRipLore1", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.NeonRipperLore1")) { OverrideColor = Color.MediumPurple });
            tooltips.Add(new TooltipLine(Mod, "NeonRipLore2", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.NeonRipperLore2")) { OverrideColor = Color.MediumPurple });
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MysteriousCircuitry>(15)
                .AddIngredient<DubiousPlating>(15)
                .AddIngredient<InfectedArmorPlating>(10)
                .AddIngredient<LifeAlloy>(5)
                .AddTile(TileID.MythrilAnvil)
                .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(3, out Func<bool> condition), condition)
                .Register();
        }
    }

    //WHY IS THIS ALL IN ONE FILE!?!??!?!?!
    [ExtendsFromMod("ThoriumMod")]
    public class NeonRipperPro : ScythePro
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/NeonRipperPro";
        public override void SafeSetDefaults()
        {
            //dustType = DustID.PurpleTorch;
            //dustCount = 4;
            scytheCount = 2;
            Projectile.width = 182;
            Projectile.height = 182;
            //dustOffset = new Vector2(-50, 11f);
            fadeOutSpeed = 30;
            rotationSpeed = 0.25f;
            Projectile.scale = 1.3f;
        }

        public override void SafeOnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 8, ModContent.ProjectileType<Nanodroid>(), Projectile.damage, 0);
            target.AddBuff(BuffID.Electrified, 60);
            base.SafeOnHitNPC(target, hit, damageDone);
        }
        public override bool PreAI()
        {
            if (Projectile.ai[0] != 0f) // Thrown mode
            {
                Player player = Main.player[Projectile.owner];
                player.heldProj = Projectile.whoAmI;

                // Face the correct way based on aim
                player.ChangeDir(Projectile.velocity.X < 0f ? 1 : -1);
                Projectile.spriteDirection = player.direction;

                // Spin faster while thrown
                Projectile.rotation += rotationSpeed * Projectile.spriteDirection * 2.5f;

                // Keep projectile alive for full ai[1] duration
                if (Projectile.ai[1] - Projectile.ai[2] > Projectile.timeLeft)
                    Projectile.timeLeft++;

                float attackTime = ++Projectile.ai[2] / Projectile.ai[1];

                // Create circular orbit (unit circle)
                Vector2 orbit = Vector2.UnitX.RotatedBy(attackTime * MathHelper.TwoPi, default);

                // Stretch orbit based on velocity magnitude and ai[0] factor
                orbit *= new Vector2(Projectile.velocity.Length(), Projectile.velocity.Length() * Projectile.ai[0] * Projectile.spriteDirection);

                // Rotate orbit to match aim
                orbit = orbit.RotatedBy(Projectile.velocity.ToRotation(), default);

                // Offset from player and subtract initial throw velocity
                Projectile.Center = player.MountedCenter + orbit - Projectile.velocity;

                // Play sound when rotation loops
                if (Projectile.rotation > Math.PI)
                {
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    Projectile.rotation -= MathHelper.TwoPi;
                }
                else if (Projectile.rotation < -Math.PI)
                {
                    SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                    Projectile.rotation += MathHelper.TwoPi;
                }

                return false; // Skip normal scythe AI
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Fade with projectile alpha
            lightColor *= MathHelper.Lerp(1f, 0f, Projectile.alpha / 255f);

            // Main scythe texture
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            Main.EntitySpriteDraw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection,
                texture.Size() / 2f,
                Projectile.scale,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );

            // Cursed flame green afterimage
            Color cursedGreen = new Color(255, 0, 255) *
                                MathHelper.Lerp(0.15f, 0f, Projectile.alpha / 255f);
            cursedGreen.A = 0;

            Texture2D slashTexture = (Texture2D)ModContent.Request<Texture2D>("InfernalEclipseWeaponsDLC/Assets/Textures/Slash_3");

            // Two forward-facing afterimages
            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation, slashTexture.Size() / 2f, Projectile.scale * 2.35f,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation, slashTexture.Size() / 2f, Projectile.scale * 2.35f,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            // Two mirrored afterimages (rotated by 180°)
            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation + MathHelper.Pi, slashTexture.Size() / 2f, Projectile.scale * 2.35f,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(slashTexture, Projectile.Center - Main.screenPosition, null, cursedGreen,
                Projectile.rotation + MathHelper.Pi, slashTexture.Size() / 2f, Projectile.scale * 2.35f,
                Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);

            return false;
        }
    }

    [ExtendsFromMod("ThoriumMod")]
    public class Nanodroid : HomingPro
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Scythes/Nanodroid";

        public override bool HomingOnPlayer { get; } = false;

        public override bool Slowdown => false;

        public override float Speed => 8f;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16, 10);
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30; // Can only attack 60 / value times a second
            Projectile.ignoreWater = true;
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
        }

        public override void SafeAI()
        {
            if (Projectile.ai[0] != -1) Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 60);
            Projectile.ai[0] = target.whoAmI;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                //Projectile.velocity.X = -oldVelocity.X;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                //Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
    }
}