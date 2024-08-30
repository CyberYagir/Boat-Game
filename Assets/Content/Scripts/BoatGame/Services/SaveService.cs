using System;
using System.Collections;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using UnityEngine;
using Zenject;

namespace Content.Scripts.BoatGame.Services
{
    public class SaveService : SaveServiceBase
    {
        private RaftDamagerService damagerService;
        private WeatherService weatherService;
        private CloudService cloudService;


        [Inject]
        private void Construct(
            SaveDataObject saveDataObject,
            CharacterService characterService,
            RaftBuildService raftBuildService,
            RaftDamagerService damagerService,
            WeatherService weatherService,
            CloudService cloudService
        )
        {
            this.cloudService = cloudService;
            this.weatherService = weatherService;
            this.damagerService = damagerService;
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
            SaveDamagers();
            SaveWeather();

            saveDataObject.SaveFile();
            Debug.Log("Save World");
        }
        

         


        private void SaveWeather()
        {
            if (weatherService != null)
            {
                saveDataObject.Global.SetWeatherData(weatherService.GetWeatherData());
            }
        }

        private void SaveDamagers()
        {
            if (damagerService != null)
            {
                saveDataObject.Global.SetDamagersData(damagerService.GetDamagersData());
            }
        }

        public void ExitFromIsland()
        {
            saveDataObject.Global.SetIslandSeed(0);
            saveDataObject.SaveFile();
        }
    }
}
