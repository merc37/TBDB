using UnityEngine;

namespace Pathfinding {
    public class Node {

        public Vector3 worldPosition;
        public Vector2Int gridPos;
        public bool isWalkable;

        public float gCost;
        public float hCost;
        public Node parent;

        public float fCost
        {
            get {
                return gCost + hCost;
            }
        }

        public Node(Vector3 worldPosition, int gridX, int gridY, bool isWalkable) {
            this.worldPosition = worldPosition;
            this.gridPos = new Vector2Int(gridX, gridY);
            this.isWalkable = isWalkable;
        }

        public void reset() {
            this.parent = null;
            this.gCost = Mathf.Infinity;
            this.hCost = Mathf.Infinity;
        }
    }
}
