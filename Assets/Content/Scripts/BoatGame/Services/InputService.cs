using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class InputService
    {
        public static bool IsLMBDown => Input.GetKeyDown(KeyCode.Mouse0);
        public static bool IsRMBPressed => Input.GetKey(KeyCode.Mouse1);

        public static Vector2 MouseAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        public static float MouseWheel => -Input.GetAxis("Mouse ScrollWheel");
    }
}