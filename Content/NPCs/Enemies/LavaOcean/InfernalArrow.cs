﻿using Microsoft.Xna.Framework;
using Polarities.Content.Buffs.Hardmode;
using Polarities.Core;
using Polarities.Global;
using Polarities.Assets;
using Polarities.Content.Items.Placeable.Banners.Items;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Polarities.Content.NPCs.Enemies.LavaOcean
{
    public class InfernalArrow : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Polarities.customNPCGlowMasks[Type] = TextureAssets.Npc[Type];
            PolaritiesNPC.canSpawnInLava.Add(Type);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.OnFire,
                    BuffID.Frostburn,
                    BuffID.OnFire3,
                    BuffID.ShadowFlame,
                    BuffID.CursedInferno,
                    BuffType<Incinerating>()
                }
            };
            // NPCID.Sets.DebuffImmunitySets/* tModPorter Removed: See the porting notes in https://github.com/tModLoader/tModLoader/pull/3453 */.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Rotation = -3 * MathHelper.PiOver4
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);

            Main.npcFrameCount[Type] = 5;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            //spawn conditions
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
			//flavor text
			this.TranslatedBestiaryEntry()
        });
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 42;
            NPC.height = 42;
            DrawOffsetY = 21;
            NPC.defense = 18;
            NPC.damage = 40;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0.4f;
            NPC.value = Item.buyPrice(silver: 10);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath6;

            Banner = NPC.type;
            BannerItem = ItemType<InfernalArrowBanner>();

            SpawnModBiomes = new int[1] { GetInstance<Content.Biomes.LavaOcean>().Type };

            NPC.hide = true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 1f, 0.5f, 0f);

            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.netUpdate = true;
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
            }

            if (NPC.velocity.Length() >= 16)
            {
                NPC.ai[1] = Main.rand.Next(180, 240);
                NPC.netUpdate = true;
            }

            if (NPC.ai[1] <= 0 && NPC.noGravity && NPC.velocity.Length() < 16 && Math.Abs(Math.Sin(-Math.PI / 4 + ((player.Center - NPC.Center).ToRotation() - NPC.rotation) / 2)) < Math.Sin(0.1))
            {
                NPC.velocity += 0.5f * (player.Center - NPC.Center) / (NPC.Center - player.Center).Length();
                NPC.ai[0] = NPC.velocity.ToRotation() - (float)Math.PI / 2;
            }
            else
            {
                NPC.velocity *= 0.99f;
                NPC.velocity += 0.05f * (player.Center - NPC.Center) / (NPC.Center - player.Center).Length();
                NPC.ai[0] = NPC.velocity.ToRotation() - (float)Math.PI / 2;
            }

            if (Math.Abs(Math.Sin((NPC.ai[0] - NPC.rotation) / 2)) < Math.Sin(0.1))
            {
                NPC.rotation = NPC.ai[0];
            }
            else if (Math.Sin(NPC.ai[0] - NPC.rotation) > 0)
            {
                NPC.rotation += 0.1f;
            }
            else
            {
                NPC.rotation -= 0.1f;
            }

            NPC.ai[1] -= 1;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //requires no lava to spawn
            if (Main.hardMode && Biomes.LavaOcean.SpawnValid(spawnInfo, requireLava: false))
            {
                return 0.5f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 4 == 0)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (5 * frameHeight);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //adapted from the vanilla weapon eneimies
            if (NPC.life > 0)
            {
                for (int num473 = 0; num473 < NPC.damage / NPC.lifeMax * 50.0; num473++)
                {
                    int num474 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[num474].noGravity = true;
                }
                return;
            }
            for (int num475 = 0; num475 < 20; num475++)
            {
                int num476 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 0, default(Color), 1.5f);
                Dust dust17 = Main.dust[num476];
                Dust dust187 = dust17;
                dust187.velocity *= 2f;
                Main.dust[num476].noGravity = true;
            }
            int num477 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + NPC.height / 2 - 10f), new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), 61, NPC.scale);
            Gore gore4 = Main.gore[num477];
            Gore gore28 = gore4;
            gore28.velocity *= 0.5f;
            num477 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + NPC.height / 2 - 10f), new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), 61, NPC.scale);
            gore4 = Main.gore[num477];
            gore28 = gore4;
            gore28.velocity *= 0.5f;
            num477 = Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.position.X, NPC.position.Y + NPC.height / 2 - 10f), new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), 61, NPC.scale);
            gore4 = Main.gore[num477];
            gore28 = gore4;
            gore28.velocity *= 0.5f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.OnFire, 60);
        }

        public override void DrawBehind(int index)
        {
            RenderTargetLayer.AddNPC<ConvectiveEnemyTarget>(index);
        }
    }
}