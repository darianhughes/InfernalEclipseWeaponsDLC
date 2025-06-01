using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.Items;
using CalamityMod.Projectiles.Melee;
using Microsoft.CodeAnalysis.Operations;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Summoner
{
    public class GrandThunderWhip : ModItem
    {
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.damage = 14;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<>();
            Item.useStyle = 1;
            Item.shootSpeed = 12f;
            Item.useTime = 34;
            Item.useAnimation = 34;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item152;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.White;
            Item.value = CalamityGlobalItem.RarityWhiteBuyPrice;
        }

        public override bool MeleePrefix() => true;

        public virtual bool Shoot(
          Player player,
          EntitySource_ItemUse_WithAmmo source,
          Vector2 position,
          Vector2 velocity,
          int type,
          int damage,
          float knockback)
        {
            return player.ownedProjectileCounts[type] < 1;
        }
    }
}
