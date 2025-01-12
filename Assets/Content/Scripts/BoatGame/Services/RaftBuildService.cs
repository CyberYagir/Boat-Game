using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.BoatGame.Characters;
using Content.Scripts.BoatGame.Characters.States;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.IslandGame.Sources;
using Content.Scripts.ItemsSystem;
using Content.Scripts.QuestsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class RaftBuildService : MonoBehaviour, IRaftBuildService
    {
        [Serializable]
        public class RaftItem
        {
            public enum ERaftType
            {
                Default,
                Storage,
                Building,
                CraftTable,
                Moored,
                Fishing,
                Furnace,
                WaterSource
            }

            [SerializeField] private ERaftType raftType;
            [SerializeField] private RaftBase raft;
            [SerializeField] private CraftObject craftObject;

            public RaftBase Raft => raft;
            public ERaftType Type => raftType;

            public CraftObject CraftObject => craftObject;
        }

        [SerializeField] private List<RaftItem> rafts;
        [SerializeField] private List<RaftBase> spawnedRafts;
        [SerializeField] private Transform holder;
        [SerializeField] private RaftTapToBuild tapToBuildRaftPrefab;
        [SerializeField] private RaftTapToBuild tapToRemoveRaftPrefab;
        [SerializeField] private RaftNodesChecker raftNodeSystem;
        [SerializeField] private IResourcesService resourcesService;
        
        [SerializeField, ReadOnly] private List<RaftStorage> storages = new List<RaftStorage>();
        [SerializeField, ReadOnly] private Transform raftEndPoint;
        
        private WorldGridService worldGridService;
        private SaveDataObject saveData;
        private GameDataObject gamedata;
        private PrefabSpawnerFabric prefabSpawnerFabric;
        private SelectionService selectionService;
        private GameStateService gameStateService;
        
        private List<RaftTapToBuild> spawnedTapBuildRafts = new List<RaftTapToBuild>();
        private CraftObject lastSelectedCraftItem;        
        
        public event Action OnChangeRaft;
        public List<RaftBase> SpawnedRafts => spawnedRafts;
        public List<RaftStorage> Storages => storages;

        public Transform Holder => holder;

        public Transform RaftEndPoint => raftEndPoint;

        public IResourcesService ResourcesService => resourcesService;
        

        [Inject]
        private void Construct(
            WorldGridService worldGridService,
            SaveDataObject saveData,
            GameStateService gameStateService,
            SelectionService selectionService,
            GameDataObject gamedata,
            INavMeshProvider navMeshProvider,
            PrefabSpawnerFabric prefabSpawnerFabric,
            IResourcesService resourcesService
        )
        {
            this.resourcesService = resourcesService;
            this.prefabSpawnerFabric = prefabSpawnerFabric;
            this.gamedata = gamedata;
            this.gameStateService = gameStateService;
            this.selectionService = selectionService;
            this.saveData = saveData;
            this.worldGridService = worldGridService;

            if (saveData.Rafts.RaftsCount == 0)
            {
                SpawnStartRaft();
            }
            else
            {
                LoadPlayerRaft(saveData, selectionService, gamedata);
            }

            gameStateService.OnChangeEState += GameStateServiceOnOnChangeEState;

            OnChangeRaft += CheckNodesSemaphoreStart;
        }

        private void CheckNodesSemaphoreStart()
        {
            if (!isCheckNodesSemaphore)
            {
                StartCoroutine(CheckNodesSemaphore());
            }
        }

        private bool isCheckNodesSemaphore = false;
        IEnumerator CheckNodesSemaphore()
        {
            if (!isCheckNodesSemaphore)
            {
                isCheckNodesSemaphore = true;
                yield return null;
                CheckNodes();
                if (gameStateService.GameState == GameStateService.EGameState.Removing)
                {
                    DestroyUslessRaftTapRemoveItems();
                }

                isCheckNodesSemaphore = false;
            }
        }
        private void CheckNodes()
        {
            print("check nodes");
            var parts = raftNodeSystem.CalculateParts(spawnedRafts, worldGridService, gamedata);
            if (parts.Count > 1)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i].CalculatePriorityIndex();
                }

                var minPriority = parts.Min(x => x.PriorityIndex);
                var part = parts.Find(x => x.PriorityIndex == minPriority);

                if (part != null)
                {
                    part.KillRafts();
                }
            }
        }


        private void LoadPlayerRaft(
            SaveDataObject saveData, 
            SelectionService selectionService, 
            GameDataObject gamedata)
        {
            for (int i = 0; i < saveData.Rafts.Rafts.Count; i++)
            {
                var raft = saveData.Rafts.Rafts[i];
                var spawned = AddRaft(raft.Pos, raft.RaftType);

                spawned.LoadData(raft.Health, raft.Uid);

                if (spawned.RaftType == RaftItem.ERaftType.Storage)
                {
                    var storage = spawned.GetComponent<RaftStorage>();
                    if (storage)
                    {
                        storage.LoadStorage(saveData.Rafts.Storages.Find(x => x.RaftUid == raft.Uid), gamedata);
                    }
                }

                if (spawned.RaftType == RaftItem.ERaftType.Furnace)
                {
                    var furnace = spawned.GetComponent<Furnace>();
                    if (furnace)
                    {
                        furnace.LoadStorage(saveData.Rafts.Furnaces.Find(x => x.RaftUid == raft.Uid), gamedata);
                    }
                }
                
                if (spawned.RaftType == RaftItem.ERaftType.WaterSource)
                {
                    var source = spawned.GetComponent<RestackableSource>();
                    if (source)
                    {
                        source.LoadStorage(saveData.Rafts.WaterSources.Find(x => x.RaftUid == raft.Uid));
                    }
                }

                var building = spawned.GetComponent<RaftBuild>();
                if (building)
                {
                    building.LoadBuild(saveData.Rafts.RaftsInBuild.Find(x => x.RaftUid == raft.Uid), gamedata, selectionService, this);
                }
            }
            
            OnChangeRaft?.Invoke();
            
        }

        private void SpawnStartRaft()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (i == j && i == 1)
                    {
                        AddRaft(new Vector3Int(i, 0, j), RaftItem.ERaftType.Storage);
                    }
                    else
                    {
                        AddRaft(new Vector3Int(i, 0, j), RaftItem.ERaftType.Default);
                    }
                }
            }
        }

        private void GameStateServiceOnOnChangeEState(GameStateService.EGameState obj)
        {
            if (obj == GameStateService.EGameState.Building)
            {
                SpawnTapToBuildRafts();
                selectionService.OnTapOnBuildingRaft += OnTapOnBuildingRaft;
            }
            else if (obj == GameStateService.EGameState.Removing)
            {
                SpawnTapToRemoveRafts();
                selectionService.OnTapOnBuildingRaft += OnTapOnRemoveRaft;
            }
            else if (obj == GameStateService.EGameState.Normal)
            {
                foreach (var stpr in spawnedTapBuildRafts)
                {
                    Destroy(stpr.gameObject);
                }

                spawnedTapBuildRafts.Clear();
                selectionService.OnTapOnBuildingRaft -= OnTapOnBuildingRaft;
                selectionService.OnTapOnBuildingRaft -= OnTapOnRemoveRaft;
            }
        }

        private void OnTapOnRemoveRaft(RaftTapToBuild obj)
        {
            var raft = spawnedRafts.Find(x => x.Coords == obj.Coords);

            if (raft != null)
            {
                raft.Kill();

                DestroyUslessRaftTapRemoveItems();
            }
        }

        private void DestroyUslessRaftTapRemoveItems()
        {
            for (int i = 0; i < spawnedTapBuildRafts.Count; i++)
            {
                var rft = spawnedRafts.Find(x => x.Coords == spawnedTapBuildRafts[i].Coords);
                if (rft == null)
                {
                    if (spawnedTapBuildRafts[i] != null)
                    {
                        Destroy(spawnedTapBuildRafts[i].gameObject);
                    }
                }
            }

            spawnedTapBuildRafts.RemoveAll(x => x == null);
        }

        private void OnTapOnBuildingRaft(RaftTapToBuild targetRaft)
        {
            var raft = AddRaft(targetRaft.Coords, RaftItem.ERaftType.Building);
            var raftBuild = raft.GetComponent<RaftBuild>();
            raftBuild.SetCraft(lastSelectedCraftItem, selectionService, this, gamedata);
            gameStateService.ChangeGameState(GameStateService.EGameState.Normal);


            resourcesService.RemoveItemsForCraft(lastSelectedCraftItem);

            if (selectionService.SelectedCharacter != null)
            {
                selectionService.SelectedCharacter.GetCharacterAction<CharActionBuilding>().SetTargetRaft(raftBuild);
                selectionService.SelectedCharacter.ActiveAction(EStateType.Building);
            }
        }

        public RaftBase AddRaft(Vector3Int cords, RaftItem.ERaftType type)
        {
            if (worldGridService.AddPoint(cords))
            {
                var raft = rafts.Find(x => x.Type == type).Raft;
                return SpawnRaftPrefab(cords, type, raft);
            }

            return null;
        }
        
        public RaftBase AddRaft(Vector3Int cords, CraftObject craft)
        {
            if (worldGridService.AddPoint(cords))
            {
                var type = rafts.Find(x => x.CraftObject != null && x.CraftObject.Uid == craft.Uid);
                var raft = type.Raft;
                saveData.CrossGame.Statistics.AddBuildRaft();
                QuestsEventBus.CallRaftBuild(type.Type);
                return SpawnRaftPrefab(cords, type.Type, raft);
            }

            return null;
        }


        private RaftBase SpawnRaftPrefab(Vector3Int cords, RaftItem.ERaftType type, RaftBase raft)
        {
            
            var rf = prefabSpawnerFabric.SpawnItem(raft, Vector3.zero, Quaternion.identity, Holder)
                .With(x => x.Init())
                .With(x => x.SetCoords(cords))
                .With(x => SpawnedRafts.Add(x));
            rf.transform.localPosition = cords;
            if (type == RaftItem.ERaftType.Storage)
            {
                var storage = rf.GetComponent<RaftStorage>();
                if (storage)
                {
                    Storages.Add(storage);
                }
            }

            rf.OnDeath += OnRaftDeath;
            OnChangeRaft?.Invoke();
            return rf;
        }

        private void OnRaftDeath(DamageObject obj)
        {
            var rf = obj as RaftBase;
         
            if (rf != null)
            {
                RemoveRaft(rf.Coords);
                Destroy(rf.gameObject);
            }
        }

        public RaftStorage FindEmptyStorage(ItemObject item, int value)
        {
            var storage = Storages.Find(x => x.IsEmptyStorage(item, value));
            return storage;
        }


        List<Vector3Int> coords = new List<Vector3Int>();


        public void SpawnTapToBuildRafts()
        {
            coords.Clear();
            for (int i = 0; i < spawnedRafts.Count; i++)
            {
                if (spawnedRafts[i].IsWalkableRaft)
                {
                    IsEmptyCell(spawnedRafts[i].Coords + Vector3Int.forward);
                    IsEmptyCell(spawnedRafts[i].Coords + Vector3Int.back);
                    IsEmptyCell(spawnedRafts[i].Coords + Vector3Int.left);
                    IsEmptyCell(spawnedRafts[i].Coords + Vector3Int.right);
                }
            }

            for (int i = 0; i < coords.Count; i++)
            {
                var id = i;
                Instantiate(tapToBuildRaftPrefab, coords[i], Quaternion.identity, Holder)
                    .With(x => spawnedTapBuildRafts.Add(x))
                    .With(x => x.SetCoords(coords[id]))
                    .With(x => x.transform.localPosition = (Vector3) coords[id]);
            }
        }
        
        public void SpawnTapToRemoveRafts()
        {
            for (int i = 0; i < spawnedRafts.Count; i++)
            {
                var id = i;
                Instantiate(tapToRemoveRaftPrefab, spawnedRafts[id].Coords + (Vector3.up * 0.1f), Quaternion.identity, Holder)
                    .With(x => spawnedTapBuildRafts.Add(x))
                    .With(x => x.SetCoords(spawnedRafts[id].Coords))
                    .With(x => x.transform.localPosition = (Vector3) spawnedRafts[id].Coords);
            }
        }

        public bool IsEmptyCell(Vector3Int crd)
        {
            if (!worldGridService.IsHavePoint(crd))
            {
                if (!coords.Contains(crd))
                {
                    coords.Add(crd);
                    return true;
                }

            }

            return false;
        }
        
        

        public void SetTargetCraft(CraftObject item)
        {
            lastSelectedCraftItem = item;
        }

        public RaftBase GetRaftPrefabByCraft(CraftObject item)
        {
            return rafts.Find(x => x.CraftObject == item).Raft;
        }

        private List<RaftStorage.StorageItem> itemsToSave = new List<RaftStorage.StorageItem>(5);
        public void RemoveRaft(Vector3Int vector3Int)
        {
            itemsToSave.Clear();
            var raft = spawnedRafts.Find(x => x.Coords == vector3Int);

            if (raft != null)
            {
                worldGridService.RemovePoint(vector3Int);
                spawnedRafts.Remove(raft);

                if (raft.RaftType == RaftItem.ERaftType.Storage)
                {
                    var storage = raft.GetComponent<RaftStorage>();

                    foreach (var it in gamedata.ConfigData.ItemsForRaftTransitionAfterDestroying)
                    {
                        if (storage.HaveItem(it))
                        {
                            itemsToSave.Add(storage.GetItem(it));
                        }
                    }
                    storage.RemoveAllFromStorage();
                    storages.Remove(storage);

                    foreach (var it in itemsToSave)
                    {
                        ResourcesService.AddItemsToAnyRafts(it);
                    }

                }
                raft.gameObject.SetActive(false);
                Destroy(raft.gameObject);
                
                OnChangeRaft?.Invoke();
            }
        }


        public RaftBase GetRaftByID(string raftID)
        {
            return spawnedRafts.Find(x => x.Uid == raftID);
        }

        public bool IsCanMoored()
        {
            foreach (var spawnedRaft in SpawnedRafts)
            {
                if (spawnedRaft.RaftType == RaftItem.ERaftType.Moored)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetEndRaftPoint(Transform spawnPointLadderPoint)
        {
            raftEndPoint = spawnPointLadderPoint;
        }

        private readonly List<RaftItem.ERaftType> notWalkableRafts = new()
        {
            RaftItem.ERaftType.Building,
            RaftItem.ERaftType.CraftTable,
            RaftItem.ERaftType.Fishing,
            RaftItem.ERaftType.Furnace
        };


        public RaftBase GetRandomWalkableRaft()
        {
            return SpawnedRafts.FindAll(x => !notWalkableRafts.Contains(x.RaftType)).GetRandomItem();
        }
    }
}
