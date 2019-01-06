using EventManagers;
using Panda;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public class GunTasks : MonoBehaviour
    {
        [SerializeField]
        private int lowAmmoThreshold = 0;

        private bool isLowOnAmmo;

        private float gunProjectileSpeed;

        private Transform gunTransform;

        private GameObjectEventManager eventManager;
        private Rigidbody2D playerRigidbody;
        private new Rigidbody2D rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();

            eventManager = GetComponent<GameObjectEventManager>();
            eventManager.StartListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
            eventManager.StartListening("AmmoCount", new UnityAction<ParamsObject>(OnAmmoCountUpdate));
            eventManager.StartListening("UpdateGunInfo", new UnityAction<ParamsObject>(OnGunInfoUpdate));
        }

        [Task]
        bool Shoot()
        {
            eventManager.TriggerEvent("OnShoot");
            return true;
        }

        [Task]
        bool Reload()
        {
            eventManager.TriggerEvent("OnReload");
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

        private void SetPlayerRigidbody(ParamsObject paramsObj)
        {
            playerRigidbody = paramsObj.Rigidbody;
            eventManager.StopListening("ReturnPlayerRigidbody", new UnityAction<ParamsObject>(SetPlayerRigidbody));
        }

        private void OnGunInfoUpdate(ParamsObject paramsObj)
        {
            gunProjectileSpeed = paramsObj.Float;
            gunTransform = paramsObj.Transform;
        }

        private void OnAmmoCountUpdate(ParamsObject paramsObj)
        {
            isLowOnAmmo = paramsObj.Int <= lowAmmoThreshold;
        }
    }
}
