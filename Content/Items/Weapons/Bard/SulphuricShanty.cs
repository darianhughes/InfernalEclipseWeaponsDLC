using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Empowerments;
using ThoriumMod.Sounds;
using ThoriumMod;
using ThoriumMod.Items;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class SulphuricShanty : BardItem
    {
        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<CriticalStrikeChance>(3);
            Empowerments.AddInfo<FlightTime>(2);
        }
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;
        public override void SetBardDefaults()
        {
            Item.damage = 50;

            // BardDamage derives from like 100 things...
            Item.DamageType = ModContent.GetInstance<BardDamage>();
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            ((ModItem)this).Item.holdStyle = 5;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.knockBack = 4;
            Item.value = Item.buyPrice(gold: 4);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = ThoriumSounds.String_Sound;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<MiniScourge>();
            Item.shootSpeed = 10f;
        }

        public override void ModifyInspirationCost(Player player, ref int cost)
        {
            cost = 1;
        }

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.BardShoot(player, source, position, velocity, type, damage, knockback);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }

        public override void HoldItemFrame(Player player)
        {
            player.itemLocation += new Vector2(0, 0f) * player.Directions;
        }

        public override void AddRecipes()
        {
        }
    }
}
