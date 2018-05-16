using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    public class BasicThetaStarPathfinding : MonoBehaviour {

        private Grid grid;

        void Awake() {
            grid = GetComponent<Grid>();
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, Collider2D collider) {
            grid.resetGrid();

            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            startNode.gCost = 0;
            startNode.parent = startNode;
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

                    // Basic Theta * "UpdateVertex()" method
                    if(currentNode.parent != null && LineOfSight(currentNode.parent, neighbor, collider)) {
                        float newMoveCost = currentNode.parent.gCost + GetStraightDistance(currentNode.parent, neighbor);
                        if(newMoveCost <= neighbor.gCost) {
                            neighbor.gCost = newMoveCost;
                            neighbor.hCost = GetStraightDistance(neighbor, targetNode);
                            neighbor.parent = currentNode.parent;

                            if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                        }
                    } else {
                        float newMoveCost = currentNode.gCost + GetStraightDistance(currentNode, neighbor);
                        if(newMoveCost < neighbor.gCost) {
                            neighbor.gCost = newMoveCost;
                            neighbor.hCost = GetStraightDistance(neighbor, targetNode);
                            neighbor.parent = currentNode;

                            if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                        }
                    }
                }
            }
            return null;
        }

        bool LineOfSight(Node nodeA, Node nodeB) {
            return !Physics2D.Raycast(nodeA.worldPosition, nodeB.worldPosition - nodeA.worldPosition, Vector3.Distance(nodeA.worldPosition, nodeB.worldPosition));
        }

        bool LineOfSight(Node nodeA, Node nodeB, Collider2D collider) {
            //return !Physics2D.Raycast(nodeA.worldPosition, nodeB.worldPosition - nodeA.worldPosition, Vector3.Distance(nodeA.worldPosition, nodeB.worldPosition));
            return !Physics2D.BoxCast(nodeA.worldPosition, collider.bounds.size, 0, (nodeB.worldPosition - nodeA.worldPosition).normalized, Vector2.Distance(nodeA.worldPosition, nodeB.worldPosition));
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

        float GetGridDistance(Node nodeA, Node nodeB) {
            int distX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
            int distY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

            if(distX > distY) {
                return Mathf.Sqrt(Mathf.Pow(grid.nodeSize, 2) * 2) * distY + grid.nodeSize * (distX - distY);
            } else {
                return Mathf.Sqrt(Mathf.Pow(grid.nodeSize, 2) * 2) * distX + grid.nodeSize * (distY - distX);
            }
        }

        float GetStraightDistance(Node nodeA, Node nodeB) {
            return Vector2.Distance(nodeA.worldPosition, nodeB.worldPosition);
        }
    }
}
