using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D projectileToBeFired;
    [SerializeField]
    private float projectileSpeed = 10;
    [SerializeField]
    protected bool automatic;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private int maxAmmo;
    [SerializeField]
    private int ammoConsumption = 1;
    [SerializeField]
    private short damage;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private AudioClip shotSound;

    protected GameObjectEventManager eventManager;
    private AudioSource audioSource;

    private bool reloading;
    private bool shotFiredCooldown;

    private float fireTime;
    private float fireTimer;

    private float reloadTimer;

    private int currAmmo;
    public int MaxAmmo { get { return maxAmmo; } }
    public int CurrentAmmo
    {
        get { return currAmmo; }
        set
        {
            if (value < 0)
            {
                currAmmo = 0;
            }
            if (value > maxAmmo)
            {
                currAmmo = maxAmmo;
            }

            eventManager.TriggerEvent(GunEvents.OnUpdateCurrentAmmo, new ParamsObject(value));

            currAmmo = value;
        }
    }

    protected virtual void Awake()
    {
        eventManager = GetComponentInParent<GameObjectEventManager>();
        audioSource = GetComponent<AudioSource>();
        CurrentAmmo = MaxAmmo;
        fireTime = 1 / fireRate;
        reloading = false;
        fireTimer = fireTime;
        reloadTimer = reloadTime;
        eventManager.StartListening(GunEvents.OnReload, new UnityAction<ParamsObject>(OnReload));
        eventManager.StartListening(GunEvents.OnShoot, new UnityAction<ParamsObject>(OnShoot));
    }

    void Start()
    {
        eventManager.TriggerEvent(GunEvents.OnUpdateMaxAmmo, new ParamsObject(MaxAmmo));
        eventManager.TriggerEvent(GunEvents.OnUpdateCurrentAmmo, new ParamsObject(CurrentAmmo));
        eventManager.TriggerEvent(GunEvents.OnUpdateGunProjectileSpeed, new ParamsObject(projectileSpeed));
        eventManager.TriggerEvent(GunEvents.OnUpdateGunDamage, new ParamsObject(damage));
        eventManager.TriggerEvent(GunEvents.OnUpdateGunTransform, new ParamsObject(transform));
        eventManager.TriggerEvent(GunEvents.OnUpdateGunFireType, new ParamsObject(automatic));
        eventManager.TriggerEvent(GunEvents.OnUpdateGunProjectile, new ParamsObject(projectileToBeFired));
        eventManager.TriggerEvent(GunEvents.OnUnlockFire);
    }

    void Update()
    {
        if (reloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                reloading = false;
                reloadTimer = reloadTime;
                CurrentAmmo = MaxAmmo;
                eventManager.TriggerEvent(GunEvents.OnReloadEnd);
            }
        }

        if (shotFiredCooldown)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                shotFiredCooldown = false;
                fireTimer = fireTime;
                eventManager.TriggerEvent(GunEvents.OnUnlockFire);
            }
        }
    }

    private void OnReload(ParamsObject paramsObj)
    {
        if (CurrentAmmo != MaxAmmo && !reloading)
        {
            reloading = true;
            eventManager.TriggerEvent(GunEvents.OnReloadStart);
        }
    }

    protected virtual void OnShoot(ParamsObject paramsObj)
    {
        bool shootOverride = paramsObj != null ? paramsObj.Bool : false;
        if ((CurrentAmmo > 0 && !reloading && !shotFiredCooldown) || shootOverride)
        {
            FireProjectile();
            shotFiredCooldown = true;
            eventManager.TriggerEvent(GunEvents.OnLockFire);
            return;
        }
        if (CurrentAmmo <= 0)
        {
            eventManager.TriggerEvent(GunEvents.OnReload);
        }
    }

    protected virtual Rigidbody2D FireProjectile()
    {
        Rigidbody2D newProjectile, shooter = transform.root.GetComponent<Rigidbody2D>();
        audioSource.PlayOneShot(shotSound);
        newProjectile = Instantiate(projectileToBeFired, transform.GetChild(0).position, Quaternion.Euler(0, 0, shooter.rotation));
        Vector2 velocity = shooter.rotation.ToVector2().normalized * projectileSpeed;
        velocity += shooter.velocity;
        newProjectile.velocity = velocity;
        newProjectile.GetComponent<DamageSource>().Damage = damage;
        newProjectile.GetComponent<DamageSource>().Source = transform.root.tag;
        CurrentAmmo -= ammoConsumption;
        return newProjectile;
    }
}
