using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class TimeService
    {
        public static float DeltaTime => Time.deltaTime * TimeRate;
        public static float TickRate => 20 * TimeScale;
        public static float TimeScale => Time.timeScale * TimeRate;

        
        public static float PlayedTime => playedTime;

        public static float TimeRate => timeRate;

        private static float playedTime;
        private static float timeRate = 1f;

        public static void ChangeTimeScale(float newValue)
        {
            Time.timeScale = newValue;
        }

        public static void AddPlayedTime()
        {
            playedTime += DeltaTime * TimeRate;
        }

        public static void SetTimeRate(float rate)
        {
            Debug.LogError("Set Rate " + rate);
            timeRate = rate;
        }

        public static void ClearPlayedTime()
        {
            playedTime = 0;
        }
    }
}
