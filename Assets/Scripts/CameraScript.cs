using UnityEngine;
using UnityEngine.Events;
using Tiled2Unity;
using EventManagers;

public class CameraScript : MonoBehaviour {
    
	private Rigidbody2D playerRigidbody;
    private LayeredTilemap map;
	private Vector3 newCameraPosition;
	private float halfWorldWidth;
	private float halfWorldHeight;
	private Transform mapTransform;
	private float mapWidth, mapHeight;

    void Awake() {
        GlobalEventManager.StartListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SetPlayerTransform));
        GlobalEventManager.StartListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetMapTransform));
    }
    
    void Start () {
        GlobalEventManager.TriggerEvent("RequestPlayerTransform");
        GlobalEventManager.TriggerEvent("RequestMapTransform");
        newCameraPosition = new Vector3(0, 0, -10);
		halfWorldHeight = Camera.main.orthographicSize;
		halfWorldWidth = (Camera.main.aspect * (halfWorldHeight*2))/2;
        map = mapTransform.GetComponent<LayeredTilemap>();
        mapWidth = map.MapSize.x;
        mapHeight = map.MapSize.y;
    }
    
	void LateUpdate () {
        newCameraPosition.Set(playerRigidbody.position.x, playerRigidbody.position.y, -10);
        if(newCameraPosition.x - halfWorldWidth < mapTransform.position.x - mapWidth / 2) {
            newCameraPosition.x = (mapTransform.position.x - mapWidth / 2) + halfWorldWidth;
        }
        if(newCameraPosition.x + halfWorldWidth > mapTransform.position.x + mapWidth / 2) {
            newCameraPosition.x = (mapTransform.position.x + mapWidth / 2) - halfWorldWidth;
        }
        if(newCameraPosition.y + halfWorldHeight > mapTransform.position.y + mapHeight / 2) {
            newCameraPosition.y = (mapTransform.position.y + mapHeight / 2) - halfWorldHeight;
        }
        if(newCameraPosition.y - halfWorldHeight < mapTransform.position.y - mapHeight / 2) {
            newCameraPosition.y = (mapTransform.position.y - mapHeight / 2) + halfWorldHeight;
        }
        transform.position = newCameraPosition;
    }

    private void SetPlayerTransform(ParamsObject paramsObj) {
        playerRigidbody = paramsObj.Transform.GetComponent<Rigidbody2D>();
        GlobalEventManager.StopListening("ReturnPlayerTransform", new UnityAction<ParamsObject>(SetPlayerTransform));
    }

    private void SetMapTransform(ParamsObject paramsObj) {
        mapTransform = paramsObj.Transform;
        GlobalEventManager.StopListening("ReturnMapTransform", new UnityAction<ParamsObject>(SetMapTransform));
    }
}
