using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using Tiled2Unity;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Tilemaps;

namespace Pathfinding
{
	//[ExecuteInEditMode]
	[RequireComponent(typeof(LayeredTilemap))]
	public class PathfindingGrid : MonoBehaviour
	{
		[SerializeField] private LayerMask unwalkableMask;
		[SerializeField] private float gridScale = 0.5f;

		//[Header("Gizmo Settings")]
		
		private LayeredTilemap _layeredTilemap;
		private PathfindingNode[,] _pathfindingGrid;

		private Vector2 _gridLocation, _gridWorldSize;
		private Vector2Int _gridSize;
		private float _nodeDiameter, _nodeRadius;
		
		void Awake()
		{
			_layeredTilemap = transform.GetComponent<LayeredTilemap>();
			
			InitializeGrid();
		}
		
		void Start ()
		{
			
		}

		[SerializeField] private Vector2 p1;
		[SerializeField] private Vector2 p2;
		private Vector2 t1, t2;
		
		void Update ()
		{
			if (t1 != p1 || t2 != p2)
			{
				t1 = p1;
				t2 = p2;
				LineOfSight2(NodeAtWorldPosition(t1), NodeAtWorldPosition(t2));
			}
		}

		void InitializeGrid()
		{
			_nodeDiameter = gridScale;
			_nodeRadius = _nodeDiameter / 2;

			_gridLocation = _layeredTilemap.MapBounds.min - (_nodeRadius * Vector3.one);
			_gridSize = new Vector2Int(Mathf.RoundToInt(_layeredTilemap.MapBounds.size.x / gridScale) + 1,
									   Mathf.RoundToInt(_layeredTilemap.MapBounds.size.y / gridScale) + 1);
			_gridWorldSize = (Vector2) _gridSize * _nodeDiameter;
			
			_pathfindingGrid = new PathfindingNode[_gridSize.x, _gridSize.y];

			var unwalkableLayers =
				_layeredTilemap.Layers.Where(layer => unwalkableMask == (unwalkableMask | 1 << layer.gameObject.layer));
			
			for (int y = 0; y < _gridSize.y; y++)
			{
				for (int x = 0; x < _gridSize.x; x++)
				{
					Vector2 worldPosition = _gridLocation
											+ Vector2.right * (x * _nodeDiameter + _nodeRadius)
											+ Vector2.up * (y * _nodeDiameter + _nodeRadius);
					
					Vector2Int gridPosition = new Vector2Int(x, y);
					
					var overlappingCells = _layeredTilemap.GetCellsOverlappingArea(worldPosition, _nodeRadius);

					bool isWalkable = true;
					foreach (var layer in unwalkableLayers)
					{
						foreach (var cell in overlappingCells)
						{
							var cell3 = new Vector3Int(cell.x, cell.y, 0);
							if (layer.GetTile(cell3) != null)
							{
								isWalkable = false;
							}
						}
					}
					
					_pathfindingGrid[x, y] = new PathfindingNode(worldPosition, gridPosition, isWalkable);
				}
			}
		}

		public List<PathfindingNode> GetNeighbors(PathfindingNode node)
		{
			List<PathfindingNode> neighbors = new List<PathfindingNode>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
						continue;

					int checkX = node.gridPosition.x + x;
					int checkY = node.gridPosition.y + y;

					if (checkX >= 0 && checkX < _gridSize.x &&
					    checkY >= 0 && checkY < _gridSize.y)
					{
						neighbors.Add( NodeAtGridPosition(checkX, checkY) );
					}
				}
			}

