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
    public class DungeonResourcesService : ResourcesServiceBase
    {
        private IRaftBuildService virtualRaftsService;


        [Inject]
        private void Construct(SaveDataObject saveData, GameDataObject gameData, IRaftBuildService virtualRaftsService)
        {
            this.virtualRaftsService = virtualRaftsService;
            foreach (var spawnedRaft in virtualRaftsService.Storages)
            {
                spawnedRaft.OnStorageChange -= OnAnyStorageChange;
                spawnedRaft.OnStorageChange += OnAnyStorageChange;
            }

            OnAnyStorageChange();
        }

        public override List<RaftStorage> GetRafts() => virtualRaftsService.Storages;
    }
}
