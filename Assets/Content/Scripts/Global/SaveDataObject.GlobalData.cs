using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.RaftDamagers;
using Content.Scripts.BoatGame.Services;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
        [Serializable]
        public class GlobalData
        {
            [Serializable]
            public class RaftDamagerData
            {
                [Serializable]
                public class SpawnedItem
                {
                    [SerializeField] private int itemIndex;
                    [SerializeField] private string raftID;
                    [SerializeField] private List<RaftDamager.RaftDamagerDataKey> keys = new List<RaftDamager.RaftDamagerDataKey>(); 
                    public SpawnedItem(int itemIndex, string raftID, List<RaftDamager.RaftDamagerDataKey> keys)
                    {
                        this.itemIndex = itemIndex;
                        this.raftID = raftID;
                        this.keys = keys;
                    }

                    public List<RaftDamager.RaftDamagerDataKey> Keys => keys;

                    public string RaftID => raftID;

                    public int ItemIndex => itemIndex;
                }
                
                [SerializeField] private float tickCount, maxTickCount;
                [SerializeField] private List<SpawnedItem> spawnedItems = new List<SpawnedItem>();


                public RaftDamagerData(float tickCount, float maxTickCount, List<SpawnedItem> spawnedItems)
                {
                    this.tickCount = tickCount;
                    this.maxTickCount = maxTickCount;
                    this.spawnedItems = spawnedItems;
                }

                public List<SpawnedItem> SpawnedItems => spawnedItems;

                public float MaxTickCount => maxTickCount;

                public float TickCount => tickCount;
            }
            
            [Serializable]
            public class WeatherData
            {
                [SerializeField] private float tickCount, maxTickCount;
                [SerializeField] private WeatherService.EWeatherType currentWeather;

                public WeatherData(float tickCount, float maxTickCount, WeatherService.EWeatherType currentWeather)
                {
                    this.tickCount = tickCount;
                    this.maxTickCount = maxTickCount;
                    this.currentWeather = currentWeather;
                }

                public WeatherService.EWeatherType CurrentWeather => currentWeather;

                public float MaxTickCount => maxTickCount;

                public float TickCount => tickCount;
            }
            
            
            [SerializeField] private float totalSecondsInGame;
            [SerializeField] private float totalSecondsOnRaft;
            [SerializeField] private int islandSeed = 0;
            [SerializeField] private int dungeonSeed = 0;
            [SerializeField] private bool shopEventCreated;
            [SerializeField] private RaftDamagerData damagersData = new RaftDamagerData(0,0, new List<RaftDamagerData.SpawnedItem>());
            [SerializeField] private WeatherData weathersData = new WeatherData(0, -1, WeatherService.EWeatherType.Сalm);

            public bool isOnIsland => IslandSeed != 0;
            public bool isInDungeon => DungeonSeed != 0;
            
            public float TotalSecondsInGame => totalSecondsInGame;

            public RaftDamagerData DamagersData => damagersData;

            public WeatherData WeathersData => weathersData;

            public int IslandSeed => islandSeed;

            public float TotalSecondsOnRaft => totalSecondsOnRaft;

            public int DungeonSeed => dungeonSeed;

            public bool ShopEventCreated => shopEventCreated;

            public void CreateShopEvent()
            {
                shopEventCreated = true;
            }

            public void SetTimePlayed(float value)
            {
                totalSecondsInGame = value;
            }

            public void AddTime(float value)
            {
                totalSecondsInGame += value;
            }
            
            public void AddTimeOnRaft(float value)
            {
                totalSecondsOnRaft = TotalSecondsOnRaft + value;
            }

            public void SetDamagersData(RaftDamagerData getDamagersDataData)
            {
                damagersData = getDamagersDataData;
            }

            public void SetWeatherData(WeatherData getWeatherData)
            {
                weathersData = getWeatherData;
            }

            public void SetIslandSeed(int selectedIslandSeed)
            {
                islandSeed = selectedIslandSeed;
            }

            public void EnterInDungeon(int data)
            {
                dungeonSeed = data;
            }

            public void ExitDungeon()
            {
                dungeonSeed = 0;
            }
        }
    }
}