using CalamityMod.Items;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod;
using ThoriumMod.Empowerments;
using ThoriumMod.Items;
using ThoriumMod.Sounds;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class CorrodedCane : ThoriumItem
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 175;
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
            Item.shoot = ModContent.ProjectileType<CalamityMod.Projectiles.Boss.OldDukeVortex>();
            Item.shootSpeed = 1f;
        }

        public override bool CanShoot(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] < 10;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
    }
}
