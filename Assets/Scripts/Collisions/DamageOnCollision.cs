using UnityEngine;
using System.Linq;

namespace Collisions {
    public class DamageOnCollision : MonoBehaviour {

        [SerializeField]
        [TagSelector]
        private string[] damageTags = new string[1];

        void OnCollisionEnter2D(Collision2D coll) {
            DamageSource damageSource = coll.gameObject.GetComponent<DamageSource>();
            if(damageSource) {
                if(damageTags.Contains(damageSource.Source)) {
                    print(damageSource.Source);
                    GetComponent<Health>().Decrease(damageSource.Damage);
                    print(GetComponent<Health>().CurrentAmount);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D coll) {
            DamageSource damageSource = coll.gameObject.GetComponent<DamageSource>();
            if(damageSource) {
                if(damageTags.Contains(damageSource.Source)) {
                    print(damageSource.Source);
                    GetComponent<Health>().Decrease(damageSource.Damage);
                    print(GetComponent<Health>().CurrentAmount);
                }
            }
        }
    }
}

