using EventManagers;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private static bool paused;

        void Awake()
        {
            paused = false;
        }

        public static bool GetButton(string buttonName)
        {
            return !paused && Input.GetButton(buttonName);
        }

        public static bool GetButtonDown(string buttonName)
        {
            return !paused && Input.GetButtonDown(buttonName);
        }

        public static bool GetButtonUp(string buttonName)
        {
            return !paused && Input.GetButtonUp(buttonName);
        }

        public static bool AnyKeyDown() {
            return Input.anyKeyDown;
        }

        public static float GetAxisRaw(string buttonName)
        {
            if(paused)
            {
                return 0;
            }
            return Input.GetAxisRaw(buttonName);
        }
    }
}
