using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.Items;
using ThoriumMod.Sounds;

namespace InfernalEclipseWeaponsDLC.Content.Items.Weapons.Healer
{
    [ExtendsFromMod("ThoriumMod")]
    public class Staff : ThoriumItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(44, 48);
            Item.value = Item.sellPrice(gold: 3);

            Item.useTime = 5;
            // Item.reuseDelay = 4;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.damage = 10;
            Item.knockBack = 4f;
            Item.noMelee = true;

            // Item.UseSound = ThoriumSounds.Trombone_Sound;
            Item.rare = ItemRarityID.LightRed;

            Item.shoot = ModContent.ProjectileType<Staff_Projectile>();
            Item.shootSpeed = 20;

            isHealer = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += velocity * 3;
        }
    }

    public class Staff_Projectile : ModProjectile
    {
        public int TicksBeforeTurn;

        public override string Texture => "InfernalEclipseWeaponsDLC/Content/Projectiles/HealerPro/Staff_Projectile";

        public override void SetDefaults()
        {
            TicksBeforeTurn = 25; // 3 seconds

            Projectile.Size = new Vector2(72, 72);
            Projectile.timeLeft = (TicksBeforeTurn * 5) - 5; // Shortened a bit so that it doesn't hit the  player
            Projectile.penetrate = 5;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= TicksBeforeTurn)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(144)); // Make star shape
                Projectile.ai[0] = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(14, 2, 72, 72), lightColor, Projectile.rotation, new Vector2(36, 36), Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
