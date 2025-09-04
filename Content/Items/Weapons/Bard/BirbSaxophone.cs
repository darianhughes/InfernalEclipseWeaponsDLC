﻿using CalamityMod.Items;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class BirbSaxophone : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Wind;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<LifeRegeneration>(2);
            Empowerments.AddInfo<MovementSpeed>(3);
            Empowerments.AddInfo<FlightTime>(3);
        }

        public override void SetBardDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<BirbSaxophonePro>();
            Item.UseSound = ThoriumSounds.Saxophone_Sound;

            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.knockBack = 1.5f;
            Item.damage = 80;
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.rare = ItemRarityID.Purple;

            InspirationCost = 3;

            ((ModItem)this).Item.useStyle = 5;
            if (!ModLoader.HasMod("Look"))
            {
                ((ModItem)this).Item.holdStyle = 3;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6, 0);
        }

        public override void HoldItemFrame(Player player)
        {
            player.itemLocation += Utils.RotatedBy(new Vector2((float)(ModLoader.HasMod("Look") ? (-4) : (-6)), (float)(ModLoader.HasMod("Look") ? 6 : 8)) * player.Directions, (double)player.itemRotation, default(Vector2));
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
            player.itemLocation.Y += 10;
        }

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
        }
    }
}
