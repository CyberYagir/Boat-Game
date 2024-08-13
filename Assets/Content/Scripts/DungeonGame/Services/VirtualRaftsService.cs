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
        [SerializeField] private RaftStorage raftStoragePrefab;
        public List<RaftStorage> Storages => storages;
        public event Action OnChangeRaft;

        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameData)
        {
            foreach (var raftStorageData in saveData.Rafts.Storages)
            {
                Instantiate(raftStoragePrefab, transform)
                    .With(x => storages.Add(x))
                    .With(x => x.LoadStorage(raftStorageData, gameData))
                    .With(x=>x.OnStorageChange += ChangeStorage);
            }
        }

        private void ChangeStorage()
        {
            OnChangeRaft?.Invoke();
        }
    }
}
