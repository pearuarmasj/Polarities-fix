﻿using Microsoft.Xna.Framework;
using System;
using Polarities.Core;
using Polarities.Global;
using Polarities.Content.Items.Vanity.PreHardmode;
using Polarities.Content.Items.Materials.PreHardmode;
using Polarities.Content.Items.Placeable.Banners.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Polarities.Content.NPCs.Enemies.Desert.PreHardmode
{
    public class NestGuardian : ModNPC
    {
        private int AttackCooldown
        {
            get => AttackCooldown = (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private int RattleCooldown
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private bool usedEggs;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);

            PolaritiesNPC.forceCountForRadar.Add(Type);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                //spawn conditions
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundDesert,
				//flavor text
				this.TranslatedBestiaryEntry()
            });
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 64;
            NPC.height = 60;
            NPC.defense = 16;
            NPC.damage = Main.expertMode ? 160 / 2 : 120;
            NPC.lifeMax = 160;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(0, 0, 5, 0);

            Banner = NPC.type;
            BannerItem = ItemType<NestGuardianBanner>();
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
            }

            if (RattleCooldown > 0)
            {
                RattleCooldown--;
            }

            if (AttackCooldown > 0)
            {
                AttackCooldown--;
                NPC.damage = Main.expertMode ? 160 : 120;
                if (NPC.frame.Y < 4 * NPC.height)
                {
                    NPC.frame.Y = 3 * NPC.height;
                }
            }
            else
            {
                NPC.damage = 0;

                if ((NPC.Center - player.Center).Length() > player.velocity.Length() * 128 * Math.Cos(player.velocity.ToRotation() - (NPC.Center - player.Center).ToRotation()))
                {
                    NPC.frame.Y = 3 * NPC.height;
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }
                else if ((NPC.Center - player.Center).Length() > 100)
                {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    if (RattleCooldown == 0)
                    {
                        SoundEngine.PlaySound(Sounds.Rattle, NPC.Center);
                        RattleCooldown = 40;
                    }
                }
                else
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.frame.Y = 8 * NPC.height;
                    NPC.width = 140 * 2 - 64;
                    NPC.position.X -= (140 - 64);
                    AttackCooldown = 60;

                    NPC.spriteDirection = -NPC.direction;
                }
            }

            if (AttackCooldown < 60 - 7)
            {
                NPC.width = 64;
                NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            }

            if (AttackCooldown == 60 - 8)
            {
                NPC.position.X += (140 - 64);
            }

            if (NPC.life < NPC.lifeMax / 2 && !usedEggs)
            {
                usedEggs = true;
                SoundEngine.PlaySound(Sounds.Rattle, NPC.Center);
                for (int i = 0; i < (Main.expertMode ? 7 : 4); i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(5, 0).RotatedByRandom(MathHelper.Pi), ProjectileType<NestGuardianEgg>(), 0, 0, Main.myPlayer);
                    }
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter == 1)
            {
                NPC.frameCounter = 0;

                if (NPC.frame.Y >= 4 * frameHeight && NPC.frame.Y < 8 * frameHeight)
                {
                    NPC.frame.Y -= 4 * frameHeight;
                }

                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y == 4 * frameHeight || NPC.frame.Y == 23 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
                if (NPC.direction == NPC.spriteDirection && NPC.frame.Y < 8 * frameHeight)
                {
                    NPC.frame.Y += 4 * frameHeight;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int i = 1; i <= 5; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("NestGuardianGore" + i).Type);
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Terraria.ModLoader.Utilities.SpawnCondition.DesertCave.Chance * 0.2f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<Rattle>(), 2));
        }
    }

    public class NestGuardianEgg : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / Projectile.width * 2;
            Projectile.velocity.Y += 0.2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
                Projectile.velocity *= 0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.velocity *= 0.9f;
            }
            if (Projectile.timeLeft < 900)
            {
                Projectile.Kill();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, NPCType<Rattler>());
            SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, Mod.Find<ModGore>("NestGuardianEggShard").Type);
            }
        }
    }
}

