using System;
using System.Collections.Generic;
using System.Globalization;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame;
using Content.Scripts.IslandGame.Scriptable;
using Content.Scripts.Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
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
                        [System.Serializable]
                        public class ActivitySkill
                        {
                            public const int MAX_MODIFY = 3;
                            
                            [SerializeField] private string activityID;
                            [SerializeField] private float modify = 1;
                            [SerializeField] private bool isActive;

                            public ActivitySkill(string activityID)
                            {
                                this.activityID = activityID;
                                isActive = true;
                            }

                            public float Modify => modify;

                            public string ActivityID => activityID;

                            public bool IsActive => isActive;

                            public void ToggleActive()
                            {
                                isActive = !isActive;
                            }

                            public void AddSkill(float add)
                            {
                                modify += add;
                                if (modify > MAX_MODIFY)
                                {
                                    modify = MAX_MODIFY;
                                }
                            }
                        }
                        
                        [System.Serializable]
                        public class TransferData
                        {
                            public enum ETransferState
                            {
                                None,
                                SendFromIsland,
                                NewOnIsland,
                                Hired
                            }
                            
                            [SerializeField] private int seed, islandLevel;
                            [SerializeField] private ETransferState transferState = ETransferState.None;
                            public TransferData(int seed, int islandLevel)
                            {
                                this.seed = seed;
                                this.islandLevel = islandLevel;
                            }
                            
                            public  void SetTransferState(ETransferState state) => transferState = state;

                            public int IslandLevel => islandLevel;

                            public int Seed => seed;

                            public ETransferState TransferState => transferState;
                        }

                        [SerializeField] private string uid;
                        [SerializeField] private string lastTimeStamp;
                        [SerializeField] private float targetStamina = 100f;
                        [SerializeField] private bool isWorking;
                        [SerializeField] private bool isStorage;
                        
                        [SerializeField] private bool isDead;
                        [SerializeField] private TransferData transferData = new TransferData(0, 0);
                        [SerializeField] private List<ActivitySkill> activities = new List<ActivitySkill>();
                        [SerializeField] private List<RaftsData.RaftStorageData.StorageItemData> storageItems = new List<RaftsData.RaftStorageData.StorageItemData>();

                        public DateTime LastTimeStamp => DateTime.Parse(LastTimeStampString, CultureInfo.InvariantCulture);
                        
                        
                        public SlaveData(string uid, TransferData transferData)
                        {
                            this.uid = uid;
                            this.transferData = transferData;
                            lastTimeStamp = DateService.ActualDateString;
                        }

                        public string Uid => uid;

                        public bool IsWorking => isWorking;

                        public bool IsStorage => isStorage;

                        public float TargetStamina => targetStamina;

                        public List<ActivitySkill> Activities => activities;

                        public string LastTimeStampString => lastTimeStamp;

                        public List<RaftsData.RaftStorageData.StorageItemData> StorageItems => storageItems;

                        public bool IsDead => isDead;

                        public TransferData TransferInfo => transferData;


                        public void Kill()
                        {
                            isDead = true;
                            activities.Clear();
                            storageItems.Clear();
                        }

                        public bool HasActivity(string slaveDataActivity)
                        {
                            var find = Activities.Find(x => x.ActivityID == slaveDataActivity);
                            if (find == null) return false;


                            return find.IsActive;
                        }

                        public void WorkToggle()
                        {
                            if (targetStamina <= 0 || Activities.Count == 0)
                            {
                                lastTimeStamp = DateService.ActualDateString;
                                isWorking = false;
                                return;
                            }
                            isWorking = !isWorking;
                            if (isWorking)
                            {
                                lastTimeStamp = DateService.ActualDateString;
                            }
                        }

                        public void SetStamina(float newStamina)
                        {
                            targetStamina = Mathf.Clamp(newStamina,0,100);
                        }

                        public void AddStamina(float eatPoints)
                        {
                            SetStamina(targetStamina + eatPoints);
                            
                        }

                        public void ToggleActivity(SlaveActivitiesObject activity)
                        {
                            ToggleActivity(activity.Uid);
                        }

                        private void ToggleActivity(string uid)
                        {
                            SlaveActivitiesObject activity;
                            var finded = activities.Find(x => x.ActivityID == uid);
                            if (finded == null)
                            {
                                activities.Add(new ActivitySkill(uid));
                            }
                            else
                            {
                                finded.ToggleActive();
                            }
                        }

                        public void SetStorage(bool b)
                        {
                            isStorage = b;
                        }

                        public void AddItemsToStorage(List<RaftStorage.StorageItem> items)
                        {
                            foreach (var it in items)
                            {
                                var savedItems = storageItems.Find(x => x.ItemID == it.Item.ID);
                                if (savedItems != null)
                                {
                                    savedItems.Add(it.Count);
                                }
                                else
                                {
                                    storageItems.Add(new RaftsData.RaftStorageData.StorageItemData(it.Item.ID, it.Count));
                                }
                            }
                        }

                        public bool RemoveItem(RaftStorage.StorageItem availableItem)
                        {
                            var find = storageItems.Find(x => x.ItemID == availableItem.Item.ID);

                            if (find != null)
                            {
                                find.Add(-availableItem.Count);
                                if (find.Count <= 0)
                                {
                                    storageItems.Remove(find);
                                }
                                return true;
                            }
                            return false;
                        }

                        public void ClearStorageItems()
                        {
                            storageItems.Clear();
                        }

                        public void AddToSkills(Dictionary<string,float> skillsData)
                        {
                            foreach (var activity in activities)
                            {
                                if (skillsData.ContainsKey(activity.ActivityID))
                                {
                                    activity.AddSkill(skillsData[activity.ActivityID]);
                                }
                            }
                        }

                        public void SetActivityData(List<ActivitySkill> slaveDataActivities)
                        {
                            activities = slaveDataActivities;
                        }

                        public void SetSlaveConvert()
                        {
                            transferData.SetTransferState(TransferData.ETransferState.Hired);
                        }
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

                    public SlaveData AddSlave(Character ch, SlaveData.TransferData transferData)
                    {
                        var slave = new SlaveData(ch.Uid, transferData);
                        slaves.Add(slave);
                        return slave;
                    }


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

                    public int SlavesCount() => slaves.Count;

                    public SlaveData GetSlave(string slaveId)
                    {
                        return slaves.Find(x => x.Uid == slaveId);
                    }

                    public void KillSlave(string slaveDataUid)
                    {
                        GetSlave(slaveDataUid).Kill();
                    }

                    public SlaveData GetSlaveByID(int i)
                    {
                        if (i >= slaves.Count) return null;
                        return slaves[i];
                    }

                    public List<SlaveData> GetTrasferedSlaves()
                    {
                        return slaves.FindAll(x => x.TransferInfo.TransferState == SlaveData.TransferData.ETransferState.NewOnIsland);
                    }

                    public void RemoveSlave(string slaveDataUid)
                    {
                        slaves.RemoveAll(x => x.Uid == slaveDataUid);
                    }
                }

                [SerializeField] private string islandName;
                [SerializeField] private Vector2Int islandPos;
                [SerializeField] private int islandSeed;
                [SerializeField] private List<Vector2Int> removedTrees = new List<Vector2Int>();
                [SerializeField] private List<DroppedItemData> droppedItems = new List<DroppedItemData>();
                [SerializeField] private List<VillageData> villagesData = new List<VillageData>();
                [SerializeField] private List<string> pillagersKilled = new List<string>();

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


                public bool IsPillagerDead(string id)
                {
                    return pillagersKilled.Contains(id);
                }

                public void AddDeadPillager(string id)
                {
                    if (!IsPillagerDead(id))
                    {
                        pillagersKilled.Add(id);
                    }
                }

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

                public void AddDroppedItem(DroppedItemBase item)
                {
                    if (droppedItems.Find(x => x.DropID == item.DropID) == null)
                    {
                        var data = new DroppedItemData(item.transform.position, item.transform.eulerAngles, item.StorageItem.Item.ID, item.DropID);
                        droppedItems.Add(data);
                    }
                }

                public void RemoveDroppedItem(DroppedItemBase droppedItem)
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
            
            [System.Serializable]
            public class PlotItem
            {
                [SerializeField] private int islandIDs;
                [SerializeField] private int plotRowNumber;
                [SerializeField] private bool isCollected;

                public PlotItem(int islandIDs, int plotRowNumber)
                {
                    this.islandIDs = islandIDs;
                    this.plotRowNumber = plotRowNumber;
                }

                public void SetCollected() => isCollected = true;

                public int PlotRowNumber => plotRowNumber;

                public int IslandIDs => islandIDs;
                public bool Collected => isCollected;
            }

            
            [SerializeField] private int worldSeed = 0;
            [SerializeField] private List<IslandData> islands = new List<IslandData>();
            [SerializeField] private List<PlotItem> plotParts = new List<PlotItem>();

            public bool IsGenerated => WorldSeed != 0;
            public int WorldSeed => worldSeed;

            public List<IslandData> Islands => islands;

            public List<PlotItem> PlotParts => plotParts;

            public void GenerateWorld(GameDataObject gameDataObject)
            {
                worldSeed = Random.Range(Int32.MinValue, Int32.MaxValue);
                if (worldSeed == 0)
                {
                    GenerateWorld(gameDataObject);
                    return;
                }

                while (islands.Count < 40 || islands.Count > 140)
                {
                    worldSeed = Random.Range(Int32.MinValue, Int32.MaxValue);
                    islands = MapNoiseGenerator.GetIslandPoints(worldSeed, gameDataObject.MapPaths, gameDataObject.ConfigData.MapNoisePreset);
                }


                GeneratePlotData(gameDataObject);
            }

            private void GeneratePlotData(GameDataObject gameDataObject)
            {
                var lines = gameDataObject.PlotLines.LinesToList();
                var rnd = new System.Random(WorldSeed);
                var plotPartsCount = islands.Count/gameDataObject.ConfigData.PlotPerIslands;

                int linesCounter = 0;
                
                for (int i = 0; i < plotPartsCount; i++)
                {
                    var island = Islands.GetRandomItem(rnd);
                        
                    while (PlotParts.Find(x => x.IslandIDs == island.IslandSeed) != null)
                    {
                        island = Islands.GetRandomItem(rnd);
                    }
                    
                    PlotParts.Add(new PlotItem(island.IslandSeed, linesCounter));
                    linesCounter++;
                    if (linesCounter >= lines.Count) linesCounter = 0;
                }
            }

            public IslandData GetIslandData(int seed)
            {
                return islands.Find(x => x.IslandSeed == seed);
            }

            public bool IsHavePlotOnIsland(int islandSeed)
            {
                return PlotParts.Find(x => x.IslandIDs == islandSeed) != null;
            }

            public int GetPlotBySeed(int islandSeed)
            {
                var item = plotParts.Find(x => x.IslandIDs == islandSeed);
                return item?.PlotRowNumber ?? -1;
            }
            public PlotItem GetPlotDataBySeed(int islandSeed)
            {
                var item = plotParts.Find(x => x.IslandIDs == islandSeed);
                return item;
            }
        }
    }
}