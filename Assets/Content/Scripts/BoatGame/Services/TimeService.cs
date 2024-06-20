using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class TimeService
    {
        public static float DeltaTime => Time.deltaTime * TimeRate;
        public static int Ticks => 20;
        public static float TickRate => Ticks * TimeScale;
        public static float TimeScale => Time.timeScale * TimeRate;

        
        public static float PlayedTime => playedTime;

        public static float TimeRate => timeRate;
        public static float UnscaledDelta => Time.unscaledDeltaTime;

        public static float PlayedBoatTime => playedBoatTime;

        private static float playedTime;
        private static float playedBoatTime;
        private static float timeRate = 1f;

        public static void ChangeTimeScale(float newValue)
        {
            Time.timeScale = newValue;
        }

        public static void AddPlayedTime()
        {
            playedTime += UnscaledDelta;
        }

        public static void SetTimeRate(float rate)
        {
            timeRate = rate;
        }

        public static void ClearPlayedTime()
        {
            playedTime = 0;
            playedBoatTime = 0;
        }

        public static void AddPlayedBoatTime()
        {
            playedBoatTime = PlayedBoatTime + DeltaTime * TimeRate;
        }
        public static void AddPlayedBoatTime(float value)
        {
            playedBoatTime = PlayedBoatTime + value;
        }
    }
}
