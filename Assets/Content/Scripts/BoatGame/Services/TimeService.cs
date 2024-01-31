using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class TimeService
    {
        public static float DeltaTime => Time.deltaTime;
        public static float TickRate = 20 * Time.timeScale;
        public static float TimeScale => Time.timeScale;
        public static float PlayedTime => playedTime;

        private static float playedTime;

        public static void ChangeTimeScale(float newValue)
        {
            Time.timeScale = newValue;
        }

        public static void AddPlayedTime()
        {
            playedTime = PlayedTime + DeltaTime;
        }

        public static void ClearPlayedTime()
        {
            playedTime = 0;
        }
    }
}
