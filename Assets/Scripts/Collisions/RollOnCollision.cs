using EventManagers;
using UnityEngine;
using System.Linq;

namespace Collisions {
    public class RollOnCollision : MonoBehaviour {

        [SerializeField]
        [TagSelector]
        private string[] damageTags = new string[1];

        private GameObjectEventManager eventManager;

        void Awake() {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        void OnTriggerEnter2D(Collider2D coll) {
            DamageSource damageSource = coll.GetComponent<DamageSource>();
            if(damageSource) {
                if(damageTags.Contains(damageSource.Source)) {
                    eventManager.TriggerEvent("Roll", new ParamsObject(coll.attachedRigidbody.velocity));
                }
            }
        }

    }
}

