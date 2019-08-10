using UnityEngine;
using EventManagers;
using Events;

namespace Collisions
{
    public class DamageOnCollision : OnCollision
    {
        private GameObjectEventManager eventManager;

        void Awake()
        {
            eventManager = GetComponentInParent<GameObjectEventManager>();
        }

        protected override bool OnCollisionEnterAction(Collision2D coll)
        {
            eventManager.TriggerEvent(HealthEvents.OnDecreaseHealth, new ParamsObject(coll.gameObject.GetComponent<DamageSource>().Damage));
            return true;
        }

        protected override bool OnTriggerEnterAction(Collider2D coll)
        {
            return false;
        }

        protected override string GetTag(GameObject go)
        {
            DamageSource dmgSrc = go.GetComponent<DamageSource>();
            if (dmgSrc != null)
            {
                return dmgSrc.Source;
            }
            return "Error";
        }
    }
}

