using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiled2Unity;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
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
	
		void Update ()
		{
		
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
		}
	}	
}