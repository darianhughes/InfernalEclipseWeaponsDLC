using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfernalEclipseWeaponsDLC.Content.Projectiles.SpearTipPro;
using InfernalEclipseWeaponsDLC.Core.NewFolder;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items.Placeable;

namespace InfernalEclipseWeaponsDLC.Common
{
    public class WeaponsGlobalItem : GlobalItem
    {
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 direction = velocity;
            InfernalWeaponsPlayer weaponPlayer = player.GetModPlayer<InfernalWeaponsPlayer>();

            if (ItemID.Sets.Spears[item.type] && (weaponPlayer.spearSearing || weaponPlayer.spearArctic))
            {
                if (weaponPlayer.spearSearing)
                {
                    Projectile.NewProjectile(source, position, direction * 2.5f, ModContent.ProjectileType<HydrogenSulfideProj>(), (int)(damage * 1.5), knockback, player.whoAmI, 0.0f, 0.0f, 0.0f);
                }

                if (weaponPlayer.spearArctic)
                {
                    Projectile.NewProjectile(source, position, direction, ModContent.ProjectileType<CryonicSpearTip>(), (int)(damage * 1.15), knockback, player.whoAmI, 0.0f, 0.0f, 0.0f);
                }
            }
            return true;
        }
    }
}
