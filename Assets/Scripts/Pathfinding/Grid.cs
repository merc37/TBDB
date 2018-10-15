using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]
namespace Pathfinding {
    public class Grid : MonoBehaviour {

        [SerializeField]
        private float gridScale;
        public float GridScale
        {
            get { return gridScale; }
            set {
                if(grid == null) gridScale = value;
            }
        }
        [SerializeField]
        private LayerMask unwalkableMask;
        public LayerMask CollisionMask
        {
            get { return unwalkableMask; }
            set { if(grid == null) unwalkableMask = value; }
        }

        private Vector3 gridPosition;
        private Vector2Int gridWorldSize;
        private Vector2Int gridSize;
        private float nodeDiameter, nodeRadius;
        private Node[,] grid;

        public float nodeSize
        {
            get {
                return nodeDiameter;
            }
        }

        void Awake() {

        }

        void Start() {
            CreateGrid();
        }

        void CreateGrid() {
            // Get map bounds
            Tiled2Unity.TiledMap tiledMap = transform.GetComponent<Tiled2Unity.TiledMap>();
            nodeDiameter = gridScale;
            nodeRadius = nodeDiameter / 2;

            // Map and grid are positioned based on their top-left corner
            gridPosition = transform.position;// + new Vector3(0.5f, -0.5f, 0);
            gridWorldSize.x = tiledMap.NumTilesWide;
            gridWorldSize.y = tiledMap.NumTilesHigh;
            gridSize.x = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSize.y = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            grid = new Node[gridSize.x, gridSize.y];

            // Populate grid
            for(int y = 0; y < gridSize.y; y++) {
                for(int x = 0; x < gridSize.x; x++) {
                    // World point is center of node
                    Vector3 worldPoint = gridPosition + Vector3.right * (x * nodeDiameter + nodeRadius) - Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !Physics2D.BoxCast(new Vector3(worldPoint.x, worldPoint.y, -1), Vector2.one * (nodeDiameter / 2), 0, Vector3.forward, 1, unwalkableMask);
                    grid[x, y] = new Node(worldPoint, x, y, walkable);
                }
            }

            resetGrid();
        }

        public Node NodeFromWorldPoint(Vector3 worldPoint) {
            Vector3 posRelativeToGrid = worldPoint - gridPosition;
            float percentX = Mathf.Clamp01(Mathf.Abs(posRelativeToGrid.x) / gridWorldSize.x);
            float percentY = Mathf.Clamp01(Mathf.Abs(posRelativeToGrid.y) / gridWorldSize.y);
            int x = Mathf.RoundToInt(percentX * (gridSize.x - 1));
            int y = Mathf.RoundToInt(percentY * (gridSize.y - 1));
            return grid[x, y];
        }

        public List<Node> GetNeighbors(Node node) {
            List<Node> neighbors = new List<Node>();

            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    if(x == 0 && y == 0) continue;

                    int checkX = node.gridPos.x + x;
                    int checkY = node.gridPos.y + y;
                    if(checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y) {
                        neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neighbors;
        }

        public void resetGrid() {
            if(grid != null) {
                foreach(Node n in grid) n.reset();
            }
        }

        public List<Node> path;
        float pathCost;
        void OnDrawGizmos() {
            if(grid != null) {
                // Draw grid bounds
                Gizmos.DrawWireCube(gridPosition + new Vector3(gridWorldSize.x / 2, -gridWorldSize.y / 2, transform.position.z), new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

                // Draw nodes
                if(path != null && path.Count > 0) pathCost = path[path.Count - 1].fCost;
                Gradient g;
                GradientColorKey[] gck;
                GradientAlphaKey[] gak;
                g = new Gradient();
                gck = new GradientColorKey[3];
                gck[0].color = Color.yellow;
                gck[0].time = 0.0F;
                gck[1].color = Color.green;
                gck[1].time = 0.5F;
                gck[2].color = Color.red;
                gck[2].time = 1.0F;
                gak = new GradientAlphaKey[2];
                gak[0].alpha = 1.0F;
                gak[0].time = 0.0F;
                gak[1].alpha = 1.0F;
                gak[1].time = 1.0F;
                g.SetKeys(gck, gak);
                foreach(Node n in grid) {
                    if(path != null && path.Contains(n)) {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter / 8));
                        if(n.parent != null) Gizmos.DrawLine(n.worldPosition, n.parent.worldPosition);
                    } else {
                        if(!n.isWalkable) {
                            Gizmos.color = Color.red;
                            Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter / 8));
                        } else if(n.fCost != Mathf.Infinity) {
                            Gizmos.color = g.Evaluate(n.gCost / pathCost);
                            Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter / 8));
                        } else {
                            Gizmos.color = Color.cyan;
                        }
                        //Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeDiameter / 8));

                        //if(n.parent != null) {
                        //    Gizmos.DrawLine(n.worldPosition, n.parent.worldPosition);
                        //}
                    }

                    if(n.fCost != Mathf.Infinity) {
                        Vector3 GUIposition = n.worldPosition + new Vector3(-nodeRadius, nodeRadius);
                        Handles.Label(GUIposition, "G:" + n.gCost.ToString("F1"));
                        Handles.Label(GUIposition + Vector3.up * -0.1f, "H:" + n.hCost.ToString("F1"));
                        Handles.Label(GUIposition + Vector3.up * -0.27f, "F:" + n.fCost.ToString("F1"));
                    }
                }
            }
        }
    }
}
