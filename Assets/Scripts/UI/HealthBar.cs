using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Events;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Image barFill = null;

        private short playerMaxHealth = 1;
        private short playerCurrentHealth = 1;

        void Awake()
        {
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateCurrentHealth, new UnityAction<ParamsObject>(OnPlayerUpdateCurrentHealth));
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateMaxHealth, new UnityAction<ParamsObject>(OnPlayerUpdateMaxHealth));
        }

        private void OnPlayerUpdateCurrentHealth(ParamsObject paramsObj)
        {
            playerCurrentHealth = paramsObj.Short;
            barFill.fillAmount = (float)playerCurrentHealth / (float)playerMaxHealth;
        }

        private void OnPlayerUpdateMaxHealth(ParamsObject paramsObj)
        {
            playerMaxHealth = paramsObj.Short;
            barFill.fillAmount = (float)playerCurrentHealth / (float)playerMaxHealth;
        }
    }
}