using InfernalEclipseWeaponsDLC.Common;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee
{
    public class ObsidianSickle : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.GitGudWeapon;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.damage = 43;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item71;
            Item.shoot = ModContent.ProjectileType<ObsidianSickleProjectile>();
            Item.shootSpeed = 10f;

            Item.GetGlobalItem<WeaponsGlobalItem>().verveineItem = true;
        }
    }
}
