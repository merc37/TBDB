using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour
{
    [SerializeField]
    protected bool autoPickup = false;

    public bool AutoPickup {
        get {
            return autoPickup;
        }
    }

    protected virtual void Awake() {
        
    }

    public abstract bool OnPickup(Transform transform);

    protected virtual void Update() {
        
    }
}
