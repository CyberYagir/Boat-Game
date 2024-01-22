using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class TimeService
    {
        public static float DeltaTime => Time.deltaTime;
        public static float TickRate = 20 * Time.timeScale;
        public static float TimeScale => Time.timeScale;


        public static void ChangeTimeScale(float newValue)
        {
            Time.timeScale = newValue;
        }
    }
}
