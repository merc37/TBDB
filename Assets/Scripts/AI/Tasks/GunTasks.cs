using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;
using Events;

namespace Enemy
{
    public class GunTasks : MonoBehaviour
    {
        [SerializeField]
        private int lowAmmoThreshold = 0;

        private bool isLowOnAmmo;

        private float gunProjectileSpeed;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        private UnityAction<ParamsObject> onPlayerSendRigidbodyUnityAction;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            onPlayerSendRigidbodyUnityAction = new UnityAction<ParamsObject>(OnPlayerSendRigidbody);
            eventManager.StartListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
            eventManager.StartListening(GunEvents.OnUpdateCurrentAmmo, new UnityAction<ParamsObject>(OnUpdateCurrentAmmo));
            eventManager.StartListening(GunEvents.OnUpdateGunProjectileSpeed, new UnityAction<ParamsObject>(OnUpdateGunProjectileSpeed));
        }

        [Task]
        bool Shoot()
        {
            eventManager.TriggerEvent(GunEvents.OnShoot);
            return true;
        }

        [Task]
        bool Reload()
        {
            eventManager.TriggerEvent(GunEvents.OnReload);
            return true;
        }

        [Task]
        bool AimAtPlayer()
        {
            float intitalProjectileTravelTime = Vector2.Distance(rigidbody.position, playerRigidbody.position) / gunProjectileSpeed;
            Vector2 intialPlayerPositionGuess = (playerRigidbody.position + (playerRigidbody.velocity * (intitalProjectileTravelTime)));
            float projectileTravelTime = Vector2.Distance(rigidbody.position, intialPlayerPositionGuess) / gunProjectileSpeed;
            Vector2 playerPositionGuess = playerRigidbody.position + (playerRigidbody.velocity * (projectileTravelTime));
            Vector2 direction = playerPositionGuess - rigidbody.position;
            rigidbody.rotation = direction.AngleFromZero();
            return true;
        }

        [Task]
        bool LowOnAmmo()
        {
            return isLowOnAmmo;
        }

        private void OnPlayerSendRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening(EnemyEvents.OnPlayerSendRigidbody, onPlayerSendRigidbodyUnityAction);
        }

        private void OnUpdateGunProjectileSpeed(ParamsObject paramsObj)
        {
            gunProjectileSpeed = paramsObj.Float;
        }

        private void OnUpdateCurrentAmmo(ParamsObject paramsObj)
        {
            isLowOnAmmo = paramsObj.Int <= lowAmmoThreshold;
        }
    }
}
