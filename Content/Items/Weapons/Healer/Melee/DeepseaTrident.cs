using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Projectiles;
using System;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Buffs.DamageOverTime;
using ThoriumMod;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer.Melee
{
    [ExtendsFromMod("ThoriumMod", "CalamityMod")]
    public class DeepseaTrident : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(78, 78);

            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;

            Item.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Item.damage = 25;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.mana = 10;
            radiantLifeCost = 3;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;

            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<DeepseaTrident_Projectile>();
            isHealer = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }

        public override void AddRecipes()
        {
            //Adding a recipe for Infernum Mode only due chests not spawning in the Abyss in Infernum.
            if (ModLoader.TryGetMod("InfernumMode", out Mod _))
            {
                CreateRecipe()
                    .AddIngredient(ItemID.Trident)
                    .AddIngredient<AbyssGravel>(5)
                    .AddIngredient<PlantyMush>(5)
                    .AddIngredient(ItemID.Bone, 3)
                    .AddTile(TileID.Anvils)
                    .Register();
            }
        }
    }

    [ExtendsFromMod("ThoriumMod", "CalamityMod")]
    public class DeepseaTrident_Projectile : ThoriumProjectile
    {
        public float RangeMax = 80;
        public float RangeMin = 40;

        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Items/Weapons/Healer/Melee/DeepseaTrident";

        public override void SetDefaults()
        {
            Projectile.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            Projectile.CloneDefaults(ProjectileID.Spear);
            RangeMax = 80;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;
            player.heldProj = Projectile.whoAmI;

            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress = Projectile.timeLeft < halfDuration ? Projectile.timeLeft / halfDuration : (duration - Projectile.timeLeft) / halfDuration;

            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * RangeMin, Projectile.velocity * RangeMax, progress);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft == Math.Floor(halfDuration) && Main.myPlayer == Projectile.owner)
            {
                IEntitySource source = Projectile.GetSource_FromAI();
                Vector2 position = Projectile.Center - Projectile.velocity * 4;
                int damage = Projectile.damage;
                float knockback = Projectile.knockBack;
                float spread = MathHelper.ToRadians(30);

                // ArcherfishShot
                int proj1 = Projectile.NewProjectile(source, position, Projectile.velocity * 3,
                    ModContent.ProjectileType<ArcherfishShot>(), damage, knockback, Projectile.owner);

                if (Main.projectile.IndexInRange(proj1))
                {
                    Projectile proj = Main.projectile[proj1];
                    proj.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                    proj.localAI[0] = 1f; // Mark as spawned by DeepseaTrident
                }

                // Bubble 1
                Projectile bubble1 = Projectile.NewProjectileDirect(source, position,
                    Projectile.velocity.RotatedBy(spread) * 3,
                    ModContent.ProjectileType<BlueBubble>(), damage, knockback, Projectile.owner,
                    ai1: Main.rand.NextFloat() + 0.5f);
                bubble1.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                bubble1.localAI[0] = 1f;

                // Scale bubble if spawned by DeepseaTrident
                bubble1.scale *= 2f;
                bubble1.position -= bubble1.Size * 0.5f; // re-center since size changed
                bubble1.Size *= 2; // expand hitbox

                // Bubble 2
                Projectile bubble2 = Projectile.NewProjectileDirect(source, position,
                    Projectile.velocity.RotatedBy(-spread) * 3,
                    ModContent.ProjectileType<BlueBubble>(), damage, knockback, Projectile.owner,
                    ai1: Main.rand.NextFloat() + 0.5f);
                bubble2.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
                bubble2.localAI[0] = 1f;

                // Scale bubble if spawned by DeepseaTrident
                bubble2.scale *= 2f;
                bubble2.position -= bubble2.Size * 0.5f; // re-center
                bubble2.Size *= 2;

                bubble2.DamageType = ThoriumDamageBase<HealerDamage>.Instance;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 180); // 3 second
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Projectile.velocity * 32 - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.ToRadians(45), texture.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
    }

    public class RiptideGlobalProjectile : GlobalProjectile
    {
        

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.localAI[0] == 1f &&
               projectile.type == ModContent.ProjectileType<BlueBubble>())
            {
                target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60); // 1 second
            }

            if (projectile.localAI[0] == 1f &&
               projectile.type == ModContent.ProjectileType<ArcherfishShot>())
            {
                target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 180); // 3 second
            }
        }
    }

}