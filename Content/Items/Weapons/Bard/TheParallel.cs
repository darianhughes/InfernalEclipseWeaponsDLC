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

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Bard
{
    public class TheParallel : BardItem
    {
        public override BardInstrumentType InstrumentType => BardInstrumentType.String;

        public override void SetStaticDefaults()
        {
            Empowerments.AddInfo<InvincibilityFrames>(2);
            Empowerments.AddInfo<FlightTime>(2);
            Empowerments.AddInfo<MovementSpeed>(2);
        }

        public override void SetBardDefaults()
        {
            Item.width = 44;
            Item.height = 46;
            Item.holdStyle = 5;
            Item.useStyle = 12;
            Item.useTime = 4;
            Item.useAnimation = 12;
            Item.reuseDelay = 20;
            Item.damage = 131;
            Item.autoReuse = true;
            Item.knockBack = 1.5f;
            Item.noMelee = true;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<TheParallellPro>();
            Item.DamageType = ThoriumDamageBase<BardDamage>.Instance;
            Item.UseSound = ThoriumSounds.SunflareString_Sound;
            InspirationCost = 1;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }

        public override bool BardShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int count = 3;
            float spread = MathHelper.ToRadians(30f); // total arc
            Vector2 behind = player.Center - player.DirectionTo(Main.MouseWorld) * 40f;

            for (int i = 0; i < count; i++)
            {
                float phase = MathHelper.Lerp(0, MathHelper.TwoPi, i / (float)count);
                float rotation = MathHelper.Lerp(-spread / 2, spread / 2, i / (float)(count - 1));
                Vector2 shootDir = Vector2.Normalize(Main.MouseWorld - behind).RotatedBy(rotation);

                int proj = Projectile.NewProjectile(
                    source,
                    behind,
                    shootDir * 14f, // Use the shootSpeed you want
                    type,
                    damage,
                    knockback,
                    player.whoAmI
                );

                if (Main.projectile.IndexInRange(proj))
                    Main.projectile[proj].ai[1] = phase; // If you want to use phase for trails or effect
            }
            return false; // Suppress vanilla projectile spawn
        }

        public override void BardModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "ParallelLore1", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.ParallelLore1")) { OverrideColor = Color.MediumPurple });
            tooltips.Add(new TooltipLine(Mod, "ParallelLore2", Language.GetTextValue("Mods.InfernalEclipseWeaponsDLC.ItemTooltip.ParallelLore2")) { OverrideColor = Color.MediumPurple });
        }
    }
}
