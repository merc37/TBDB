using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSpriteShadow : MonoBehaviour {
    
    [SerializeField]
    private bool castShadows = true;
    [SerializeField]
    private bool receiveShadows = true;

    void Start () {
        if(castShadows) {
            GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        } else {
            GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        GetComponent<Renderer>().receiveShadows = receiveShadows;
    }
}
