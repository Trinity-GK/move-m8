using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace NpcSlapper.Projectiles
{
	public class MoveM8Projectile : ModProjectile
	{
		// Use the same texture as the item
		public override string Texture => "NpcSlapper/Items/MoveM8";

		private const float MaxExtension = 80f;
		private const int ExtendTime = 30;
		private const int RetractTime = 30;

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.friendly = false; // Don't deal damage normally
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.ownerHitCheck = true;
			Projectile.hide = false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			// Kill projectile if player stops using item
			if (player.dead || !player.active || player.itemAnimation <= 0)
			{
				Projectile.Kill();
				return;
			}

			// Face cursor direction
			Vector2 toMouse = Main.MouseWorld - player.MountedCenter;
			player.direction = toMouse.X > 0 ? 1 : -1;

			// Calculate extension based on item animation
			float progress = 1f - (player.itemAnimation / (float)player.itemAnimationMax);
			float extension = MaxExtension * (float)Math.Sin(progress * Math.PI);

			// Set rotation to point at cursor
			Projectile.rotation = toMouse.ToRotation();

			// Position projectile extending from player
			Vector2 direction = toMouse.SafeNormalize(Vector2.UnitX);
			Projectile.Center = player.MountedCenter + direction * extension;

			// Keep player's arm pointing at projectile
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

			// Check for NPC slapping at max extension
			if (Projectile.ai[0] == 0 && extension > MaxExtension * 0.8f)
			{
				TrySlapNPC(player);
				Projectile.ai[0] = 1; // Mark as already slapped this swing
			}

			// Increment timer
			Projectile.ai[1]++;
		}

		private void TrySlapNPC(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return;

			Vector2 mouseWorld = Main.MouseWorld;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (!npc.active)
					continue;

				// Check if mouse is over this NPC or projectile touches NPC
				bool mouseOver = npc.Hitbox.Contains((int)mouseWorld.X, (int)mouseWorld.Y);
				bool projectileTouch = Projectile.Hitbox.Intersects(npc.Hitbox);

				if (mouseOver || projectileTouch)
				{
					SlapNPC(player, npc);
					break;
				}
			}
		}

		private void SlapNPC(Player player, NPC npc)
		{
			Vector2 direction = npc.Center - player.Center;
			if (direction != Vector2.Zero)
				direction.Normalize();
			else
				direction = new Vector2(1, 0);

			float knockbackStrength = 12f;
			npc.velocity = direction * knockbackStrength;
			npc.velocity.Y -= 6f;

			for (int d = 0; d < 15; d++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, DustID.Cloud, direction.X * 3f, direction.Y * 3f);
			}

			CombatText.NewText(npc.Hitbox, Color.Yellow, "MOVE M8!", true);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

			// Add 45 degrees to rotation so sprite points correctly (assuming diagonal sprite)
			float drawRotation = Projectile.rotation + MathHelper.PiOver4;

			Main.EntitySpriteDraw(
				texture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				drawRotation,
				origin,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			return false;
		}

		public override bool? CanDamage()
		{
			return false; // Don't deal damage
		}
	}
}
