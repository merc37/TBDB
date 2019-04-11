using EventManagers;
using UnityEngine;
using Events;
using UnityEngine.Events;
using System;

namespace Player
{
    public class PlayerAbilityInventory : MonoBehaviour
    {
        private GameObjectEventManager eventManager;

        private PlayerAbility[] playerAbilities;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
            eventManager.StartListening(PlayerEvents.OnSetAbility, new UnityAction<ParamsObject>(OnSetAbility));
        }

        void Start()
        {
            playerAbilities = GetComponentsInChildren<PlayerAbility>();
            eventManager.TriggerEvent(PlayerEvents.OnUpdateAbility1, new ParamsObject(playerAbilities[0].transform));
            eventManager.TriggerEvent(PlayerEvents.OnUpdateAbility2, new ParamsObject(playerAbilities[1].transform));
            eventManager.TriggerEvent(PlayerEvents.OnUpdateAbility3, new ParamsObject(playerAbilities[2].transform));
        }

        void Update()
        {
            if(PlayerInput.GetButtonDown("Ability1"))
            {
                playerAbilities[0].AbilityStart();
            }

            if(PlayerInput.GetButtonDown("Ability2"))
            {
                playerAbilities[1].AbilityStart();
            }

            if(PlayerInput.GetButtonDown("Ability3"))
            {
                playerAbilities[2].AbilityStart();
            }
        }

        private void OnSetAbility(ParamsObject paramsObj)
        {
            Transform abilityTransform = paramsObj.Transform;
            abilityTransform.SetParent(transform);
            abilityTransform.SetSiblingIndex(paramsObj.Int);
            if(transform.childCount > 3) {
                Destroy(transform.GetChild(3));
            }
            playerAbilities = GetComponentsInChildren<PlayerAbility>();
            string evt = PlayerEvents.OnUpdateAbility1.Replace("1", "" + (paramsObj.Int + 1));
            eventManager.TriggerEvent(evt, paramsObj);
        }
    }
}
