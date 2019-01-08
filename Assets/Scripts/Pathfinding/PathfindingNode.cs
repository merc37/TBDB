using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.PlayerLoop;

namespace Pathfinding
{
    public class PathfindingNode
    {
        public float GCost;
        public float HCost;
        public float FCost
        {
            get { return GCost + HCost; }
        }

        public PathfindingNode Parent;
        public bool IsWalkable;

        // Possibly temporary variables, try to remove to increase memory efficiency
        public Vector2Int GridPosition;
        public Vector2 WorldPosition;

        public PathfindingNode(Vector2 worldPosition, Vector2Int gridPosition, bool isWalkable)
        {
            WorldPosition = worldPosition;
            GridPosition = gridPosition;
            IsWalkable = isWalkable;
            Reset();
        }

        public void Reset()
        {
            Parent = null;
            GCost = Mathf.Infinity;
            HCost = Mathf.Infinity;
        }

        internal void DrawGizmos(float nodeRadius, bool showPathCost)
        {
            if(!IsWalkable)
                Gizmos.color = Color.red;
            else if(float.IsPositiveInfinity(FCost))
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.white;

            if(showPathCost)
                Gizmos.DrawWireCube(WorldPosition, Vector3.one * nodeRadius * 1.9f);
            else
                Gizmos.DrawWireCube(WorldPosition, Vector3.one * nodeRadius * 0.5f);

            if(showPathCost && !float.IsPositiveInfinity(FCost))
            {
                Vector3 GUIposition = (Vector3)(WorldPosition + new Vector2(-nodeRadius, nodeRadius)) - Vector3.forward;
                //Handles.Label(GUIposition, "G:" + GCost.ToString("F1"));
                //Handles.Label(GUIposition + Vector3.up * -0.1f, "H:" + HCost.ToString("F1"));
                //Handles.Label(GUIposition + Vector3.up * -0.27f, "F:" + FCost.ToString("F1"));
                Handles.Label(GUIposition + Vector3.up * -0.1f, "X:" + WorldPosition.x);
                Handles.Label(GUIposition + Vector3.up * -0.3f, "Y:" + WorldPosition.y);
            }
        }
    }
}
