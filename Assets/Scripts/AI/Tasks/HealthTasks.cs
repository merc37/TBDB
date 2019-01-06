using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class HealthTasks : MonoBehaviour
    {
        [SerializeField]
        private int lowHealthThreshold = 3;

        private bool isLowOnHealth;
        private bool isHealthZero;

        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("HealthPoints", new UnityAction<ParamsObject>(OnHealthPointUpdate));
        }

        [Task]
        bool LowOnHealth()
        {
            return isLowOnHealth;
        }

        [Task]
        bool HasZeroHealth()
        {
            return isHealthZero;
        }

        private void OnHealthPointUpdate(ParamsObject paramsObj)
        {
            isLowOnHealth = paramsObj.Int <= lowHealthThreshold;
            isHealthZero = paramsObj.Int <= 0;
        }
    }
}
