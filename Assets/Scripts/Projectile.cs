using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private string ownerType;
    public string OwnerType
    {
        get {
            return ownerType;
        }
        set {
            if(UnityEditorInternal.InternalEditorUtility.tags.Contains<string>(value)) {
                ownerType = value;
            } else {
                ownerType = "Untagged";
            }
        }
    }
    private int damage;
    public int Damage
    {
        get {
            return damage;
        }
        set {
            damage = value;
        }
    }
}
