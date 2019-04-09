using EventManagers;
using UnityEngine;
using Events;
using UnityEngine.Events;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        private static bool paused;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening(PlayerEvents.OnInventoryToggle, new UnityAction<ParamsObject>(OnInventoryToggle));
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

        public static float GetAxisRaw(string buttonName)
        {
            if(paused)
            {
                return 0;
            }
            return Input.GetAxisRaw(buttonName);
        }

        private void OnInventoryToggle(ParamsObject paramsObj)
        {
            paused = !paused;
        }
    }
}
