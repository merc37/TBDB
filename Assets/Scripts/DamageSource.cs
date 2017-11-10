using UnityEngine;
using System.Linq;

public class DamageSource : MonoBehaviour {

    private string source;
    public string Source
    {
        get {
            return source;
        }
        set {
            if(UnityEditorInternal.InternalEditorUtility.tags.Contains<string>(value)) {
                source = value;
            } else {
                source = "Untagged";
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
