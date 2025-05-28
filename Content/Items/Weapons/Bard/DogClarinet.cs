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

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class DogClarinet : BardItem
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
            Item.width = 62;
            Item.height = 44;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<DogClarinetSnipingBlast>();
            Item.UseSound = ThoriumSounds.Clarinet_Sound;

            // TBD
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.knockBack = 1.5f;
            Item.damage = 5175;
            Item.shootSpeed = 22f;

            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();

            InspirationCost = 2;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 12 * player.direction;
            player.itemLocation.Y += 10;
        }

        //public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        //{
        //}

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
            Color lerpedColor = Color.Lerp(Color.White, new Color(255, 105, 180), (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 4.0) * 0.5 + 0.5));

            TooltipLine line1 = new(Mod, "UniversalLore1", "Kos had made his final stand. Used all of his tricks up his sleeve. He had lost to them. However, it doesn't mean Eclipse had lost.");
            line1.OverrideColor = Color.MediumPurple;
            tooltips.Add(line1);

            TooltipLine line2 = new(Mod, "UniversalLore2", "Using every inch of power he had remaining in his soul, he summoned the true god slayer: The Devourer of Gods.");
            line2.OverrideColor = Color.MediumPurple;
            tooltips.Add(line2);
        }
    }
}
