using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MagicPro;
using InfernalEclipseWeaponsDLC.Common;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Magic
{
    public class LightChain : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 160;
            Item.width = 48;
            Item.height = 48;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 0.1f;
            Item.mana = 30;
            Item.crit = 6;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.shoot = ModContent.ProjectileType<LightChainProjectile>();
            Item.shootSpeed = 1f; // so the staff "held item" aims in the right direction when shooting lol
            Item.autoReuse = false;

            Item.GetGlobalItem<WeaponsGlobalItem>().verveineItem = true;
        }

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile oldestChain = null;
            List<int> alreadyChained = new List<int>();

            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == Item.shoot && projectile.active && projectile.owner == player.whoAmI)
                {
                    alreadyChained.Add((int)projectile.ai[1]);
                    if (oldestChain == null || oldestChain.timeLeft > projectile.timeLeft)
                    {
                        oldestChain = projectile;
                    }
                }
            }

            NPC target = null;
            NPC targetAlreadychained = null; // lower priority
            float minimalDistance = 80f; // 5 tiles range around cursor
            foreach (NPC npc in Main.npc)
            {
                float distance = npc.Distance(Main.MouseWorld);
                if (distance < minimalDistance && Collision.CanHitLine(player.Center, 2, 2, npc.Center, 2, 2) && npc.active && !npc.friendly && !npc.CountsAsACritter && !npc.dontTakeDamage && npc.Center.Distance(player.Center) < LightChainProjectile.MaxRange)
                {
                    minimalDistance = distance;

                    if (alreadyChained.Contains(npc.whoAmI))
                    {
                        targetAlreadychained = npc;
                    }
                    else
                    {
                        target = npc;
                    }
                }
            }

            if (target == null && targetAlreadychained != null)
            {
                target = targetAlreadychained;

                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == Item.shoot && projectile.active && projectile.owner == player.whoAmI && (int)projectile.ai[1] == targetAlreadychained.whoAmI)
                    {
                        projectile.ai[2] = 1; // kill the duplicate chain
                        projectile.friendly = false;
                        projectile.netUpdate = true;
                        break;
                    }
                }
            }
            else if (alreadyChained.Count > 2 && target != null)
            {
                oldestChain.ai[2] = 1; // kill the oldest chain
                oldestChain.friendly = false;
                oldestChain.netUpdate = true;
            }

            if (target != null)
            { // ai0 = "seed" for chain pattern
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI, Main.rand.Next(9999), target.whoAmI);
            }

            return false;
        }

        public override bool CanUseItem(Player player)
        { // There are cleaner ways than duplicating this part of the code :/
            if (!Main.mouseLeftRelease) return false; // must release left click 

            List<int> alreadyChained = new List<int>();
            NPC target = null;
            NPC targetAlreadychained = null; // lower priority
            float minimalDistance = 80f; // 5 tiles range around cursor

            foreach (NPC npc in Main.npc)
            {
                float distance = npc.Distance(Main.MouseWorld);
                if (distance < minimalDistance && Collision.CanHitLine(player.Center, 2, 2, npc.Center, 2, 2) && npc.active && !npc.friendly && !npc.CountsAsACritter && !npc.dontTakeDamage && npc.Center.Distance(player.Center) < LightChainProjectile.MaxRange)
                {
                    minimalDistance = distance;

                    if (alreadyChained.Contains(npc.whoAmI))
                    {
                        targetAlreadychained = npc;
                    }
                    else
                    {
                        target = npc;
                    }
                }
            }

            if (target == null && targetAlreadychained != null)
            {
                target = targetAlreadychained;
            }

            if (target != null)
            {
                return base.CanUseItem(player);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddTile(TileID.MythrilAnvil)
                .AddIngredient(ItemID.HallowedBar, 20)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.PixieDust, 10)
                .AddIngredient(ItemID.AncientBattleArmorMaterial)
                .Register();
        }
    }
}
