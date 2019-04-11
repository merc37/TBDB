using UnityEngine;
using Player;
using UnityEngine.UI;

public class UIAbilityStatusPanel : MonoBehaviour
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

    private Image statusImage;

    void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        image = images[1];
        statusImage = images[0];
    }

    void Update()
    {
        if(Ability != null)
        {
            if(Ability.IsAbilityActive())
            {
                statusImage.fillAmount = 1;
                statusImage.color = Color.red;
            }

            if(Ability.IsAbilityReady())
            {
                statusImage.fillAmount = 1;
                statusImage.color = Color.white;
            }

            if(Ability.IsAbilityOnCooldown())
            {
                statusImage.color = Color.blue;
                statusImage.fillAmount = Ability.GetCooldownTimer().GetTimerPercentage();
            }
        }
    }
}
