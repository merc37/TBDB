using Player;
using UnityEngine;

public class AbilityPickup : Pickup
{
    public PlayerAbility Ability {
        set {
            GetComponent<SpriteRenderer>().sprite = value.AbilityIcon();
            value.transform.SetParent(transform, false);
        }
    }

    public override bool OnPickup(Transform transformPickerUpper) {
        PlayerAbilityInventory abilityInventory = transformPickerUpper.GetComponentInChildren<PlayerAbilityInventory>();
        if(abilityInventory == null) {
            return false;
        }
        abilityInventory.AddPlayerAbility(transform.GetComponentInChildren<PlayerAbility>());
        return true;
    }
}
