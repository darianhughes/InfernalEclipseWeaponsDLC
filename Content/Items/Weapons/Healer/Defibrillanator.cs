using CalamityMod.Items;
using CalamityMod;
using CalamityMod.Projectiles.Magic;
using InfernalEclipseWeaponsDLC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Utilities;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.CustomRecipes;
using static System.Net.Mime.MediaTypeNames;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    [ExtendsFromMod("ThoriumMod", "CalamityMod")]
    public class Defibrillanator : ThoriumItem
    {
        static Asset<Texture2D> inventoryTexture;

        public override void SetStaticDefaults()
        {
            // Use the proper texture path
            inventoryTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Healer/Defibrillanator_Inventory", AssetRequestMode.ImmediateLoad);
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(10, 10);
            Item.value = Item.sellPrice(gold: 3);

            Item.damage = 10;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Item.noMelee = true;
            Item.mana = 3;
            radiantLifeCost = 3;
            Item.damage = 33;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.UseSound = SoundID.Item24;

            Item.shoot = ModContent.ProjectileType<LightningArc>();
            Item.shootSpeed = 8f;

            isHealer = true;
            healDisplay = true;
            healAmount = 5;
            healType = HealType.Ally;

            CalamityGlobalItem modItem = Item.Calamity();
            modItem.UsesCharge = true;
            modItem.MaxCharge = 50f;
            modItem.ChargePerUse = 0.04f;
        }

        public override bool CanUseItem(Player player)
        {
            // Handle right-click (alt function)
            if (player.altFunctionUse == 2)
            {
                // Right-click = heal arc no Radiant cost
                radiantLifeCost = 0;
            }
            else
            {
                // Left-click = attack arc normal cost
                radiantLifeCost = 3;
            }

            return base.CanUseItem(player);
        }


        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<HealingLightningArc>();
            position += velocity;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MysteriousCircuitry>(5)
                .AddIngredient<DubiousPlating>(4)
                .AddIngredient<AerialiteBar>(3)
                .AddIngredient<SeaPrism>(6)
                .AddTile(TileID.Anvils)
                .AddCondition(ArsenalTierGatedRecipe.ConstructRecipeCondition(1, out Func<bool> condition), condition)
                .Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float customScale = scale * 0.75f; // shrink to 75% of normal

            Main.EntitySpriteDraw(
                inventoryTexture.Value,
                position,
                inventoryTexture.Value.Bounds,
                drawColor,
                0f,
                inventoryTexture.Value.Bounds.Size() / 2f, // center origin
                customScale,
                SpriteEffects.None
            );

            return false; // prevent default drawing
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.EntitySpriteDraw(inventoryTexture.Value, Item.position, inventoryTexture.Value.Bounds, lightColor, rotation, Vector2.Zero, scale, SpriteEffects.None);
            return false;
        }
    }

    [ExtendsFromMod("CalamityMod")]
    public class HealingLightningArc : LightningArc
    {
        public HashSet<Player> healedAlready = [];

        public int prevX;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // --- Dust + light ---
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Torch,
                    Projectile.velocity * -0.25f,
                    150,
                    Color.Pink,
                    1.2f
                ).noGravity = true;
            }
            Lighting.AddLight(Projectile.Center, 1.0f, 0.85f, 0.4f);

            // --- One-time setup ---
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }

            // --- Find teammate target ---
            Player target = null;
            float closestDist = 200f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team)
                    {
                        float dist = Vector2.Distance(Projectile.Center, p.Center);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            target = p;
                        }
                    }
                }
            }

            // --- Arc path simulation ---
            Vector2 arcVec;
            float jitterScale = 10f;

            if (target != null)
            {
                // Pull the arc direction slightly toward target
                Vector2 toTarget = target.Center - Projectile.Center;
                float dist = toTarget.Length();
                toTarget.Normalize();

                // Add random jitter to keep lightning-like arcs
                arcVec = toTarget * dist * 0.15f + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * jitterScale;
            }
            else
            {
                // No target – purely random arcing forward
                arcVec = Projectile.velocity * 5f + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6)) * 5f;
            }

            // --- Draw arc dust trail ---
            Vector2 start = Projectile.Center;
            for (int j = 0; j < 20; j++)
            {
                int dust = Dust.NewDust(start, 0, 0, DustID.Torch, 0f, 0f, 150, Color.Pink, 1.2f);
                Main.dust[dust].noGravity = true;
                start += arcVec / 20f;
            }

            // --- Advance projectile position along the arc ---
            Projectile.position = start;

            // --- Heal teammates on contact ---
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.active && !p.dead && p.whoAmI != owner.whoAmI)
                {
                    if (owner.team != 0 && owner.team == p.team)
                    {
                        if (Projectile.Hitbox.Intersects(p.Hitbox))
                        {
                            HealTeammateThorium(owner, p, baseHeal: 5);
                            Projectile.Kill();
                            break;
                        }
                    }
                }
            }
        }

        private void HealTeammateThorium(Player healer, Player target, int baseHeal)
        {
            if (healer.whoAmI != Main.myPlayer) return;
            if (healer == target) return;
            if (healer.team == 0 || healer.team != target.team) return;

            if (baseHeal <= 0 && healer.GetModPlayer<ThoriumPlayer>().healBonus <= 0)
                return; // Nothing to heal

            HealerHelper.HealPlayer(
                healer,
                target,
                healAmount: baseHeal, // <-- don't add healBonus here
                recoveryTime: 60,
                healEffects: true
            );
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (length > 6f)
            {
                vector *= 6f / length;
            }
        }
    }
}