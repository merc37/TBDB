using System.Collections;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(LayeredTilemap))]
	public class PathfindingGrid : MonoBehaviour
	{
		[SerializeField] private LayerMask unwalkableMask;
		[SerializeField] private float gridScale = 0.5f;

		[Header("Gizmo Settings")]
		[SerializeField] private bool showOnlyTestedNodes = true;
		
		private LayeredTilemap _layeredTilemap;
		private PathfindingNode[,] _pathfindingGrid;
		
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
			
			_gridSize = new Vector2Int(Mathf.RoundToInt(_layeredTilemap.MapSize.x / gridScale),
									   Mathf.RoundToInt(_layeredTilemap.MapSize.y / gridScale));
			
			_pathfindingGrid = new PathfindingNode[_gridSize.x, _gridSize.y];
			
			for (int y = 0; y < 2*_gridSize.y; y++)
			{
				for (int x = 0; x < 2*_gridSize.x; x++)
				{
					Vector3 worldPosition = transform.position
					                     + Vector3.right * (x * _nodeDiameter + _nodeRadius) - _gridSize.x
					                     - Vector3.up * (y * _nodeDiameter + _nodeRadius);
					
					Vector2Int gridPosition = new Vector2Int(x, y);

					bool isWalkable = true;
					foreach (var layer in _layeredTilemap.Layers)
					{
						var cellPosition = layer.WorldToCell(worldPosition);
						if (layer.GetTile(cellPosition) != null
						    && layer.gameObject.layer == unwalkableMask)
						{
							isWalkable = false;
						}
					}
					
					_pathfindingGrid[x, y] = new PathfindingNode(worldPosition, gridPosition, true);
				}
			}
		}

		void OnDrawGizmos()
		{
			foreach (var node in _pathfindingGrid)
			{
				if (!showOnlyTestedNodes
				    || node.fCost == Mathf.Infinity)
				{
					node.DrawGizmos(_nodeRadius);
				}
			}
		}
	}	
}