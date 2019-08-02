using EventManagers;
using UnityEngine;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public static bool Paused;

        void Awake()
        {
            Paused = false;
        }

        public static bool GetButton(string buttonName)
        {
            return !Paused && Input.GetButton(buttonName);
        }

        public static bool GetButtonDown(string buttonName)
        {
            return !Paused && Input.GetButtonDown(buttonName);
        }

        public static bool GetButtonUp(string buttonName)
        {
            return !Paused && Input.GetButtonUp(buttonName);
        }

        public static bool AnyKeyDown() {
            return !Paused && Input.anyKeyDown;
        }

        public static float GetAxisRaw(string buttonName)
        {
            if(Paused)
            {
                return 0;
            }
            return Input.GetAxisRaw(buttonName);
        }
    }
}
