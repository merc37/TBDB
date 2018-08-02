using EventManagers;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour {
    
    [SerializeField]
    private Rigidbody2D projectileToBeFired;
    [SerializeField]
    private int speed;
    [SerializeField]
    protected bool automatic;
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

    protected GameObjectEventManager eventManager;
    private AudioSource audioSource;
    private float fireTime;
    private float lastFireTime;
    private float deltaFireTime;
    private bool reloading;
    private float lastReloadTime;
    private float deltaReloadCheckTime;

    private Rigidbody2D playerRigidbody;

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
            eventManager.TriggerEvent("AmmoCount", paramsObject);

            currAmmo = value;
        }
    }

    void Awake() {
        eventManager = GetComponentInParent<GameObjectEventManager>();
        audioSource = GetComponent<AudioSource>();
        CurrentAmmo = MaxAmmo;
        fireTime = 1 / fireRate;
        lastFireTime = 0;
        reloading = false;
        lastReloadTime = 0;
        eventManager.StartListening("OnReload", new UnityAction<ParamsObject>(OnReload));
        eventManager.StartListening("OnShoot", new UnityAction<ParamsObject>(OnShoot));
    }

    void Start() {
        UpdateGunInfo();
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

    protected virtual void OnShoot(ParamsObject paramsObj) {
        if (CurrentAmmo > 0 && !reloading) {
            deltaFireTime = Time.time - lastFireTime;
            if (deltaFireTime >= fireTime) {
                lastFireTime = Time.time;
                FireProjectile();
            }
        }
    }

    Rigidbody2D newProjectile = null;
    protected virtual Rigidbody2D FireProjectile() {
        audioSource.PlayOneShot(shotSound);
        newProjectile = (Rigidbody2D)Instantiate(projectileToBeFired, transform.GetChild(0).position, transform.rotation);
        Vector2 velocity = newProjectile.transform.up.normalized * speed;
        velocity += transform.root.GetComponent<Rigidbody2D>().velocity;
        newProjectile.velocity = velocity;
        newProjectile.GetComponent<DamageSource>().Damage = damage;
        newProjectile.GetComponent<DamageSource>().Source = transform.root.tag;
        CurrentAmmo--;
        return newProjectile;
    }

    private void UpdateGunInfo() {
        ParamsObject paramsObj = new ParamsObject(transform);
        paramsObj.Float = speed;
        paramsObj.Int = damage;
        eventManager.TriggerEvent("UpdateGunInfo", paramsObj);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(newProjectile != null) {
            Gizmos.DrawSphere(newProjectile.position, .5f);
        }
    }
}
