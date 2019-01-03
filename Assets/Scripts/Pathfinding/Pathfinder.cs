using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace Pathfinding
{
	[RequireComponent(typeof(PathfindingGrid))]
	public class Pathfinder : MonoBehaviour
	{
		private PathfindingGrid _pathfindingGrid;
		
		void Awake()
		{
			_pathfindingGrid = GetComponent<PathfindingGrid>();
		}
		
		void Start()
		{

		}

		void Update()
		{

		}
		
		public List<PathfindingNode> FindPath(Vector2 startPosition, Vector2 targetPosition, float maxDistance = Mathf.Infinity)
		{
			_pathfindingGrid.Reset();

			var startNode = _pathfindingGrid.NodeAtWorldPosition(startPosition);
			var targetNode = _pathfindingGrid.NodeAtWorldPosition(targetPosition);

			// Try to find valid node if target is not walkable
			if (!targetNode.isWalkable)
			{
				var walkableNeighbors = _pathfindingGrid.GetNeighbors(targetNode).Where(n => n.isWalkable);
				if (!walkableNeighbors.Any())
				{
					return null;
				}

				float minDistToStart = Mathf.Infinity;
				foreach (var node in walkableNeighbors)
				{
					var dist = Vector2.Distance(startPosition, node.worldPosition);
					if (dist < minDistToStart)
					{
						targetNode = node;
						minDistToStart = dist;
					}
				}
			}

			// Find path
			List<PathfindingNode> openSet = new List<PathfindingNode>();
			HashSet<PathfindingNode> closedSet = new HashSet<PathfindingNode>();

			startNode.gCost = 0;
			startNode.parent = startNode;
			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				var currentNode = openSet[0];
				foreach (var node in openSet)
				{
					if (node.fCost < currentNode.fCost
						|| (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
					{
						currentNode = node;
					}
				}

				openSet.Remove(currentNode);
				closedSet.Add(currentNode);

				if (currentNode == targetNode)
				{
					return RetracePath(startNode, targetNode);
				}

				var neighbors = _pathfindingGrid.GetNeighbors(currentNode);
				foreach (var neighbor in neighbors)
				{
					if (!neighbor.isWalkable || closedSet.Contains(neighbor)
						|| neighbor.fCost > maxDistance)
					{
						continue;
					}
					
					// Basic Theta Star update vertex
					if (currentNode.parent != null && _pathfindingGrid.LineOfSight2(currentNode, currentNode.parent))
					{
						float newMoveCost = currentNode.parent.gCost +
						                    Vector2.Distance(currentNode.parent.worldPosition, neighbor.worldPosition);
						if (newMoveCost < neighbor.gCost)
						{
							neighbor.gCost = newMoveCost;
							neighbor.hCost = Vector2.Distance(neighbor.worldPosition, targetNode.worldPosition);
							neighbor.parent = currentNode.parent;
							
							if (!openSet.Contains(neighbor))
								openSet.Add(neighbor);
						}
					}
					else
					{
						float newMoveCost = currentNode.gCost + Vector2.Distance(currentNode.worldPosition, neighbor.worldPosition);
						if (newMoveCost < neighbor.gCost)
						{
							neighbor.gCost = newMoveCost;
							neighbor.hCost = Vector2.Distance(neighbor.worldPosition, targetNode.worldPosition);
							neighbor.parent = currentNode;
					
							if (!openSet.Contains(neighbor))
								openSet.Add(neighbor);
						}
					}
				}
			}

			return null;
		}

		private List<PathfindingNode> RetracePath(PathfindingNode start, PathfindingNode target)
		{
			List<PathfindingNode> path = new List<PathfindingNode>();

			var node = target;
			while (node != null && node != start)
			{
				path.Add(node);
				node = node.parent;
			}

			return path;
		}
	}
}
