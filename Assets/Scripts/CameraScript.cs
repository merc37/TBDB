using UnityEngine;
using UnityEngine.Events;
using Tiled2Unity;
using EventManagers;

public class CameraScript : MonoBehaviour {
    
	private Rigidbody2D playerRigidbody;
	private TiledMap map;
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
        map = mapTransform.GetComponent<TiledMap>();
        mapWidth = map.NumTilesWide;
        mapHeight = map.NumTilesHigh;
    }
    
	void LateUpdate () {
        newCameraPosition.Set(playerRigidbody.position.x, playerRigidbody.position.y, -10);
        if(newCameraPosition.x - halfWorldWidth < mapTransform.position.x) {
            newCameraPosition.x = mapTransform.position.x + halfWorldWidth;
        }
        if(newCameraPosition.x + halfWorldWidth > mapTransform.position.x + mapWidth) {
            newCameraPosition.x = (mapTransform.position.x + mapWidth) - halfWorldWidth;
        }
        if(newCameraPosition.y + halfWorldHeight > mapTransform.position.y) {
            newCameraPosition.y = mapTransform.position.y - halfWorldHeight;
        }
        if(newCameraPosition.y - halfWorldHeight < mapTransform.position.y - mapHeight) {
            newCameraPosition.y = (mapTransform.position.y - mapHeight) + halfWorldHeight;
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
