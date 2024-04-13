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
        private RaftDamagerService damagerService;
        private WeatherService weatherService;


        [Inject]
        private void Construct(
            SaveDataObject saveDataObject, 
            CharacterService characterService, 
            RaftBuildService raftBuildService, 
            RaftDamagerService damagerService,
            WeatherService weatherService)
        {
            this.weatherService = weatherService;
            this.damagerService = damagerService;
            this.raftBuildService = raftBuildService;
            this.characterService = characterService;
            this.saveDataObject = saveDataObject;
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            SaveWorld();
        }

        private void OnApplicationQuit()
        {
            SaveWorld();
        }


        public void SaveWorld()
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
            
            if (damagerService != null)
            {
                saveDataObject.Global.SetDamagersData(damagerService.GetDamagersData());
            }
            if (weatherService != null)
            {
                saveDataObject.Global.SetWeatherData(weatherService.GetWeatherData());
            }

            saveDataObject.SaveFile();
        }
    }
}
