using EventManagers;
using UnityEngine;
using UI;
using Events;

namespace Player
{
    public class PlayerAbilityInventory : MonoBehaviour
    {

        [SerializeField]
        private UIAbilityStatusPanels uiStatusPanels;
        [SerializeField]
        private UIAbilityInventory uiAbilityInventory;

        [SerializeField]
        private AbilityPickup abilityPickupPrefab;
        [SerializeField]
        private Transform abilityDummyPrefab;

        [SerializeField]
        private int preferredReplacementSlot = 0;

        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                PlayerAbility ability = transform.GetChild(i).GetComponent<PlayerAbility>();
                if (ability != null)
                {
                    uiStatusPanels.SetPlayerAbility(i, ability);
                }
            }
        }

        void Update()
        {
            if (Input.GetButtonDown("Inventory"))
            {
                uiAbilityInventory.gameObject.SetActive(!uiAbilityInventory.gameObject.activeSelf);
            }

            PlayerAbility playerAbility;
            if (PlayerInput.GetButtonDown("Ability1"))
            {
                playerAbility = GetPlayerAbility(0);
                if (playerAbility != null)
                {
                    playerAbility.AbilityStart();
                }
            }

            if (PlayerInput.GetButtonDown("Ability2"))
            {
                playerAbility = GetPlayerAbility(1);
                if (playerAbility != null)
                {
                    playerAbility.AbilityStart();
                }
            }

            if (PlayerInput.GetButtonDown("Ability3"))
            {
                playerAbility = GetPlayerAbility(2);
                if (playerAbility != null)
                {
                    playerAbility.AbilityStart();
                }
            }
        }

        public PlayerAbility GetPlayerAbility(int index)
        {
            Transform abilityChild = transform.GetChild(index);
            if (abilityChild != null)
            {
                return abilityChild.GetComponent<PlayerAbility>();
            }
            return null;
        }

        public void SwapPlayerAbility(int indexOne, int indexTwo)
        {
            Transform indexOneTransform = transform.GetChild(indexOne);
            Transform indexTwoTransform = transform.GetChild(indexTwo);
            if (Mathf.Abs(indexOne - indexTwo) > 1)
            {
                transform.GetChild(indexTwo).SetSiblingIndex(indexOne);
            }
            indexOneTransform.SetSiblingIndex(indexTwo);

            uiStatusPanels.SetPlayerAbility(indexOneTransform.GetSiblingIndex(), indexOneTransform.GetComponent<PlayerAbility>());
            uiStatusPanels.SetPlayerAbility(indexTwoTransform.GetSiblingIndex(), indexTwoTransform.GetComponent<PlayerAbility>());

            eventManager.TriggerEvent($"OnUpdateAbility{indexOneTransform.GetSiblingIndex()}", new ParamsObject(indexOneTransform.GetComponent<PlayerAbility>()));
            eventManager.TriggerEvent($"OnUpdateAbility{indexTwoTransform.GetSiblingIndex()}", new ParamsObject(indexTwoTransform.GetComponent<PlayerAbility>()));
        }

        public bool AddPlayerAbility(PlayerAbility value)
        {
            for (int i = 0; i < 3; i++)
            {
                if (GetPlayerAbility(i) == null)
                {
                    SetPlayerAbility(i, value);
                    return true;
                }
            }
            SetPlayerAbility(preferredReplacementSlot, value);
            return true;
        }

        public bool SetPlayerAbility(int index, PlayerAbility value)
        {
            PlayerAbility droppedPlayerAbility = GetPlayerAbility(index);
            if (GetPlayerAbility(index) != null)
            {
                Instantiate(abilityPickupPrefab, transform.root.position, Quaternion.identity).Ability = droppedPlayerAbility;
            }

            value.transform.SetParent(transform);
            value.transform.SetSiblingIndex(index);

            uiStatusPanels.SetPlayerAbility(index, value);

            eventManager.TriggerEvent($"OnUpdateAbility{index}", new ParamsObject(value));

            return true;
        }

        public bool DropPlayerAbility(int index)
        {
            PlayerAbility droppedPlayerAbility = GetPlayerAbility(index);
            if (droppedPlayerAbility == null)
            {
                return false;
            }
            Instantiate(abilityPickupPrefab, transform.root.position, Quaternion.identity).Ability = droppedPlayerAbility;

            Instantiate(abilityDummyPrefab, transform).SetSiblingIndex(index);

            uiStatusPanels.SetPlayerAbility(index, null);

            eventManager.TriggerEvent($"OnUpdateAbility{index}", null);

            return true;
        }
    }
}
