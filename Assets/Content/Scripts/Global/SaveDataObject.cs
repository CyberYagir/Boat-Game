using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.ManCreator;
using Content.Scripts.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;
using Range = DG.DemiLib.Range;

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
                public class Storage
                {
                    [SerializeField] private EResourceTypes resourceType;
                    [SerializeField] private List<StorageItem> itemList;

                    public Storage(EResourceTypes resourceType, List<StorageItem> itemList)
                    {
                        this.resourceType = resourceType;
                        this.itemList = itemList;
                    }

                    public List<StorageItem> ItemList => itemList;

                    public EResourceTypes ResourceType => resourceType;
                }
                
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
                [SerializeField] private List<Storage> storagesData;


                public RaftStorage(string raftUid, List<Storage> storagesData)
                {
                    this.raftUid = raftUid;
                    this.storagesData = storagesData;
                }

                public List<Storage> StoragesData => storagesData;

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
                    List<RaftStorage.Storage> raftStorages = new List<RaftStorage.Storage>();


                    for (int i = 0; i < raftStorage.Items.Count; i++)
                    {
                        List<RaftStorage.StorageItem> items = new List<RaftStorage.StorageItem>();

                        for (int j = 0; j < raftStorage.Items[i].ItemObjects.Count; j++)
                        {
                            items.Add(new RaftStorage.StorageItem(raftStorage.Items[i].ItemObjects[j].Item.ID, raftStorage.Items[i].ItemObjects[j].Count));
                        }

                        raftStorages.Add(new RaftStorage.Storage(raftStorage.Items[i].ResourcesType, items));
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
                [SerializeField] private Vector2Int islandPos;
                [SerializeField] private int islandSeed;

                public IslandData(Vector2Int islandPos, int islandSeed)
                {
                    this.islandPos = islandPos;
                    this.islandSeed = islandSeed;
                }

                public int IslandSeed => islandSeed;

                public Vector2Int IslandPos => islandPos;
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
        }
        
        [System.Serializable]
        public class GlobalData
        {
            [System.Serializable]
            public class RaftDamager
            {
                [System.Serializable]
                public class SpawnedItem
                {
                    [SerializeField] private int itemIndex;
                    [SerializeField] private string raftID;
                }
                [SerializeField] private int tickCount, maxTickCount;
                [SerializeField] private List<SpawnedItem> spawnedItems = new List<SpawnedItem>();
                
                

            }
            [SerializeField] private float totalSecondsInGame;
            [SerializeField] private RaftDamager damagers = new RaftDamager();
            
            public float TotalSecondsInGame => totalSecondsInGame;

            public void SetTimePlayed(float value)
            {
                totalSecondsInGame = value;
            }

            public void AddTime(float value)
            {
                totalSecondsInGame += value;
            }
        }
        
        [SerializeField] private CharactersData charactersData = new CharactersData();
        [SerializeField] private RaftsData raftsData = new RaftsData();
        [SerializeField] private MapData mapData = new MapData();
        [SerializeField] private GlobalData globalData = new GlobalData();
        public CharactersData Characters => charactersData;

        public RaftsData Rafts => raftsData;

        public MapData Map => mapData;

        public GlobalData Global => globalData;

        public override void InstallBindings()
        {
            Container.Bind<SaveDataObject>().FromInstance(this).AsSingle();
            LoadFile();
        }

        protected string GetPathFolder()
        {
#if UNITY_EDITOR
            return Directory.GetParent(Application.dataPath)?.FullName;
#endif
#if UNITY_ANDROID
            return Application.persistentDataPath;
#endif

        }

        protected virtual string GetFilePath() => GetPathFolder() + @"\data.dat";
        
        [Button]
        public void SaveFile()
        {
            if (!string.IsNullOrEmpty(GetPathFolder()))
            {
                var json = JsonUtility.ToJson(this);
                File.WriteAllText(GetFilePath(), json, Encoding.Unicode);
            }
            else
            {
                Debug.LogError("Path error!");
            }
        }

        [Button]
        public virtual void LoadFile()
        {
            var file = GetFilePath();
            if (File.Exists(file))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(file), this);
            }
            else
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(ScriptableObject.CreateInstance<SaveDataObject>()), this);
                SaveFile();
            }
        }
        
        [Button]
        public void DeleteFile()
        {
            var file = GetFilePath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            LoadFile();
        }
        [Button]
        public void OpenFile()
        {
            Application.OpenURL(Directory.GetParent(GetFilePath()).FullName);
        }

        public void SetRaftsData(RaftsData newRaftsData)
        {
            raftsData = newRaftsData;
        }
    }
}