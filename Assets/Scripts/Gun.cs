using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour {

    [SerializeField]
    private Transform spriteTransform;
    [SerializeField]
    private Rigidbody2D projectileToBeFired;
    [SerializeField]
    private int speed;
    [SerializeField]
    private Vector2 barrelOffset;

    private GameObjectEventManager eventManager;
    private float automaticFireTimer;

    private Rigidbody2D lastFired;
    public Rigidbody2D LastFired
    {
        get {
            return lastFired;
        }
    }

    [SerializeField]
    private int maxAmmo;
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

            currAmmo = value;
        }
    }

    //[SerializeField]
    //private bool automatic;
    //[SerializeField]
    //private float automaticFireTime;

    void Start() {
        eventManager = GetComponentInParent<GameObjectEventManager>();
        lastFired = null;
        CurrentAmmo = MaxAmmo;
        eventManager.StartListening("OnShoot", new UnityAction(OnShoot));
        //automaticFireTimer = automaticFireTime;
    }

    private void OnShoot() {
        if(CurrentAmmo > 0) {
            //if(automatic) {
            //    automaticFireTimer -= Time.deltaTime;
            //    if(automaticFireTimer <= 0) {
            //        automaticFireTimer = automaticFireTime;
            //        fireProjectile();
            //    }
            //} else {
            //    fireProjectile();
            //}
            FireProjectile();
        }
    }

    private void FireProjectile() {
        Rigidbody2D newProjectile = (Rigidbody2D)Instantiate(projectileToBeFired, transform.position + new Vector3(barrelOffset.x, barrelOffset.y, -1), spriteTransform.rotation);
        newProjectile.AddForce(newProjectile.transform.up * speed, ForceMode2D.Impulse);
        lastFired = newProjectile;
        CurrentAmmo--;
    }
}
