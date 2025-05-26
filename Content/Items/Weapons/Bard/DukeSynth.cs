using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class DukeSynth : BardItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override BardInstrumentType InstrumentType => BardInstrumentType.Electronic;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Damage>(3);
            Empowerments.AddInfo<CriticalStrikeChance>(2);
            Empowerments.AddInfo<FlatDamage>(2);
            //"Viral Wisdom"
        }

        public override void SetBardDefaults()
        {
            Item.damage = 175;
            InspirationCost = 1;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.autoReuse = true;
            Item.useStyle = 5;
            Item.noMelee = true;
            Item.knockBack = 20f;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.UseSound = SoundID.Zombie53;
            Item.shoot = ModContent.ProjectileType<SulfurSpirit>();
            Item.shootSpeed = 20f;
        }

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

    }
}
