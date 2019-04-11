using Player;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityReceptacle : MonoBehaviour
{
    private Image image;

    private PlayerAbility _ability;

    public PlayerAbility Ability
    {
        get {
            return _ability;
        }

        set {
            _ability = value;
            image.sprite = value.AbilityIcon();
        }
    }

    void Awake()
    {
        image = GetComponent<Image>();
    }
}
