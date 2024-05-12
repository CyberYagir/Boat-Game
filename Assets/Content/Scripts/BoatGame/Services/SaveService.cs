using System;
using System.Collections;
using System.Collections.Generic;
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
            
            
            
            StartCoroutine(AutoSave());
        }


        IEnumerator AutoSave()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(300);
                SaveWorld();
            }
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
            if (characterService.SpawnedCharacters.Count == 0) return;
            
            
            List<bool> notDead = new List<bool>();

            for (int i = 0; i < characterService.SpawnedCharacters.Count; i++)
            {
                if (!characterService.SpawnedCharacters[i].NeedManager.IsDead)
                {
                    notDead.Add(true);
                }
            }
            
            if (characterService.SpawnedCharacters.Count == 1 && notDead.Count == 0) return;
            

            characterService.SaveCharacters();

            var raftsData = new SaveDataObject.RaftsData();
            for (int i = 0; i < raftBuildService.SpawnedRafts.Count; i++)
            {
                raftsData.AddSpawnedRaft(raftBuildService.SpawnedRafts[i]);
            }
            saveDataObject.Global.AddTime(TimeService.PlayedTime);
            saveDataObject.Global.AddTimeOnRaft(TimeService.PlayedBoatTime);
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
            
            Debug.Log("Save World");
        }

        public void ExitFromIsland()
        {
            saveDataObject.Global.SetIslandSeed(0);
            saveDataObject.SaveFile();
        }
    }
}
