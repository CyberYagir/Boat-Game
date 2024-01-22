using System.Collections;
using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.Weather
{
    public class WeatherStorm : MonoBehaviour, IWeatherProvider
    {
        [SerializeField] private Material waterMaterial;
        [SerializeField] private Light sun;
        
        
        [SerializeField, ColorUsage(true, true)] private Color waterColor;
        [SerializeField, ColorUsage(true, true)] private Color horizonColor;
        [SerializeField, ColorUsage(true, true)] private Color shallowColor;
        [SerializeField] private ParticleSystem stormRain;
        private float startIntence = 0;
        
        
        private WeatherService.WeatherModifiers weatherModifiers;
        private float sunDarkValue = 0.35f;

        public WeatherService.WeatherModifiers WeatherModifiers => weatherModifiers;
        
        public void Init(WeatherService.WeatherModifiers weatherModifiers)
        {
            this.weatherModifiers = weatherModifiers;
        }

        public void ForwardWeather()
        {
            waterMaterial.DOColor(waterColor, Shader.PropertyToID("_BaseColor"), 5f);
            waterMaterial.DOColor(horizonColor, Shader.PropertyToID("_HorizonColor"), 5f);
            waterMaterial.DOFloat(2f, Shader.PropertyToID("_WaveHeight"), 5f);
            waterMaterial.DOColor(shallowColor, Shader.PropertyToID("_ShallowColor"), 5f);

            startIntence = sun.intensity;

            sun.DOIntensity(sunDarkValue, 5f);
            
            stormRain.Play(true);

            StartCoroutine(LightningWait());
        }

        public void BackWeather()
        {
            StopAllCoroutines();

            sun.intensity = sunDarkValue;
            sun.DOIntensity(startIntence, 10f);
            
            
            stormRain.Stop(true);
            
        }


        IEnumerator LightningWait()
        {
            while (true)
            {
                float wait = Random.Range(25, 60);
                yield return new WaitForSeconds(wait);

                for (int i = 0; i < Random.Range(1, 3); i++)
                {
                    sun.intensity = 5;
                    yield return new WaitForSeconds(0.1f);
                    sun.intensity = sunDarkValue;
                    yield return new WaitForSeconds(0.1f);
                }



                yield return null;
            }
        }
    }
}
