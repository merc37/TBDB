using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
    public class LazyThetaStarPathfinding : MonoBehaviour {

        private Grid grid;

        void Awake() {
            grid = GetComponent<Grid>();
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, Collider2D collider, float maxDistance) {

            Node startNode = grid.NodeFromWorldPoint(startPos);
            //print("Pos: " + startPos);
            //print("Node: " + startNode.worldPosition);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);

            if(!targetNode.isWalkable) {//Maybe?
                return null;
            }

            grid.resetGrid();

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

                if(currentNode.parent != null && !LineOfSight(currentNode.parent, currentNode)) {
                    currentNode.gCost = Mathf.Infinity;
                    foreach(Node neighbor in grid.GetNeighbors(currentNode)) {
                        if(neighbor.parent != null && !closedSet.Contains(neighbor.parent)) {
                            float newMoveCost = neighbor.parent.gCost + GetStraightDistance(currentNode, neighbor);
                            if(newMoveCost <= currentNode.gCost) {
                                currentNode.gCost = newMoveCost;
                                currentNode.parent = neighbor.parent;
                                currentNode.hCost = GetStraightDistance(neighbor, targetNode);
                            }


                        }
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == targetNode) {
                    // Path found, return path
                    return RetracePath(startNode, targetNode);
                }

                foreach(Node neighbor in grid.GetNeighbors(currentNode)) {

                    if(!neighbor.isWalkable || closedSet.Contains(neighbor)) {
                        continue;
                    }

                    //
                    float newMoveCost = currentNode.gCost + GetStraightDistance(currentNode, neighbor);
                    if(newMoveCost < neighbor.gCost) {
                        neighbor.parent = currentNode.parent;
                        neighbor.gCost = currentNode.parent.gCost + GetStraightDistance(currentNode, neighbor);
                        neighbor.hCost = GetStraightDistance(neighbor, targetNode);
                    }

                    if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
            return null;
        }

        bool LineOfSight(Node nodeA, Node nodeB) {
            return !Physics2D.Raycast(nodeA.worldPosition, nodeB.worldPosition - nodeA.worldPosition, Vector3.Distance(nodeA.worldPosition, nodeB.worldPosition));
        }

        bool LineOfSight(Node nodeA, Node nodeB, Collider2D collider) {
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

        float GetStraightDistance(Node nodeA, Node nodeB) {
            return Vector2.Distance(nodeA.worldPosition, nodeB.worldPosition);
        }
    }
}
