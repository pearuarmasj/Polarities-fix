﻿using Polarities.Content.NPCs.Bosses.PreHardmode.Gigabat;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Polarities.Content.Items.Consumables.Summons.PreHardmode
{
    public class GigabatSummonItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = (1);

            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(NPCType<Gigabat>()) && (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight);
        }

        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, NPCType<Gigabat>());
            SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/NPC_Killed_4")
            {
                Volume = 2f,
                Pitch = -8f
            }, player.position);
            return true;
        }
    }
}