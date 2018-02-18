using System.Collections.Generic;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
public class LightObstacleImporter : Tiled2Unity.ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props) {
        
        if(gameObject && gameObject.transform.parent.name == "lightObstacle") {
            Bounds gameObjectBounds = gameObject.GetComponent<Collider2D>().bounds;

            //TODO: Support more shapes
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = gameObject.name;
            cube.transform.parent = gameObject.transform.parent;
            if(props.ContainsKey("type") && props["type"] == "halfWall") {
                cube.transform.position = new Vector3(gameObjectBounds.center.x, gameObjectBounds.center.y, gameObjectBounds.center.z - MapConstants.HALF_WALL_HEIGHT/2);
                cube.transform.localScale = new Vector3(gameObjectBounds.size.x, gameObjectBounds.size.y, MapConstants.HALF_WALL_HEIGHT);
            }
            if(props.ContainsKey("type") && props["type"] == "fullWall") {
                cube.transform.position = new Vector3(gameObjectBounds.center.x, gameObjectBounds.center.y, gameObjectBounds.center.z - MapConstants.FULL_WALL_HEIGHT /2);
                cube.transform.localScale = new Vector3(gameObjectBounds.size.x, gameObjectBounds.size.y, MapConstants.FULL_WALL_HEIGHT);
            }
            MeshRenderer mr = cube.GetComponent<MeshRenderer>();
            mr.receiveShadows = true;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            Object.DestroyImmediate(cube.GetComponent<Collider>());

            Object.DestroyImmediate(gameObject);
        }

    }

    public void CustomizePrefab(GameObject gameObject) {
        
    }
}
