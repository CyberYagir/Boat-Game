using UnityEngine;

namespace Content.Scripts.BoatGame.Services
{
    public static class InputService
    {
        public static bool IsLMBDown => Input.GetKeyDown(KeyCode.Mouse0);
        public static bool IsLMBPressed => Input.GetKey(KeyCode.Mouse0);
        public static bool IsRMBPressed => Input.GetKey(KeyCode.Mouse1);

        public static Vector2 MoveAxis => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        public static Vector2 MouseAxis => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        public static float MouseWheel => -Input.GetAxis("Mouse ScrollWheel");
        public static Vector3 MousePosition => Input.mousePosition;
        public static bool SpaceHold => Input.GetKey(KeyCode.Space);
    }
}
