﻿using Polarities.Assets.Dusts;
using Polarities.Content.Items.Placeable.Furniture;
using Polarities.Content.Items.Placeable.Blocks.Fractal;
using Polarities.Content.Items.Materials.PreHardmode;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Polarities.Content.Items.Placeable.Furniture.Fractal
{
    public class FractalSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.MetalSink);
            Item.createTile = ModContent.TileType<FractalSinkTile>();
            Item.placeStyle = 0;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<FractalBrick>(), 6)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class FractalSinkTile : SinkTileBase
    {
        public override int MyDustType => ModContent.DustType<FractalMatterDust>();
        public override int DropItem => ModContent.ItemType<FractalSink>();
    }
}