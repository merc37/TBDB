using UnityEngine;
using System.Linq;
using EventManagers;

namespace Collisions {
    public class DamageOnCollision : MonoBehaviour {

        private GameObjectEventManager eventManager;

        [SerializeField]
        [TagSelector]
        private string[] damageTags = new string[1];

        void Awake() {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        void OnCollisionEnter2D(Collision2D coll) {
            DamageSource damageSource = coll.gameObject.GetComponent<DamageSource>();
            if(damageSource) {
                if(damageTags.Contains(damageSource.Source)) {
                    eventManager.TriggerEvent("DecreaseHealth", new ParamsObject(damageSource.Damage));
                }
            }
        }

        //void OnTriggerEnter2D(Collider2D coll) {
        //    DamageSource damageSource = coll.gameObject.GetComponent<DamageSource>();
        //    if(damageSource) {
        //        if(damageTags.Contains(damageSource.Source)) {
        //            eventManager.TriggerEvent("DecreaseHealth", new ParamsObject(damageSource.Damage));
        //        }
        //    }
        //}
    }
}

