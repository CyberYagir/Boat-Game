using System;
using System.Collections;
using Content.Scripts.BoatGame;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.DungeonGame.Services
{

    public class DungeonSaveService : SaveServiceBase
    {
        [Inject]
        private void Construct(
            SaveDataObject saveDataObject,
            DungeonCharactersService characterService,
            VirtualRaftsService raftBuildService
        )
        {
            this.raftBuildService = raftBuildService;
            this.characterService = characterService;
            this.saveDataObject = saveDataObject;

            StartCoroutine(AutoSave());
        }
        
        
        
        public override void SaveWorld()
        {
            if (SaveCharacters()) return;
            SaveRafts();
            ChangeTime();

            saveDataObject.SaveFile();
            
            Debug.Log("Save World");
        }

        public override void SaveRafts()
        {
            saveDataObject.Rafts.ClearStoragesData();
            foreach (var raftStorage in raftBuildService.Storages)
            {
                saveDataObject.Rafts.SetSpawnedStorage(raftStorage.GetComponent<RaftBase>());
            }
        }
    }
}
