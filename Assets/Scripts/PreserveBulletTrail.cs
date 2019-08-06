using UnityEngine;

public class PreserveBulletTrail : MonoBehaviour
{
    void OnDestroy()
    {
        transform.DetachChildren();
    }
}
