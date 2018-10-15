using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityScript.Scripting;

[ExecuteInEditMode]
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

    private Vector2Int _mapSize;
    public Vector2Int MapSize
    {
        get { return _mapSize; }
    }

    private Vector3Int _mapCenterTile;
    public Vector3Int MapCenterTile
    {
        get { return _mapCenterTile; }
    }

    void Awake()
    {
        _grid = GetComponent<UnityEngine.Grid>();
        _tilemapLayers = GetComponentsInChildren<Tilemap>();

        _mapSize = new Vector2Int();
        _bottomMostTile = new Vector3Int();
        foreach (Tilemap layer in _tilemapLayers)
        {
            if (layer.size.x > _mapSize.x)
                _mapSize.x = layer.size.x;

            if (layer.size.y > _mapSize.y)
                _mapSize.y = layer.size.y;
        }

        _mapCenterTile = _grid.WorldToCell(transform.position);
    }
}
