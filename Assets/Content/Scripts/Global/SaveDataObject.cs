using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.RaftDamagers;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/SaveData", fileName = "SaveDataObject", order = 0)]
    public class SaveDataObject : ScriptableObjectInstaller
    {
        [Serializable]
        public class CharactersData
        {
            [SerializeField] private List<Character> characters = new List<Character>();

            public int Count => characters.Count;

            public void AddCharacter(Character character)
            {
                characters.Add(character);
            }

            public Character GetCharacter(int i)
            {
                return characters[i];
            }

            public void RemoveCharacter(string targetUid)
            {
                characters.RemoveAll(x => x.Uid == targetUid);
            }
        }
        
        [Serializable]
        public class RaftsData
        {
            [Serializable]
            public class RaftData
            {
                [SerializeField] private string uid;
                [SerializeField] private Vector3Int pos;
                [SerializeField] private float health;
                [SerializeField] private RaftBuildService.RaftItem.ERaftType raftType;

                public RaftData(string uid, Vector3Int pos, float health, RaftBuildService.RaftItem.ERaftType raftType)
                {
                    this.uid = uid;
                    this.pos = pos;
                    this.health = health;
                    this.raftType = raftType;
                }

                public RaftBuildService.RaftItem.ERaftType RaftType => raftType;

                public float Health => health;

                public Vector3Int Pos => pos;

                public string Uid => uid;
            }

            [Serializable]
            public class RaftAdditionalData
            {
                [SerializeField] protected string raftUid;
                public string RaftUid => raftUid;
            }

            [Serializable]
            public class RaftStorageData : RaftAdditionalData
            {
                [Serializable]
                public class StorageItemData
                {
                    [SerializeField] private string itemID;
                    [SerializeField] private int count;

                    public StorageItemData(string itemID, int count)
                    {
                        this.itemID = itemID;
                        this.count = count;
                    }

                    public int Count => count;

                    public string ItemID => itemID;
                }

                [SerializeField] private List<StorageItemData> storagesData;


                public RaftStorageData(string raftUid, List<StorageItemData> storagesData)
                {
                    this.raftUid = raftUid;
                    this.storagesData = storagesData;
                }

                public List<StorageItemData> StoragesData => storagesData;
            }
            
            [Serializable]
            public class RaftCraft : RaftAdditionalData
            {
                [SerializeField] private string craftID;
                [SerializeField] private float buildedTime;

                public RaftCraft(string raftUid, string craftID, float buildedTime)
                {
                    this.raftUid = raftUid;
                    this.craftID = craftID;
                    this.buildedTime = buildedTime;
                }

                public float BuildedTime => buildedTime;

                public string CraftID => craftID;
            }
            
            [Serializable]
            public class RaftFurnace : RaftAdditionalData
            {
                [SerializeField] private RaftStorageData.StorageItemData smeltItem = new(string.Empty, 0); 
                [SerializeField] private RaftStorageData.StorageItemData fuelItem = new(string.Empty, 0);
                [SerializeField] private RaftStorageData.StorageItemData resultItem = new(string.Empty, 0);
                [SerializeField] private int progressTicks;
                [SerializeField] private int fuelTicks;
                [SerializeField] private int maxFuelTicks;

                public RaftFurnace(string uid, RaftStorage.StorageItem furnaceSmeltedItem, RaftStorage.StorageItem furnaceFuelItem, RaftStorage.StorageItem furnaceResultItem, int furnaceProgressionTicks, int furnaceFuelTicks, int furnaceMaxFuelTicks)
                {
                    raftUid = uid;
                    
                    if (furnaceSmeltedItem.Item != null)
                    {
                        smeltItem = new RaftStorageData.StorageItemData(furnaceSmeltedItem.Item.ID, furnaceSmeltedItem.Count);
                    }

                    if (furnaceFuelItem.Item != null)
                    {
                        fuelItem = new RaftStorageData.StorageItemData(furnaceFuelItem.Item.ID, furnaceFuelItem.Count);
                    }

                    if (furnaceResultItem.Item != null)
                    {
                        resultItem = new RaftStorageData.StorageItemData(furnaceResultItem.Item.ID, furnaceResultItem.Count);
                    }

                    progressTicks = furnaceProgressionTicks;
                    fuelTicks = furnaceFuelTicks;
                    maxFuelTicks = furnaceMaxFuelTicks;
                }

                public int MaxFuelTicks => maxFuelTicks;

                public int FuelTicks => fuelTicks;

                public int ProgressTicks => progressTicks;

                public RaftStorageData.StorageItemData ResultItem => resultItem;

                public RaftStorageData.StorageItemData FuelItem => fuelItem;

                public RaftStorageData.StorageItemData SmeltItem => smeltItem;
            }
            
            
            [Serializable]
            public class RaftWaterSource : RaftAdditionalData
            {
                [SerializeField] private int currentTicks;
                [SerializeField] private int stack;


                public RaftWaterSource(string uid, int currentTicks, int stack)
                {
                    this.raftUid = uid;
                    this.currentTicks = currentTicks;
                    this.stack = stack;
                }

                public int Stack => stack;

                public int CurrentTicks => currentTicks;
            }
            
            [SerializeField] private List<RaftData> rafts = new List<RaftData>();
            [SerializeField] private List<RaftStorageData> storages = new List<RaftStorageData>();
            [SerializeField] private List<RaftFurnace> furnaces = new List<RaftFurnace>();
            [SerializeField] private List<RaftWaterSource> waterSources = new List<RaftWaterSource>();
            [SerializeField] private List<RaftCraft> raftsInBuild = new List<RaftCraft>();

            public List<RaftCraft> RaftsInBuild => raftsInBuild;

            public List<RaftStorageData> Storages => storages;

            public List<RaftData> Rafts => rafts;

            public int RaftsCount => rafts.Count;

            public List<RaftFurnace> Furnaces => furnaces;

            public List<RaftWaterSource> WaterSources => waterSources;

            public void AddSpawnedRaft(RaftBase spawnedRaft)
            {
                rafts.Add(
                    new RaftData(
                        spawnedRaft.Uid,
                        spawnedRaft.Coords,
                        spawnedRaft.Health,
                        spawnedRaft.RaftType
                    ));
                
                
                ConfigureStorage();
                ConfigureBuildRaft();
                ConfigureFurnaceRaft();
                ConfigureWaterSources();
                
                void ConfigureStorage()
                {
                    RaftStorage raftStorage = spawnedRaft.GetComponent<RaftStorage>();
                    if (raftStorage != null)
                    {
                        List<RaftStorageData.StorageItemData> raftStorages = new List<RaftStorageData.StorageItemData>();


                        for (int i = 0; i < raftStorage.Items.Count; i++)
                        {
                            if (raftStorage.Items[i].Item != null)
                            {
                                raftStorages.Add(new RaftStorageData.StorageItemData(raftStorage.Items[i].Item.ID, raftStorage.Items[i].Count));
                            }
                        }

                        storages.Add(new RaftStorageData(spawnedRaft.Uid, raftStorages));
                    }
                }

                void ConfigureBuildRaft()
                {
                    var raftBuild = spawnedRaft.GetComponent<RaftBuild>();
                    if (raftBuild != null)
                    {
                        raftsInBuild.Add(new RaftCraft(spawnedRaft.Uid, raftBuild.Craft.Uid, raftBuild.Time));
                    }
                }
                
                void ConfigureFurnaceRaft()
                {
                    var furnace = spawnedRaft.GetComponent<Furnace>();
                    if (furnace != null)
                    {
                        furnaces.Add(new RaftFurnace(furnace.GetComponent<RaftBase>().Uid, furnace.SmeltedItem, furnace.FuelItem, furnace.ResultItem, furnace.ProgressionTicks, furnace.FuelTicks, furnace.MaxFuelTicks));
                    }
                }
                
                void ConfigureWaterSources()
                {
                    var source = spawnedRaft.GetComponent<RestackableSource>();
                    if (source != null)
                    {
                        WaterSources.Add(new RaftWaterSource(source.GetComponent<RaftBase>().Uid, source.TicksTimer, source.Stack));
                    }
                }
            }

            
        }

        [Serializable]
        public class MapData
        {
            [Serializable]
            public class IslandData
            {
                [Serializable]
                public class DroppedItemData
                {
                    [SerializeField] private Vector3 pos;
                    [SerializeField] private Vector3 rot;
                    [SerializeField] private string itemID;
                    [SerializeField] private string dropID;

                    public DroppedItemData(Vector3 pos, Vector3 rot, string itemID, string dropID)
                    {
                        this.pos = pos;
                        this.rot = rot;
                        this.itemID = itemID;
                        this.dropID = dropID;
                    }

                    public DroppedItemData()
                    {
                        
                    }

                    public string ItemID => itemID;
                    public Vector3 Rot => rot;
                    public Vector3 Pos => pos;
                    public string DropID => dropID;
                }

                [Serializable]
                public class VillageData
                {
                    [Serializable]
                    public class VillagerData
                    {
                        [SerializeField] private string uid;
                        [SerializeField] private bool isDead;

                        public VillagerData(string uid)
                        {
                            this.uid = uid;
                        }

                        public bool IsDead => isDead;

                        public string Uid => uid;
                    }
                    [Serializable]
                    public class SlaveData
                    {
                        [SerializeField] private string uid;

                        public SlaveData(string uid)
                        {
                            this.uid = uid;
                        }

                        public string Uid => uid;
                    }

                    [SerializeField] private string uid;
                    [SerializeField] private int socialRating = 0;
                    [SerializeField] private List<VillagerData> villagers = new List<VillagerData>();
                    [SerializeField] private List<SlaveData> slaves = new List<SlaveData>();
                    public event Action<int> OnChangeSocialRaiting;
                    
                    
                    public string Uid => uid;

                    public int SocialRating => socialRating;


                    public VillageData(string villageUid)
                    {
                        uid = villageUid;
                    }


                    public void AddVillager(string uid) => villagers.Add(new VillagerData(uid));

                    public VillagerData GetVillager(string uid) => villagers.Find(x => x.Uid == uid);

                    public void AddSlave(Character ch) => slaves.Add(new SlaveData(ch.Uid));
                    

                    public void AddSocialRating(int value)
                    {
                        socialRating += value;
                        OnChangeSocialRaiting?.Invoke(socialRating);
                    }

                    public System.Random GetRandom()
                    {
                        return new System.Random(Uid.GetHashCode());
                    }

                    public bool IsHaveSlave(string characterUid)
                    {
                        return slaves.Find(x => x.Uid == characterUid) != null;
                    }
                }

                [SerializeField] private string islandName;
                [SerializeField] private Vector2Int islandPos;
                [SerializeField] private int islandSeed;
                [SerializeField] private List<Vector2Int> removedTrees = new List<Vector2Int>();
                [SerializeField] private List<DroppedItemData> droppedItems = new List<DroppedItemData>();
                [SerializeField] private List<VillageData> villagesData = new List<VillageData>();
                
                public IslandData(Vector2Int islandPos, int islandSeed)
                {
                    this.islandPos = islandPos;
                    this.islandSeed = islandSeed;
                }

                public int IslandSeed => islandSeed;

                public Vector2Int IslandPos => islandPos;

                public List<DroppedItemData> DroppedItems => droppedItems;

                public List<VillageData> VillagesData => villagesData;

                public string IslandName => islandName;


                public void AddDestroyedTreePos(Vector2Int pos)
                {
                    removedTrees.Add(pos);
                }

                public void SetIslandName(string str)
                {
                    islandName = str;
                }

                public bool IsTreeDestroyed(Vector2Int intPos)
                {
                    return removedTrees.Contains(intPos);
                }

                public void AddDroppedItem(DroppedItem item)
                {
                    if (droppedItems.Find(x => x.DropID == item.DropID) == null)
                    {
                        var data = new DroppedItemData(item.transform.position, item.transform.eulerAngles, item.Item.ID, item.DropID);
                        droppedItems.Add(data);
                    }
                }

                public void RemoveDroppedItem(DroppedItem droppedItem)
                {
                    droppedItems.RemoveAll(x=>x.DropID == droppedItem.DropID);
                }

                public VillageData AddVillage(string villageUid)
                {
                    var village = VillagesData.Find(x => x.Uid == villageUid);
                    if (village == null)
                    {
                        village = new VillageData(villageUid);
                        VillagesData.Add(village);
                    }

                    return village;
                }

                public VillageData GetVillage(string uid) => villagesData.Find(x => x.Uid == uid);

                public bool HasVillage() => villagesData.Count != 0;
            }

            [SerializeField] private int worldSeed = 0;
            [SerializeField] private List<IslandData> islands;


            public bool IsGenerated => WorldSeed != 0;
            public int WorldSeed => worldSeed;

            public List<IslandData> Islands => islands;


            public void SetSeed(GameDataObject gameDataObject)
            {
                worldSeed = Random.Range(Int32.MinValue, Int32.MaxValue);
                if (worldSeed == 0)
                {
                    SetSeed(gameDataObject);
                    return;
                }

                islands = MapNoiseGenerator.GetIslandPoints(worldSeed, gameDataObject.MapPaths, gameDataObject.ConfigData.MapNoisePreset);
            }

            public IslandData GetIslandData(int seed)
            {
                return islands.Find(x => x.IslandSeed == seed);
            }
        }
        
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
            [SerializeField] private RaftDamagerData damagersData = new RaftDamagerData(0,0, new List<RaftDamagerData.SpawnedItem>());
            [SerializeField] private WeatherData weathersData = new WeatherData(0, -1, WeatherService.EWeatherType.Сalm);

            public bool isOnIsland => IslandSeed != 0;
            
            public float TotalSecondsInGame => totalSecondsInGame;

            public RaftDamagerData DamagersData => damagersData;

            public WeatherData WeathersData => weathersData;

            public int IslandSeed => islandSeed;

            public float TotalSecondsOnRaft => totalSecondsOnRaft;

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
        }
        
        [Serializable]
        public class TutorialsData
        {
            [SerializeField] private bool clickTutorial;
            [SerializeField] private bool eatTutorial;
            [SerializeField] private bool storageTutorial;
            [SerializeField] private bool levelUpTutorial;

            public bool StorageTutorial => storageTutorial;

            public bool EatTutorial => eatTutorial;

            public bool ClickTutorial => clickTutorial;

            public bool LevelUpTutorial => levelUpTutorial;


            public void ClickTutorialSet()
            {
                clickTutorial = true;
                
                Debug.Log("Complete Action Tutorial");
            }
            
            public void EatTutorialSet()
            {
                eatTutorial = true;
                Debug.Log("Complete Eat Tutorial");
            }
            
            public void StorageTutorialSet()
            {
                storageTutorial = true;
                Debug.Log("Complete Storage Tutorial");
            }
            
            public void LevelUpTutorialSet()
            {
                levelUpTutorial = true;
                Debug.Log("Complete LevelUp Tutorial");
            }
        }
        
        
        [SerializeField] private CharactersData charactersData = new CharactersData();
        [SerializeField] private RaftsData raftsData = new RaftsData();
        [SerializeField] private MapData mapData = new MapData();
        [SerializeField] private GlobalData globalData = new GlobalData();
        [SerializeField] private TutorialsData tutorialsData = new TutorialsData();
        


        public CharactersData Characters => charactersData;

        public RaftsData Rafts => raftsData;

        public MapData Map => mapData;

        public GlobalData Global => globalData;

        public TutorialsData Tutorials => tutorialsData;


        public override void InstallBindings()
        {
            Container.Bind<SaveDataObject>().FromInstance(this).AsSingle();
            LoadFile();
        }
        
        public MapData.IslandData GetTargetIsland()
        {
            return mapData.GetIslandData(globalData.IslandSeed);
        }

        protected string GetPathFolder()
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE_WIN
            return Directory.GetParent(Application.dataPath)?.FullName;