			return neighbors;
		}

		public PathfindingNode NodeAtGridPosition(int gridPositionX, int gridPositionY)
		{
			return _pathfindingGrid[gridPositionX, gridPositionY];
		}
		
		public PathfindingNode NodeAtGridPosition(Vector2Int gridPosition)
		{
			return NodeAtGridPosition(gridPosition.x, gridPosition.y);
		}

		public PathfindingNode NodeAtWorldPosition(Vector2 worldPosition)
		{
			var relativePosition = worldPosition - _gridLocation;
			
			if (relativePosition.x < 0 || relativePosition.x > _gridWorldSize.x ||
			    relativePosition.y < 0 || relativePosition.y > _gridWorldSize.y)
			{
				throw new ArgumentOutOfRangeException("Requested world position is not within the grid.");
			}

			float percentX = Mathf.Clamp01(relativePosition.x / _gridWorldSize.x);
			float percentY = Mathf.Clamp01(relativePosition.y / _gridWorldSize.y);
			int x = Mathf.RoundToInt(percentX * (_gridSize.x - 1));
			int y = Mathf.RoundToInt(percentY * (_gridSize.y - 1));

			return NodeAtGridPosition(x, y);
		}

		//private List<PathfindingNode> test;
		
		public bool LineOfSight(PathfindingNode nodeA, PathfindingNode nodeB, float width = 0)
		{
			
			// if slope > 1 sample at ys
			// if slope < 1 samlpe at xs
			
			bool transposeResult = false;
			Vector2Int n1, n2;
			
			var vect = nodeB.gridPosition - nodeA.gridPosition;
			if (Mathf.Abs(vect.y) > Mathf.Abs(vect.x))
			{
				transposeResult = true;
				n1 = new Vector2Int(nodeA.gridPosition.y, nodeA.gridPosition.x);
				n2 = new Vector2Int(nodeB.gridPosition.y, nodeB.gridPosition.x);
			}
			else
			{
				n1 = nodeA.gridPosition;
				n2 = nodeB.gridPosition;
			}

			vect = n2 - n1;
			float slope = (float) vect.y / vect.x;
			
			HashSet<PathfindingNode> intersectedNodes = new HashSet<PathfindingNode>();

			for (float ix = 0.5f; ix <= vect.x; ix++)
			{
				float y = n1.y + slope * ix;

				Vector2Int left = new Vector2Int();
				Vector2Int right = new Vector2Int();

				left.x = Mathf.FloorToInt(n1.x + ix);
				right.x = Mathf.CeilToInt(n1.x + ix);
				
				if (y % 0.5f == 0)
				{
					left.y = Mathf.FloorToInt(y);
					right.y = Mathf.CeilToInt(y);
				}
				else
				{
					left.y = Mathf.RoundToInt(y);
					right.y = left.y;
				}

				if (transposeResult)
				{
					var t = left.x;
					left.x = left.y;
					left.y = t;

					t = right.x;
					right.x = right.y;
					right.y = t;
				}

				intersectedNodes.Add(NodeAtGridPosition(left));
				intersectedNodes.Add(NodeAtGridPosition(right));
			}

			//test = intersectedNodes.ToList();

			return false;
		}

		//private List<Vector2> minor;

		public bool LineOfSight2(PathfindingNode nodeA, PathfindingNode nodeB, float width = 0)
		{
			int limit = 100;
			int count = 0;
			//minor = new List<Vector2>();
			
			List<PathfindingNode> intersectedNodes = new List<PathfindingNode>();
			
			var dir = ((Vector2) (nodeB.gridPosition - nodeA.gridPosition)).normalized;
			int signX = Math.Sign(dir.x);
			int signY = Math.Sign(dir.y);
			float slope = dir.y / dir.x;

			var currentCell = nodeA;
			Vector2 lastIntersection = nodeA.gridPosition;
			float nextX = nodeA.gridPosition.x + 0.5f * signX;
			float nextY = nodeA.gridPosition.y + 0.5f * signY;
			
			intersectedNodes.Add(currentCell);
			
			while (currentCell != nodeB)
			{
				if (count >= limit)
					return false;

				float y = (nextX - lastIntersection.x) * slope + lastIntersection.y;
				Vector2 xInt = new Vector2(nextX, y);
				float tx = dir.x == 0 ? Mathf.Infinity : Vector2.Distance(lastIntersection, xInt);

				float x = (nextY - lastIntersection.y) / slope + lastIntersection.x;
				Vector2 yInt = new Vector2(x, nextY);
				float ty = dir.y == 0 ? Mathf.Infinity : Vector2.Distance(lastIntersection, yInt);

				if (tx == ty)
				{
					
					currentCell = NodeAtGridPosition( 	(signX == 1 ? Mathf.CeilToInt(nextX) : Mathf.FloorToInt(nextX)),
														(signY == 1 ? Mathf.CeilToInt(nextY) : Mathf.FloorToInt(nextY))		);
					intersectedNodes.Add(currentCell);
					intersectedNodes.Add(NodeAtGridPosition(Mathf.FloorToInt(nextX), Mathf.CeilToInt(nextY)));
					intersectedNodes.Add(NodeAtGridPosition(Mathf.CeilToInt(nextX), Mathf.FloorToInt(nextY)));
					lastIntersection = xInt;
					nextX += 1 * signX;
					nextY += 1 * signY;

					//minor.Add(xInt);
				}
				else if (tx < ty)
				{
					currentCell = NodeAtGridPosition( 	(signX == 1 ? Mathf.CeilToInt(nextX) : Mathf.FloorToInt(nextX)),
														(signY == 1 ? Mathf.FloorToInt(nextY) : Mathf.CeilToInt(nextY))		);
					intersectedNodes.Add(currentCell);
					lastIntersection = xInt;
					nextX += 1 * signX;
					
					//minor.Add(xInt);
				}
				else
				{
					currentCell = NodeAtGridPosition( 	(signX == 1 ? Mathf.FloorToInt(nextX) : Mathf.CeilToInt(nextX)),
														(signY == 1 ? Mathf.CeilToInt(nextY) : Mathf.FloorToInt(nextY))		);
					intersectedNodes.Add(currentCell);
					lastIntersection = yInt;
					nextY += 1 * signY;
					
					//minor.Add(yInt);
				}

				count++;
			}

			//test = intersectedNodes;

			return intersectedNodes.All(n => n.isWalkable);
		}
		
		public void Reset()
		{
			foreach (var node in _pathfindingGrid)
			{
				node.Reset();
			}
		}

		void OnDrawGizmosSelected()
		{
			if (_pathfindingGrid != null)
			{
				foreach (var node in _pathfindingGrid)
				{
					node.DrawGizmos(_nodeRadius, false);
				}
			}

			/*Gizmos.color = Color.yellow;
			if (test != null)
			{
				Gizmos.DrawLine(t1, t2);
				foreach (var node in test)
				{
					Gizmos.DrawWireSphere(node.worldPosition, _nodeRadius/2);
				}
			}

			Gizmos.DrawWireSphere(_gridLocation, .02f);
			if (minor != null)
			{
				Gizmos.color = Color.cyan;
				foreach (var v in minor)
				{
					var c = _gridLocation + Vector2.one * _nodeDiameter * 0.5f + v * 0.5f;
					Gizmos.DrawWireSphere(c, _nodeRadius/4);
				}
			}*/
		}
	}	
}