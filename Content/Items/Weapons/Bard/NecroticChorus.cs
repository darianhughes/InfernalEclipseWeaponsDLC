using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.NecrooticChorus;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class NecroticChorus : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.Brass;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<Defense>(3);
            Empowerments.AddInfo<Damage>(2);
            Empowerments.AddInfo<LifeRegeneration>(2);
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetBardDefaults()
        {
            Item.damage = 50;
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 20f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = ThoriumSounds.Bard_Horn;
            Item.shoot = ModContent.ProjectileType<NecroticChorusWisp>();
            Item.shootSpeed = 4f;

            InspirationCost = 3;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanShoot(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] < 10;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.X -= 4 * player.direction;
            player.itemLocation.Y -= 18;
            player.itemRotation = 0;

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Item.shootSpeed = 4f;
            if (player.altFunctionUse == 2)
            {
                Item.shootSpeed = 2f;

                type = ModContent.ProjectileType<NecroticChorusPro>();
                velocity *= 10f;
                position.X += 38 * player.direction;
                position.Y -= 18;
            }
        }
    }
}
