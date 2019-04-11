using Player;
using UnityEngine;
using UnityEngine.Events;

namespace EventManagers
{
    public class ParamsObject
    {
        public Transform Transform { get; set; }
        public GameObject GameObject { get; set; }
        public Rigidbody2D Rigidbody { get; set; }
        public Collider2D Collider { get; set; }
        public Vector2 Vector2 { get; set; }
        public Vector3 Vector3 { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public short Short { get; set; }
        public bool Bool { get; set; }
        public string String { get; set; }
        public LayerMask LayerMask { get; set; }
        public dynamic Dynamic { get; set; }

        public ParamsObject(Transform transform)
        {
            Transform = transform;
        }

        public ParamsObject(GameObject gameObject) {
            GameObject = gameObject;
        }

        public ParamsObject(Rigidbody2D rigidbody)
        {
            Rigidbody = rigidbody;
        }

        public ParamsObject(Collider2D collider)
        {
            Collider = collider;
        }

        public ParamsObject(Vector2 vector2)
        {
            Vector2 = vector2;
        }

        public ParamsObject(Vector3 vector3)
        {
            Vector3 = vector3;
        }

        public ParamsObject(int @int)
        {
            Int = @int;
        }

        public ParamsObject(float @float)
        {
            Float = @float;
        }

        public ParamsObject(short @short)
        {
            Short = @short;
        }

        public ParamsObject(bool @bool)
        {
            Bool = @bool;
        }

        public ParamsObject(string @string)
        {
            String = @string;
        }

        public ParamsObject(LayerMask layerMask)
        {
            LayerMask = layerMask;
        }

        public ParamsObject(dynamic dyn)
        {
            Dynamic = dyn;
        }
    }

    public class ParamsEvent : UnityEvent<ParamsObject>
    {

    }
}
