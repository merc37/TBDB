using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DamageSource))]
public class ShotgunBlast : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D slugPrefab;
    [SerializeField]
    private int slugCount = 7;
    [SerializeField]
    private float blastDegree = 15;

    private new Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rigidbody.velocity.magnitude > 0)
        {
            Rigidbody2D slug;
            for (int i = 0; i < slugCount; i++)
            {
                slug = Instantiate(slugPrefab, rigidbody.position, Quaternion.Euler(0, 0, rigidbody.rotation + Random.Range(-blastDegree, blastDegree)));
                slug.velocity = slug.rotation.ToVector2().normalized * rigidbody.velocity.magnitude;

                DamageSource dmgSrc = GetComponent<DamageSource>(), slugDmgSrc = slug.GetComponent<DamageSource>();
                if (dmgSrc != null && slugDmgSrc != null)
                {
                    slugDmgSrc.Set(dmgSrc.Source, dmgSrc.Damage);
                }
            }

            Destroy(gameObject);
        }
    }
}
