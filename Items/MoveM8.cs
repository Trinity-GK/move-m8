using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NpcSlapper.Items
{
	public class MoveM8 : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.noUseGraphic = true; // Hide item, projectile handles visuals
			Item.damage = 0;
			Item.knockBack = 0;
			Item.DamageType = DamageClass.Melee;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<Projectiles.MoveM8Projectile>();
			Item.shootSpeed = 1f;
			Item.channel = true; // Hold to keep thrusting
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Silk, 10);
			recipe.AddIngredient(ItemID.IronBar, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
