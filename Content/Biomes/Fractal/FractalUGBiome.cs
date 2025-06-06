﻿using Terraria;
using Terraria.ModLoader;

namespace Polarities.Content.Biomes.Fractal
{
    public class FractalUGBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            var tile = player.Center.ToTileCoordinates();

            return player.InModBiome<FractalBiome>() && Main.tile[tile].WallType != 0 && !Main.wallHouse[Main.tile[tile].WallType] && tile.Y >= FractalSubworld.skyHeight;
        }

        public override float GetWeight(Player player)
        {
            return 0.7f;
        }

        public override string MapBackground => "Polarities/Content/Biomes/Fractal/FractalMapBackground";
        public override string BackgroundPath => MapBackground;
        public override string BestiaryIcon => "Polarities/Content/Biomes/Fractal/FractalUGBestiaryIcon";

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/FractalPalace");
    }
}