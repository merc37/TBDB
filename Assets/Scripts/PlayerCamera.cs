using UnityEngine;
using UnityEngine.Events;
using EventManagers;
using Events;

public class PlayerCamera : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private Vector3 newCameraPosition;

    private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;

    void Awake()
    {
        onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
        GlobalEventManager.StartListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        newCameraPosition = new Vector3(0, 0, transform.position.z);
    }

    void Update()
    {
        newCameraPosition.Set(playerRigidbody.position.x, playerRigidbody.position.y, newCameraPosition.z);
    }

    void LateUpdate()
    {
        transform.position = newCameraPosition;
    }

    private void OnPlayerSendRigidbody(ParamsObject paramsObj)
    {
        playerRigidbody = paramsObj.Rigidbody;
        GlobalEventManager.StopListening(PlayerGlobalEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
    }
}
