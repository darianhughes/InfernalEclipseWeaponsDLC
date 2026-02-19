using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Sounds;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using InfernalEclipseWeaponsDLC.Content.Projectiles.RangedPro.Void;
using SOTS.Void;
using CalamityMod.Items.Materials;
using SOTS.Items.Celestial;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Ranged.Void
{
    [JITWhenModsEnabled("SOTS")]
    [ExtendsFromMod("SOTS")]
    public class ThunderboltActionSniperVoid : VoidItem
    {
        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Ranged/Void/ThunderboltActionSniper";
        public override bool IsLoadingEnabled(Mod mod) => ModLoader.HasMod("SOTS");
        public override void SafeSetDefaults()
        {
            Item.damage = 2990;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            //Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<VoidBolt>();
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;
            Item.width = 120;
            Item.height = 3;
            Item.crit = 30;
        }
        public override int GetVoid(Player player) => 20;

        public override Vector2? HoldoutOffset() => new Vector2(-25, -2f);
        public override void HoldItem(Player player) => player.scope = true;
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // This is to prevent it from shooting through walls if you're too close, but I'm not sure how I like this.
            // It was taken from Example Mod, so I don't really care if it get removed.
            // Test, and if someone actually important to this mod happens, feel free to remove this chunk.
            Vector2 muzzleOffset = Vector2.Normalize(velocity + new Vector2(0, -0.6f)) * 95f;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // So why does with just not increase volume?
            SoundEngine.PlaySound(CommonCalamitySounds.LargeWeaponFireSound with { Volume = 10f }, player.position);
            SoundEngine.PlaySound(SoundID.Thunder with { Volume = 10f }, player.position);

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<VoidBolt>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SniperRifle)
                .AddIngredient<SanguiteBar>(15)
                .AddIngredient<ArmoredShell>(3)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}