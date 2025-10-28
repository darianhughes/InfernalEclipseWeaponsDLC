using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using ThoriumMod.Empowerments;
using ThoriumMod;
using ThoriumMod.Items;
using ThoriumMod.Items.BardItems;
using Microsoft.Xna.Framework;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro.NukePros;
using InfernalEclipseWeaponsDLC.Core.NewFolder;
using ThoriumMod.Sounds;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class FroststeelGong : BigInstrumentItemBase
    {
        public override bool ForceDisableAutoReuse => true;

        public override BardInstrumentType InstrumentType => BardInstrumentType.Percussion;

        public override bool CanChangePitch => false;

        public override void SafeSetStaticDefaults()
        {
            Empowerments.AddInfo<AquaticAbility>(2);
            Empowerments.AddInfo<FlightTime>(2);
            Empowerments.AddInfo<JumpHeight>(2);
            Empowerments.AddInfo<MovementSpeed>(2);
            Empowerments.AddInfo<InvincibilityFrames>(2);
            Empowerments.AddInfo<ResourceMaximum>(2);
            Empowerments.AddInfo<ResourceRegen>(2);
        }

        public override void SafeSetBardDefaults()
        {
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = false; // Hide item while using, same as SteelDrum
            Item.holdStyle = 3;       // Use the same hold animation
            Item.width = 30;
            Item.height = 30;
            Item.shootSpeed = 15;
            Item.shoot = ModContent.ProjectileType<FroststeelPulse>(); // now the pulse
            Item.autoReuse = false; // optional: single strike per use
            Item.damage = 1000;
            Item.knockBack = 4f;
            Item.UseSound = ThoriumSounds.Cymbals;
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            InspirationCost = 10;
        }

        public override void UseItemFrame(Player player)
        {
            base.UseItemFrame(player);
        }

        public override void AddRecipes()
        {
            // Always get Thorium (we depend on it anyway)
            Mod thorium = ModLoader.GetMod("ThoriumMod");
            Mod calamity = null;
            Mod clamity = null;
            Mod ragnarok = null;

            // Try to safely get Calamity and Ragnarok
            ModLoader.TryGetMod("CalamityMod", out calamity);
            ModLoader.TryGetMod("Clamity", out clamity);
            ModLoader.TryGetMod("RagnarokMod", out ragnarok);

            Recipe recipe = CreateRecipe();
            if (ModLoader.TryGetMod("ThoriumMod", out Mod thoriumMod))
            {
                recipe.AddIngredient(thoriumMod.Find<ModItem>("FrostwindCymbals").Type, 1);
                recipe.AddIngredient(thoriumMod.Find<ModItem>("SteelDrum").Type, 1);
            }
            if (clamity != null)
            {
                recipe.AddIngredient(clamity.Find<ModItem>("EnchantedMetal").Type, 8);
            }
            else
            {
                recipe.AddIngredient(calamity.Find<ModItem>("AuricBar").Type, 8);
                recipe.AddIngredient(calamity.Find<ModItem>("EndothermicEnergy").Type, 20);
            }
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.Register();
            base.AddRecipes();
        }

        public override void Shoot_OnSuccess(Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(1f, 1f) * 20f;
                Dust d = Dust.NewDustPerfect(player.Center + offset, DustID.Snow, offset.SafeNormalize(Vector2.Zero) * 50f, 100, default, 1.2f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item50, player.Center); // frosty chime sound
        }

        public override void SafeBardShoot(int success, int level, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position,
    Vector2 velocity, int type, int damage, float knockback)
        {
            // Fire Froststeel Pulse directly from the player’s center
            Projectile.NewProjectile(
                source,
                player.Center,
                Vector2.Zero, // no movement, it grows in place
                ModContent.ProjectileType<FroststeelPulse>(),
                damage,
                knockback,
                player.whoAmI
            );
        }
    }
}
