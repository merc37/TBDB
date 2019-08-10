using UnityEngine;

namespace Collisions
{
    public class DestroyOnCollision : OnCollision
    {
        protected override bool OnCollisionEnterAction(Collision2D coll)
        {
            Destroy(gameObject);
            return true;
        }

        protected override bool OnTriggerEnterAction(Collider2D coll)
        {
            Destroy(gameObject);
            return true;
        }
    }
}

