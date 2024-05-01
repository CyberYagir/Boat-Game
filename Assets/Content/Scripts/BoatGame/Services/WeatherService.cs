using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Weather;
using Content.Scripts.Boot;
using Content.Scripts.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Content.Scripts.BoatGame.Services
{
    public class WeatherService : MonoBehaviour
    {
        public enum EWeatherType
        {
            Сalm,
            Windy,
            Storm,
            Rain
        }
        [System.Serializable]
        public class WeatherModifiers
        {
            [SerializeField] private EWeatherType weatherType;
            [SerializeField] private float hunger = 1;
            [SerializeField] private float thirsty = 1;
            [SerializeField] private float weight;

            [SerializeField] private GameObject weatherProvider;

            private IWeatherProvider iWeatherProvider;
            
            
            public float Weight => weight;

            public float Thirsty => thirsty;

            public float Hunger => hunger;

            public EWeatherType WeatherType => weatherType;

            public IWeatherProvider IWeatherProvider => iWeatherProvider;


            public void Init()
            {
                iWeatherProvider = weatherProvider.GetComponent<IWeatherProvider>();
                IWeatherProvider.Init(this);
            }
            
            public void Add(float deltaTime)
            {
                weight = Mathf.Clamp01(weight + deltaTime);
            }


            public void SetPercents(float hunger, float thirsty)
            {
                this.hunger = hunger;
                this.thirsty = thirsty;
            }
        }

        [SerializeField] private List<WeatherModifiers> weathers;

        [SerializeField, ReadOnly] private WeatherModifiers currentModifiers = new WeatherModifiers();
        [SerializeField, ReadOnly] private EWeatherType currentWeather = EWeatherType.Сalm;
        [SerializeField, ReadOnly] private int nextWeatherTicks;
        [SerializeField] private int ticks;

        public Action<EWeatherType> OnChangeWeather;

        public WeatherModifiers CurrentModifiers => currentModifiers;

        public EWeatherType CurrentWeather => currentWeather;

        [Inject]
        private void Construct(TickService tickService, SaveDataObject saveDataObject, ScenesService scenesService)
        {
            SetTicksCount();
            tickService.OnTick += OnTick;

            foreach (var w in weathers)
            {
                w.Init();
            }

            if (saveDataObject.Global.WeathersData.MaxTickCount > 0)
            {
                currentWeather = saveDataObject.Global.WeathersData.CurrentWeather;
                ticks = (int)saveDataObject.Global.WeathersData.TickCount;
                nextWeatherTicks = (int)saveDataObject.Global.WeathersData.MaxTickCount;
            }

            if (scenesService.GetActiveScene() == ESceneName.IslandGame)
            {
                currentWeather = EWeatherType.Сalm;
                tickService.OnTick -= OnTick;
            }

            PlayStartWeather();  
        }

        private void PlayStartWeather()
        {
            var calm = weathers.Find(x => x.WeatherType == CurrentWeather);
            calm.IWeatherProvider.ForwardWeather();
            currentModifiers.SetPercents(calm.Hunger, calm.Thirsty);
            OnChangeWeather?.Invoke(CurrentWeather);
        }

        private void SetTicksCount()
        {
            nextWeatherTicks = Random.Range(900, 2500);
        }

        private void OnTick(float delta)
        {
            ticks++;
            if (ticks >= nextWeatherTicks)
            {
                ticks = 0;
                StartCoroutine(MoveWeather());
                SetTicksCount();
            }
        }

        IEnumerator MoveWeather()
        {
            var oldWeather = weathers.Find(x=>x.WeatherType == CurrentWeather);
            currentWeather = Extensions.GetRandomEnum<EWeatherType>();
            
            var newWeather = weathers.Find(x=>x.WeatherType == CurrentWeather);


            if (oldWeather.WeatherType != newWeather.WeatherType)
            {
                ticks = 0;

                oldWeather.IWeatherProvider.BackWeather();
                newWeather.IWeatherProvider.ForwardWeather();

                float time = 0;
                while (time < 1f)
                {
                    time += TimeService.DeltaTime;
                    yield return null;
                    oldWeather.Add(-TimeService.DeltaTime);
                    newWeather.Add(TimeService.DeltaTime);

                    var hunger = ((oldWeather.Hunger * oldWeather.Weight) +
                                  (newWeather.Hunger * newWeather.Weight));
                    var thirsty = ((oldWeather.Thirsty * oldWeather.Weight) +
                                   (newWeather.Thirsty * newWeather.Weight));

                    CurrentModifiers.SetPercents(hunger, thirsty);
                }

                yield return new WaitForSeconds(2f);
                OnChangeWeather.Invoke(newWeather.WeatherType);
            }
        }

        public SaveDataObject.GlobalData.WeatherData GetWeatherData()
        {
            return new SaveDataObject.GlobalData.WeatherData(ticks, nextWeatherTicks, CurrentWeather);
        }
    }
}
