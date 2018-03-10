using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[Tiled2Unity.CustomTiledImporter]
public class PathfindingGridImporter : Tiled2Unity.ICustomTiledImporter {

    public void CustomizePrefab(GameObject gameObject) {
		Grid grid = gameObject.AddComponent<Grid>();
		grid.GridScale = 0.5f;
		grid.CollisionMask = LayerMask.NameToLayer("Blocks");

		gameObject.AddComponent<BasicThetaStarPathfinding>();
    }

	public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
		
	}
}
