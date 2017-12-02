using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
public class WallHeightImporter : Tiled2Unity.ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props) {

    }

    public void CustomizePrefab(GameObject gameObject) {
        foreach(Transform child in gameObject.transform) {
            if(child.name == "halfWall") {
                child.position = new Vector3(child.position.x, child.position.y, -MapConstants.HALF_WALL_HEIGHT);
            }
            if(child.name == "fullWall") {
                child.position = new Vector3(child.position.x, child.position.y, -MapConstants.FULL_WALL_HEIGHT);
            }
        }
    }

}
