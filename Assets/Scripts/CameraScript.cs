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
        newCameraPosition = new Vector3(0, 0, transform.position.z);
		halfWorldHeight = Camera.main.orthographicSize;
		halfWorldWidth = (Camera.main.aspect * (halfWorldHeight*2))/2;
        map = mapTransform.GetComponent<TiledMap>();
        mapWidth = map.NumTilesWide;
        mapHeight = map.NumTilesHigh;
    }

    private void Update() {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newCameraPosition.Set(playerRigidbody.position.x, playerRigidbody.position.y, newCameraPosition.z);
        //if(playerRigidbody.position.x > mouseWorldPos.x - halfWorldWidth) {
        //    if(playerRigidbody.position.x < mouseWorldPos.x + halfWorldWidth) {
        //        newCameraPosition.Set(mouseWorldPos.x, newCameraPosition.y, newCameraPosition.z);
        //    }
        //}
    }

    void LateUpdate () {
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
