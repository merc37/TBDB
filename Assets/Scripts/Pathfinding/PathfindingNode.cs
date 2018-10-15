using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.PlayerLoop;

namespace Pathfinding
{
	[ExecuteInEditMode]
	public class PathfindingNode
	{
		public float gCost;
		public float hCost;
		public float fCost
		{
			get { return gCost + hCost; }
		}

		public PathfindingNode parent;
		public bool isWalkable;

		// Possibly temporary variables, try to remove to increase efficiency
		public Vector2Int gridPosition;
		public Vector3 worldPosition;

		public PathfindingNode(Vector3 worldPosition, Vector2Int gridPosition, bool isWalkable)
		{
			this.worldPosition = worldPosition;
			this.gridPosition = gridPosition;
			this.isWalkable = isWalkable;
			Reset();
		}

		public void Reset()
		{
			parent = null;
			gCost = Mathf.Infinity;
			hCost = Mathf.Infinity;
		}

		internal void DrawGizmos(float nodeRadius)
		{
			if (!isWalkable)
				Gizmos.color = Color.red;
			else if (fCost != Mathf.Infinity)
				Gizmos.color = Color.blue;
			else
				Gizmos.color = Color.white;
			
			Gizmos.DrawWireCube(worldPosition, Vector3.one * nodeRadius);
			
			if(fCost != Mathf.Infinity)
			{
				Vector3 GUIposition = worldPosition + new Vector3(-nodeRadius, nodeRadius);
				Handles.Label(GUIposition, "G:" + gCost.ToString("F1"));
				Handles.Label(GUIposition + Vector3.up * -0.1f, "H:" + hCost.ToString("F1"));
				Handles.Label(GUIposition + Vector3.up * -0.27f, "F:" + fCost.ToString("F1"));
			}
		}
	}
}
