using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class HealthTasks : MonoBehaviour
    {
        [SerializeField]
        private short lowHealthThreshold = 3;

        private bool isLowOnHealth;
        private bool isHealthZero;

        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening(HealthEvents.OnUpdateCurrentHealth, new UnityAction<ParamsObject>(OnUpdateCurrentHealth));
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

        private void OnUpdateCurrentHealth(ParamsObject paramsObj)
        {
            isLowOnHealth = paramsObj.Short <= lowHealthThreshold;
            isHealthZero = paramsObj.Short <= 0;
        }
    }
}
