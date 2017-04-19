using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Tiled2Unity;

public class CameraScript : MonoBehaviour {

	[SerializeField]
	private Transform player;
	[SerializeField]
	private TiledMap map;

	private Vector3 newCameraPosition;
	private float halfWorldWidth;
	private float halfWorldHeight;
	private Transform mapTransform;
	private float mapWidth, mapHeight;

	// Use this for initialization
	void Start () {
		newCameraPosition = new Vector3(0, 0, -10);
		halfWorldHeight = Camera.main.orthographicSize;
		halfWorldWidth = (Camera.main.aspect * (halfWorldHeight*2))/2;
		mapTransform = map.GetComponent<Transform>();
		mapWidth = map.NumTilesWide+1.3025f;
		mapHeight = map.NumTilesHigh+1.3025f;
	}

	// LateUpdate is called once per frame, but after all other updates
	void LateUpdate () {
		newCameraPosition.Set(player.position.x, player.position.y, -10);
		if(newCameraPosition.x-halfWorldWidth < mapTransform.position.x) {
			newCameraPosition.x = mapTransform.position.x + halfWorldWidth;
		}
		if(newCameraPosition.x+halfWorldWidth > mapTransform.position.x+mapWidth) {
			newCameraPosition.x = (mapTransform.position.x + mapWidth) - halfWorldWidth;
		}
		if(newCameraPosition.y+halfWorldHeight > mapTransform.position.y) {
			newCameraPosition.y = mapTransform.position.y - halfWorldHeight;
		}
		if(newCameraPosition.y-halfWorldHeight < mapTransform.position.y-mapHeight) {
			newCameraPosition.y = (mapTransform.position.y-mapHeight) + halfWorldHeight;
		}
		transform.position = newCameraPosition;
	}
}
