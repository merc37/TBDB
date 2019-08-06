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
        [SerializeField]
        private LayerMask gunProjectileBlockMask;

        private bool isLowOnAmmo;
        private bool isAmmoZero;
        private bool isReloading;
        private bool readyToFire;

        private bool gunLineOfSightToPlayer;
        private bool gunLineOfSightToPlayerAimTarget;

        private int gunLineOfSightToPlayerPreviousFrameCount;
        private int playerAimTargetLineOfSightPreviousFrameCount;
        private int gunLineOfSightToPlayerAimTargetPreviousFrameCount;

        private float gunProjectileSpeed;

        private Vector2 aimTarget;

        private Rigidbody2D gunProjectile;
        private Transform gunTransform;

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
            eventManager.StartListening(GunEvents.OnReloadStart, new UnityAction<ParamsObject>(OnReloadStart));
            eventManager.StartListening(GunEvents.OnReloadEnd, new UnityAction<ParamsObject>(OnReloadEnd));
            eventManager.StartListening(GunEvents.OnUnlockFire, new UnityAction<ParamsObject>(OnUnlockFire));
            eventManager.StartListening(GunEvents.OnLockFire, new UnityAction<ParamsObject>(OnLockFire));
            eventManager.StartListening(GunEvents.OnUpdateGunProjectile, new UnityAction<ParamsObject>(OnUpdateGunProjectile));
            eventManager.StartListening(GunEvents.OnUpdateGunTransform, new UnityAction<ParamsObject>(OnUpdateGunTransform));
        }

        [Task]
        bool Shoot()
        {
            eventManager.TriggerEvent(GunEvents.OnShoot, new ParamsObject(aimTarget));
            return true;
        }

        [Task]
        bool Reload()
        {
            eventManager.TriggerEvent(GunEvents.OnReload);
            return true;
        }

        [Task]
        bool SetAimTargetToPlayer()
        {
            float intitalProjectileTravelTime = Vector2.Distance(rigidbody.position, playerRigidbody.position) / gunProjectileSpeed;
            Vector2 intialPlayerPositionGuess = (playerRigidbody.position + (playerRigidbody.velocity * (intitalProjectileTravelTime)));
            float projectileTravelTime = Vector2.Distance(rigidbody.position, intialPlayerPositionGuess) / gunProjectileSpeed;
            aimTarget = playerRigidbody.position + (playerRigidbody.velocity * (projectileTravelTime));
            return true;
        }

        [Task]
        bool SetRotationToAimTarget()
        {
            Vector2 direction = aimTarget - rigidbody.position;
            rigidbody.rotation = direction.ToAngle();
            return true;
        }

        [Task]
        bool SetRotationToAimTargetLimited(float degrees)
        {
            Vector2 direction = aimTarget - rigidbody.position;
            float angle = direction.ToAngle();
            float degreeDistance = Mathf.Abs(rigidbody.rotation - angle);
            if (degreeDistance > degrees)
            {
                rigidbody.rotation += Mathf.Sign(rigidbody.rotation - angle) * degrees;
                return true;
            }
            rigidbody.rotation = angle;
            return true;
        }

        [Task]
        bool GunLineOfSightToAimTarget()
        {
            if (Time.frameCount == gunLineOfSightToPlayerAimTargetPreviousFrameCount)
            {
                return gunLineOfSightToPlayerAimTarget;
            }

            gunLineOfSightToPlayerAimTargetPreviousFrameCount = Time.frameCount;

            Collider2D projectileCollider = gunProjectile.gameObject.GetComponent<Collider2D>();
            Vector2 origin = gunTransform.GetChild(0).position;
            Vector2 direction = aimTarget - origin;
            float distance = Vector2.Distance(aimTarget, origin);
            if (projectileCollider.IsBoxCollider())
            {
                gunLineOfSightToPlayerAimTarget = !Physics2D.BoxCast(origin, ((BoxCollider2D)projectileCollider).size, direction.ToAngle(), direction, distance, gunProjectileBlockMask);
                return gunLineOfSightToPlayerAimTarget;
            }
            else
            {
                gunLineOfSightToPlayerAimTarget = !Physics2D.CircleCast(origin, ((CircleCollider2D)projectileCollider).radius, direction, distance, gunProjectileBlockMask);
                return gunLineOfSightToPlayerAimTarget;
            }
        }

        [Task]
        bool GunLineOfSightToPlayer()
        {
            if (Time.frameCount == gunLineOfSightToPlayerPreviousFrameCount)
            {
                return gunLineOfSightToPlayer;
            }

            gunLineOfSightToPlayerPreviousFrameCount = Time.frameCount;

            Collider2D projectileCollider = gunProjectile.gameObject.GetComponent<Collider2D>();
            Vector2 origin = gunTransform.GetChild(0).position;
            Vector2 direction = playerRigidbody.position - origin;
            float distance = Vector2.Distance(playerRigidbody.position, origin);
            if (projectileCollider.IsBoxCollider())
            {
                gunLineOfSightToPlayer = !Physics2D.BoxCast(origin, ((BoxCollider2D)projectileCollider).size, direction.ToAngle(), direction, distance, gunProjectileBlockMask);
                return gunLineOfSightToPlayer;
            }
            else
            {
                gunLineOfSightToPlayer = !Physics2D.CircleCast(origin, ((CircleCollider2D)projectileCollider).radius, direction, distance, gunProjectileBlockMask);
                return gunLineOfSightToPlayer;
            }
        }

        [Task]
        bool IsLowOnAmmo()
        {
            return isLowOnAmmo;
        }

        [Task]
        bool IsAmmoZero()
        {
            return isAmmoZero;
        }

        [Task]
        bool IsReloading()
        {
            return isReloading;
        }

        [Task]
        bool ReadyToFire()
        {
            return readyToFire;
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
            isAmmoZero = paramsObj.Int <= 0;
        }

        private void OnReloadStart(ParamsObject paramsObj)
        {
            isReloading = true;
        }

        private void OnReloadEnd(ParamsObject paramsObj)
        {
            isReloading = false;
        }

        private void OnUnlockFire(ParamsObject paramsObj)
        {
            readyToFire = true;
        }

        private void OnLockFire(ParamsObject paramsObj)
        {
            readyToFire = false;
        }

        private void OnUpdateGunProjectile(ParamsObject paramsObj)
        {
            gunProjectile = paramsObj.Rigidbody;
        }

        private void OnUpdateGunTransform(ParamsObject paramsObj)
        {
            gunTransform = paramsObj.Transform;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aimTarget, .3f);
        }
    }
}
