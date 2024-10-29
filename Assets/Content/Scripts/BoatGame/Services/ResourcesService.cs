using System;
using System.Collections.Generic;
using Content.Scripts.BoatGame.UI.UIEquipment;
using Content.Scripts.CraftsSystem;
using Content.Scripts.Global;
using Content.Scripts.ItemsSystem;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class ResourcesService : ResourcesServiceBase
    {
        private IRaftBuildService raftBuildService;

        [Inject]
        private void Construct(IRaftBuildService raftBuildService, GameDataObject gameData)
        {
            this.raftBuildService = raftBuildService;

            raftBuildService.OnChangeRaft += OnRaftsChanges;

            OnRaftsChanges();
        }

        private void OnRaftsChanges()
        {
            foreach (var spawnedRaft in raftBuildService.Storages)
            {
                spawnedRaft.OnStorageChange -= OnAnyStorageChange;
                spawnedRaft.OnStorageChange += OnAnyStorageChange;
            }
        }

        public override List<RaftStorage> GetRafts() => raftBuildService.Storages;

    }
}
