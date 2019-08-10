using UnityEngine;
using NaughtyAttributes;
using System.Linq;

namespace Collisions
{
    public abstract class OnCollision : MonoBehaviour
    {
        [SerializeField]
        private bool allTags = false;

        [DisableIf("allTags")]
        [SerializeField]
        private bool multipleTags = false;

        [HideIf("multipleTags")]
        [DisableIf("allTags")]
        [SerializeField]
        [TagSelector]
        private string collisionTag;

        [ShowIf("multipleTags")]
        [DisableIf("allTags")]
        [SerializeField]
        [TagSelector]
        private string[] collisionTags;

        void OnCollisionEnter2D(Collision2D coll)
        {
            if (allTags)
            {
                OnCollisionEnterAction(coll);
                return;
            }

            if (!multipleTags)
            {
                if (collisionTag == GetTag(coll.gameObject))
                {
                    OnCollisionEnterAction(coll);
                }

                return;
            }

            if (collisionTags.Contains(GetTag(coll.gameObject)))
            {
                OnCollisionEnterAction(coll);
            }
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (allTags)
            {
                OnTriggerEnterAction(coll);
                return;
            }

            if (!multipleTags)
            {
                if (collisionTag == GetTag(coll.gameObject))
                {
                    OnTriggerEnterAction(coll);
                }

                return;
            }

            if (collisionTags.Contains(GetTag(coll.gameObject)))
            {
                OnTriggerEnterAction(coll);
            }
        }

        protected virtual string GetTag(GameObject go)
        {
            return go.tag;
        }

        protected abstract bool OnCollisionEnterAction(Collision2D coll);
        protected abstract bool OnTriggerEnterAction(Collider2D coll);
    }
}
