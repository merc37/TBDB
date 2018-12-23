using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Tiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityScript.Scripting;

//[ExecuteInEditMode]
public class LayeredTilemap : MonoBehaviour
{
    private UnityEngine.Grid _grid;
    public UnityEngine.Grid Grid
    {
        get { return _grid; }
    }
    
    private Tilemap[] _tilemapLayers;
    public Tilemap[] Layers
    {
        get { return _tilemapLayers; }
    }

    private BoundsInt _mapBounds;
    public BoundsInt MapBounds
    {
        get { return _mapBounds; }
    }

    private Vector3 _centerOffsetFromTransform;

    void Awake()
    {
        _grid = GetComponent<UnityEngine.Grid>();
        _tilemapLayers = GetComponentsInChildren<Tilemap>();

        ComputeBoundsFromLayers();
        _centerOffsetFromTransform = _mapBounds.center - transform.position;
    }

    private void ComputeBoundsFromLayers()
    {
        _mapBounds = new BoundsInt();
        
        foreach (Tilemap layer in _tilemapLayers)
        {
            //layer.CompressBounds();
            _mapBounds.min = Vector3Int.Min(layer.cellBounds.min, _mapBounds.min);
            _mapBounds.max = Vector3Int.Max(layer.cellBounds.max, _mapBounds.max);
        }

        _mapBounds.min += Vector3Int.FloorToInt(transform.position);
        _mapBounds.max += Vector3Int.CeilToInt(transform.position);
    }

    /// <summary>
    /// Returns the cell positions overlapped by the box defined by max and min.
    /// </summary>
    /// <param name="min">Bottom left corner in world space.</param>
    /// <param name="max">Top right corner in world space.</param>
    /// <returns>List of cell positions.</returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public List<Vector2Int> GetCellsOverlappingArea(Vector2 min, Vector2 max)
    {
        if (Vector2.Min(min, max) != min)
        {
            throw new InvalidEnumArgumentException("min is not strictly less than max");
        }

        List<Vector2Int> cells = new List<Vector2Int>();

        var minCell = _grid.WorldToCell(min);
        var maxCell = _grid.WorldToCell(max);
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                cells.Add(new Vector2Int(x, y));
            }
        }

        return cells;
    }

    public List<Vector2Int> GetCellsOverlappingArea(Vector2 center, float radius)
    {
        Vector2 min = center - (radius * Vector2.one);
        Vector2 max = center + (radius * Vector2.one);
        return GetCellsOverlappingArea(min, max);
    }

    void OnDrawGizmosSelected()
    {
        // Draw bounds
        Gizmos.color = Color.cyan;
        
        var topLeft = _mapBounds.min + (_mapBounds.size.y * Vector3.up);
        var botRight = _mapBounds.min + (_mapBounds.size.x * Vector3.right);
        Gizmos.DrawLine(_mapBounds.min, topLeft);
        Gizmos.DrawLine(topLeft, _mapBounds.max);
        Gizmos.DrawLine(_mapBounds.max, botRight);
        Gizmos.DrawLine(botRight, _mapBounds.min);
        
        Gizmos.DrawLine(topLeft, botRight);
        Gizmos.DrawLine(_mapBounds.min, _mapBounds.max);
    }
}
