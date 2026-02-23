using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using InfernalEclipseWeaponsDLC.Content.Projectiles.MeleePro.TetherBlade;
using InfernalEclipseWeaponsDLC.Common;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Melee
{
    public class TetherBlade : ModItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return WeaponConfig.Instance.AIGenedWeapons;
        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 42;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<TetherBladeProjectile>();

            Item.GetGlobalItem<WeaponsGlobalItem>().verveineItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && player.HasBuff(BuffID.ChaosState))
            {
                int projType = ModContent.ProjectileType<TetherBladeProjectileBlink>();
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == projType && projectile.owner == player.whoAmI && projectile.active)
                    {
                        return base.CanUseItem(player);
                    }
                }

                return false; // This is to prevent the player from throwing a blade if they have the chaos debuff
                // this is done here and not in Shoot() so use sounds and animations are not triggered
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && Main.mouseRightRelease)
            { // fire weapon
                int projType = ModContent.ProjectileType<TetherBladeProjectileBlink>();

                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == projType && projectile.owner == player.whoAmI && projectile.active)
                    {
                        // tp player
                        player.Teleport(new Vector2(projectile.Center.X - player.width * 0.5f, projectile.Center.Y - player.height * 0.5f), TeleportationStyleID.QueenSlimeHook);
                        SoundEngine.PlaySound(SoundID.Item67.WithPitchOffset(-0.25f), player.Center);
                        player.AddBuff(BuffID.ChaosState, 120);
                        projectile.Kill();

                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active && !npc.friendly && npc.Distance(player.Center) < 160f && !npc.dontTakeDamage && !npc.CountsAsACritter)
                            {
                                Projectile newProjectile2 = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, Item.damage * 3, Item.knockBack * 3f, player.whoAmI);
                                newProjectile2.ai[0] = (npc.Center - player.Center).RotatedByRandom(MathHelper.ToRadians(10)).ToRotation() - MathHelper.PiOver2;
                                newProjectile2.ai[1] = 32f; // thrust timer
                                newProjectile2.ai[2] = -2; // arm stretch
                            }
                        }

                        return false;
                    }
                }

                // create tp projectile

                Projectile newProjectile = Projectile.NewProjectileDirect(source, player.Center, velocity, projType, Item.damage, Item.knockBack, player.whoAmI);

                return false;
            }
            else
            { // stab
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == Item.shoot && projectile.owner == player.whoAmI && projectile.active && projectile.ai[2] >= 0)
                    { // flips the player near the end of the swing animation
                        projectile.ai[2] = -1;
                        projectile.netUpdate = true;
                    }
                }

                Projectile newProjectile = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
                newProjectile.ai[0] = (Main.MouseWorld - player.Center).RotatedByRandom(MathHelper.ToRadians(25)).ToRotation() - MathHelper.PiOver2;
                newProjectile.ai[1] = 32f; // thrust timer
                newProjectile.ai[2] = Main.rand.Next(4); // arm stretch
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddTile(TileID.Anvils);
            recipe.AddIngredient(ItemID.Gladius);
            recipe.AddIngredient(ItemID.CrystalShard, 12);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.UnicornHorn);
            recipe.Register();
        }
    }
}
