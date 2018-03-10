﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public Vector3 worldPosition;
	public IntVector2 gridPos;
	public bool isWalkable;

	public float gCost;
	public float hCost;
	public Node parent;

	public float fCost {
		get {
			return gCost + hCost;
		}
	}

	public Node(Vector3 worldPosition, int gridX, int gridY, bool isWalkable) {
		this.worldPosition = worldPosition;
		this.gridPos = new IntVector2(gridX, gridY);
		this.isWalkable = isWalkable;
	}

	public void reset() {
		this.parent = null;
		this.gCost = 0;
		this.hCost = 0;
	}
}
