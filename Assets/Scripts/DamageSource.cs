using UnityEngine;
using System.Linq;

public class DamageSource : MonoBehaviour
{

    private string source;
    public string Source
    {
        get
        {
            return source;
        }
        set
        {
            if(UnityEditorInternal.InternalEditorUtility.tags.Contains(value))
            {
                source = value;
            }
            else
            {
                source = "Untagged";
            }
        }
    }

    public short Damage
    {
        get; set;
    }
}
