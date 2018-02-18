using System.Collections.Generic;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
public class ShaderMaterialImporter : Tiled2Unity.ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props) {
        
    }

    public void CustomizePrefab(GameObject gameObject) {
        MeshRenderer[] mrs = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer mr in mrs) {
            mr.receiveShadows = true;
            //mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            mr.sharedMaterial.shader = Shader.Find("Standard");
        }
    }
}
