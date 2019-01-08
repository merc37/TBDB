using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
    [RequireComponent(typeof(LayeredTilemap))]
    public class PathfindingGrid : MonoBehaviour
    {
        [SerializeField] private LayerMask _unwalkableMask;
        [SerializeField] private float _gridScale = 0.5f;

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

        void Start()
        {

        }

        void Update()
        {

        }

        void InitializeGrid()
        {
            _nodeDiameter = _gridScale;
            _nodeRadius = _nodeDiameter / 2;

            _gridLocation = _layeredTilemap.MapBounds.min - (_nodeRadius * Vector3.one);
            _gridSize = new Vector2Int(Mathf.RoundToInt(_layeredTilemap.MapBounds.size.x / _gridScale) + 1,
                                       Mathf.RoundToInt(_layeredTilemap.MapBounds.size.y / _gridScale) + 1);
            _gridWorldSize = (Vector2)_gridSize * _nodeDiameter;

            _pathfindingGrid = new PathfindingNode[_gridSize.x, _gridSize.y];

            var unwalkableLayers =
                _layeredTilemap.Layers.Where(layer => _unwalkableMask == (_unwalkableMask | 1 << layer.gameObject.layer));

            for(int y = 0; y < _gridSize.y; y++)
            {
                for(int x = 0; x < _gridSize.x; x++)
                {
                    Vector2 worldPosition = _gridLocation
                                            + Vector2.right * (x * _nodeDiameter + _nodeRadius)
                                            + Vector2.up * (y * _nodeDiameter + _nodeRadius);

                    Vector2Int gridPosition = new Vector2Int(x, y);

                    var overlappingCells = _layeredTilemap.GetCellsOverlappingArea(worldPosition, _nodeRadius);

                    bool isWalkable = true;
                    foreach(var layer in unwalkableLayers)
                    {
                        foreach(var cell in overlappingCells)
                        {
                            var cellIndex = new Vector3Int(cell.x, cell.y, 0);
                            if(layer.GetTile(cellIndex) != null)
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

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0)
                        continue;

                    int checkX = node.GridPosition.x + x;
                    int checkY = node.GridPosition.y + y;

                    if(checkX >= 0 && checkX < _gridSize.x &&
                        checkY >= 0 && checkY < _gridSize.y)
                    {
                        neighbors.Add(NodeAtGridPosition(checkX, checkY));
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

            if(relativePosition.x < 0 || relativePosition.x > _gridWorldSize.x ||
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

        // TODO Add support for LineOfSight to take line width into account
        public bool LineOfSight(PathfindingNode nodeA, PathfindingNode nodeB)
        {
            // Simple limit to the number of cells that can checked in one call
            // Prevents runaway computation
            // TODO Make sure excecution always terminates so this hard limit can be removed
            int limit = 100;
            int count = 0;

            List<PathfindingNode> intersectedNodes = new List<PathfindingNode>();

            var dir = ((Vector2)(nodeB.GridPosition - nodeA.GridPosition)).normalized;
            int signX = Math.Sign(dir.x);
            int signY = Math.Sign(dir.y);
            float slope = dir.y / dir.x;

            var currentCell = nodeA;
            Vector2 lastIntersection = nodeA.GridPosition;
            float nextX = nodeA.GridPosition.x + 0.5f * signX;
            float nextY = nodeA.GridPosition.y + 0.5f * signY;

            intersectedNodes.Add(currentCell);

            while(currentCell != nodeB)
            {
                if(count >= limit)
                    return false;

                float y = (nextX - lastIntersection.x) * slope + lastIntersection.y;
                Vector2 xInt = new Vector2(nextX, y);
                float tx = dir.x == 0 ? Mathf.Infinity : Vector2.Distance(lastIntersection, xInt);

                float x = (nextY - lastIntersection.y) / slope + lastIntersection.x;
                Vector2 yInt = new Vector2(x, nextY);
                float ty = dir.y == 0 ? Mathf.Infinity : Vector2.Distance(lastIntersection, yInt);

                if(tx == ty)
                {
                    currentCell = NodeAtGridPosition((signX == 1 ? Mathf.CeilToInt(nextX) : Mathf.FloorToInt(nextX)),
                                                        (signY == 1 ? Mathf.CeilToInt(nextY) : Mathf.FloorToInt(nextY)));
                    intersectedNodes.Add(currentCell);
                    intersectedNodes.Add(NodeAtGridPosition(Mathf.FloorToInt(nextX), Mathf.CeilToInt(nextY)));
                    intersectedNodes.Add(NodeAtGridPosition(Mathf.CeilToInt(nextX), Mathf.FloorToInt(nextY)));
                    lastIntersection = xInt;
                    nextX += 1 * signX;
                    nextY += 1 * signY;
                }
                else if(tx < ty)
                {
                    currentCell = NodeAtGridPosition((signX == 1 ? Mathf.CeilToInt(nextX) : Mathf.FloorToInt(nextX)),
                                                        (signY == 1 ? Mathf.FloorToInt(nextY) : Mathf.CeilToInt(nextY)));
                    intersectedNodes.Add(currentCell);
                    lastIntersection = xInt;
                    nextX += 1 * signX;
                }
                else
                {
                    currentCell = NodeAtGridPosition((signX == 1 ? Mathf.FloorToInt(nextX) : Mathf.CeilToInt(nextX)),
                                                        (signY == 1 ? Mathf.CeilToInt(nextY) : Mathf.FloorToInt(nextY)));
                    intersectedNodes.Add(currentCell);
                    lastIntersection = yInt;
                    nextY += 1 * signY;
                }

                count++;
            }

            return intersectedNodes.All(n => n.IsWalkable);
        }

        public void Reset()
        {
            foreach(var node in _pathfindingGrid)
            {
                node.Reset();
            }
        }

        void OnDrawGizmosSelected()
        {
            if(_pathfindingGrid != null)
            {
                foreach(var node in _pathfindingGrid)
                {
                    node.DrawGizmos(_nodeRadius, false);
                }
            }
        }
    }
}