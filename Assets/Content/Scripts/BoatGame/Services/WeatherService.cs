using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.BoatGame.Weather;
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

        [Inject]
        private void Construct(TickService tickService)
        {
            SetTicksCount();
            tickService.OnTick += OnTick;

            foreach (var w in weathers)
            {
                w.Init();
            }

            var calm = weathers.Find(x => x.WeatherType == currentWeather);
            calm.IWeatherProvider.ForwardWeather();
            currentModifiers.SetPercents(calm.Hunger, calm.Thirsty);
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
            var oldWeather = weathers.Find(x=>x.WeatherType == currentWeather);
            currentWeather = Extensions.GetRandomEnum<EWeatherType>();
            
            var newWeather = weathers.Find(x=>x.WeatherType == currentWeather);


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
    }
}
