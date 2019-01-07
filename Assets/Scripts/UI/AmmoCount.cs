using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Events;

namespace UI
{
    public class AmmoCount : MonoBehaviour
    {
        [SerializeField]
        private Text ammoCountText;

        private int playerCurrentAmmo = 1;
        private int playerMaxAmmo = 1;

        void Awake()
        {
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateCurrentAmmo, new UnityAction<ParamsObject>(OnPlayerUpdateCurrentAmmo));
            GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerUpdateMaxAmmo, new UnityAction<ParamsObject>(OnPlayerUpdateMaxAmmo));
        }

        private void OnPlayerUpdateCurrentAmmo(ParamsObject paramsObj)
        {
            playerCurrentAmmo = paramsObj.Int;
            ammoCountText.text = playerCurrentAmmo + "/" + playerMaxAmmo;
        }

        private void OnPlayerUpdateMaxAmmo(ParamsObject paramsObj)
        {
            playerMaxAmmo = paramsObj.Int;
            ammoCountText.text = playerCurrentAmmo + "/" + playerMaxAmmo;
        }
    }
}