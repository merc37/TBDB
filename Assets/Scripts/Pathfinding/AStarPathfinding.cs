using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    public class AStarPathfinding : MonoBehaviour {

        private Grid grid;

        void Awake() {
            grid = GetComponent<Grid>();
        }

        public List<Node> FindPath(Vector2 startPos, Vector2 targetPos) {
            grid.resetGrid();

            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0) {
                Node currentNode = openSet[0];
                for(int i = 1; i < openSet.Count; i++) {
                    if(openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)) {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == targetNode) {
                    // Path found, return path
                    return RetracePath(startNode, targetNode);
                }

                foreach(Node neighbor in grid.GetNeighbors(currentNode)) {
                    if(!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                    // A* "UpdateVertex()" method
                    float newMoveCost = currentNode.fCost + GetManhattanDistance(currentNode, neighbor);
                    if(newMoveCost < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = newMoveCost;
                        neighbor.hCost = GetManhattanDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

                        if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                    }
                }
            }
            return null;
        }

        List<Node> RetracePath(Node startNode, Node endNode) {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while(currentNode != startNode /*&& path.Count < 50*/) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();

            return path;
        }

        float GetManhattanDistance(Node nodeA, Node nodeB) {
            int distX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
            int distY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

            return distX + distY;
        }
    }
}
