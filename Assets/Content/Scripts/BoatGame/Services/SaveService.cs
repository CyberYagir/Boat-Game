using System;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class SaveService : MonoBehaviour
    {
        private SaveDataObject saveDataObject;
        private CharacterService characterService;
        private RaftBuildService raftBuildService;


        [Inject]
        private void Construct(SaveDataObject saveDataObject, CharacterService characterService, RaftBuildService raftBuildService)
        {
            this.raftBuildService = raftBuildService;
            this.characterService = characterService;
            this.saveDataObject = saveDataObject;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            SaveBoatWorld();
        }

        private void OnApplicationQuit()
        {
            SaveBoatWorld();
        }


        public void SaveBoatWorld()
        {
            characterService.SaveCharacters();

            var raftsData = new SaveDataObject.RaftsData();
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                raftsData.AddSpawnedRaft(raftBuildService.SpawnedRafts[i]);
            }
            saveDataObject.Global.AddTime(TimeService.PlayedTime);
            TimeService.ClearPlayedTime();
            saveDataObject.SetRaftsData(raftsData);
            saveDataObject.SaveFile();
        }
    }
}
