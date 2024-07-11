using System;
using System.Collections;
using Content.Scripts.BoatGame.Services;
using Content.Scripts.Game;
using UnityEngine;
using UnityEngine.Networking;

namespace Content.Scripts.Boot
{
    [System.Serializable]
    public class JsonDateTime
    {
        [SerializeField] private int day;
        [SerializeField] private int month;
        [SerializeField] private int year;
        
        [SerializeField] private int hours;
        [SerializeField] private int minutes;
        [SerializeField] private int seconds;
        
        [SerializeField] private string type;


        public DateTime GetDateTime() => new DateTime(year, month, day, hours, minutes, seconds);


    }
    
    public class ProjectInstaller : MonoBinder
    {
       


        private ScenesService scenesService;
        public override void InstallBindings()
        {
            BindService<ScenesService>();
            BindService<WebDateGetUpdateService>();

            scenesService = Container.Resolve<ScenesService>();
        }
        

        private void Update()
        {
            TimeService.AddPlayedTime();
            if (scenesService.GetActiveScene() is ESceneName.BoatGame or ESceneName.Map)
            {
                TimeService.AddPlayedBoatTime();
            }
        }
    }
}