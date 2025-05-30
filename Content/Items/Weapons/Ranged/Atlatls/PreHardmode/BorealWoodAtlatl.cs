﻿using Microsoft.Xna.Framework;
using Polarities.Content.Items.Weapons.Ranged.Atlatls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Polarities.Content.Items.Weapons.Ranged.Atlatls.PreHardmode
{
    public class BorealWoodAtlatl : AtlatlBase
    {
        public override Vector2[] ShotDistances => new Vector2[] { new Vector2(30) };

        public override void SetDefaults()
        {
            Item.SetWeaponValues(9, 3, 0);
            Item.DamageType = DamageClass.Ranged;

            Item.width = 36;
            Item.height = 34;

            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Dart;

            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BorealWood, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
