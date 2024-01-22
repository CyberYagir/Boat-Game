using Content.Scripts.BoatGame.Services;

namespace Content.Scripts.BoatGame.Weather
{
    public interface IWeatherProvider
    {
        public WeatherService.WeatherModifiers WeatherModifiers { get; }


        public void Init(WeatherService.WeatherModifiers weatherModifiers);

        public void ForwardWeather();
        public void BackWeather();
    }
}
