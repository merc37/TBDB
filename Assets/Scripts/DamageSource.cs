using UnityEngine;
using System.Linq;

public class DamageSource : MonoBehaviour
{

    private string _source;
    public string Source
    {
        get
        {
            return _source;
        }
        private set
        {
            if (UnityEditorInternal.InternalEditorUtility.tags.Contains(value))
            {
                _source = value;
            }
            else
            {
                _source = "Untagged";
            }
        }
    }

    public short Damage
    {
        get; private set;
    }

    public void Set(string src, short dmg)
    {
        Source = src;
        Damage = dmg;
    }
}
