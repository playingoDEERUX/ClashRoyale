﻿using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Home.Decks;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Home
    {
        [JsonProperty("clan_info")] public AllianceInfo AllianceInfo = new AllianceInfo();

        [JsonProperty("arena")] public Arena Arena = new Arena();
        [JsonProperty("chests")] public Chests.Chests Chests = new Chests.Chests();
        [JsonProperty("deck")] public Deck Deck = new Deck();
        [JsonProperty("shop")] public Shop.Shop Shop = new Shop.Shop();

        public Home()
        {
            Deck.Home = this;
            Shop.Home = this;
            Chests.Home = this;
            Arena.Home = this;
        }

        public Home(long id, string token)
        {
            Id = id;
            UserToken = token;

            PreferredDeviceLanguage = "EN";

            Gold = 100;
            Diamonds = 1000000;

            Name = "NoName";
            ExpLevel = 1;

            Deck.Home = this;
            Deck.Initialize();

            Shop.Home = this;
            Shop.Refresh();

            Chests.Home = this;
        }

        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("token")] public string UserToken { get; set; }
        [JsonProperty("name_set")] public int NameSet { get; set; }
        [JsonProperty("ip")] public string IpAddress { get; set; }
        [JsonProperty("high_id")] public int HighId { get; set; }
        [JsonProperty("low_id")] public int LowId { get; set; }
        [JsonProperty("language")] public string PreferredDeviceLanguage { get; set; }
        [JsonProperty("fcb_id")] public string FacebookId { get; set; }

        // Shop
        [JsonProperty("shop_day")] public int ShopDay { get; set; }

        // Resources
        [JsonProperty("diamonds")] public int Diamonds { get; set; }
        [JsonProperty("gold")] public int Gold { get; set; }

        // Crownchest
        [JsonProperty("crowns")] public int Crowns { get; set; }
        [JsonProperty("new_crowns")] public int NewCrowns { get; set; }

        // Player Stats
        [JsonProperty("exp_level")] public int ExpLevel { get; set; }
        [JsonProperty("exp_points")] public int ExpPoints { get; set; }

        [JsonIgnore]
        public long Id
        {
            get => ((long) HighId << 32) | (LowId & 0xFFFFFFFFL);
            set
            {
                HighId = Convert.ToInt32(value >> 32);
                LowId = (int) value;
            }
        }

        public void BuyResourcePack(int id)
        {
            var packs = Csv.Tables.Get(Csv.Files.ResourcePacks).GetDataWithInstanceId<ResourcePacks>(id);
            var amount = packs.Amount;
            var diamondCost = 1;

            if (amount > 100)
            {
                if (amount > 1000)
                    if (amount > 10000)
                        if (amount > 100000)
                        {
                            if (amount >= 1000000)
                                diamondCost = 45000;
                        }
                        else
                        {
                            diamondCost = 4500;
                        }
                    else
                        diamondCost = 500;
                else
                    diamondCost = 60;
            }
            else
            {
                diamondCost = 8;
            }


            Gold += amount;
            Diamonds -= diamondCost;
        }

        public void AddExpPoints(int expPoints)
        {
            if (ExpLevel <= 13)
            {
                ExpPoints += expPoints;

                for (var i = ExpLevel; i < 13; i++)
                {
                    var data = Csv.Tables.Get(Csv.Files.ExpLevels).GetDataWithInstanceId<ExpLevels>(ExpLevel - 1);
                    if (data.ExpToNextLevel <= ExpPoints)
                    {
                        ExpLevel++;
                        ExpPoints -= data.ExpToNextLevel;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void AddCrowns(int crowns)
        {
            if (Crowns + crowns <= 20) NewCrowns += crowns;
        }

        public bool UseGold(int amount)
        {
            if (Gold - amount >= 0)
            {
                Gold -= amount;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     This will be called when a user is in home state
        /// </summary>
        public void Reset()
        {
            Crowns += NewCrowns;
            NewCrowns = 0;
        }
    }
}