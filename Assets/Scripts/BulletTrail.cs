using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    void OnDestroy() {
        transform.GetChild(0).SetParent(null);
    }
}
