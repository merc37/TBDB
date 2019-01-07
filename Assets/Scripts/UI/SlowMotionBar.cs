using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using EventManagers;
using Events;

namespace UI
{
    public class SlowMotionBar : MonoBehaviour
    {

        [SerializeField]
        private Image barFill;

        private float playerMaxSlowMotionTime = 1;
        private float playerCurrentSlowMotionTime = 1;

        void Awake()
        {
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateMaxSlowMotionTime, new UnityAction<ParamsObject>(OnPlayerUpdateMaxSlowMotionTime));
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateCurrentSlowMotionTime, new UnityAction<ParamsObject>(OnPlayerUpdateCurrentSlowMotionTime));
        }

        private void OnPlayerUpdateMaxSlowMotionTime(ParamsObject paramsObj)
        {
            playerMaxSlowMotionTime = paramsObj.Float;
            barFill.fillAmount = playerCurrentSlowMotionTime / playerMaxSlowMotionTime;
        }

        private void OnPlayerUpdateCurrentSlowMotionTime(ParamsObject paramsObj)
        {
            playerCurrentSlowMotionTime = paramsObj.Float;
            barFill.fillAmount = playerCurrentSlowMotionTime / playerMaxSlowMotionTime;
        }

    }
}