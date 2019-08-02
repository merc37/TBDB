using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIAbilityInventory : MonoBehaviour
    {
        [SerializeField]
        private Image uiAbilityIconPrefab;
        [SerializeField]
        private PlayerAbilityInventory playerAbilityInventory;

        [SerializeField]
        private Color selectColor;
        [SerializeField]
        private Color highlightColor;
        [SerializeField]
        private float dropTime = 1f;

        private Timer dropTimer;

        private int _abilitySelectedIndex;
        private int AbilitySelectedIndex
        {
            get {
                return _abilitySelectedIndex;
            }

            set {
                if (value == Constants.NullInt) {
                    _abilitySelectedIndex = value;
                    return;
                }

                if (value < 0) {
                    value = 2;
                }

                if (value > 2) {
                    value = 0;
                }

                if (_abilitySelectedIndex != Constants.NullInt) {
                    Transform iconOne = transform.GetChild(_abilitySelectedIndex).GetChild(0);
                    Transform iconTwoParent = transform.GetChild(value);
                    Transform iconOneParent = iconOne.parent;
                    if (iconTwoParent.childCount > 0) {
                        iconTwoParent.GetChild(0).SetParent(iconOneParent, false);
                    }
                    iconOne.SetParent(iconTwoParent, false);

                    playerAbilityInventory.SwapPlayerAbility(_abilitySelectedIndex, value);

                    transform.GetChild(_abilitySelectedIndex).GetComponent<Image>().color = Color.white;
                }

                transform.GetChild(value).GetComponent<Image>().color = selectColor;

                _abilitySelectedIndex = value;
                _abilityHighlightedIndex = value;
            }
        }

        private int _abilityHighlightedIndex;
        private int AbilityHighlightedIndex
        {
            get {
                return _abilityHighlightedIndex;
            }

            set {
                if(value == Constants.NullInt) {
                    transform.GetChild(_abilityHighlightedIndex).GetComponent<Image>().color = Color.white;
                    _abilityHighlightedIndex = value;
                    return;
                }

                if (value < 0) {
                    value = 2;
                }

                if (value > 2) {
                    value = 0;
                }

                if(_abilityHighlightedIndex != Constants.NullInt) {
                    transform.GetChild(_abilityHighlightedIndex).GetComponent<Image>().color = Color.white;
                }
                transform.GetChild(value).GetComponent<Image>().color = highlightColor;

                _abilityHighlightedIndex = value;
            }
        }

        private float axis;
        private bool axisInUse;

        void Awake() {
            AbilitySelectedIndex = Constants.NullInt;
            AbilityHighlightedIndex = Constants.NullInt;
            dropTimer = new Timer(dropTime);
        }

        void Update() {
            if (Input.GetButtonDown("Interact")) {
                dropTimer.Reset();
                if (AbilitySelectedIndex == Constants.NullInt) {
                    if(AbilityHighlightedIndex != Constants.NullInt) {
                        if (playerAbilityInventory.GetPlayerAbility(AbilityHighlightedIndex) != null) {
                            AbilitySelectedIndex = AbilityHighlightedIndex;
                        }
                    }
                } else if(AbilitySelectedIndex != Constants.NullInt) {
                    transform.GetChild(AbilitySelectedIndex).GetComponent<Image>().color = highlightColor;
                    AbilitySelectedIndex = Constants.NullInt;
                }
            }

            if (Input.GetButton("Interact")) {
                if (AbilityHighlightedIndex != Constants.NullInt) {
                    if(playerAbilityInventory.GetPlayerAbility(AbilityHighlightedIndex) != null) {
                        if (dropTimer.Tick()) {
                            playerAbilityInventory.DropPlayerAbility(AbilityHighlightedIndex);
                            transform.GetChild(AbilityHighlightedIndex).GetComponent<Image>().color = highlightColor;
                            Destroy(transform.GetChild(AbilityHighlightedIndex).GetChild(0).gameObject);
                        }
                    }
                }
            }

            axis = Input.GetAxisRaw("HorizontalMovement");
            if (axis != 0 && !axisInUse) {
                axisInUse = true;
                if (AbilitySelectedIndex != Constants.NullInt) {
                    AbilitySelectedIndex += (int)Mathf.Sign(axis);
                }

                if (AbilitySelectedIndex == Constants.NullInt) {
                    AbilityHighlightedIndex += (int)Mathf.Sign(axis);
                }
            }

            if(axis == 0) {
                axisInUse = false;
            }
        }

        void OnEnable() {
            PlayerInput.Paused = true;
            for (int i = 0; i < 3; i++) {
                PlayerAbility playerAbility = playerAbilityInventory.GetPlayerAbility(i);
                if (playerAbility != null) {
                    if(AbilityHighlightedIndex == Constants.NullInt) {
                        AbilityHighlightedIndex = i;
                    }
                    Instantiate(uiAbilityIconPrefab, transform.GetChild(i)).sprite = playerAbility.AbilityIcon();
                }
            }
        }

        void OnDisable() {
            AbilitySelectedIndex = Constants.NullInt;
            AbilityHighlightedIndex = Constants.NullInt;
            for (int i = 0; i < 3; i++) {
                Transform iconSlot = transform.GetChild(i);
                if(iconSlot.childCount > 0) {
                    Destroy(iconSlot.GetChild(0).gameObject);
                }
            }
            PlayerInput.Paused = false;
        }
    }
}
