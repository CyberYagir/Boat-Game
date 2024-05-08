using System;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class RaftBuildService : MonoBehaviour
    {
        [System.Serializable]
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
                Furnace
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
        
        
        [SerializeField, ReadOnly] private List<RaftStorage> storages = new List<RaftStorage>();
        [SerializeField, ReadOnly] private Transform raftEndPoint;
        private WorldGridService worldGridService;
        private SaveDataObject saveData;
        private List<RaftTapToBuild> spawnedTapBuildRafts = new List<RaftTapToBuild>();
        private CraftObject lastSelectedCraftItem;
        
        public event Action OnChangeRaft;
        public List<RaftBase> SpawnedRafts => spawnedRafts;
        public List<RaftStorage> Storages => storages;

        public Transform Holder => holder;

        public Transform RaftEndPoint => raftEndPoint;

        [Inject]
        private void Construct(
            WorldGridService worldGridService, 
            SaveDataObject saveData, 
            GameStateService gameStateService, 
            SelectionService selectionService,
            GameDataObject gamedata,
            INavMeshProvider navMeshProvider,
            PrefabSpawnerFabric prefabSpawnerFabric)
        {
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

                var storage = spawned.GetComponent<RaftStorage>();
                if (storage)
                {
                    storage.LoadStorage(saveData.Rafts.Storages.Find(x => x.RaftUid == raft.Uid), gamedata);
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
            else
            {
                foreach (var stpr in spawnedTapBuildRafts)
                {
                    Destroy(stpr.gameObject);
                }
                spawnedTapBuildRafts.Clear();
                selectionService.OnTapOnBuildingRaft -= OnTapOnBuildingRaft;
            }
        }

        private void OnTapOnBuildingRaft(RaftTapToBuild targetRaft)
        {
            var raft = AddRaft(targetRaft.Coords, RaftItem.ERaftType.Building);
            raft.GetComponent<RaftBuild>().SetCraft(lastSelectedCraftItem, selectionService, this, gamedata);
            gameStateService.ChangeGameState(GameStateService.EGameState.Normal);

            foreach (var ing in lastSelectedCraftItem.Ingredients)
            {
                for (int i = 0; i < ing.Count; i++)
                {
                    foreach (var raftStorage in Storages)
                    {
                        raftStorage.RemoveFromStorage(ing.ResourceName);
                    }
                }
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

            if (type == RaftItem.ERaftType.Furnace)
            {
                var furnace = rf.GetComponent<Furnace>();
                if (furnace)
                {
                    //furnace.Init(tickService);
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

        public RaftStorage FindEmptyStorage(int value)
        {
            var storage = Storages.Find(x => x.IsEmptyStorage(value));
            return storage;
        }

        public RaftStorage FindResourceInStorages(EResourceTypes type)
        {
            return Storages.Find(x => x.GetResourceByType(type) > 0);
        }

        List<Vector3Int> coords = new List<Vector3Int>();
        private SelectionService selectionService;
        private GameStateService gameStateService;

        public void SpawnTapToBuildRafts()
        {
            coords.Clear();
            for (int i = 0; i < spawnedRafts.Count; i++)
            {
                if (!(spawnedRafts[i] is RaftBuild))
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

        public void RemoveRaft(Vector3Int vector3Int)
        {
            var raft = spawnedRafts.Find(x => x.Coords == vector3Int);

            if (raft != null)
            {
                worldGridService.RemovePoint(vector3Int);
                spawnedRafts.Remove(raft);

                if (raft.RaftType == RaftItem.ERaftType.Storage)
                {
                    var storage = raft.GetComponent<RaftStorage>();
                    storage.RemoveAllFromStorage();
                    storages.Remove(storage);
                }
                raft.gameObject.SetActive(false);
                Destroy(raft.gameObject);
                
                OnChangeRaft?.Invoke();
            }
        }

        private List<RaftStorage> emptyStoragesArray = new List<RaftStorage>(10);
        private GameDataObject gamedata;
        private PrefabSpawnerFabric prefabSpawnerFabric;

        public List<RaftStorage> FindEmptyStorages(int value)
        {
            emptyStoragesArray.Clear();
            foreach (var raftStorage in Storages)
            {
                if (raftStorage.IsEmptyStorage(value))
                {
                    emptyStoragesArray.Add(raftStorage);
                }
            }
            return emptyStoragesArray;
        }

        public RaftBase GetRaftByID(string raftID)
        {
            return spawnedRafts.Find(x => x.Uid == raftID);
        }

        public bool IsCanMoored()
        {
            foreach (var spawnedRaft in SpawnedRafts)
            {
                if (spawnedRaft.RaftType == RaftBuildService.RaftItem.ERaftType.Moored)
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
    }
}
