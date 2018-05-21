using EventManagers;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour {
    
    [SerializeField]
    private Rigidbody2D projectileToBeFired;
    [SerializeField]
    private int speed;
    [SerializeField]
    private bool automatic;
    [SerializeField]
    private float fireRate;
    [SerializeField]
    private int maxAmmo;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float reloadTime;
    [SerializeField]
    private AudioClip shotSound;

    private GameObjectEventManager eventManager;
    private AudioSource audioSource;
    private float fireTime;
    private float lastFireTime;
    private float deltaFireTime;
    private bool reloading;
    private float lastReloadTime;
    private float deltaReloadCheckTime;

    private Rigidbody2D lastFired;
    public Rigidbody2D LastFired
    {
        get {
            return lastFired;
        }
    }

    private int currAmmo;
    public int MaxAmmo {get {return maxAmmo;}}
    public int CurrentAmmo
    {
        get { return currAmmo; }
        set {
            if(value < 0) {
                currAmmo = 0;
            }
            if(value > maxAmmo) {
                currAmmo = maxAmmo;
            }

            ParamsObject paramsObject = new ParamsObject(value);
            paramsObject.Float = (float)MaxAmmo;
            eventManager.TriggerEvent("UpdateAmmoCount", paramsObject);

            currAmmo = value;
        }
    }

    void Start() {
        eventManager = GetComponentInParent<GameObjectEventManager>();
        audioSource = GetComponent<AudioSource>();
        lastFired = null;
        CurrentAmmo = MaxAmmo;
        fireTime = 1 / fireRate;
        lastFireTime = 0;
        reloading = false;
        lastReloadTime = 0;
        eventManager.StartListening("OnReload", new UnityAction<ParamsObject>(OnReload));
        if (automatic) {
            eventManager.StartListening("OnAutoShoot", new UnityAction<ParamsObject>(OnAutoShoot));
        } else {
            eventManager.StartListening("OnShoot", new UnityAction<ParamsObject>(OnSingleShoot));
        }
    }

    void Update() {
        if (reloading) {
            deltaReloadCheckTime = Time.time - lastReloadTime;
            if (deltaReloadCheckTime >= reloadTime) {
                reloading = false;
                CurrentAmmo = MaxAmmo;
            }
        }
    }

    private void OnReload(ParamsObject paramsObj) {
        if(CurrentAmmo != MaxAmmo && !reloading) {
            reloading = true;
            lastReloadTime = Time.time;
        }
    }

    private void OnSingleShoot(ParamsObject paramsObj) {
        if (CurrentAmmo > 0 && !reloading) {
            deltaFireTime = Time.time - lastFireTime;
            if (deltaFireTime >= fireTime) {
                lastFireTime = Time.time;
                FireProjectile();
            }
        }
    }

    private void OnAutoShoot(ParamsObject paramsObj) {
        if (CurrentAmmo > 0 && !reloading) {
            deltaFireTime = Time.time - lastFireTime;
            if (deltaFireTime >= fireTime) {
                lastFireTime = Time.time;
                FireProjectile();
            }
        }
    }

    private void FireProjectile() {
        audioSource.PlayOneShot(shotSound);
        Rigidbody2D newProjectile = (Rigidbody2D)Instantiate(projectileToBeFired, transform.GetChild(0).position, transform.rotation);
        Vector2 velocity = newProjectile.transform.up * speed;
        velocity += transform.root.GetComponent<Rigidbody2D>().velocity;
        newProjectile.velocity = velocity;
        newProjectile.GetComponent<DamageSource>().Damage = damage;
        newProjectile.GetComponent<DamageSource>().Source = transform.root.tag;
        lastFired = newProjectile;
        CurrentAmmo--;
    }
}
