using UnityEngine;
using UnityEngine.Events;

namespace EventManagers {
    public class ParamsObject {
        public Transform Transform { get; set; }
        public Rigidbody2D Rigidbody { get; set; }
        public Vector2 Vector2 { get; set; }
        public Vector3 Vector3 { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public string String { get; set; }
        public dynamic Dynamic { get; set; }

        public ParamsObject(Transform transform) {
            Transform = transform;
        }

        public ParamsObject(Rigidbody2D rigidbody) {
            Rigidbody = rigidbody;
        }

        public ParamsObject(Vector2 vector2) {
            Vector2 = vector2;
        }

        public ParamsObject(Vector3 vector3) {
            Vector3 = vector3;
        }

        public ParamsObject(int @int) {
            Int = @int;
        }

        public ParamsObject(float @float) {
            Float = @float;
        }

        public ParamsObject(string @string) {
            String = @string;
        }

        public ParamsObject(dynamic dyn) {
            Dynamic = dyn;
        }
    }

    public class ParamsEvent : UnityEvent<ParamsObject> {

    }
}
