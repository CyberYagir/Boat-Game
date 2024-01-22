using Content.Scripts.BoatGame.Services;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.BoatGame.Weather
{
    public class WeatherWindy : MonoBehaviour, IWeatherProvider
    {
        [SerializeField] private Material waterMaterial;

        [SerializeField, ColorUsage(true, true)] private Color waterColor;

        [SerializeField, ColorUsage(true, true)] private Color shallowColor;

        [SerializeField] private ParticleSystem windParticle;
        
        private WeatherService.WeatherModifiers weatherModifiers;
        
        
        
        public WeatherService.WeatherModifiers WeatherModifiers => weatherModifiers;
        
        public void Init(WeatherService.WeatherModifiers weatherModifiers)
        {
            this.weatherModifiers = weatherModifiers;
        }

        public void ForwardWeather()
        {
            waterMaterial.DOColor(waterColor, Shader.PropertyToID("_BaseColor"), 5f);
            waterMaterial.DOColor(waterColor * 2f, Shader.PropertyToID("_HorizonColor"), 5f);
            waterMaterial.DOColor(shallowColor, Shader.PropertyToID("_ShallowColor"), 5f);
            
            waterMaterial.DOFloat(0.5f, Shader.PropertyToID("_WaveHeight"), 5f);
            
            windParticle.Play(true);
        }

        public void BackWeather()
        {
            windParticle.Stop(true);
        }
    }
}
