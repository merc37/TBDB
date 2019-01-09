using EventManagers;
using UnityEngine;
using UnityEngine.Events;
using Events;

public class Gun : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D projectileToBeFired;
    [SerializeField]
    private float projectileSpeed;
    [SerializeField]
    protected bool automatic;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private int maxAmmo;
    [SerializeField]
    private short damage;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private AudioClip shotSound;

    protected GameObjectEventManager eventManager;
    private AudioSource audioSource;

    private bool reloading;

    private float fireTime;
    private float lastFireTime;
    private float deltaFireTime;

    private float reloadTimer;

    private int currAmmo;
    public int MaxAmmo { get { return maxAmmo; } }
    public int CurrentAmmo
    {
        get { return currAmmo; }
        set
        {
            if(value < 0)
            {
                currAmmo = 0;
            }
            if(value > maxAmmo)
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
        lastFireTime = 0;
        reloading = false;
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
    }

    void Update()
    {
        if(reloading)
        {
            reloadTimer -= Time.deltaTime;
            if(reloadTimer <= 0)
            {
                reloading = false;
                reloadTimer = reloadTime;
                CurrentAmmo = MaxAmmo;
                eventManager.TriggerEvent(GunEvents.OnReloadEnd);
            }
        }
    }

    private void OnReload(ParamsObject paramsObj)
    {
        if(CurrentAmmo != MaxAmmo && !reloading)
        {
            reloading = true;
            eventManager.TriggerEvent(GunEvents.OnReloadStart);
        }
    }

    protected virtual void OnShoot(ParamsObject paramsObj)
    {
        if(CurrentAmmo > 0 && !reloading)
        {
            deltaFireTime = Time.time - lastFireTime;
            if(deltaFireTime >= fireTime)
            {
                lastFireTime = Time.time;
                FireProjectile();
            }
        }
    }

    Rigidbody2D newProjectile = null;//TODO: delete, just for gizmo to be drawn
    protected virtual Rigidbody2D FireProjectile()
    {
        audioSource.PlayOneShot(shotSound);
        newProjectile = Instantiate(projectileToBeFired, transform.GetChild(0).position, transform.rotation);
        Vector2 velocity = newProjectile.transform.up.normalized * projectileSpeed;
        velocity += transform.root.GetComponent<Rigidbody2D>().velocity;
        newProjectile.velocity = velocity;
        newProjectile.GetComponent<DamageSource>().Damage = damage;
        newProjectile.GetComponent<DamageSource>().Source = transform.root.tag;
        CurrentAmmo -= 1;
        return newProjectile;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(newProjectile != null)
        {
            Gizmos.DrawSphere(newProjectile.position, .3f);
        }
    }
}
