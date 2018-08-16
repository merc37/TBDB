using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[Tiled2Unity.CustomTiledImporter]
public class PathfindingImporter : Tiled2Unity.ICustomTiledImporter {

    public void CustomizePrefab(GameObject gameObject) {
		Pathfinding.Grid grid = gameObject.AddComponent<Pathfinding.Grid>();
		grid.GridScale = 0.5f;
		grid.CollisionMask = LayerMask.NameToLayer("Blocks");

		gameObject.AddComponent<BasicThetaStarPathfinding>();
    }

	public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
		
	}
}
