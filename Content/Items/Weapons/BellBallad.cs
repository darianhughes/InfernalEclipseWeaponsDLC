using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoriumMod.Items;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using CalamityMod.Items;
using Terraria.ModLoader;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ID;
using InfernalEclipseWeaponsDLC.Core.Players;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons
{
    public class BellBallad : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Percussion;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(2);
            Empowerments.AddInfo<LifeRegeneration>(1);
            Empowerments.AddInfo<FlightTime>(1);
        }

        public override void SetBardDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.holdStyle = ItemHoldStyleID.HoldFront;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ProjectileID.None;
            Item.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Item.UseSound = SoundID.Item35;

            // TBD
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.knockBack = 1.5f;
            Item.damage = 83;
            Item.shootSpeed = 14f;

            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();

            InspirationCost = 2;
        }

        public override void BardHoldItem(Player player)
        {
            if(player.whoAmI == Main.myPlayer)
            {
                // Spawn and bind projectiles if not bound
                WeaponPlayer weaponPlayer = player.GetModPlayer<WeaponPlayer>();
                weaponPlayer.BellBalladEleum ??= Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<BellBalladEleum>(), Item.damage, Item.knockBack, Main.myPlayer, ai0: 0).ModProjectile as BellBalladEleum;
                weaponPlayer.BellBalladHavoc ??= Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<BellBalladHavoc>(), Item.damage, Item.knockBack, Main.myPlayer, ai0: 1).ModProjectile as BellBalladHavoc;
                weaponPlayer.BellBalladSunlight ??= Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<BellBalladSunlight>(), Item.damage, Item.knockBack, Main.myPlayer, ai0: 2).ModProjectile as BellBalladSunlight;
            }
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
        }

        public override bool? BardUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                WeaponPlayer weaponPlayer = player.GetModPlayer<WeaponPlayer>();
                weaponPlayer.BellBalladEleum?.Shoot(Item.damage, Item.knockBack);
                weaponPlayer.BellBalladHavoc?.Shoot(Item.damage, Item.knockBack);
                weaponPlayer.BellBalladSunlight?.Shoot(Item.damage, Item.knockBack);
                return true;
            }
            return base.BardUseItem(player);
        }

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
        }
    }
}