#endif
#if UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return null;
#endif
        }

        protected virtual string GetFilePath() => GetPathFolder() + @"\data.dat";
        
        [Button]
        public virtual void SaveFile()
        {

            string json = "";
            
#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            if (!string.IsNullOrEmpty(GetPathFolder()))
            {
                json = JsonUtility.ToJson(this);
                File.WriteAllText(GetFilePath(), json, Encoding.Unicode);
            }
            else
            {
                Debug.LogError("Path error!");
            }
#endif
#if UNITY_WEBGL
            json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(PrefsSaveKey, json);
#endif
        }

        [Button]
        public virtual void LoadFile()
        {
#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            var file = GetFilePath();
            if (File.Exists(file))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(file), this);
                return;
            }
#endif
#if UNITY_WEBGL
            var json = PlayerPrefs.GetString(PrefsSaveKey);
            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
                return;
            }
#endif
            
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(CreateInstance<SaveDataObject>()), this);
            SaveFile();

        }

        [Button]
        public void DeleteFile()
        {

#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            var file = GetFilePath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
#endif
#if UNITY_WEBGL
            PlayerPrefs.DeleteAll();
#endif
            LoadFile();
        }

        [Button]
        public void OpenFile()
        {
            Application.OpenURL(Directory.GetParent(GetFilePath())?.FullName);
        }

        public void SetRaftsData(RaftsData newRaftsData)
        {
            raftsData = newRaftsData;
        }
    }
    
}