﻿using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;
using static System.Net.Mime.MediaTypeNames;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class CorrodedCane : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            isHealer = true;

            Item.damage = 175;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 40;

            Item.useAnimation = 40;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 20f;

            Item.mana = 20;

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<CorrodedCanePro>();
            Item.shootSpeed = 1f;
            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
        }

        public override bool CanShoot(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if(player.ownedProjectileCounts[Item.shoot] == 1)
            {
                mult = 0.5f; // Half mana to sustain CorrodedCanePro vortex, if already spawned
            }
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
            player.itemLocation.Y -= 12;
            player.itemRotation = MathHelper.Pi / 16 * player.direction; 
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center;
        }
    }
}
