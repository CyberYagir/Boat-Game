using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.IslandGame.Sources;
using UnityEngine;

namespace Content.Scripts.Global
{
    public partial class SaveDataObject
    {
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

                    public void Add(int itCount)
                    {
                        count += itCount;
                    }
                }

                [SerializeField] private List<StorageItemData> storagesData;


                public RaftStorageData(string raftUid, List<StorageItemData> storagesData)
                {
                    this.raftUid = raftUid;
                    this.storagesData = storagesData;
                }

                public List<StorageItemData> StoragesData => storagesData;

                public void AddToStorage(List<RaftStorage.StorageItem> items)
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        var it = storagesData.Find(x => x.ItemID == items[i].Item.ID);
                        if (it != null)
                        {
                            it.Add(it.Count);
                        }
                        else
                        {
                            storagesData.Add(new StorageItemData(items[i].Item.ID, items[i].Count));
                        }
                    }
                }

                public bool RemoveFromStorage(RaftStorage.StorageItem item)
                {
                    var it = storagesData.Find(x => x.ItemID == item.Item.ID);
                    if (it != null)
                    {
                        it.Add(-item.Count);

                        if (it.Count <= 0)
                        {
                            storagesData.Remove(it);
                        }

                        return true;
                    }

                    return false;
                }
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
                
                
                ConfigureStorage(spawnedRaft);
                ConfigureBuildRaft();
                ConfigureFurnaceRaft();
                ConfigureWaterSources();
                
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

            public void ClearStoragesData()
            {
                storages.Clear();
            }
            public void SetSpawnedStorage(RaftBase spawnedRaft)
            {
                ConfigureStorage(spawnedRaft);
            }
            
            void ConfigureStorage(RaftBase spawnedRaft)
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
        }
    }
}