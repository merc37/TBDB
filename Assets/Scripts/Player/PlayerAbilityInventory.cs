using EventManagers;
using UnityEngine;
using Events;
using UnityEngine.Events;
using System;
using UI;

namespace Player
{
    public class PlayerAbilityInventory : MonoBehaviour {

        [SerializeField]
        private UIAbilityStatusPanels statusPanels;

        private GameObjectEventManager eventManager;

        private PlayerAbility[] _playerAbilities;

        private bool abilitySwapMode = false;

        private int swapOne = -1, swapTwo = -1;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
            _playerAbilities = new PlayerAbility[3];
        }

        void Start()
        {
            PlayerAbility[] initialPlayerAbilities = GetComponentsInChildren<PlayerAbility>();
            for(int i = 0; i < initialPlayerAbilities.Length; i++) {
                SetPlayerAbility(i, initialPlayerAbilities[i]);
            }
        }

        void Update()
        {
            if(!abilitySwapMode && PlayerInput.GetButtonDown("AbilitySwap")) {
                abilitySwapMode = true;
                statusPanels.OnSwapModeActivate();
                return;
            }

            if (abilitySwapMode) {
                if(swapOne != -1 && swapTwo != -1) {
                    PlayerAbility temp = GetPlayerAbility(swapOne);
                    SetPlayerAbility(swapOne, GetPlayerAbility(swapTwo));
                    SetPlayerAbility(swapTwo, temp);

                    DeactivateSwapUI();
                    return;
                }

                if (PlayerInput.GetButtonDown("Ability1")) {
                    if(swapOne == -1) {
                        swapOne = 0;
                        statusPanels.OnHighlightActivate(0);
                        return;
                    }
                    swapTwo = 0;
                    return;
                }

                if (PlayerInput.GetButtonDown("Ability2")) {
                    if (swapOne == -1) {
                        swapOne = 1;
                        statusPanels.OnHighlightActivate(1);
                        return;
                    }
                    swapTwo = 1;
                    return;
                }

                if (PlayerInput.GetButtonDown("Ability3")) {
                    if (swapOne == -1) {
                        swapOne = 2;
                        statusPanels.OnHighlightActivate(2);
                        return;
                    }
                    swapTwo = 2;
                    return;
                }

                if(PlayerInput.AnyKeyDown()) {
                    DeactivateSwapUI();
                }
                return;
            }

            if (PlayerInput.GetButtonDown("Ability1"))
            {
                GetPlayerAbility(0).AbilityStart();
            }

            if(PlayerInput.GetButtonDown("Ability2"))
            {
                GetPlayerAbility(1).AbilityStart();
            }

            if(PlayerInput.GetButtonDown("Ability3"))
            {
                GetPlayerAbility(2).AbilityStart();
            }
        }

        private void DeactivateSwapUI() {
            swapOne = -1;
            swapTwo = -1;
            abilitySwapMode = false;
            statusPanels.OnSwapModeDeactivate();
            statusPanels.OnHighlightDeactivate(0);
            statusPanels.OnHighlightDeactivate(1);
            statusPanels.OnHighlightDeactivate(2);
        }

        public PlayerAbility GetPlayerAbility(int index) {
            return _playerAbilities[index];
        }

        public void SetPlayerAbility(int index, PlayerAbility value) {
            _playerAbilities[index] = value;
            eventManager.TriggerEvent($"OnUpdateAbility{index + 1}", new ParamsObject(GetPlayerAbility(index).transform));
            statusPanels.SetPlayerAbility(index, GetPlayerAbility(index));
        }
    }
}
