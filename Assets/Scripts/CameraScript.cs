using UnityEngine;
using UnityEngine.Events;
using EventManagers;
using Events;

public class CameraScript : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Vector3 newCameraPosition;
    private float halfWorldWidth;
    private float halfWorldHeight;
    private Transform mapTransform;
    private float mapWidth, mapHeight;

    private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;
    private UnityAction<ParamsObject> onMapSendTransformUnityAction;

    void Awake()
    {
        onMapSendTransformUnityAction = new UnityAction<ParamsObject>(OnMapSendTransform);
        GlobalEventManager.StartListening(MapGlobalEvents.OnMapSendTransform, onMapSendTransformUnityAction);
        onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
    }

    void Start()
    {
        newCameraPosition = new Vector3(0, 0, transform.position.z);
        halfWorldHeight = Camera.main.orthographicSize;
        halfWorldWidth = (Camera.main.aspect * (halfWorldHeight * 2)) / 2;
    }

    private void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newCameraPosition.Set(playerRigidbody.position.x, playerRigidbody.position.y, newCameraPosition.z);
        //if(playerRigidbody.position.x > mouseWorldPos.x - halfWorldWidth) {
        //    if(playerRigidbody.position.x < mouseWorldPos.x + halfWorldWidth) {
        //        newCameraPosition.Set(mouseWorldPos.x, newCameraPosition.y, newCameraPosition.z);
        //    }
        //}
    }

    void LateUpdate()
    {
        transform.position = newCameraPosition;
    }

    /*void LateUpdate () {
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
    }*/

    private void OnPlayerSendRigidbody(ParamsObject paramsObj)
    {
        playerRigidbody = paramsObj.Rigidbody;
        GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
    }

    private void OnMapSendTransform(ParamsObject paramsObj)
    {
        //map = paramsObj.Transform.GetComponent<TiledMap>();
        //mapWidth = map.NumTilesWide;
        //mapHeight = map.NumTilesHigh;
        GlobalEventManager.StopListening(MapGlobalEvents.OnMapSendTransform, onMapSendTransformUnityAction);
    }
}
