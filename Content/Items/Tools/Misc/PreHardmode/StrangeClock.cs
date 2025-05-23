﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Polarities.Content.Items.Tools.Misc.PreHardmode
{
    public class StrangeClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = (1);
            // DisplayName.SetDefault("Strange Clock");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item29;
        }

        public override void HoldItem(Player player)
        {
            if (player.channel)
            {
                player.itemTime = 1;
                player.itemAnimation = 1;

                //accelerate time
                PolaritiesSystem.timeAccelerate = true;
            }
        }
    }
}