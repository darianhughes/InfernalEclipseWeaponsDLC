using InfernalEclipseWeaponsDLC.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CustomRecipes;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using InfernalEclipseWeaponsDLC.Content.Projectiles;
using InfernalEclipseWeaponsDLC.Content.Projectiles.BardPro;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes;
using InfernalEclipseWeaponsDLC.Content.Projectiles.HealerPro.Scythes.GammaKnife;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Localization;
using ThoriumMod;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Items.BossThePrimordials.Dream;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Projectiles.Healer;
using ThoriumMod.Projectiles.Scythe;
using ThoriumMod.Tiles;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    public class GammaKnife : ScytheItem
    {
        // store projectile IDs ourselves
        private int swingProj;
        private int thrownProj;

        public override void SetStaticDefaults()
        {
            SetStaticDefaultsToScythe();
        }

        public static readonly SoundStyle Sound = new($"{nameof(InfernalEclipseWeaponsDLC)}/Assets/Effects/Sounds/Swing", 2)
        {
            PitchVariance = 0.25f
        };

        public override void SetDefaults()
        {
            SetDefaultsToScythe();

            // assign our local fields (works regardless of ScytheItem internals)
            swingProj = ModContent.ProjectileType<GammaKnifeProjectile>();
            thrownProj = ModContent.ProjectileType<GammaKnifeThrownProjectile>();

            scytheSoulCharge = 3;

            Item.damage = 400;
            Item.knockBack = 2f;

            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.width = 86;
            Item.height = 74;

            Item.UseSound = Sound;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            // default to swing projectile
            Item.shoot = swingProj;
            Item.shootSpeed = 6f;

            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;

            Item.GetGlobalItem<CalamityGlobalItem>().UsesCharge = true;
            Item.GetGlobalItem<CalamityGlobalItem>().MaxCharge = 190f;
            Item.GetGlobalItem<CalamityGlobalItem>().ChargePerUse = 0.1f;

            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // Count active thrown projectiles
            int thrownCount = player.ownedProjectileCounts[thrownProj];

            // If a thrown projectile exists, disable using the item entirely
            if (thrownCount > 0)
                return false;

            bool alt = player.HasAltFunctionUse();

            Item.shootSpeed = alt ? 14f : 6f;
            Item.shoot = alt ? thrownProj : swingProj;

            return true;
        }
    }
}