using CalamityMod.Projectiles.Magic;
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
            Item.DamageType = ThoriumDamageBase<HealerTool>.Instance;
            Item.noMelee = true;
            Item.mana = 1;
            Item.damage = 10;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Blue; // ?
            Item.UseSound = SoundID.Item24; // ?

            Item.shoot = ModContent.ProjectileType<LightningArc>();
            Item.shootSpeed = 8f;

            isHealer = true;
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
            if (player.altFunctionUse != 2)
            {
                player.Hurt(new Player.HurtInfo() { Damage = 3, DamageSource = new PlayerDeathReason() { SourceItem = Item } });
                player.immune = false;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
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
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }

            Vector2 arc = Vector2.Zero;
            float distance = 10f;
            bool found = false;
            Player target = null;
            bool flag2 = false;
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.whoAmI == Projectile.owner || healedAlready.Contains(player))
                    continue;
                Vector2 newArc = player.Center - Projectile.Center;
                float newDistance = newArc.LengthSquared();
                if (newDistance < (distance * distance))
                {
                    arc = newArc;
                    distance = newDistance;
                    found = true;
                    target = player;
                }
            }

            Vector2 center = Projectile.Center;
            if (found)
            {
                arc += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * distance / 30f;
                if (flag2)
                {
                    prevX++;
                    arc += new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)) * prevX;
                }
            }
            else
            {
                arc = (Projectile.velocity + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6))) * 5f;
            }

            for (int j = 0; j < 20; j++)
            {
                int num4 = Dust.NewDust(center, Projectile.width, Projectile.height, DustID.PinkTorch);
                Main.dust[num4].velocity = new Vector2(0f);
                center += arc / 20f;
            }

            Projectile.position = center;

        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Cancel();
            if (target.whoAmI == Projectile.owner && Projectile.timeLeft > 1000 - 60) return;
            healedAlready.Add(target);
            target.Heal(3);
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