using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{
    public class VirtualRaftsService : MonoBehaviour, IRaftBuildService
    {
        [SerializeField] private List<RaftStorage> storages = new List<RaftStorage>();
        [SerializeField] private List<RaftBase> spawnedRafts = new List<RaftBase>();
        [SerializeField] private RaftStorage raftStoragePrefab;
        [SerializeField] private RaftBase raftBasePrefab;
        private IResourcesService resourcesService;
        public List<RaftStorage> Storages => storages;
        public List<RaftBase> SpawnedRafts => spawnedRafts;
        public Transform Holder => transform;
        public IResourcesService ResourcesService => resourcesService;
        public Transform RaftEndPoint => null;
        public event Action OnChangeRaft;


        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameData, IResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
            foreach (var raftStorageData in saveData.Rafts.Storages)
            {
                Instantiate(raftStoragePrefab, transform)
                    .With(x => storages.Add(x))
                    .With(x => x.LoadStorage(raftStorageData, gameData))
                    .With(x=>x.OnStorageChange += ChangeStorage)
                    .With(x=>spawnedRafts.Add(x.GetComponent<RaftBase>()))
                    .With(x=>x.GetComponent<RaftBase>().LoadData(100, raftStorageData.RaftUid));
            }

            foreach (var raft in saveData.Rafts.Rafts)
            {
                Instantiate(raftBasePrefab, transform)
                    .With(x=>x.ChangeType(raft.RaftType))
                    .With(x=>spawnedRafts.Add(x))
                    .With(x=>x.LoadData(100, raft.Uid));
            }
        }

        private void ChangeStorage()
        {
            OnChangeRaft?.Invoke();
        }
        
        public RaftBase GetRaftByID(string raftID)
        {
            throw new NotImplementedException();
        }

        public RaftBase GetRandomWalkableRaft()
        {
            throw new NotImplementedException();
        }

        public RaftStorage FindEmptyStorage(ItemObject item, int value)
        {
            throw new NotImplementedException();
        }

        public void SetEndRaftPoint(Transform spawnPointLadderPoint)
        {
            throw new NotImplementedException();
        }

        public bool IsCanMoored()
        {
            throw new NotImplementedException();
        }

        public void SetTargetCraft(CraftObject item)
        {
            throw new NotImplementedException();
        }
    }
}
