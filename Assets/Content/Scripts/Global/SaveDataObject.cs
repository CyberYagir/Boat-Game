using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.RaftDamagers;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
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
        [System.Serializable]
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
        
        [System.Serializable]
        public class RaftsData
        {
            [System.Serializable]
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

            [System.Serializable]
            public class RaftStorage
            {
                [System.Serializable]
                public class StorageItem
                {
                    [SerializeField] private string itemID;
                    [SerializeField] private int count;

                    public StorageItem(string itemID, int count)
                    {
                        this.itemID = itemID;
                        this.count = count;
                    }

                    public int Count => count;

                    public string ItemID => itemID;
                }
                [SerializeField] private string raftUid;
                [SerializeField] private List<StorageItem> storagesData;


                public RaftStorage(string raftUid, List<StorageItem> storagesData)
                {
                    this.raftUid = raftUid;
                    this.storagesData = storagesData;
                }

                public List<StorageItem> StoragesData => storagesData;

                public string RaftUid => raftUid;
            }
            
            [System.Serializable]
            public class RaftCraft
            {
                [SerializeField] private string raftUid;
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

                public string RaftUid => raftUid;
            }
            
            [SerializeField] private List<RaftData> rafts = new List<RaftData>();
            [SerializeField] private List<RaftStorage> storages = new List<RaftStorage>();
            [SerializeField] private List<RaftCraft> raftsInBuild = new List<RaftCraft>();

            public List<RaftCraft> RaftsInBuild => raftsInBuild;

            public List<RaftStorage> Storages => storages;

            public List<RaftData> Rafts => rafts;

            public int RaftsCount => rafts.Count;

            public void AddSpawnedRaft(RaftBase spawnedRaft)
            {
                rafts.Add(
                    new RaftData(
                        spawnedRaft.Uid,
                        spawnedRaft.Coords,
                        spawnedRaft.Health,
                        spawnedRaft.RaftType
                    ));
                
                
                BoatGame.RaftStorage raftStorage = spawnedRaft.GetComponent<BoatGame.RaftStorage>();
                if (raftStorage != null)
                {
                    List<RaftStorage.StorageItem> raftStorages = new List<RaftStorage.StorageItem>();


                    for (int i = 0; i < raftStorage.Items.Count; i++)
                    {
                        if (raftStorage.Items[i].Item != null)
                        {
                            raftStorages.Add(new RaftStorage.StorageItem(raftStorage.Items[i].Item.ID, raftStorage.Items[i].Count));
                        }
                    }

                    storages.Add(new RaftStorage(spawnedRaft.Uid, raftStorages));
                }

                var raftBuild = spawnedRaft.GetComponent<RaftBuild>();
                if (raftBuild != null)
                {
                    raftsInBuild.Add(new RaftCraft(spawnedRaft.Uid, raftBuild.Craft.Uid, raftBuild.Time));
                }
            }
        }

        [System.Serializable]
        public class MapData
        {
            [System.Serializable]
            public class IslandData
            {
                [System.Serializable]
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

                [SerializeField] private Vector2Int islandPos;
                [SerializeField] private int islandSeed;
                [SerializeField] private List<Vector2Int> removedTrees = new List<Vector2Int>();
                [SerializeField] private List<DroppedItemData> droppedItems = new List<DroppedItemData>();

                public IslandData(Vector2Int islandPos, int islandSeed)
                {
                    this.islandPos = islandPos;
                    this.islandSeed = islandSeed;
                }

                public int IslandSeed => islandSeed;

                public Vector2Int IslandPos => islandPos;

                public List<DroppedItemData> DroppedItems => droppedItems;


                public void AddDestroyedTreePos(Vector2Int pos)
                {
                    removedTrees.Add(pos);
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

                islands = MapNoiseGenerator.GetIslandPoints(worldSeed, gameDataObject.MapPaths);
            }

            public IslandData GetIslandData(int seed)
            {
                return islands.Find(x => x.IslandSeed == seed);
            }
        }
        
        [System.Serializable]
        public class GlobalData
        {
            [System.Serializable]
            public class RaftDamagerData
            {
                [System.Serializable]
                public class SpawnedItem
                {
                    [SerializeField] private int itemIndex;
                    [SerializeField] private string raftID;
                    [SerializeField] private List<RaftDamager.RaftDamagerDataKey> keys = new List<BoatGame.RaftDamagers.RaftDamager.RaftDamagerDataKey>(); 
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
            
            [System.Serializable]
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
        
        [System.Serializable]
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
                
                Debug.LogError("Complete Action Tutorial");
            }
            
            public void EatTutorialSet()
            {
                eatTutorial = true;
            }
            
            public void StorageTutorialSet()
            {
                storageTutorial = true;
            }
            
            public void LevelUpTutorialSet()
            {
                levelUpTutorial = true;
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
            
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(ScriptableObject.CreateInstance<SaveDataObject>()), this);
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