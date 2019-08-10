using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerRedirect : PlayerAbility
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override bool StartAbility()
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction;

            DamageSource dmgSrc;
            Rigidbody2D[] projectiles = FindObjectsOfType<DamageSource>().Where(ds => ds.gameObject.layer == LayerMask.NameToLayer("Projectiles")).Select(ds => ds.GetComponent<Rigidbody2D>()).ToArray();
            foreach (Rigidbody2D projectile in projectiles)
            {
                dmgSrc = projectile.GetComponent<DamageSource>();
                if (dmgSrc != null)
                {
                    dmgSrc.Set(transform.root.tag, dmgSrc.Damage);
                }
                direction = (mouseWorldPosition - projectile.position).normalized;
                projectile.velocity = direction * projectile.velocity.magnitude;
            }
            AbilityEnd();
            return true;
        }
    }
}
