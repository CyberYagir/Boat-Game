using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
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
        public List<RaftStorage> Storages => storages;
        public List<RaftBase> SpawnedRafts => throw new NotImplementedException();
        public event Action OnChangeRaft;

        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameData)
        {
            foreach (var raftStorageData in saveData.Rafts.Storages)
            {
                Instantiate(raftStoragePrefab, transform)
                    .With(x => storages.Add(x))
                    .With(x => x.LoadStorage(raftStorageData, gameData))
                    .With(x=>x.OnStorageChange += ChangeStorage)
                    .With(x=>spawnedRafts.Add(x.GetComponent<RaftBase>()))
                    .With(x=>x.GetComponent<RaftBase>().LoadData(100, raftStorageData.RaftUid));
            }
        }

        private void ChangeStorage()
        {
            OnChangeRaft?.Invoke();
        }
    }
}
